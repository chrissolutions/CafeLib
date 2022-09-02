using CafeLib.Core.Numerics;
using System.Linq;
using CafeLib.Core.Buffers;
using CafeLib.Core.Encodings;
using Xunit;
using System;

namespace CafeLib.Core.UnitTests
{
    public class NumericsTests
    {
        private static readonly HexEncoder Hex = new();
        private static readonly HexReverseEncoder HexReverse = new();

        #region UInt160 Tests

        [Fact]
        public void UInt160_Zero_Test()
        {
            UInt160 new160 = new();
            Assert.Equal(UInt160.Zero, new160);
        }

        [Theory]
        [InlineData("c2eaba3b9c29575322c6e24fdc1b49bdfe405bad")]
        [InlineData("0xc2eaba3b9c29575322c6e24fdc1b49bdfe405bad")]
        public void UInt160_HexByteOrder_Test(string hex)
        {
            var lowOrderedBytes = Hex.Decode(hex);
            var highOrderedBytes = lowOrderedBytes.Reverse().ToArray();

            var uint160 = UInt160.FromHex(hex);
            var uint160Reverse = UInt160.FromHex(hex, true);
            Assert.Equal(uint160.ToArray(), highOrderedBytes);
            Assert.Equal(uint160Reverse.ToArray(), lowOrderedBytes);

            Assert.Equal(hex, hex.StartsWith("0x") ? $"0x{uint160}" : $"{uint160}");
            Assert.Equal(hex, hex.StartsWith("0x") ? $"0x{uint160Reverse.ToHex(true)}" : $"{uint160Reverse.ToHex(true)}");
        }

        [Fact]
        public void UInt160_ByteAccess_Test()
        {
            var i = new UInt160
            {
                [0] = 0x21,
                [UInt160.Length-1] = 0xfe
            };
            var str = i.ToString();
            Assert.Equal("fe00000000000000000000000000000000000021", str);
        }

        [Theory]
        [InlineData("c2eaba3b9c29575322c6e24fdc1b49bdfe405bad")]
        public void UInt160_Create_From_ReadonlyByteSpan(string hex)
        {
            var bytes = Hex.Decode(hex);
            var readonlyByteSpan = new ReadOnlyByteSpan(bytes);

            UInt160 uint160 = new(readonlyByteSpan);
            Assert.Equal(bytes, uint160.ToArray());
            Assert.Equal(bytes, uint160[..UInt160.Length]);
        }

        [Theory]
        [InlineData("c2eaba3b9c29575322c6e24fdc1b49bdfe405bad")]
        public void UInt160_ByteSpan_Test(string hex)
        {
            var bytes = Hex.Decode(hex);
            var readonlyByteSpan = new ReadOnlyByteSpan(bytes);

            var uint160 = new UInt160(readonlyByteSpan);
            var byteSpan = uint160.Span;

            Assert.Equal(bytes, uint160.ToArray());
            Assert.Equal(bytes, byteSpan.ToArray());
        }

        [Theory]
        [InlineData("c2eaba3b9c29575322c6e24fdc1b49bdfe405bad")]
        [InlineData("0xc2eaba3b9c29575322c6e24fdc1b49bdfe405bad")]
        public void UInt160_FromHex_Test(string hex)
        {
            var uint160 = UInt160.FromHex(hex);
            var uint160Reverse = UInt160.FromHex(hex, true);
            var hexReverse = HexReverse.Encode(Hex.Decode(hex));
            var hexReverseExpected = hex.StartsWith("0x") ? $"0x{hexReverse}" : $"{hexReverse}";

            Assert.Equal(hex, hex.StartsWith("0x") ? $"0x{uint160}" : $"{uint160}");
            Assert.Equal(hexReverseExpected, hex.StartsWith("0x") ? $"0x{uint160Reverse}" : $"{uint160Reverse}");
        }

        [Theory]
        [InlineData("c2eaba3b9c29575322c6e24fdc1b49bdfe405bad")]
        [InlineData("0xc2eaba3b9c29575322c6e24fdc1b49bdfe405bad")]
        public void UInt160_ToHex_Test(string hex)
        {
            var uint160 = UInt160.FromHex(hex);

            Assert.Equal(hex, hex.StartsWith("0x") ? $"0x{uint160.ToHex(false)}" : $"{uint160.ToHex(false)}");
            var hexReverse = HexReverse.Encode(Hex.Decode(hex));
            var hexReverseExpected = hex.StartsWith("0x") ? $"0x{hexReverse}" : $"{hexReverse}";
            Assert.Equal(hexReverseExpected, hex.StartsWith("0x") ? $"0x{uint160.ToHex(true)}" : $"{uint160.ToHex(true)}");
        }

        [Theory]
        [InlineData("c2eaba3b9c29575322c6e24fdc1b49bdfe405bad")]
        [InlineData("0xc2eaba3b9c29575322c6e24fdc1b49bdfe405bad")]
        public void UInt160_ToHex_Reverse_Test(string hex)
        {
            var uint160 = UInt160.FromHex(hex, true);

            Assert.Equal(hex, hex.StartsWith("0x") ? $"0x{uint160.ToHex(true)}" : $"{uint160.ToHex(true)}");
            var hexReverse = HexReverse.Encode(Hex.Decode(hex));
            var hexReverseExpected = hex.StartsWith("0x") ? $"0x{hexReverse}" : $"{hexReverse}";
            Assert.Equal(hexReverseExpected, hex.StartsWith("0x") ? $"0x{uint160.ToHex(false)}" : $"{uint160.ToHex(false)}");
        }

        #endregion

        #region UInt256 Tests

        [Fact]
        public void UInt256_ByteAccess_Test()
        {
            var i = new UInt256
            {
                [0] = 0x21,
                [UInt256.Length-1] = 0xfe
            };
            var str = i.ToString();
            Assert.Equal("fe00000000000000000000000000000000000000000000000000000000000021", str);
        }

        [Theory]
        [InlineData("0xfedcba9876543210fedcba9876543210fedcba9876543210fedcba9876543210")]
        public void UInt256_Create_From_ReadonlyByteSpan(string hex)
        {
            var bytes = Hex.Decode(hex);
            var readonlyByteSpan = new ReadOnlyByteSpan(bytes);

            var uint256 = new UInt256(readonlyByteSpan);
            var new256 = new UInt256();
            uint256.CopyTo(new256.Span);

            Assert.Equal(bytes, uint256.ToArray());
            Assert.Equal(bytes, new256.ToArray());
            Assert.Equal(bytes, new256[..UInt256.Length]);
        }

        [Fact]
        public void UInt256_Create_From_UInt256()
        {
            var fbf = new byte[] { 0xfe, 0xdc, 0xba, 0x98, 0x76, 0x54, 0x32, 0x10, 0xfe, 0xdc, 0xba, 0x98, 0x76, 0x54, 0x32, 0x10, 0xfe, 0xdc, 0xba, 0x98, 0x76, 0x54, 0x32, 0x10, 0xfe, 0xdc, 0xba, 0x98, 0x76, 0x54, 0x32, 0x10 };
            var readonlyByteSpan = new ReadOnlyByteSpan(fbf);

            var uint256 = new UInt256(readonlyByteSpan);
            var new256 = new UInt256(uint256);

            Assert.Equal(fbf, uint256.ToArray());
            Assert.Equal(fbf, new256.ToArray());
        }

        [Fact]
        public void UInt256_ByteSpan_Test()
        {
            var fbf = new byte[] { 0xfe, 0xdc, 0xba, 0x98, 0x76, 0x54, 0x32, 0x10, 0xfe, 0xdc, 0xba, 0x98, 0x76, 0x54, 0x32, 0x10, 0xfe, 0xdc, 0xba, 0x98, 0x76, 0x54, 0x32, 0x10, 0xfe, 0xdc, 0xba, 0x98, 0x76, 0x54, 0x32, 0x10 };
            var readonlyByteSpan = new ReadOnlyByteSpan(fbf);

            var uint256 = new UInt256(readonlyByteSpan);
            var byteSpan = uint256.Span;

            Assert.Equal(fbf, uint256.ToArray());
            Assert.Equal(fbf, byteSpan.ToArray());
        }

        [Theory]
        [InlineData("fedcba9876543210fedcba9876543210fedcba9876543210fedcba9876543210")]
        public void UInt256_HexByteOrder_Test(string hex)
        {
            var lowOrderedBytes = Hex.Decode(hex);
            var highOrderedBytes = lowOrderedBytes.Reverse().ToArray();

            var uint256 = UInt256.FromHex(hex);
            var uint256Reverse = UInt256.FromHex(hex, true);
            Assert.Equal(uint256.ToArray(), highOrderedBytes);
            Assert.Equal(uint256Reverse.ToArray(), lowOrderedBytes);

            Assert.Equal(hex, uint256.ToString());
            Assert.Equal(hex, uint256Reverse.ToHex(true));
        }

        [Fact]
        public void UInt256_LeftShift_Test()
        {
            var a = new UInt256(1);
            Assert.Equal(new UInt256(1, 0, 0, 0), a);
            Assert.Equal(new UInt256(0, 1, 0, 0), a << 64);
            Assert.Equal(new UInt256(0, 4, 0, 0), a << 66);
            Assert.Equal(new UInt256(0, 0, 1, 0), a << (64 + 64));
            Assert.Equal(new UInt256(0, 0, 4, 0), a << (66 + 64));
            Assert.Equal(new UInt256(0, 0, 0, 1), a << (64 + 64 + 64));
            Assert.Equal(new UInt256(0, 0, 0, 4), a << (66 + 64 + 64));
        }

        [Theory]
        [InlineData("988119d6cca702beb1748f4eb497e316467f69580ffa125aa8bcb6fb63dce237")]
        [InlineData("fedcba9876543210fedcba9876543210fedcba9876543210fedcba9876543210")]
        [InlineData("0x000000000019d6689c085ae165831e934ff763ae46a2a6c172b3f1b60a8ce26f")]
        [InlineData("0x4a5e1e4baab89f3a32518a88c31bc87f618f76673e2cc77ab2127b7afdeda33b")]
        public void UInt256_FromHex_Test(string hex)
        {
            var uint256 = UInt256.FromHex(hex);
            var uint256Reverse = UInt256.FromHex(hex, true);
            var hexReverse = HexReverse.Encode(Hex.Decode(hex));
            var hexReverseExpected = hex.StartsWith("0x") ? $"0x{hexReverse}" : $"{hexReverse}";

            Assert.Equal(hex, hex.StartsWith("0x") ? $"0x{uint256}" : $"{uint256}");
            Assert.Equal(hexReverseExpected, hex.StartsWith("0x") ? $"0x{uint256Reverse}" : $"{uint256Reverse}");
        }

        [Theory]
        [InlineData("988119d6cca702beb1748f4eb497e316467f69580ffa125aa8bcb6fb63dce237")]
        [InlineData("fedcba9876543210fedcba9876543210fedcba9876543210fedcba9876543210")]
        [InlineData("0x000000000019d6689c085ae165831e934ff763ae46a2a6c172b3f1b60a8ce26f")]
        [InlineData("0x4a5e1e4baab89f3a32518a88c31bc87f618f76673e2cc77ab2127b7afdeda33b")]
        public void UInt256_ToHex_Test(string hex)
        {
            var uint256 = UInt256.FromHex(hex);

            Assert.Equal(hex, hex.StartsWith("0x") ? $"0x{uint256.ToHex(false)}" : $"{uint256.ToHex(false)}");
            var hexReverse = HexReverse.Encode(Hex.Decode(hex));
            var hexReverseExpected = hex.StartsWith("0x") ? $"0x{hexReverse}" : $"{hexReverse}";
            Assert.Equal(hexReverseExpected, hex.StartsWith("0x") ? $"0x{uint256.ToHex(true)}" : $"{uint256.ToHex(true)}");
        }

        [Theory]
        [InlineData("988119d6cca702beb1748f4eb497e316467f69580ffa125aa8bcb6fb63dce237")]
        [InlineData("fedcba9876543210fedcba9876543210fedcba9876543210fedcba9876543210")]
        [InlineData("0x000000000019d6689c085ae165831e934ff763ae46a2a6c172b3f1b60a8ce26f")]
        [InlineData("0x4a5e1e4baab89f3a32518a88c31bc87f618f76673e2cc77ab2127b7afdeda33b")]
        public void UInt256_ToHex_Reverse_Test(string hex)
        {
            var uint256 = UInt256.FromHex(hex, true);

            Assert.Equal(hex, hex.StartsWith("0x") ? $"0x{uint256.ToHex(true)}" : $"{uint256.ToHex(true)}");
            var hexReverse = HexReverse.Encode(Hex.Decode(hex));
            var hexReverseExpected = hex.StartsWith("0x") ? $"0x{hexReverse}" : $"{hexReverse}";
            Assert.Equal(hexReverseExpected, hex.StartsWith("0x") ? $"0x{uint256.ToHex(false)}" : $"{uint256.ToHex(false)}");
        }

        #endregion

        #region UInt512 Tests

        [Fact]
        public void UInt512_ByteAccess_Test()
        {
            var i = new UInt512
            {
                [0] = 0x21,
                [UInt512.Length - 1] = 0xfe
            };
            var str = i.ToString();
            Assert.Equal("fe000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000021", str);
        }

        [Theory]
        [InlineData("fedcba9876543210fedcba9876543210fedcba9876543210fedcba9876543210fedcba9876543210fedcba9876543210fedcba9876543210fedcba9876543210")]
        public void UInt512_ByteSpan_Test(string hex)
        {
            var bytes = Hex.Decode(hex);
            var readonlyByteSpan = new ReadOnlyByteSpan(bytes);

            var uint512 = new UInt512(readonlyByteSpan);
            var byteSpan = uint512.Span;

            Assert.Equal(bytes, uint512.ToArray());
            Assert.Equal(bytes, byteSpan.ToArray());
        }

        [Theory]
        [InlineData("fedcba9876543210fedcba9876543210fedcba9876543210fedcba9876543210fedcba9876543210fedcba9876543210fedcba9876543210fedcba9876543210")]
        [InlineData("0x6a22314c74794d45366235416e4d6f70517242504c6b3446474e3855427568784b71726e0101337b2274223a32302e36322c2268223a35392c2270223a313031")]
        public void UInt512_HexByteOrder_Test(string hex)
        {
            var lowOrderedBytes = Hex.Decode(hex);
            var highOrderedBytes = lowOrderedBytes.Reverse().ToArray();

            var uint512 = UInt512.FromHex(hex);
            var uint512Reverse = UInt512.FromHex(hex, true);
            Assert.Equal(uint512.ToArray(), highOrderedBytes);
            Assert.Equal(uint512Reverse.ToArray(), lowOrderedBytes);

            Assert.Equal(hex, hex.StartsWith("0x") ? $"0x{uint512}" : $"{uint512}");
            Assert.Equal(hex, hex.StartsWith("0x") ? $"0x{uint512Reverse.ToHex(true)}" : $"{uint512Reverse.ToHex(true)}");
        }

        [Theory]
        [InlineData("fedcba9876543210fedcba9876543210fedcba9876543210fedcba9876543210fedcba9876543210fedcba9876543210fedcba9876543210fedcba9876543210")]
        [InlineData("0x6a22314c74794d45366235416e4d6f70517242504c6b3446474e3855427568784b71726e0101337b2274223a32302e36322c2268223a35392c2270223a313031")]
        public void UInt512_FromHex_Test(string hex)
        {
            var uint512 = UInt512.FromHex(hex);
            var uint512Reverse = UInt512.FromHex(hex, true);
            var hexReverse = HexReverse.Encode(Hex.Decode(hex));
            var hexReverseExpected = hex.StartsWith("0x") ? $"0x{hexReverse}" : $"{hexReverse}";

            Assert.Equal(hex, hex.StartsWith("0x") ? $"0x{uint512}" : $"{uint512}");
            Assert.Equal(hexReverseExpected, hex.StartsWith("0x") ? $"0x{uint512Reverse}" : $"{uint512Reverse}");
        }

        [Theory]
        [InlineData("fedcba9876543210fedcba9876543210fedcba9876543210fedcba9876543210fedcba9876543210fedcba9876543210fedcba9876543210fedcba9876543210")]
        [InlineData("0x6a22314c74794d45366235416e4d6f70517242504c6b3446474e3855427568784b71726e0101337b2274223a32302e36322c2268223a35392c2270223a313031")]
        public void UInt512_ToHex_Test(string hex)
        {
            var uint512 = UInt512.FromHex(hex, true);

            Assert.Equal(hex, hex.StartsWith("0x") ? $"0x{uint512.ToHex(true)}" : $"{uint512.ToHex(true)}");
            var hexReverse = HexReverse.Encode(Hex.Decode(hex));
            var hexReverseExpected = hex.StartsWith("0x") ? $"0x{hexReverse}" : $"{hexReverse}";
            Assert.Equal(hexReverseExpected, hex.StartsWith("0x") ? $"0x{uint512.ToHex(false)}" : $"{uint512.ToHex(false)}");
        }

        [Theory]
        [InlineData("fedcba9876543210fedcba9876543210fedcba9876543210fedcba9876543210fedcba9876543210fedcba9876543210fedcba9876543210fedcba9876543210")]
        [InlineData("0x6a22314c74794d45366235416e4d6f70517242504c6b3446474e3855427568784b71726e0101337b2274223a32302e36322c2268223a35392c2270223a313031")]
        public void UInt512_ToHex_Reverse_Test(string hex)
        {
            var uint512 = UInt512.FromHex(hex, true);

            Assert.Equal(hex, hex.StartsWith("0x") ? $"0x{uint512.ToHex(true)}" : $"{uint512.ToHex(true)}");
            var hexReverse = HexReverse.Encode(Hex.Decode(hex));
            var hexReverseExpected = hex.StartsWith("0x") ? $"0x{hexReverse}" : $"{hexReverse}";
            Assert.Equal(hexReverseExpected, hex.StartsWith("0x") ? $"0x{uint512.ToHex(false)}" : $"{uint512.ToHex(false)}");
        }

        #endregion
    }
}
