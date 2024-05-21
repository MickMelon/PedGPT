using System.Diagnostics;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using FxMediator.Server;
using IntelliPed.FiveM.Server.Controllers;
using IntelliPed.FiveM.Server.Hubs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ObjectPool;

namespace IntelliPed.FiveM.Server;

public class Program : BaseScript
{
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

        IWebHost host = new WebHostBuilder()
            .UseConfiguration(config)
            .UseKestrel()
            .ConfigureServices(services =>
            {
                services
                    .AddMvc()
                    .AddApplicationPart(typeof(NavigationController).Assembly);

                services.AddSingleton<ObjectPoolProvider>(new DefaultObjectPoolProvider());
                services.AddSingleton<IHostingEnvironment>(new HostingEnvironment());
                services.AddSingleton<DiagnosticSource>(new DiagnosticListener("IntelliPed"));

                services.AddSingleton(baseScriptProxy);
                services.AddSingleton<ServerMediator>();

                services.AddSignalR();
            })
            .Configure(app =>
            {
                app.UseCors();
                app.UseMvcWithDefaultRoute();
                app.UseSignalR(configure =>
                {
                    configure.MapHub<MyHub>("/my-hub");
                });
            })
            .ConfigureLogging(_ => _.AddConsole())
            .Build();

        await Task.Run(host.Run);
    }
}