using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;

namespace Nmb.Shared.Initialization
{
    public static class InitializationHelpers
    { 
        public static void LoadAllAssemblies(bool includeFramework = false)
        {
            // Storage to ensure not loading the same assembly twice and optimize calls to GetAssemblies()
            var loaded = new ConcurrentDictionary<string, bool>();

            // Filter to avoid loading all the .net framework
            bool ShouldLoad(string assemblyName)
            {
                return (includeFramework || NotNetFramework(assemblyName))
                    && !loaded.ContainsKey(assemblyName);
            }

            static bool NotNetFramework(string assemblyName)
            {
                return !assemblyName.StartsWith("Microsoft.")
                    && !assemblyName.StartsWith("System.")
                    && !assemblyName.StartsWith("Newtonsoft.")
                    && assemblyName != "netstandard";
            }

            void LoadReferencedAssembly(Assembly assembly)
            {
                // Check all referenced assemblies of the specified assembly
                foreach (var an in assembly.GetReferencedAssemblies().Where(a => ShouldLoad(a.FullName)))
                {
                    // Load the assembly and load its dependencies
                    LoadReferencedAssembly(Assembly.Load(an)); // AppDomain.CurrentDomain.Load(name)
                    loaded.TryAdd(an.FullName, true);
                    System.Diagnostics.Debug.WriteLine($"\n>> Referenced assembly => {an.FullName}");
                }
            }

            // Populate already loaded assemblies
            System.Diagnostics.Debug.WriteLine($">> Already loaded assemblies:");
            foreach (var a in AppDomain.CurrentDomain.GetAssemblies().Where(a => ShouldLoad(a.FullName)))
            {
                loaded.TryAdd(a.FullName, true);
                System.Diagnostics.Debug.WriteLine($">>>> {a.FullName}");
            }
            var alreadyLoaded = loaded.Keys.Count();
            var sw = new System.Diagnostics.Stopwatch();

            // Loop on loaded assembliesto load dependencies (it includes Startup assembly so should load all the dependency tree) 
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies().Where(a => NotNetFramework(a.FullName)))
                LoadReferencedAssembly(assembly);

            // Debug
            System.Diagnostics.Debug.WriteLine($"\n>> Assemblies loaded after scann ({(loaded.Keys.Count - alreadyLoaded)} assemblies in {sw.ElapsedMilliseconds} ms):");
            foreach (var a in loaded.Keys.OrderBy(k => k))
                System.Diagnostics.Debug.WriteLine($">>>> {a}");
        }
    }
}
