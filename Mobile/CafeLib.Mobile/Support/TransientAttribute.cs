using System;
// ReSharper disable UnusedMember.Global

namespace CafeLib.Mobile.Support
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class TransientAttribute : Attribute
    {
    }
}
