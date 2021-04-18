﻿#region Copyright
// Copyright (c) 2020 TonesNotes
// Distributed under the Open BSV software license, see the accompanying file LICENSE.
#endregion

using System.Collections.Generic;
using CafeLib.Bitcoin.Crypto;
using CafeLib.Bitcoin.Extensions;
using CafeLib.Bitcoin.Keys;
using Xunit;

namespace CafeLib.Bitcoin.UnitTests.Keys
{
    public class MnemonicTests
    {
        [Fact]
        public void RecoverLastWord() {
            var valid = new List<string>();
            var words = "sword victory much blossom cradle sense boy float soda render arrive";
            foreach (var word in Mnemonic.WordLists[Mnemonic.Languages.English]) {
                var t = $"{words} {word}";
                if (Mnemonic.IsValid(t)) {
                    valid.Add(t);
                }
            }
            Assert.Equal(128, valid.Count);
        }

        [Fact]
        public void ElectrumStandardMnemonic() 
        {
            var words = "sword victory much blossom cradle sense boy float soda render arrive arrive";
            var h = Hashes.HmacSha512("Seed version".Utf8ToBytes(), words.Utf8NormalizedToBytes());
            var hb = h.Span;
        }

        [Fact]
        public void Base6AndBase10()
        {
            //var e = new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };
            var e = new byte[] { 0, 1 };
            var s10 = Mnemonic.ToDigitsBase10(e);
            var s6 = Mnemonic.ToDigitsBase6(e);
            var bn10 = Mnemonic.Base10ToBigInteger(s10);
            var bn6 = Mnemonic.Base6ToBigInteger(s6);
        }

        [Fact]
        public void WordListsComplete()
        {
            Assert.True(Mnemonic.WordLists[Mnemonic.Languages.English].Length == 2048);
            Assert.True(Mnemonic.WordLists[Mnemonic.Languages.English][0] == "abandon");
            Assert.True(Mnemonic.WordLists[Mnemonic.Languages.Spanish].Length == 2048);
            Assert.True(Mnemonic.WordLists[Mnemonic.Languages.Spanish][0] == "ábaco");
            Assert.True(Mnemonic.WordLists[Mnemonic.Languages.French].Length == 2048);
            Assert.True(Mnemonic.WordLists[Mnemonic.Languages.French][0] == "abaisser");
            Assert.True(Mnemonic.WordLists[Mnemonic.Languages.Italian].Length == 2048);
            Assert.True(Mnemonic.WordLists[Mnemonic.Languages.Italian][0] == "abaco");
            Assert.True(Mnemonic.WordLists[Mnemonic.Languages.Japanese].Length == 2048);
            Assert.True(Mnemonic.WordLists[Mnemonic.Languages.Japanese][0] == "あいこくしん");
            Assert.True(Mnemonic.WordLists[Mnemonic.Languages.PortugueseBrazil].Length == 2048);
            Assert.True(Mnemonic.WordLists[Mnemonic.Languages.PortugueseBrazil][0] == "abacate");
            Assert.True(Mnemonic.WordLists[Mnemonic.Languages.ChineseSimplified].Length == 2048);
            Assert.True(Mnemonic.WordLists[Mnemonic.Languages.ChineseSimplified][0] == "的");
            Assert.True(Mnemonic.WordLists[Mnemonic.Languages.ChineseTraditional].Length == 2048);
            Assert.True(Mnemonic.WordLists[Mnemonic.Languages.ChineseTraditional][0] == "的");
        }

        [Fact]
        public void IsValid()
        {
            Assert.True(Mnemonic.IsValid("afirmar diseño hielo fideo etapa ogro cambio fideo toalla pomelo número buscar"));

            Assert.False(Mnemonic.IsValid("afirmar diseño hielo fideo etapa ogro cambio fideo hielo pomelo número buscar"));

            Assert.False(Mnemonic.IsValid("afirmar diseño hielo fideo etapa ogro cambio fideo hielo pomelo número oneInvalidWord"));

            Assert.False(Mnemonic.IsValid("totally invalid phrase"));

            Assert.True(Mnemonic.IsValid("caution opprimer époque belote devenir ficeler filleul caneton apologie nectar frapper fouiller"));
        }

        [Fact]
        public void Constructors()
        {
            var words = "afirmar diseño hielo fideo etapa ogro cambio fideo toalla pomelo número buscar";
            var m1 = new Mnemonic(words);
            Assert.Equal(Mnemonic.Languages.Spanish, m1.Language);
            Assert.Equal(m1.Words, Mnemonic.FromWords(words).Words);

            var m2 = new Mnemonic(m1.Entropy, m1.Language);
            Assert.Equal(m1.Words, m2.Words);
            Assert.Equal(m2.Words, Mnemonic.FromEntropy(m1.Entropy, m1.Language).Words);

            var m3 = new Mnemonic(new byte[] { 5, 40, 161, 175, 172, 69, 19, 67, 74, 26, 196, 233, 87, 10, 119, 18 }, Mnemonic.Languages.Spanish);
            Assert.Equal(m1.Words, m3.Words);

            var m4 = new Mnemonic(length:256);
            Assert.Equal(24, m4.Words.Split(' ').Length);
            Assert.Equal(24, Mnemonic.FromLength(256).Words.Split(' ').Length);

        }

        [Fact]
        public void WordListLength()
        {
            Assert.Equal(12, new Mnemonic(32 * 4).Words.Split(' ').Length);
            Assert.Equal(15, new Mnemonic(32 * 5).Words.Split(' ').Length);
            Assert.Equal(18, new Mnemonic(32 * 6).Words.Split(' ').Length);
            Assert.Equal(21, new Mnemonic(32 * 7).Words.Split(' ').Length);
            Assert.Equal(24, new Mnemonic(32 * 8).Words.Split(' ').Length);
        }

        [Fact]
        public void ToStringIsWords()
        {
            var m = new Mnemonic();
            Assert.Equal(m.Words, m.ToString());
        }

        [Fact]
        public void FromBase6()
        {
            var rolls1 = "10000000000000000000000000000000000000000000000002";
            var m1 = Mnemonic.FromBase6(rolls1);
            Assert.Equal("acoustic abandon abandon abandon anchor cancel pole advance naive alpha noodle slogan", m1.Words);

            var rolls2 = "20433310335200331223501035145525323501554453150402";
            var m2 = Mnemonic.FromBase6(rolls2);
            Assert.Equal("little jar barrel spatial tenant business manual cabin pig nerve trophy purity", m2.Words);

            var rolls3 = "2043331033520033122350103533025405142024330443100234401130333301433333523345145525323501554453150402";
            var m3 = Mnemonic.FromBase6(rolls3, 256);
            Assert.Equal("little jar crew spice goat sell journey behind used choose eyebrow property audit firm later blind invite fork camp shock floor reduce submit bronze", m3.Words);
        }
    }
}
