using System;

// ReSharper disable UnusedMember.Global

namespace CafeLib.Mobile.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class TransientAttribute : Attribute
    {
    }
}