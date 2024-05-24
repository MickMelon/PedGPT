using System;
using System.Collections.Generic;
using CitizenFX.Core;
using FxMediator.Client;
using IntelliPed.FiveM.Client.Extensions;
using IntelliPed.FiveM.Shared.Requests.Heartbeats;
using IntelliPed.Messages.Heartbeats;

namespace IntelliPed.FiveM.Client.Scripts;

public class HeartbeatScript : BaseScript
{
    public HeartbeatScript()
    {
        ClientMediator mediator = new();
        mediator.AddRequestHandler<HeartbeatRpcRequest, HeartbeatRpcReply>(OnHeartbeat);
    }

    private HeartbeatRpcReply OnHeartbeat(HeartbeatRpcRequest request)
    {
        Ped ped = (Ped)Entity.FromNetworkId(request.PedNetworkId);

        string streetName = ped.GetCurrentStreet();

        HeartbeatRpcReply reply = new()
        {
            StreetName = streetName,
            Health = ped.Health,
            NearbyPeds = FindNearbyPeds(ped),
            NearbyVehicles = FindNearbyVehicles(ped),
        };

        return reply;
    }

    private static List<NearbyPed> FindNearbyPeds(Ped ped)
    {
        List<NearbyPed> nearbyPeds = [];

        foreach (Ped nearbyPed in World.GetAllPeds())
        {
            if (nearbyPed == ped)
            {
                continue;
            }

            float distance = ped.Position.DistanceToSquared(nearbyPed.Position);
            float direction = CalculateAngle(ped.Position, nearbyPed.Position);

            bool isInVehicle = nearbyPed.IsInVehicle();

            nearbyPeds.Add(new NearbyPed
            {
                PedNetworkId = nearbyPed.NetworkId,
                IsPlayer = nearbyPed.IsPlayer,
                IsInVehicle = isInVehicle,
                VehicleNetworkId = isInVehicle ? nearbyPed.CurrentVehicle.NetworkId : null,
                Distance = distance,
                Direction = direction,
                X = nearbyPed.Position.X,
                Y = nearbyPed.Position.Y,
                Z = nearbyPed.Position.Z,
            });
        }

        return nearbyPeds;
    }

    private static List<NearbyVehicle> FindNearbyVehicles(Ped ped)
    {
        List<NearbyVehicle> nearbyVehicles = [];

        foreach (Vehicle nearbyVehicle in World.GetAllVehicles())
        {
            if (nearbyVehicle == ped.CurrentVehicle)
            {
                continue;
            }

            float distance = ped.Position.DistanceToSquared(nearbyVehicle.Position);
            float direction = CalculateAngle(ped.Position, nearbyVehicle.Position);

            bool isBeingDriven = nearbyVehicle.Driver != null;

            nearbyVehicles.Add(new NearbyVehicle
            {
                VehicleNetworkId = nearbyVehicle.NetworkId,
                IsBeingDriven = isBeingDriven,
                DriverPedNetworkId = isBeingDriven ? nearbyVehicle.Driver?.NetworkId : null,
                Distance = distance,
                Direction = direction,
                X = nearbyVehicle.Position.X,
                Y = nearbyVehicle.Position.Y,
                Z = nearbyVehicle.Position.Z,
            });
        }

        return nearbyVehicles;
    }

    public static Vector3 CalculateDirection(Vector3 from, Vector3 to)
    {
        // Subtract the vectors to get the direction
        Vector3 direction = to - from;

        // Normalize the result to get a unit vector
        direction.Normalize();

        return direction;
    }

    public static float CalculateAngle(Vector3 from, Vector3 to)
    {
        // Get the direction vector
        Vector3 direction = CalculateDirection(from, to);

        // Assuming the forward vector is along the Y-axis
        Vector3 forward = new Vector3(0.0f, 1.0f, 0.0f);

        // Calculate the angle between the direction and the forward vector
        float dotProduct = Vector3.Dot(forward, direction);
        double angle = Math.Acos(dotProduct) * (180.0f / (float)Math.PI);

        // Determine the sign of the angle using the cross product
        Vector3 crossProduct = Vector3.Cross(forward, direction);

        // If the cross product points in the negative Z direction, the angle is negative
        if (crossProduct.Z < 0)
        {
            angle = 360.0f - angle;
        }

        return (float)angle;
    }
}