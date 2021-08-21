using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using static System.Runtime.InteropServices.OSPlatform;
using static System.Runtime.InteropServices.Architecture;
using static System.Runtime.InteropServices.RuntimeInformation;
using PlatformInfo = System.ValueTuple<System.Runtime.InteropServices.OSPlatform, System.Runtime.InteropServices.Architecture>;
using System.Reflection;
using System.Collections.Concurrent;
// ReSharper disable InconsistentNaming

namespace Secp256k1Net
{
    public static class LibPathResolver
    {

        private static readonly Dictionary<PlatformInfo, (string Prefix, string LibPrefix, string Extension)> PlatformPaths = new Dictionary<PlatformInfo, (string, string, string)>
        {
            [(Windows, X64)] = ("win-x64", "", ".dll"),
            [(Windows, X86)] = ("win-x86", "", ".dll"),
            [(Linux, X64)] = ("linux-x64", "lib", ".so"),
            [(OSX, X64)] = ("osx-x64", "lib", ".dylib"),
        };

        private static readonly OSPlatform[] SupportedPlatforms = { Windows, OSX, Linux };
        private static string SupportedPlatformDescriptions() => string.Join("\n", PlatformPaths.Keys.Select(GetPlatformDesc));

        private static string GetPlatformDesc((OSPlatform OS, Architecture Arch) info) => $"{info.OS}; {info.Arch}";

        private static readonly OSPlatform CurrentOSPlatform = SupportedPlatforms.FirstOrDefault(IsOSPlatform);
        private static readonly PlatformInfo CurrentPlatformInfo = (CurrentOSPlatform, ProcessArchitecture);
        private static readonly Lazy<string> CurrentPlatformDesc = new Lazy<string>(() => GetPlatformDesc((CurrentOSPlatform, ProcessArchitecture)), true);

        private static readonly ConcurrentDictionary<PlatformInfo, string> Cache = new ConcurrentDictionary<PlatformInfo, string>();

        public static List<string> ExtraNativeLibSearchPaths = new List<string>();

        public static string Resolve(string library)
        {
            if (Cache.TryGetValue(CurrentPlatformInfo, out string result))
            {
                return result;
            }
            if (!PlatformPaths.TryGetValue(CurrentPlatformInfo, out (string Prefix, string LibPrefix, string Extension) platform))
            {
                throw new Exception(string.Join("\n", $"Unsupported platform: {CurrentPlatformDesc.Value}", "Must be one of:", SupportedPlatformDescriptions()));
            }

            var searchedPaths = new HashSet<string>();

            foreach (var containerDir in GetSearchLocations())
            {
                foreach (var libPath in SearchContainerPaths(containerDir, library, platform))
                {
                    if (!searchedPaths.Contains(libPath) && File.Exists(libPath))
                    {
                        Cache.TryAdd(CurrentPlatformInfo, libPath);
                        return libPath;
                    }
                    searchedPaths.Add(libPath);
                }
            }

            throw new Exception($"Platform can be supported but '{library}' lib not found for {CurrentPlatformDesc.Value} at: {Environment.NewLine}{string.Join(Environment.NewLine, searchedPaths)}");

        }

        private static IEnumerable<string> GetSearchLocations()
        {
            yield return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            yield return Path.GetDirectoryName(Assembly.GetCallingAssembly().Location);
            yield return Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location);
            foreach(var extraPath in ExtraNativeLibSearchPaths)
            {
                yield return extraPath;
            }
            // If the this lib is being executed from its nuget package directory then the native
            // files should be found up a couple directories.
            yield return Path.GetFullPath(
                Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? throw new InvalidOperationException(),
                "../../content"));
        }

        private static IEnumerable<string> SearchContainerPaths(string containerDir, string library, (string Prefix, string LibPrefix, string Extension) platform)
        {
            foreach(var subDir in GetSearchSubDir(library, platform))
            {
                yield return Path.Combine(containerDir, subDir);
                yield return Path.Combine(containerDir, "publish", subDir);
            }
        }

        private static IEnumerable<string> GetSearchSubDir(string library, (string Prefix, string LibPrefix, string Extension) platform)
        {
            var libFileName = platform.LibPrefix + library + platform.Extension;

            yield return libFileName;
            yield return Path.Combine(platform.Prefix, libFileName);
            yield return Path.Combine("native", platform.Prefix, libFileName);
            yield return Path.Combine("runtimes", platform.Prefix, "native", libFileName);
        }
    }
}
