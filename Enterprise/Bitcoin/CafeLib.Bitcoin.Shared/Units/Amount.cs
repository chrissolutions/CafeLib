#region Copyright
// Copyright (c) 2020 TonesNotes
// Distributed under the Open BSV software license, see the accompanying file LICENSE.
#endregion

using System;
using System.Diagnostics.CodeAnalysis;

namespace CafeLib.Bitcoin.Shared.Units {
    public struct Amount : IComparable<Amount>, IComparable
    {
        public static IAmountExchangeRate ExchangeRate { get; set; }

        /// <summary>
        /// long.MaxValue is 9_223_372_036_854_775_807
        /// max satoshis         2_100_000_000_000_000  (2.1 gigamegs :-)
        /// </summary>
        long _satoshis;

        public static Amount Zero = new Amount(0L);
        /// <summary>
        /// This is a value slightly higher than the maximum number of satoshis that will ever be in circulation: 21 million coins, 2.1 gigameg satoshis.
        /// 2_100_000_000_000_000
        /// </summary>
        public static Amount MaxValue = new Amount(2_100_000_000_000_000);
        /// <summary>
        /// This is the negated value slightly higher than the maximum number of satoshis that will ever be in circulation: -21 million coins, -2.1 gigameg satoshis.
        /// -2_100_000_000_000_000
        /// </summary>
        public static Amount MinValue = new Amount(-2_100_000_000_000_000);

        public long Satoshis { get => _satoshis; private set => _satoshis = value; }

        public Amount(long satoshis) => _satoshis = satoshis;

        public Amount(ulong satoshis) { checked { _satoshis = (long)satoshis; } }

        public static bool TryParse(string text, BitcoinUnit unit, out Amount amount)
        {
            amount = default;
            if (!decimal.TryParse(text.Replace("_", ""), out var value)) return false;
            amount = new Amount(value, unit);
            return true;
        }

        /// <summary>
        /// decimal has 28-29 significant digts with a exponent range to shift that either
        /// all to the left of the decimal or to the right.
        /// </summary>
        /// <param name="amount"></param>
        /// <param name="unit"></param>
        public Amount(decimal amount, BitcoinUnit unit)
        {
            checked
            {
                _satoshis = (long)(amount * (long)unit);
            }
        }

        public Amount(long amount, BitcoinUnit unit)
        {
            checked { _satoshis = amount * (long)unit; }
        }

        public Amount(ulong amount, BitcoinUnit unit)
        {
            checked { _satoshis = (long)amount * (long)unit; }
        }

        public static implicit operator Amount(long value) => new Amount(value);
        public static implicit operator Amount(ulong value) => new Amount(checked((long)value));
        public static implicit operator long(Amount value) => value.Satoshis;
        public static implicit operator ulong(Amount value) => checked((ulong)value.Satoshis);

        public override string ToString() => ToString(@group: true, units: false, unit: BitcoinUnit.mBSV);

        public decimal ToBitcoin() => (decimal)Satoshis / (long)(BitcoinUnit.BSV);

        public string ToString(bool group, bool units, BitcoinUnit unit = BitcoinUnit.mBSV)
        {
            // Satoshis
            // 2_100_000_000_000_000
            // mBSV
            // 21_000_000_000.000_00
            // BSV
            // 21_000_000.000_000_00
            var s = _satoshis;
            var m = false;
            if (s < 0) {
                m = true;
                s = -s;
            }
            var f = s % (long)unit;
            var i = s / (long)unit;
            var r = unit switch {
             BitcoinUnit.BSV     => $"{(m ? "-" : " ")}{i:#,0}.{f:000_000_00}",
             BitcoinUnit.mBSV    => $"{(m ? "-" : " ")}{i:#,0}.{f:000_00}",
             BitcoinUnit.Bit     => $"{(m ? "-" : " ")}{i:#,0}.{f:00}",
             BitcoinUnit.Satoshi => $"{(m ? "-" : " ")}{i:#,0}",
             _ => throw new NotImplementedException()
            };
            r = r.Replace(',', '_');
            if (!group) r = r.Replace("_", "");
            if (units) r = r += $" {unit}";
            return r;
        }

        public static string ToString(long value) => new Amount(value).ToString();

        public override int GetHashCode() => _satoshis.GetHashCode();
        public override bool Equals(object obj) => obj is Amount amount && this == amount;
        public bool Equals(Amount o) => _satoshis == o._satoshis;
        public static bool operator ==(Amount x, Amount y) => x.Equals(y);
        public static bool operator !=(Amount x, Amount y) => !(x == y);

        public static bool operator >(Amount x, Amount y) => x.CompareTo(y) > 0;
        public static bool operator <(Amount x, Amount y) => x.CompareTo(y) < 0;

        public static bool operator >=(Amount x, Amount y) => x.CompareTo(y) >= 0;
        public static bool operator <=(Amount x, Amount y) => x.CompareTo(y) <= 0;

        public int CompareTo([AllowNull] Amount other) => Satoshis.CompareTo(other.Satoshis);

        public int CompareTo(object obj) {
            return obj switch
            {
                Amount a => CompareTo(a),
                long l => Satoshis.CompareTo(l),
                ulong ul => Satoshis.CompareTo(ul),
                int i => Satoshis.CompareTo(i),
                uint ui => Satoshis.CompareTo(ui),
                _ => throw new NotImplementedException()
            };
        }

        //public static Amount operator -(Amount a, Amount b) => a + -b;
        //public static Amount operator +(Amount a, long b) => a + new Amount(b);
        //public static Amount operator +(Amount a, Amount b) => (a.Satoshis + b.Satoshis).ToAmount();
        //public static Amount operator -(Amount a) => (-a.Satoshis).ToAmount();
    }
}
