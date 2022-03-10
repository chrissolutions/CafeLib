using System;
using Microsoft.JSInterop;

namespace CafeLib.Blazor.Interop
{
    public abstract class JsInteropObject
    {
        /// <summary>
        /// Javascript interop object instance.
        /// </summary>
        protected IJSObjectReference Instance { get; }

        /// <summary>
        /// Javascript interop object constructor.
        /// </summary>
        /// <param name="instance">javascript object instance</param>
        /// <exception cref="ArgumentNullException">thrown if instance is null</exception>
        protected JsInteropObject(IJSObjectReference instance)
        {
            Instance = instance ?? throw new ArgumentNullException(nameof(instance));
        }
    }
}