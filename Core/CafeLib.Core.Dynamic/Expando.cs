﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using CafeLib.Core.Extensions;
using ICollection = System.Collections.ICollection;

namespace CafeLib.Core.Dynamic
{
    /// <summary>
    /// Class that provides extensible properties and methods to an
    /// existing object when cast to dynamic. This
    /// dynamic object stores 'extra' properties in a dictionary or
    /// checks the actual properties of the instance passed via 
    /// constructor.
    /// 
    /// This class can be subclassed to extend an existing type or 
    /// you can pass in an instance to extend. Properties (both
    /// dynamic and strongly typed) can be accessed through an 
    /// indexer.
    /// 
    /// This type allows you three ways to access its properties:
    /// 
    /// Directly: any explicitly declared properties are accessible
    /// Dynamic: dynamic cast allows access to dictionary and native properties/methods
    /// Dictionary: Any of the extended properties are accessible via IDictionary interface
    /// </summary>
    public class Expando : DynamicObject
    {
        /// <summary>
        /// Instance of object passed in
        /// </summary>
        private object _instance;

        /// <summary>
        /// Cached type of the instance
        /// </summary>
        private Type _instanceType;


        private PropertyInfo[] _instancePropertyInfo;
        private IEnumerable<PropertyInfo> InstancePropertyInfo
        {
            get
            {
                if (_instancePropertyInfo == null && _instance != null)
                    _instancePropertyInfo = _instance.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
                return _instancePropertyInfo;
            }
        }

        /// <summary>
        /// String Dictionary that contains the extra dynamic values
        /// stored on this object/instance
        /// </summary>        
        /// <remarks>Using PropertyBag to support XML Serialization of the dictionary</remarks>
        public IDictionary<string, object> Properties { get; } = new ConcurrentDictionary<string, object>();  

        /// <summary>
        /// This constructor just works off the internal dictionary and any 
        /// public properties of this object.
        /// 
        /// Note you can subclass Expando.
        /// </summary>
        public Expando()
        {
            Initialize(this);
        }

        /// <summary>
        /// Allows passing in an existing instance variable to 'extend'.        
        /// </summary>
        /// <remarks>
        /// You can pass in null here if you don't want to 
        /// check native properties and only check the Dictionary!
        /// </remarks>
        /// <param name="instance"></param>
        public Expando(object instance)
        {
            Initialize(instance);
        }

        /// <summary>
        /// Create an Expando from a dictionary
        /// </summary>
        /// <param name="dict"></param>
        public Expando(IDictionary<string, object> dict)
        {
            var expando = this;

            Initialize(expando);

            Properties = new ConcurrentDictionary<string, object>();

            foreach (var (key, kvpValue) in dict)
            {
                switch (kvpValue)
                {
                    case IDictionary<string, object> _:
                        var expandoVal = new Expando(kvpValue);
                        expando[key] = expandoVal;
                        break;

                    case ICollection collection:
                        var objList = new List<object>();
                        foreach (var item in collection)
                        {
                            if (item is IDictionary<string, object>)
                            {
                                var expandoItem = new Expando(item);
                                objList.Add(expandoItem);
                            }
                            else
                            {
                                objList.Add(item);
                            }
                        }
                        expando[key] = objList;
                        break;

                    default:
                        expando[key] = kvpValue;
                        break;
                }
            }
        }

        protected void Initialize(object instance)
        {
            _instance = instance;
            if (instance != null)
                _instanceType = instance.GetType();
        }


        /// <summary>
        /// Return both instance and dynamic names.
        /// 
        /// Important to return both so JSON serialization with 
        /// Json.NET works.
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return GetProperties(true).Select(prop => prop.Key);
        }

        /// <summary>
        /// Try to retrieve a member by name first from instance properties
        /// followed by the collection entries.
        /// </summary>
        /// <param name="binder"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = null;

            // first check the Properties collection for member
            if (Properties.Keys.Contains(binder.Name))
            {
                result = Properties[binder.Name];
                return true;
            }

            // Next check for Public properties via Reflection
            if (_instance != null)
            {
                try
                {
                    return GetProperty(_instance, binder.Name, out result);
                }
                catch
                {
                    // ignored
                }
            }

            // failed to retrieve a property
            return false;
        }


        /// <summary>
        /// Property setter implementation tries to retrieve value from instance 
        /// first then into this object
        /// </summary>
        /// <param name="binder"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            // first check to see if there's a native property to set
            try
            {
                if (SetProperty(binder.Name, value))
                    return true;
            }
            catch
            {
                return false;
            }

            // no match - set or add to dictionary
            Properties.AddOrUpdate(binder.Name, value, (k, v) => value);
            return true;
        }

        /// <summary>
        /// Dynamic invocation method. Currently allows only for Reflection based
        /// operation (no ability to add methods dynamically).
        /// </summary>
        /// <param name="binder"></param>
        /// <param name="args"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            result = null;
            if (_instance != null)
            {
                try
                {
                    // check instance passed in for methods to invoke
                    if (InvokeMethod(binder.Name, args, out result))
                        return true;
                }
                catch
                {
                    // ignored
                }
            }

            return false;
        }


        /// <summary>
        /// Reflection Helper method to retrieve a property
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="name"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        protected bool GetProperty(object instance, string name, out object result)
        {
            result = null;
            instance ??= this;

            var miArray = _instanceType.GetMember(name, BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.Instance);
            if (miArray.Length > 0)
            {
                var mi = miArray[0];
                if (mi.MemberType == MemberTypes.Property)
                {
                    result = ((PropertyInfo)mi).GetValue(instance, null);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Reflection helper method to set a property value
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected bool SetProperty(string name, object value)
        {
            var miArray = _instanceType.GetMember(name, BindingFlags.Public | BindingFlags.SetProperty | BindingFlags.Instance);
            if (miArray.Length > 0)
            {
                var mi = miArray[0];
                if (mi.MemberType == MemberTypes.Property)
                {
                    ((PropertyInfo)mi).SetValue(_instance, value, null);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Reflection helper method to invoke a method
        /// </summary>
        /// <param name="name"></param>
        /// <param name="args"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        protected bool InvokeMethod(string name, object[] args, out object result)
        {
            // Look at the instanceType
            var miArray = _instanceType.GetMember(name,
                                    BindingFlags.InvokeMethod |
                                    BindingFlags.Public | BindingFlags.Instance);

            if (miArray.Length > 0)
            {
                var mi = miArray[0] as MethodInfo;
                result = mi?.Invoke(_instance, args);
                return true;
            }

            result = null;
            return false;
        }

        /// <summary>
        /// Convenience method that provides a string Indexer 
        /// to the Properties collection AND the strongly typed
        /// properties of the object by name.
        /// 
        /// // dynamic
        /// exp["Address"] = "112 nowhere lane"; 
        /// // strong
        /// var name = exp["StronglyTypedProperty"] as string; 
        /// </summary>
        /// <remarks>
        /// The getter checks the Properties dictionary first
        /// then looks in PropertyInfo for properties.
        /// The setter checks the instance properties before
        /// checking the Properties dictionary.
        /// </remarks>
        /// <param name="key"></param>
        /// 
        /// <returns></returns>
        public object this[string key]
        {
            get
            {
                try
                {
                    // try to get from properties collection first
                    return Properties[key];
                }
                catch (KeyNotFoundException)
                {
                    // try reflection on instanceType
                    if (GetProperty(_instance, key, out var result))
                        return result;

                    // nope doesn't exist
                    throw;
                }
            }
            set
            {
                if (Properties.ContainsKey(key))
                {
                    Properties[key] = value;
                    return;
                }

                // check instance for existence of type first
                var miArray = _instanceType.GetMember(key, BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.Instance);
                if (miArray.Length > 0)
                    SetProperty(key, value);
                else
                    Properties[key] = value;
            }
        }


        /// <summary>
        /// Returns and the properties of 
        /// </summary>
        /// <param name="includeInstanceProperties"></param>
        /// <returns></returns>
        public IEnumerable<KeyValuePair<string, object>> GetProperties(bool includeInstanceProperties = false)
        {
            if (includeInstanceProperties && _instance != null)
            {
                foreach (var prop in this.InstancePropertyInfo)
                    yield return new KeyValuePair<string, object>(prop.Name, prop.GetValue(_instance, null));
            }

            foreach (var key in Properties.Keys)
                yield return new KeyValuePair<string, object>(key, this.Properties[key]);
        }

        /// <summary>
        /// Checks whether a property exists in the Property collection
        /// or as a property on the instance
        /// </summary>
        /// <param name="item"></param>
        /// <param name="includeInstanceProperties"></param>
        /// <returns></returns>
        public bool Contains(KeyValuePair<string, object> item, bool includeInstanceProperties = false)
        {
            var result = false;
            result |= Properties.ContainsKey(item.Key);

            if (!result && includeInstanceProperties && _instance != null)
            {
                result |= InstancePropertyInfo.Any(prop => prop.Name == item.Key);
            }

            return result;
        }
    }
}
