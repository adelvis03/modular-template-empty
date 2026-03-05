using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using TemplateName.Shared.Abstractions.Modules;

namespace TemplateName.Shared.Infrastructure.Modules;

public static class ModuleLoader
{
    public static IList<Assembly> LoadAssemblies(IConfiguration configuration, string modulePart)
    {
        // Start with already loaded assemblies
        var assemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();
        var loadedLocations = assemblies.Where(a => !a.IsDynamic).Select(a => a.Location).ToHashSet(StringComparer.InvariantCultureIgnoreCase);

        // Get all DLLs in the base directory that are not already loaded
        var files = Directory.Exists(AppDomain.CurrentDomain.BaseDirectory)
            ? Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.dll")
                .Where(f => !loadedLocations.Contains(f))
                .ToList()
            : new List<string>();

        if (!files.Any())
        {
            return assemblies;
        }

        // Filter out disabled modules based on configuration
        var enabledFiles = new List<string>();
        foreach (var file in files)
        {
            // Only consider module DLLs that contain the specified modulePart
            if (!file.Contains(modulePart, StringComparison.InvariantCultureIgnoreCase))
            {
                continue;
            }

            // Try to derive module name safely
            var fileName = Path.GetFileNameWithoutExtension(file);
            var moduleName = fileName.Replace(modulePart, "", StringComparison.InvariantCultureIgnoreCase).Trim('.');
            if (string.IsNullOrWhiteSpace(moduleName))
            {
                moduleName = fileName.ToLowerInvariant();
            }

            var enabled = configuration.GetValue<bool>($"{moduleName}:module:enabled", true);
            if (enabled)
            {
                enabledFiles.Add(file);
            }
        }

        // Safely load assemblies
        foreach (var file in enabledFiles)
        {
            try
            {
                var asmName = AssemblyName.GetAssemblyName(file);
                assemblies.Add(AppDomain.CurrentDomain.Load(asmName));
            }
            catch
            {
                // Skip files that cannot be loaded
            }
        }

        return assemblies;
    }

    public static IList<IModule> LoadModules(IEnumerable<Assembly> assemblies)
    {
        if (assemblies == null || !assemblies.Any())
            return new List<IModule>();

        return assemblies
            .SelectMany(a =>
            {
                try
                {
                    return a.GetTypes();
                }
                catch
                {
                    return Array.Empty<Type>();
                }
            })
            .Where(t => typeof(IModule).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
            .OrderBy(t => t.Name)
            .Select(t =>
            {
                try
                {
                    return Activator.CreateInstance(t) as IModule;
                }
                catch
                {
                    return null;
                }
            })
            .Where(m => m != null)
            .ToList();
    }
}

