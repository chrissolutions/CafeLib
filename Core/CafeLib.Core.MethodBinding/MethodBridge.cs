using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CafeLib.Core.Extensions;
// ReSharper disable UnusedMember.Global

namespace CafeLib.Core.MethodBinding
{
    public abstract class MethodBridge
    {
        #region Private Members

        private readonly Dictionary<string, Delegate> _bridgeMap = new Dictionary<string, Delegate>();

        #endregion

        #region Constructors

        protected MethodBridge()
        {
            // Populate the selector map.
            Populate();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Invoke the export method.
        /// </summary>
        /// <param name="exportName">export method name</param>
        /// <param name="args">arguments</param>
        public virtual void Invoke(string exportName, object[] args)
        {
            // Obtain the export handler.
            var handler = _bridgeMap[exportName];

            // Obtain the handler parameters; skip Closure type and convert to arguments.
            var arguments = ConvertArguments(handler.Method.GetParameters().Skip(1).ToArray());

            // invoke the handler.
            handler.DynamicInvoke(arguments);

            object[] ConvertArguments(IEnumerable<ParameterInfo> parameters)
            {
                var index = 0;
                return parameters.Select(p => index < args.Length
                    ? Converter.ConvertTo(p.ParameterType, args[index++])
                    : null).ToArray();
            }
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Map the method alias and to its action delegate in the bridge map.
        /// </summary>
        /// <param name='exportName'>export name</param>
        /// <param name="methodInfo">method info</param>
		private void MapBridgeEntry(string exportName, MethodInfo methodInfo)
		{
			var handler = methodInfo.CreateDelegate(this);
			_bridgeMap.AddOrUpdate(exportName, handler, (k, v) => handler);
		}

        /// <summary>
        /// Populate the bridge map for this instance.
        /// </summary>
        private void Populate()
        {
			foreach (var methodInfo in GetType().GetTypeInfo().DeclaredMethods)
            {
                var attr = methodInfo.GetCustomAttribute<MethodExportAttribute>();
                if (attr != null)
                {
                    MapBridgeEntry(attr.ExportName ?? methodInfo.Name, methodInfo);
                }
			}
        }

        #endregion
    }
}
