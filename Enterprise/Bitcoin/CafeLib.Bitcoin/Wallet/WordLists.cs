#region Copyright
// Copyright (c) 2020 TonesNotes
// Distributed under the Open BSV software license, see the accompanying file LICENSE.
#endregion

using System;
using System.Collections.Generic;
using System.IO;
using CafeLib.Bitcoin.Extensions;

namespace CafeLib.Bitcoin.Wallet
{
    public static class WordLists
    {
        private static readonly IDictionary<Languages, string[]> CultureMap = new Dictionary<Languages, string[]>();

        public static string[] GetWords(Languages language)
        {
            if (CultureMap.TryGetValue(language, out var words)) return words;
            var manifest = $"{typeof(WordLists).Namespace}.Cultures.{language.GetName()}.words";
            using var stream = typeof(WordLists).Assembly.GetManifestResourceStream(manifest);
            using var reader = new StreamReader(stream ?? throw new NotSupportedException(nameof(language)));
            var text = reader.ReadToEnd();
            words = text.Split("\n");
            CultureMap.Add(language, words);
            return words;
        }
    }
}
