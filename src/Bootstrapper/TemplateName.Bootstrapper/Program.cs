using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using TemplateName.Shared.Abstractions;
using TemplateName.Shared.Infrastructure;
using TemplateName.Shared.Infrastructure.Logging;
using TemplateName.Shared.Infrastructure.Modules;

var builder = WebApplication
    .CreateBuilder(args);

builder.Host.ConfigureModules().UseLogging();

var assemblies = ModuleLoader.LoadAssemblies(builder.Configuration, "TemplateName.Modules.");
var modules = ModuleLoader.LoadModules(assemblies);

builder.Services.AddModularInfrastructure(builder.Configuration, assemblies, modules);

foreach (var module in modules)
{
    module.Register(builder.Services, builder.Configuration);
}

var app = builder.Build();

app.MapControllers();
app.UseModularInfrastructure();

foreach (var module in modules)
{
    module.Use(app);
}

app.MapGet("/", (AppInfo appInfo) => appInfo).WithTags("API").WithName("Info");

app.MapGet("/ping", () => "pong").WithTags("API").WithName("Pong");

app.MapGet("/modules", (ModuleInfoProvider moduleInfoProvider) => moduleInfoProvider.Modules);

foreach (var module in modules)
{
    module.Expose(app);
}

assemblies.Clear();
modules.Clear();

app.Run();


