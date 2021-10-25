using System;
using System.Collections.Generic;
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

        /// <summary>
        /// LineReader constructor
        /// </summary>
        /// <param name="filePath">path to file</param>
        /// <exception cref="ArgumentNullException">argument null exception</exception>
        public LineReader(string filePath)
        {
            FilePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
            _streamReader = File.OpenText(FilePath);
            SeekOrigin();
        }

        /// <summary>
        /// Read a line from reader.
        /// </summary>
        /// <returns>read line or null if at end of stream</returns>
        public async Task<string> ReadLineAsync()
        {
            if (EndOfStream) return null;
            var line = await _streamReader.ReadLineAsync().ConfigureAwait(false);
            ++CurrentLine;
            return line;
        }

        /// <summary>
        /// Read all lines
        /// </summary>
        /// <param name="filePath">path to file</param>
        /// <returns>readonly list of strings</returns>
        /// <exception cref="FileNotFoundException">file not found exception</exception>
        public static Task<IReadOnlyList<string>> ReadAllLinesAsync(string filePath)
        {
            if (!File.Exists(filePath)) throw new FileNotFoundException(nameof(filePath));
            using var reader = new LineReader(filePath);
            return reader.ReadAllLinesAsync();
        }

        /// <summary>
        /// Read all lines.
        /// </summary>
        /// <returns>Readonly list of string</returns>
        public async Task<IReadOnlyList<string>> ReadAllLinesAsync()
        {
            if (EndOfStream) return null;

            List<string> lines = new();
            string line;
            while ((line = await ReadLineAsync().ConfigureAwait(false)) != null)
            {
                lines.Add(line);
            }

            CurrentLine = lines.Count;
            return lines;
        }

        /// <summary>
        /// Position to the origin of the reader.
        /// </summary>
        public void SeekOrigin()
        {
            CurrentLine = 0;
            _streamReader.DiscardBufferedData();
            _streamReader.BaseStream.Seek(0, System.IO.SeekOrigin.Begin);
        }

        /// <summary>
        /// Position to a particular line.
        /// </summary>
        /// <param name="lineIndex">line index position</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">argument exception</exception>
        public async Task SeekLine(int lineIndex)
        {
            // Verify argument.
            lineIndex = lineIndex >= 0 ? lineIndex : throw new ArgumentException(nameof(lineIndex));
            var target = 0;

            switch (lineIndex)
            {
                case var _ when lineIndex == CurrentLine:
                    return;

                case var _ when lineIndex < CurrentLine:
                    SeekOrigin();
                    target = lineIndex;
                    break;

                case var _ when lineIndex > CurrentLine:
                    target = lineIndex;
                    break;
            }

            var line = "";
            while (CurrentLine < target && line != null)
            {
                line = await ReadLineAsync().ConfigureAwait(false);
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
