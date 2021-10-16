using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CafeLib.Data.Mapping
{
    internal class PropertyConverterMap : IEnumerable<PropertyConverter>
    {
        private readonly IDictionary<string, PropertyConverter> _propertyMap;

        public PropertyConverterMap(IEnumerable<PropertyConverter> converters)
        {
            _propertyMap = converters.ToDictionary(x => x.PropertyInfo.Name.ToLowerInvariant(), x => x);
        }

        public IEnumerator<PropertyConverter> GetEnumerator()
        {
            return _propertyMap.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public PropertyConverter this[string key]
        {
            get => _propertyMap.TryGetValue(key.ToLowerInvariant(), out var converter) ? converter : null;
            set => _propertyMap[key.ToLowerInvariant()] = value;
        }
    }
}