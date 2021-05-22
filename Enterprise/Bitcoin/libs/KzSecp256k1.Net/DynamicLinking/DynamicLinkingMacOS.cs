using System;
using System.Runtime.InteropServices;
// ReSharper disable InconsistentNaming

namespace Secp256k1Net.DynamicLinking
{
    static class DynamicLinkingMacOS
    {
        private const string DllName = "libdl";

        [DllImport(DllName)]
        public static extern IntPtr dlopen(string path, int flags);

        [DllImport(DllName)]
        public static extern int dlclose(IntPtr handle);

        [DllImport(DllName)]
        public static extern IntPtr dlerror();

        [DllImport(DllName)]
        public static extern IntPtr dlsym(IntPtr handle, string name);
    }
}
