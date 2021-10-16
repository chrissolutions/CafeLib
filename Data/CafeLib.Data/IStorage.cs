using System;
using CafeLib.Core.Data;

namespace CafeLib.Data
{
    /// <summary>
    /// Data access layer storage interface.
    /// </summary>
    public interface IStorage : IDisposable
    {
        IRepository<T> CreateRepository<T>() where T : class, IEntity;

        IRepository<T> GetRepository<T>() where T : class, IEntity;

        void Close();
    }
}
