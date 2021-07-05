﻿#region Copyright
// Copyright (c) 2020 TonesNotes
// Distributed under the Open BSV software license, see the accompanying file LICENSE.
#endregion

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using CafeLib.Core.Extensions;

namespace CafeLib.Bitcoin.Wallet
{
    public static class WordLists
    {
        public static readonly Lazy<IDictionary<Languages, string[]>> Cultures = new Lazy<IDictionary<Languages, string[]>>(() => new ConcurrentDictionary<Languages, string[]>());

        public static string[] GetWords(Languages language)
        {
            return Cultures.Value.GetOrAdd(language, () => LoadWords(language));
        }

        private static string[] LoadWords(Languages language)
        {
            var manifest = $"{typeof(WordLists).Namespace}.Cultures.{language.GetName()}.words";
            using var stream = typeof(WordLists).Assembly.GetManifestResourceStream(manifest);
            using var reader = new StreamReader(stream ?? throw new NotSupportedException(nameof(language)));
            var text = reader.ReadToEnd();
            return text.Split("\n", StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
