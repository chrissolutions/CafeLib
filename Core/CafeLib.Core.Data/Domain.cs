using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using CafeLib.Core.Data.Caches;
using CafeLib.Core.Extensions;
// ReSharper disable UnusedMember.Global

namespace CafeLib.Core.Data
{
    public abstract class Domain
    {
        private readonly ConcurrentDictionary<Type, Domain> _domainCache = new ConcurrentDictionary<Type, Domain>();

        #region Automatic Properties

        public PropertyCache PropertyCache { get; }
        public TableCache TableCache { get; }

        #endregion

        #region Constructors

        protected Domain()
        {
            InitEntityTypes().ForEach(x => _domainCache.AddOrUpdate(x, this, (k, v) => this));
            TableCache = new TableCache(this);
            PropertyCache = new PropertyCache(this);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get the context entity model types.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Type> GetEntityTypes() => _domainCache.Keys;

        /// <summary>
        /// Find the context entities model types.
        /// </summary>
        /// <param name="filter">filter applied to the entity types</param>
        /// <returns></returns>
        protected virtual IEnumerable<Type> FindEntityTypes(Func<Type, bool> filter)
        {
            return GetType().Assembly
                .GetTypes()
                .Where(filter)
                .ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        internal Domain GetDomain<T>() where T : IEntity => GetDomain(typeof(T));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        internal Domain GetDomain(Type type) => _domainCache[type];

        #endregion

        #region Helpers

        /// <summary>
        /// Init context entity model types.
        /// </summary>
        /// <returns></returns>
        private IEnumerable<Type> InitEntityTypes()
        {
            return FindEntityTypes(x => x != typeof(IEntity)
                                        && typeof(IEntity).IsAssignableFrom(x));
        }

        #endregion 
    }
}