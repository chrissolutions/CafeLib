﻿#region Copyright
// Copyright (c) 2020 TonesNotes
// Distributed under the Open BSV software license, see the accompanying file LICENSE.
#endregion

using System;
using System.IO;
using CafeLib.Bitcoin.Persistence;
using Xunit;

namespace CafeLib.Bitcoin.UnitTests.Persist
{
    public class KzMemoryMappedFileTests : IClassFixture<TempFoldersClassFixture>
    {
        private readonly TempFoldersClassFixture _tempFolders;

        public KzMemoryMappedFileTests(TempFoldersClassFixture tempFolders)
        {
            _tempFolders = tempFolders;
        }

        [Fact]
        public void CreateAndCleanup()
        {
            var file = Path.Combine(_tempFolders.CreateRandomTempFolder(), "CreateAndCleanup.mmf");
            using (var mmf = new MemoryFile(file, 100))
            {
                Assert.Equal(100, mmf.FileLength);
                var spanAll = mmf.GetSpan();
                var span40Len10 = mmf.GetSpan(40, 10);
                var bytes = new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
                bytes.CopyTo(span40Len10);
                Assert.Equal(7, spanAll[47]);
                spanAll[^1] = 42;
                mmf.Flush();
            }
            using (var mmf = new MemoryFile(file)) 
            {
                //Assert.Equal(100, mmf.FileLength);
                var span40Len10 = mmf.GetSpan(40, 10);
                Assert.Equal(9, span40Len10[9]);
                var spanAll = mmf.GetSpan();
                spanAll[^1] = 43;
                mmf.Flush();
            }
        }
    }

    public class TempFoldersClassFixture : IDisposable
    {
        private readonly string _folder;

        public TempFoldersClassFixture()
        {
            _folder = Path.Combine(Directory.GetCurrentDirectory(), "TempFolders");
        }

        public string CreateRandomTempFolder()
        {
            var path = Path.Combine(_folder, Guid.NewGuid().ToString());
            Directory.CreateDirectory(path);
            return path;
        }

        public void Dispose()
        {
            Directory.Delete(_folder, true);
        }
    }

}
