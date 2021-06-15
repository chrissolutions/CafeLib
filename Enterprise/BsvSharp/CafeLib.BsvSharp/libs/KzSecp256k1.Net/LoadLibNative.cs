using Secp256k1Net.DynamicLinking;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Secp256k1Net
{
    public static class LoadLibNative
    {
        private static readonly PlatformOS PlatformOS = GetPlatformOS();

        public static IntPtr LoadLibrary(string libPath)
        {
            // ReSharper disable once InconsistentNaming
            const int RTLD_NOW = 2;
            var libPtr = PlatformOS switch
            {
                PlatformOS.Windows => DynamicLinkingWindows.LoadLibrary(libPath),
                PlatformOS.Linux => DynamicLinkingLinux.dlopen(libPath, RTLD_NOW),
                PlatformOS.MacOS => DynamicLinkingMacOS.dlopen(libPath, RTLD_NOW),
                _ => throw new Exception($"Unsupported platform: {RuntimeInformation.OSDescription}. The supported platforms are: {string.Join(", ", new[] { OSPlatform.Windows, OSPlatform.OSX, OSPlatform.Linux })}")
            };

            return libPtr != IntPtr.Zero
                ? libPtr 
                : throw new Exception($"Library loading failed, file: {libPath}", GetLastError());
        }

        public static void CloseLibrary(IntPtr lib)
        {
            if (lib == IntPtr.Zero) return;

            switch (PlatformOS)
            {
                case PlatformOS.Windows:
                    DynamicLinkingWindows.FreeLibrary(lib);
                    break;

                case PlatformOS.Linux:
                    DynamicLinkingLinux.dlclose(lib);
                    break;

                case PlatformOS.MacOS:
                    DynamicLinkingMacOS.dlclose(lib);
                    break;

                default:
                    throw new NotSupportedException("Unsupported platform");
            }
        }

        public static Exception GetLastError()
        {
            switch (PlatformOS)
            {
                case PlatformOS.Windows:
                    return new Win32Exception(Marshal.GetLastWin32Error());

                case PlatformOS.Linux:
                {
                    var errorPtr = DynamicLinkingLinux.dlerror();
                    return new Exception(errorPtr != IntPtr.Zero ? Marshal.PtrToStringAnsi(errorPtr) : "Error information could not be found");
                }

                case PlatformOS.MacOS:
                {
                    var errorPtr = DynamicLinkingLinux.dlerror();
                    return new Exception(errorPtr != IntPtr.Zero ? Marshal.PtrToStringAnsi(errorPtr) : "Error information could not be found");
                }

                default:
                    throw new NotSupportedException("Unsupported platform");
            }

        }

        public static TDelegate GetDelegate<TDelegate>(IntPtr libPtr, string symbolName)
        {
            var functionPtr = PlatformOS switch
            {
                PlatformOS.Windows => DynamicLinkingWindows.GetProcAddress(libPtr, symbolName),
                PlatformOS.Linux => DynamicLinkingLinux.dlsym(libPtr, symbolName),
                PlatformOS.MacOS => DynamicLinkingMacOS.dlsym(libPtr, symbolName),
                _ => throw new Exception($"Unsupported platform: {RuntimeInformation.OSDescription}. The supported platforms are: {string.Join(", ", new[] { OSPlatform.Windows, OSPlatform.OSX, OSPlatform.Linux })}")
            };

            return functionPtr != IntPtr.Zero
                ? Marshal.GetDelegateForFunctionPointer<TDelegate>(functionPtr)
                : throw new Exception($"Library symbol failed, symbol: {symbolName}", GetLastError());
        }

        private static PlatformOS GetPlatformOS()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return PlatformOS.Windows;
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return PlatformOS.Linux;
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return PlatformOS.MacOS;
            }

            throw new NotSupportedException($"Unsupported platform: {RuntimeInformation.OSDescription}. The supported platforms are: {string.Join(", ", new[] { OSPlatform.Windows, OSPlatform.OSX, OSPlatform.Linux })}");
        }
    }
}
