using System;

namespace CafeLib.Core.MethodBinding
{
    /// <summary>
    /// Web export attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class MethodExportAttribute : Attribute
    {
        /// <summary>
        /// MethodExportAttribute constructor
        /// </summary>
        /// <param name="exportName">export alias</param>
        public MethodExportAttribute(string exportName)
        {
            ExportName = exportName;
        }

        /// <summary>
        /// Gets or sets the alias.
        /// </summary>
        /// <value>
        /// The alias.
        /// </value>
        public string ExportName { get; }
    }
}
