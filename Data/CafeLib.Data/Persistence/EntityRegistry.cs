using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using CafeLib.Core.Data;
using CafeLib.Core.Extensions;
using CafeLib.Core.IoC;
using CafeLib.Core.Support;
using CafeLib.Data.Mapping;

namespace CafeLib.Data.Persistence
{
    internal class EntityRegistry : IDisposable
    {
        #region Member Variables

        private readonly ServiceRegistry _container;
        private bool _disposed;

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public EntityRegistry(IStorage storage)
        {
            // Create registry.
            _container = (ServiceRegistry)IocFactory.CreateRegistry();

            // Add repositories to registry.
            InitRepositories((StorageBase)storage);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Add repository to registry.
        /// </summary>
        /// <typeparam name="TEntity">IEntity type</typeparam>
        /// <param name="repository">repository</param>
        //public T Add<T, TU>(T repository) where T : class, IRepository<TU> where TU : class, IEntity
        public IRepository<TEntity> Add<TEntity>(IRepository<TEntity> repository) where TEntity : class, IEntity
        {
            if (repository == null) throw new ArgumentNullException(nameof(repository));
            _container.AddSingleton(_ => repository);
            return repository;
        }

        /// <summary>
        /// Return the repository for the type.
        /// </summary>
        /// <typeparam name="T">entity type</typeparam>
        /// <returns>the repository or null</returns>
        public IRepository<T> Find<T>() where T : class, IEntity
        {
            return _container.TryResolve(typeof(IRepository<T>), out var repository)
                ? (IRepository<T>) repository
                : throw new KeyNotFoundException(typeof(T).Name);
        }

        /// <summary>
        /// Dispose.
        /// </summary>
        public void Dispose()
        {
            Dispose(!_disposed);
            _disposed = true;
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose entity registry.
        /// </summary>
        /// <param name="disposing">disposing flag</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposing) return;
            _container?.Dispose();
            _disposed = true;
        }

        #endregion

        #region Helpers

        private void InitRepositories(IStorage storage)
        {
            var registerMethod = _container.GetType()
                .GetMethods()
                .First(x => x.Name == "AddSingleton" && x.IsGenericMethod && x.GetParameters().Length == 1);

            ((StorageBase)storage).ConnectionInfo.Domain.GetEntityTypes().ForEach(x =>
            {
                Type repoType;
                object repo;

                if (typeof(IMappedEntity).IsAssignableFrom(x))
                {
                    repoType = typeof(MappedRepository<,>).MakeGenericType(x.Seek(typeof(MappedEntity<,>)).GenericTypeArguments);
                    repo = Creator.CreateInstance(repoType, storage);
                }
                else
                {
                    repoType = typeof(Repository<>).MakeGenericType(x ?? throw new ArgumentNullException(nameof(x)));
                    repo = Creator.CreateInstance(repoType, storage);
                }

                var repoInterface = typeof(IRepository<>).MakeGenericType(x);
                var method = registerMethod.MakeGenericMethod(repoInterface);
                method.Invoke(_container, new[] { BuildFactory(repoInterface, repo) });
            });
        }

        private static object BuildFactory(Type repoInterface, object repo)
        {
            var funcType = typeof(Func<,>).MakeGenericType(typeof(IServiceResolver), repoInterface);
            var parameter = Expression.Parameter(typeof(IServiceResolver));
            var body = Expression.Constant(repo);
            return Expression.Lambda(funcType, body, parameter).Compile();
        }

        #endregion
    }
}
