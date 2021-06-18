using System;
using System.Collections.Generic;
using System.Reflection;
using CafeLib.Core.Extensions;

namespace Secp256k1Net
{
    internal class SymbolNameAttribute : Attribute
    {
        private static readonly IDictionary<string, Type> Map = new Dictionary<string, Type>();

        public string Name { get; set; }

        public SymbolNameAttribute(string name)
        {
            Name = name;
            Map.GetOrAdd(name, () => Assembly.GetExecutingAssembly().GetType(name));
        }
    }
}