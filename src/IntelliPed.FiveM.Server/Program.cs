using System;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using FxMediator.Server;
using IntelliPed.FiveM.Server.Hubs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace IntelliPed.FiveM.Server;

public class Program : BaseScript
{
    public static IServiceProvider Services { get; private set; } = null!;
    public static IServiceProvider ScopedServices => Services.CreateScope().ServiceProvider;
    private IWebHost _host = null!;

    [EventHandler("onResourceStart")]
    private async void OnResourceStart(string resourceName)
    {
        if (API.GetCurrentResourceName() != resourceName)
        {
            return;
        }

        BaseScriptProxy baseScriptProxy = new(Players);

        IConfigurationRoot config = new ConfigurationBuilder()
            .AddEnvironmentVariables(prefix: "ASPNETCORE_")
            .Build();

        _host = new WebHostBuilder()
            .UseConfiguration(config)
            .UseKestrel()
            .ConfigureServices(services =>
            {
                services.AddSingleton(baseScriptProxy);
                services.AddSingleton<ServerMediator>();
                services.AddSignalR();
            })
            .Configure(app =>
            {
                app.UseSignalR(configure =>
                {
                    configure.MapHub<AgentHub>("/agent-hub");
                });
            })
            .ConfigureLogging(_ => _.AddConsole())
            .Build();

        Services = _host.Services;

        await Task.Run(_host.Run);
    }

    [EventHandler("onResourceStop")]
    private async void OnResourceStop(string resourceName)
    {
        if (API.GetCurrentResourceName() != resourceName)
        {
            return;
        }
        
        await _host.StopAsync();
    }
}