using System;
using Microsoft.Extensions.Logging;
using Xunit;

namespace CafeLib.Core.UnitTests.Services
{
    public interface IDisposableService
    {
        void DoThing(int number);

        bool IsDisposed { get; }
    }

    public class DisposableService : IDisposableService, IDisposable
    {
        private readonly ILogger _logger;
        private int _disposed;

        public DisposableService(ILogger logger)
        {
            _logger = logger;
        }

        public bool IsDisposed => _disposed > 0;

        public void DoThing(int number)
        {
            _logger.LogInformation($"Doing the thing {number}");
        }

        public void Dispose()
        {
            Dispose(!IsDisposed);
            _disposed++;
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose service provider
        /// </summary>
        /// <param name="disposing"></param>
        protected void Dispose(bool disposing)
        {
            if (!disposing) return;
            Assert.True(!IsDisposed);
        }
    }
}