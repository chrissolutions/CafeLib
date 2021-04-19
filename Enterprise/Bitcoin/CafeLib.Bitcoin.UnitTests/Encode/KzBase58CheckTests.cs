using System;
using CafeLib.Bitcoin.Encoding;
using Xunit;

namespace CafeLib.Bitcoin.UnitTests.Encode
{
    public class KzBase58CheckTests
    {
        private class TestCase { public bool Ok; public byte Version; public string Hex; public string Base58; }

        private readonly TestCase[] _testCases = {
            new TestCase { Ok = false, Base58 = "1AGNa15ZQXAZagFiqJ2i7Z2DPU2J6hW62i", Hex = "65a16059864a2fdbc7c99a4723a8395bc6f188eb", Version = 0 },
            new TestCase { Ok = true, Base58 = "1AGNa15ZQXAZUgFiqJ2i7Z2DPU2J6hW62i", Hex = "65a16059864a2fdbc7c99a4723a8395bc6f188eb", Version = 0 },
            new TestCase { Ok = true, Base58 = "3CMNFxN1oHBc4R1EpboAL5yzHGgE611Xou", Hex = "74f209f6ea907e2ea48f74fae05782ae8a665257", Version = 5 },
            new TestCase { Ok = true, Base58 = "mo9ncXisMeAoXwqcV5EWuyncbmCcQN4rVs", Hex = "53c0307d6851aa0ce7825ba883c6bd9ad242b486", Version = 111 },
            new TestCase { Ok = true, Base58 = "2N2JD6wb56AfK4tfmM6PwdVmoYk2dCKf4Br", Hex = "6349a418fc4578d10a372b54b45c280cc8c4382f", Version = 196 },
            new TestCase { Ok = true, Base58 = "5Kd3NBUAdUnhyzenEwVLy9pBKxSwXvE9FMPyR4UKZvpe6E3AgLr", Hex = "eddbdc1168f1daeadbd3e44c1e3f8f5a284c2029f78ad26af98583a499de5b19", Version = 128 },
            new TestCase { Ok = true, Base58 = "9213qJab2HNEpMpYNBa7wHGFKKbkDn24jpANDs2huN3yi4J11ko", Hex = "36cb93b9ab1bdabf7fb9f2c04f1b9cc879933530ae7842398eef5a63a56800c2", Version = 239 },
            new TestCase { Ok = true, Base58 = "1Ax4gZtb7gAit2TivwejZHYtNNLT18PUXJ", Hex = "6d23156cbbdcc82a5a47eee4c2c7c583c18b6bf4", Version = 0 },
            new TestCase { Ok = true, Base58 = "3QjYXhTkvuj8qPaXHTTWb5wjXhdsLAAWVy", Hex = "fcc5460dd6e2487c7d75b1963625da0e8f4c5975", Version = 5 },
            new TestCase { Ok = true, Base58 = "n3ZddxzLvAY9o7184TB4c6FJasAybsw4HZ", Hex = "f1d470f9b02370fdec2e6b708b08ac431bf7a5f7", Version = 111 },
            new TestCase { Ok = true, Base58 = "2NBFNJTktNa7GZusGbDbGKRZTxdK9VVez3n", Hex = "c579342c2c4c9220205e2cdc285617040c924a0a", Version = 196 },
            new TestCase { Ok = true, Base58 = "5K494XZwps2bGyeL71pWid4noiSNA2cfCibrvRWqcHSptoFn7rc", Hex = "a326b95ebae30164217d7a7f57d72ab2b54e3be64928a19da0210b9568d4015e", Version = 128 },
            new TestCase { Ok = true, Base58 = "93DVKyFYwSN6wEo3E2fCrFPUp17FtrtNi2Lf7n4G3garFb16CRj", Hex = "d6bca256b5abc5602ec2e1c121a08b0da2556587430bcf7e1898af2224885203", Version = 239 },
            new TestCase { Ok = true, Base58 = "1C5bSj1iEGUgSTbziymG7Cn18ENQuT36vv", Hex = "7987ccaa53d02c8873487ef919677cd3db7a6912", Version = 0 },
            new TestCase { Ok = true, Base58 = "3AnNxabYGoTxYiTEZwFEnerUoeFXK2Zoks", Hex = "63bcc565f9e68ee0189dd5cc67f1b0e5f02f45cb", Version = 5 },
            new TestCase { Ok = true, Base58 = "n3LnJXCqbPjghuVs8ph9CYsAe4Sh4j97wk", Hex = "ef66444b5b17f14e8fae6e7e19b045a78c54fd79", Version = 111 },
            new TestCase { Ok = true, Base58 = "2NB72XtkjpnATMggui83aEtPawyyKvnbX2o", Hex = "c3e55fceceaa4391ed2a9677f4a4d34eacd021a0", Version = 196 },
            new TestCase { Ok = true, Base58 = "5KaBW9vNtWNhc3ZEDyNCiXLPdVPHCikRxSBWwV9NrpLLa4LsXi9", Hex = "e75d936d56377f432f404aabb406601f892fd49da90eb6ac558a733c93b47252", Version = 128 },
            new TestCase { Ok = true, Base58 = "927CnUkUbasYtDwYwVn2j8GdTuACNnKkjZ1rpZd2yBB1CLcnXpo", Hex = "44c4f6a096eac5238291a94cc24c01e3b19b8d8cef72874a079e00a242237a52", Version = 239 },
            new TestCase { Ok = true, Base58 = "1Gqk4Tv79P91Cc1STQtU3s1W6277M2CVWu", Hex = "adc1cc2081a27206fae25792f28bbc55b831549d", Version = 0 },
            new TestCase { Ok = true, Base58 = "33vt8ViH5jsr115AGkW6cEmEz9MpvJSwDk", Hex = "188f91a931947eddd7432d6e614387e32b244709", Version = 5 },
            new TestCase { Ok = true, Base58 = "mhaMcBxNh5cqXm4aTQ6EcVbKtfL6LGyK2H", Hex = "1694f5bc1a7295b600f40018a618a6ea48eeb498", Version = 111 },
            new TestCase { Ok = true, Base58 = "2MxgPqX1iThW3oZVk9KoFcE5M4JpiETssVN", Hex = "3b9b3fd7a50d4f08d1a5b0f62f644fa7115ae2f3", Version = 196 },
            new TestCase { Ok = true, Base58 = "5HtH6GdcwCJA4ggWEL1B3jzBBUB8HPiBi9SBc5h9i4Wk4PSeApR", Hex = "091035445ef105fa1bb125eccfb1882f3fe69592265956ade751fd095033d8d0", Version = 128 },
            new TestCase { Ok = true, Base58 = "92xFEve1Z9N8Z641KQQS7ByCSb8kGjsDzw6fAmjHN1LZGKQXyMq", Hex = "b4204389cef18bbe2b353623cbf93e8678fbc92a475b664ae98ed594e6cf0856", Version = 239 },
            new TestCase { Ok = true, Base58 = "1JwMWBVLtiqtscbaRHai4pqHokhFCbtoB4", Hex = "c4c1b72491ede1eedaca00618407ee0b772cad0d", Version = 0 },
            new TestCase { Ok = true, Base58 = "3QCzvfL4ZRvmJFiWWBVwxfdaNBT8EtxB5y", Hex = "f6fe69bcb548a829cce4c57bf6fff8af3a5981f9", Version = 5 },
            new TestCase { Ok = true, Base58 = "mizXiucXRCsEriQCHUkCqef9ph9qtPbZZ6", Hex = "261f83568a098a8638844bd7aeca039d5f2352c0", Version = 111 },
            new TestCase { Ok = true, Base58 = "2NEWDzHWwY5ZZp8CQWbB7ouNMLqCia6YRda", Hex = "e930e1834a4d234702773951d627cce82fbb5d2e", Version = 196 },
            new TestCase { Ok = true, Base58 = "5KQmDryMNDcisTzRp3zEq9e4awRmJrEVU1j5vFRTKpRNYPqYrMg", Hex = "d1fab7ab7385ad26872237f1eb9789aa25cc986bacc695e07ac571d6cdac8bc0", Version = 128 },
            new TestCase { Ok = true, Base58 = "91cTVUcgydqyZLgaANpf1fvL55FH53QMm4BsnCADVNYuWuqdVys", Hex = "037f4192c630f399d9271e26c575269b1d15be553ea1a7217f0cb8513cef41cb", Version = 239 },
            new TestCase { Ok = true, Base58 = "19dcawoKcZdQz365WpXWMhX6QCUpR9SY4r", Hex = "5eadaf9bb7121f0f192561a5a62f5e5f54210292", Version = 0 },
            new TestCase { Ok = true, Base58 = "37Sp6Rv3y4kVd1nQ1JV5pfqXccHNyZm1x3", Hex = "3f210e7277c899c3a155cc1c90f4106cbddeec6e", Version = 5 },
            new TestCase { Ok = true, Base58 = "myoqcgYiehufrsnnkqdqbp69dddVDMopJu", Hex = "c8a3c2a09a298592c3e180f02487cd91ba3400b5", Version = 111 },
            new TestCase { Ok = true, Base58 = "2N7FuwuUuoTBrDFdrAZ9KxBmtqMLxce9i1C", Hex = "99b31df7c9068d1481b596578ddbb4d3bd90baeb", Version = 196 },
            new TestCase { Ok = true, Base58 = "5KL6zEaMtPRXZKo1bbMq7JDjjo1bJuQcsgL33je3oY8uSJCR5b4", Hex = "c7666842503db6dc6ea061f092cfb9c388448629a6fe868d068c42a488b478ae", Version = 128 },
            new TestCase { Ok = true, Base58 = "93N87D6uxSBzwXvpokpzg8FFmfQPmvX4xHoWQe3pLdYpbiwT5YV", Hex = "ea577acfb5d1d14d3b7b195c321566f12f87d2b77ea3a53f68df7ebf8604a801", Version = 239 },
            new TestCase { Ok = true, Base58 = "13p1ijLwsnrcuyqcTvJXkq2ASdXqcnEBLE", Hex = "1ed467017f043e91ed4c44b4e8dd674db211c4e6", Version = 0 },
            new TestCase { Ok = true, Base58 = "3ALJH9Y951VCGcVZYAdpA3KchoP9McEj1G", Hex = "5ece0cadddc415b1980f001785947120acdb36fc", Version = 5 }
        };

        [Fact]
        public void KzBase58CheckTestCases()
        {
            var h = Encoders.Hex;
            var e = Encoders.Base58Check;
            foreach (var tc in _testCases)
            {
                var hex = h.Decode(tc.Hex);
                var ok = e.TryDecode(tc.Base58, out var buf);
                Assert.Equal(tc.Ok, ok);
                if (!ok) continue;
                var ver = buf[0];
                var data = buf.AsSpan()[1..].ToArray();
                Assert.Equal(tc.Version, ver);
                Assert.Equal(hex, data);
                Assert.Equal(tc.Base58, e.Encode(buf));
            }
        }
    }
}