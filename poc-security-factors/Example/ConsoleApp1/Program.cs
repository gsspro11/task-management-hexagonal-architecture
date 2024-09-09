using Poc.Factors.Extensions;
using ConsoleApp1;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

var services = builder.Services;

services.AddPocFactors(builder.Configuration.GetSection("Factors"));
services.AddSingleton<App>();

using IHost host = builder.Build();

var app = host.Services.GetService<App>();
await app.AppRun();

await host.RunAsync();