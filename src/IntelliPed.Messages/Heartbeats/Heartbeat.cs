using IntelliPed.Messages.Common;
using System.Collections.Generic;
using System.Text;

namespace IntelliPed.Messages.Heartbeats;

public record Heartbeat
{
    public required Coordinates Coordinates { get; init; }
    public required string StreetName { get; init; }
    public required int Health { get; init; }
    public required List<NearbyPed> NearbyPeds { get; init; }
    public required List<NearbyVehicle> NearbyVehicles { get; init; }

    public override string ToString()
    {
        StringBuilder builder = new();

        builder.AppendLine($"Coordinates: {Coordinates}");
        builder.AppendLine($"StreetName: {StreetName}");
        builder.AppendLine($"Health: {Health}");

        //if (NearbyPeds != null && NearbyPeds.Any())
        //{
        //    builder.AppendLine("Nearby People (( Peds )):");
        //    foreach (NearbyPed ped in NearbyPeds)
        //    {
        //        builder.AppendLine($"  PedNetworkId: {ped.PedNetworkId}");
        //        builder.AppendLine($"  IsPlayer: {ped.IsPlayer}");
        //        builder.AppendLine($"  IsInVehicle: {ped.IsInVehicle}");
        //        builder.AppendLine($"  VehicleNetworkId: {ped.VehicleNetworkId}");
        //        builder.AppendLine($"  Distance: {ped.Distance}");
        //        builder.AppendLine($"  Direction: {ped.Direction}");
        //        builder.AppendLine($"  X: {ped.X}");
        //        builder.AppendLine($"  Y: {ped.Y}");
        //        builder.AppendLine($"  Z: {ped.Z}");
        //        builder.AppendLine();
        //    }
        //}

        //if (NearbyVehicles != null && NearbyVehicles.Any())
        //{
        //    builder.AppendLine("Nearby Vehicles:");
        //    foreach (NearbyVehicle vehicle in NearbyVehicles)
        //    {
        //        builder.AppendLine($"  VehicleNetworkId: {vehicle.VehicleNetworkId}");
        //        builder.AppendLine($"  IsBeingDriven: {vehicle.IsBeingDriven}");
        //        builder.AppendLine($"  DriverPedNetworkId: {vehicle.DriverPedNetworkId}");
        //        builder.AppendLine($"  Distance: {vehicle.Distance}");
        //        builder.AppendLine($"  Direction: {vehicle.Direction}");
        //        builder.AppendLine($"  X: {vehicle.X}");
        //        builder.AppendLine($"  Y: {vehicle.Y}");
        //        builder.AppendLine($"  Z: {vehicle.Z}");
        //        builder.AppendLine();
        //    }
        //}

        return builder.ToString();
    }
}

public record NearbyPed
{
    public required int PedNetworkId { get; init; }
    public required bool IsPlayer { get; init; }
    public required bool IsInVehicle { get; init; }
    public required int? VehicleNetworkId { get; init; }
    public required float Distance { get; init; }
    public required float Direction { get; init; }
    public required float X { get; init; }
    public required float Y { get; init; }
    public required float Z { get; init; }
}

public record NearbyVehicle
{
    public required int VehicleNetworkId { get; init; }
    public required bool IsBeingDriven { get; init; }
    public required int? DriverPedNetworkId { get; init; }
    public required float Distance { get; init; }
    public required float Direction { get; init; }
    public required float X { get; init; }
    public required float Y { get; init; }
    public required float Z { get; init; }
}