﻿using System;
using System.Linq;
using CafeLib.BsvSharp.Encoding;
using Xunit;

namespace CafeLib.BsvSharp.UnitTests.Encode
{
    public class KzBase58CheckTests
    {
        [Theory]
        [InlineData("1AGNa15ZQXAZUgFiqJ2i7Z2DPU2J6hW62i", "65a16059864a2fdbc7c99a4723a8395bc6f188eb", 0)]
        [InlineData("3CMNFxN1oHBc4R1EpboAL5yzHGgE611Xou", "74f209f6ea907e2ea48f74fae05782ae8a665257", 5)]
        [InlineData("mo9ncXisMeAoXwqcV5EWuyncbmCcQN4rVs", "53c0307d6851aa0ce7825ba883c6bd9ad242b486", 111)]
        [InlineData("2N2JD6wb56AfK4tfmM6PwdVmoYk2dCKf4Br", "6349a418fc4578d10a372b54b45c280cc8c4382f", 196)]
        [InlineData("5Kd3NBUAdUnhyzenEwVLy9pBKxSwXvE9FMPyR4UKZvpe6E3AgLr", "eddbdc1168f1daeadbd3e44c1e3f8f5a284c2029f78ad26af98583a499de5b19", 128)]
        [InlineData("9213qJab2HNEpMpYNBa7wHGFKKbkDn24jpANDs2huN3yi4J11ko", "36cb93b9ab1bdabf7fb9f2c04f1b9cc879933530ae7842398eef5a63a56800c2", 239)]
        [InlineData("1Ax4gZtb7gAit2TivwejZHYtNNLT18PUXJ", "6d23156cbbdcc82a5a47eee4c2c7c583c18b6bf4", 0)]
        [InlineData("3QjYXhTkvuj8qPaXHTTWb5wjXhdsLAAWVy", "fcc5460dd6e2487c7d75b1963625da0e8f4c5975", 5)]
        [InlineData("n3ZddxzLvAY9o7184TB4c6FJasAybsw4HZ", "f1d470f9b02370fdec2e6b708b08ac431bf7a5f7", 111)]
        [InlineData("2NBFNJTktNa7GZusGbDbGKRZTxdK9VVez3n", "c579342c2c4c9220205e2cdc285617040c924a0a", 196)]
        [InlineData("5K494XZwps2bGyeL71pWid4noiSNA2cfCibrvRWqcHSptoFn7rc", "a326b95ebae30164217d7a7f57d72ab2b54e3be64928a19da0210b9568d4015e", 128)]
        [InlineData("93DVKyFYwSN6wEo3E2fCrFPUp17FtrtNi2Lf7n4G3garFb16CRj", "d6bca256b5abc5602ec2e1c121a08b0da2556587430bcf7e1898af2224885203", 239)]
        [InlineData("1C5bSj1iEGUgSTbziymG7Cn18ENQuT36vv", "7987ccaa53d02c8873487ef919677cd3db7a6912", 0)]
        [InlineData("3AnNxabYGoTxYiTEZwFEnerUoeFXK2Zoks", "63bcc565f9e68ee0189dd5cc67f1b0e5f02f45cb", 5)]
        [InlineData("n3LnJXCqbPjghuVs8ph9CYsAe4Sh4j97wk", "ef66444b5b17f14e8fae6e7e19b045a78c54fd79", 111)]
        [InlineData("2NB72XtkjpnATMggui83aEtPawyyKvnbX2o", "c3e55fceceaa4391ed2a9677f4a4d34eacd021a0", 196)]
        [InlineData("5KaBW9vNtWNhc3ZEDyNCiXLPdVPHCikRxSBWwV9NrpLLa4LsXi9", "e75d936d56377f432f404aabb406601f892fd49da90eb6ac558a733c93b47252", 128)]
        [InlineData("927CnUkUbasYtDwYwVn2j8GdTuACNnKkjZ1rpZd2yBB1CLcnXpo", "44c4f6a096eac5238291a94cc24c01e3b19b8d8cef72874a079e00a242237a52", 239)]
        [InlineData("1Gqk4Tv79P91Cc1STQtU3s1W6277M2CVWu", "adc1cc2081a27206fae25792f28bbc55b831549d", 0)]
        [InlineData("33vt8ViH5jsr115AGkW6cEmEz9MpvJSwDk", "188f91a931947eddd7432d6e614387e32b244709", 5)]
        [InlineData("mhaMcBxNh5cqXm4aTQ6EcVbKtfL6LGyK2H", "1694f5bc1a7295b600f40018a618a6ea48eeb498", 111)]
        [InlineData("2MxgPqX1iThW3oZVk9KoFcE5M4JpiETssVN", "3b9b3fd7a50d4f08d1a5b0f62f644fa7115ae2f3", 196)]
        [InlineData("5HtH6GdcwCJA4ggWEL1B3jzBBUB8HPiBi9SBc5h9i4Wk4PSeApR", "091035445ef105fa1bb125eccfb1882f3fe69592265956ade751fd095033d8d0", 128)]
        [InlineData("92xFEve1Z9N8Z641KQQS7ByCSb8kGjsDzw6fAmjHN1LZGKQXyMq", "b4204389cef18bbe2b353623cbf93e8678fbc92a475b664ae98ed594e6cf0856", 239)]
        [InlineData("1JwMWBVLtiqtscbaRHai4pqHokhFCbtoB4", "c4c1b72491ede1eedaca00618407ee0b772cad0d", 0)]
        [InlineData("3QCzvfL4ZRvmJFiWWBVwxfdaNBT8EtxB5y", "f6fe69bcb548a829cce4c57bf6fff8af3a5981f9", 5)]
        [InlineData("mizXiucXRCsEriQCHUkCqef9ph9qtPbZZ6", "261f83568a098a8638844bd7aeca039d5f2352c0", 111)]
        [InlineData("2NEWDzHWwY5ZZp8CQWbB7ouNMLqCia6YRda", "e930e1834a4d234702773951d627cce82fbb5d2e", 196)]
        [InlineData("5KQmDryMNDcisTzRp3zEq9e4awRmJrEVU1j5vFRTKpRNYPqYrMg", "d1fab7ab7385ad26872237f1eb9789aa25cc986bacc695e07ac571d6cdac8bc0", 128)]
        [InlineData("91cTVUcgydqyZLgaANpf1fvL55FH53QMm4BsnCADVNYuWuqdVys", "037f4192c630f399d9271e26c575269b1d15be553ea1a7217f0cb8513cef41cb", 239)]
        [InlineData("19dcawoKcZdQz365WpXWMhX6QCUpR9SY4r", "5eadaf9bb7121f0f192561a5a62f5e5f54210292", 0)]
        [InlineData("37Sp6Rv3y4kVd1nQ1JV5pfqXccHNyZm1x3", "3f210e7277c899c3a155cc1c90f4106cbddeec6e", 5)]
        [InlineData("myoqcgYiehufrsnnkqdqbp69dddVDMopJu", "c8a3c2a09a298592c3e180f02487cd91ba3400b5", 111)]
        [InlineData("2N7FuwuUuoTBrDFdrAZ9KxBmtqMLxce9i1C", "99b31df7c9068d1481b596578ddbb4d3bd90baeb", 196)]
        [InlineData("5KL6zEaMtPRXZKo1bbMq7JDjjo1bJuQcsgL33je3oY8uSJCR5b4", "c7666842503db6dc6ea061f092cfb9c388448629a6fe868d068c42a488b478ae", 128)]
        [InlineData("93N87D6uxSBzwXvpokpzg8FFmfQPmvX4xHoWQe3pLdYpbiwT5YV", "ea577acfb5d1d14d3b7b195c321566f12f87d2b77ea3a53f68df7ebf8604a801", 239)]
        [InlineData("13p1ijLwsnrcuyqcTvJXkq2ASdXqcnEBLE", "1ed467017f043e91ed4c44b4e8dd674db211c4e6", 0)]
        [InlineData("3ALJH9Y951VCGcVZYAdpA3KchoP9McEj1G", "5ece0cadddc415b1980f001785947120acdb36fc", 5)]
        public void KzBase58CheckTestCases(string base58, string hex, int version)
        {
            var h = Encoders.Hex;
            var e = Encoders.Base58Check;
            var hexBytes = h.Decode(hex);
            var buf = e.Decode(base58);
            var ver = buf[0];
            var data = buf[1..];
            Assert.Equal(version, ver);
            Assert.Equal(hexBytes, data);
            Assert.Equal(base58, e.Encode(buf));
        }


        [Theory]
        //[InlineData("1AGNa15ZQXAZagFiqJ2i7Z2DPU2J6hW62i", "65a16059864a2fdbc7c99a4723a8395bc6f188eb", 0)]
        [InlineData("1AGNa15ZQXAZagFiqJ2i7Z2DPU2J6hW62i")]
        public void KzBase58CheckFailureTestCases(string base58)
        {
            var decodeBytes = Array.Empty<byte>();

           Assert.Throws<FormatException>(() => decodeBytes = Encoders.Base58Check.Decode(base58));
            Assert.True(Array.Empty<byte>().SequenceEqual(decodeBytes));
        }
    }
}