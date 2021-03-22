using System;
using System.IO;
using System.Threading.Tasks;
// ReSharper disable UnusedMember.Global

namespace CafeLib.Core.FileIO
{
    public class LineReader : IDisposable
    {
        private StreamReader _streamReader;
        private bool _disposed;

        public int CurrentLine { get; private set; }

        public bool EndOfStream => _streamReader.EndOfStream;

        public string FilePath { get; }

        public LineReader(string filePath)
        {
            FilePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
            _streamReader = File.OpenText(FilePath);
            SeekOrigin();
        }

        public async Task<string> ReadLineAsync()
        {
            if (!EndOfStream)
            {
                var line = await _streamReader.ReadLineAsync().ConfigureAwait(false);
                ++CurrentLine;
                return line;
            }
            else
            {
                return null;
            }
        }

        public void SeekOrigin()
        {
            CurrentLine = 0;
            _streamReader.DiscardBufferedData();
            _streamReader.BaseStream.Seek(0, System.IO.SeekOrigin.Begin);
        }

        public async Task SeekLine(int lineIndex)
        {
            // Verify argument.
            lineIndex = lineIndex >= 0 ? lineIndex : throw new ArgumentException(nameof(lineIndex));
            var target = 0;
            var index = 0;

            switch (lineIndex)
            {
                case var _ when lineIndex == CurrentLine:
                    return;

                case var _ when lineIndex < CurrentLine:
                    SeekOrigin();
                    index = 0;
                    target = lineIndex;
                    break;

                case var _ when lineIndex > CurrentLine:
                    index = CurrentLine;
                    target = lineIndex;
                    break;
            }

            while (index < target && !_streamReader.EndOfStream)
            {
                var _ = await ReadLineAsync();
            }
        }

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
                _streamReader?.Dispose();
            }
            finally
            {
                _streamReader = null;
            }
        }

        #endregion
    }
}
