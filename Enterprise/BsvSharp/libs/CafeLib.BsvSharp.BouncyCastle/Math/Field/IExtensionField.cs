﻿namespace CafeLib.BsvSharp.BouncyCastle.Math.Field
{
    public interface IExtensionField
        : IFiniteField
    {
        IFiniteField Subfield { get; }

        int Degree { get; }
    }
}
