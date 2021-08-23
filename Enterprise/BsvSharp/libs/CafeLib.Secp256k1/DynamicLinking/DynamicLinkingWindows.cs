using System;
using System.Runtime.InteropServices;

namespace Secp256k1Net.DynamicLinking
{
    internal static class DynamicLinkingWindows
    {
        private const string Kernel32 = "kernel32";

        [DllImport(Kernel32, SetLastError = true)]
        public static extern IntPtr LoadLibrary(string path);

        [DllImport(Kernel32, SetLastError = true)]
        public static extern int FreeLibrary(IntPtr module);

        [DllImport(Kernel32, SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = true)]
        public static extern IntPtr GetProcAddress(IntPtr module, string procName);
    }
}
