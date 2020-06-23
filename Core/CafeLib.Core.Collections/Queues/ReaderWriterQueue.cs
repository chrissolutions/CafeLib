using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
// ReSharper disable UnusedMember.Global

namespace CafeLib.Core.Collections.Queues
{
    /// <summary>
    /// ReaderWriterQueue.
    /// </summary>
    public class ReaderWriterQueue<T> : IQueue<T>, IDisposable
    {
        #region Private Members

        private SemaphoreSlim _producer;
        private SemaphoreSlim _consumer;
        private ConcurrentQueue<T> _queue;
        private bool _disposed;

        #endregion

        #region Automatic Properties

        /// <summary>
        /// Count of requests in queue.
        /// </summary>
        public int Count => _queue.Count;

        #endregion

        #region Constructors

        /// <summary>
        /// QueueCore constructor.
        /// </summary>
        public ReaderWriterQueue()
        {
            Clear();
        }

        /// <summary>
        /// QueueCore finalizer.
        /// </summary>
        ~ReaderWriterQueue()
        {
            Dispose(false);
        }

        #endregion

        #region Methods
		
        /// <summary>
        /// Clears the queue.
        /// </summary>
        public void Clear()
        {
            Dispose(true);
            _producer = new SemaphoreSlim(0, int.MaxValue);
            _consumer = new SemaphoreSlim(int.MaxValue, int.MaxValue);
            _queue = new ConcurrentQueue<T>();
        }

        /// <summary>
        /// Enqueue a request context and release the semaphore that
        /// a thread is waiting on.
        /// </summary>
        public void Enqueue(T item)
        {
            _queue.Enqueue(item);
            _consumer.Wait();
            _producer.Release();
        }

        /// <summary>
        /// Dequeue a request.
        /// </summary>
        public T Dequeue()
        {
            _producer.Wait();
            _consumer.Release();
            _queue.TryDequeue(out var item);
            return item;
        }

        public bool TryDequeue(out T result)
        {
            _producer.Wait();
            _consumer.Release();
            return _queue.TryDequeue(out result);
        }

        public bool TryPeek(out T result)
        {
            return _queue.TryPeek(out result);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _queue.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Disposes the concurrent queue.
        /// </summary>
        public void Dispose()
        {
            Dispose(!_disposed);
            _disposed = true;
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose concurrent queue.
        /// </summary>
        /// <param name="disposing">indicate whether the queue is disposing</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposing) return;
            try
            {
                _producer?.Dispose();
                _consumer?.Dispose();
            }
            finally
            {
                _producer = null;
                _consumer = null;
            }
        }

        #endregion
    }
}
