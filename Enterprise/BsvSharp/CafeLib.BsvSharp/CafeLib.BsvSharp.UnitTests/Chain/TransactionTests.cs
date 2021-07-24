#region Copyright
// Copyright (c) 2020 TonesNotes
// Distributed under the Open BSV software license, see the accompanying file LICENSE.
#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CafeLib.BsvSharp.Builders;
using CafeLib.BsvSharp.Chain;
using CafeLib.BsvSharp.Encoding;
using CafeLib.BsvSharp.Keys;
using CafeLib.BsvSharp.Numerics;
using CafeLib.BsvSharp.Persistence;
using CafeLib.BsvSharp.Scripting;
using CafeLib.Core.Extensions;
using Newtonsoft.Json.Linq;
using Xunit;

namespace CafeLib.BsvSharp.UnitTests.Chain
{
    public class TransactionTests
    {
        private const string DataFolder = @"..\..\..\data";

        class TxInfo
        {
            public UInt256 Hash { get; set; }
            public int Index { get; set; }
            public string ScriptPubKey { get; set; }
        }

        class TxInput
        {
            /// <summary>
            /// ScriptSig as hex string.
            /// </summary>
            public List<TxInfo> Transactions { get; set; }
            public string Serialized { get; set; }
            public ScriptFlags VerifyFlags { get; set; }
        }

        private static IEnumerable<TxInput> GetValidTransactions()
        {
            var values = new List<TxInput>();
            var jarray = JArray.Parse(File.ReadAllText(Path.Combine(DataFolder, "tx_valid.json")));

            foreach (var json in jarray)
            {
                var jtoken = FindValueInArray(json);
                if (jtoken.Type == JTokenType.String && jtoken.Parent?.Count >= 3)
                {
                    var txInput = GetTransactionInput(json);
                    values.Add(txInput);
                }
            }

            return values;

            static JToken FindValueInArray(JToken token)
            {
                if (!(token is JArray items)) return string.Empty;

                foreach (var json in items)
                {
                    return json.Type == JTokenType.Array ? FindValueInArray((JArray)json) : json;
                }

                return string.Empty;
            }

            static TxInput GetTransactionInput(JToken token)
            {
                var transactions = new List<TxInfo>();
                var parent = token.Parent;
                ScriptFlags verify = 0U;

                while (token != null)
                {
                    token = FindValueInArray(token);
                    parent = token.Parent;
                    var prevTxHash = parent?[0]?.Value<string>();
                    var prevTxIndex = parent?[1]?.Value<string>();
                    var prevScript = parent?[2]?.Value<string>();
                    transactions.Add(BuildTransactionInfo(prevTxHash, prevTxIndex, prevScript));
                    token = parent?.Next;
                }

                var flags = parent?.Parent?.Parent?[2]?.Value<string>() ?? "NONE";
                flags.Split(',').ForEach(x => verify |= Enum.Parse<ScriptFlags>($"VERIFY_{x}"));

                return new TxInput
                {
                    Transactions = transactions,
                    Serialized = parent?.Parent?.Parent?[1]?.Value<string>(),
                    VerifyFlags = verify
                };
            }
        }


        private TransactionBuilder BuildCreditingTransaction(Script scriptPubKey, long nValue)
        {
            var tx = new TransactionBuilder
            {
                LockTime = 0,
                Version = 1,
            };

            tx.AddIn(new OutPoint(), new ScriptBuilder().Push(0).Push(0));
            tx.AddOut(scriptPubKey, nValue);
            return tx;
        }

        TransactionBuilder BuildSpendingTransaction(Script scriptSig, TransactionBuilder txCredit)
        {
            var tx = new TransactionBuilder
            {
                LockTime = 0,
                Version = 1,
            };

            tx.AddIn(new OutPoint(), new ScriptBuilder().Push(0).Push(0));
            //tx.AddOut(scriptPubKey, nValue);
            return tx;
        }

        private static TxInfo BuildTransactionInfo(string hash, string index, string scriptPubKey)
        {
            return new TxInfo
            {
                Hash = new UInt256(Encoders.HexReverse.Decode(hash)),
                Index = int.Parse(index),
                ScriptPubKey = scriptPubKey
            };
        }

        [Fact]
        public void Verify_Valid_Transaction()
        {
            GetValidTransactions()
                .Where(x => (x.VerifyFlags & ScriptFlags.VERIFY_P2SH) == 0)
                .ForEach(x =>
                {
                    var expectedHash = x.Transactions.First().Hash;
                    var expectedIndex = x.Transactions.First().Index;
                    var transaction = new Transactions.Transaction(Encoders.Hex.Decode(x.Serialized));
                    var previousHash = transaction.Inputs.First().PrevOut.TxHash;
                    var previousIndex = transaction.Inputs.First().PrevOut.Index;
                    Assert.Equal(expectedHash, previousHash);
                    Assert.Equal(expectedIndex, previousIndex);
                });
        }

        [Fact]
        public void Parse_Transaction_Version_As_Signed_Integer()
        {
            var transaction = new Transactions.Transaction("ffffffff0000ffffffff");
            Assert.Equal(-1, transaction.Version);
            Assert.Equal(0xffffffff, transaction.LockTime);
        }

        [Fact]
        public void Deserialize_Transaction()
        {
            const string txHex = "01000000015884e5db9de218238671572340b207ee85b628074e7e467096c267266baf77a4000000006a473044022013fa3089327b50263029265572ae1b022a91d10ac80eb4f32f291c914533670b02200d8a5ed5f62634a7e1a0dc9188a3cc460a986267ae4d58faf50c79105431327501210223078d2942df62c45621d209fab84ea9a7a23346201b7727b9b45a29c4e76f5effffffff0150690f00000000001976a9147821c0a3768aa9d1a37e16cf76002aef5373f1a888ac00000000";
            var writer = new ByteDataWriter();

            var transaction = new Transactions.Transaction(txHex);
            transaction.WriteTo(writer);

            var serializedHex = Encoders.Hex.Encode(writer.Span);
            Assert.Equal(txHex, serializedHex);
        }

        [Fact]
        public void Coinbase_Transaction()
        {
            const string txHex = "01000000010000000000000000000000000000000000000000000000000000000000000000ffffffff0704ffff001d0104ffffffff0100f2052a0100000043410496b538e853519c726a2c91e61ec11600ae1390813a627c66fb8be7947be63c52da7589379515d4e0a604f8141781e62294721166bf621e73a82cbf2342c858eeac00000000";
            var transaction = new Transactions.Transaction(txHex);
            Assert.True(transaction.IsCoinbase);
        }

        [Fact]
        public void Spend_Transaction()
        {
            //var fromTx = new Transactions.Transaction("a477af6b2667c29670467e4e0728b685ee07b240235771862318e29ddbe58458");
            //var fromTx = new Transactions.Transaction(fromTx.Hash, , );
            //fromTx.SpendFrom()
            //var fromAddress = new Address("mszYqVnqKoQx4jcTdJXxwKAissE3Jbrrc1");
            //var txHash =
            //"txId": 'a477af6b2667c29670467e4e0728b685ee07b240235771862318e29ddbe58458',
            //"outputIndex": 0,
            //"scriptPubKey": P2PKHLockBuilder(fromAddress).getScriptPubkey().toString(),
            //"satoshis": BigInt.from(1000000)



            //var simpleUtxoWith1000000Satoshis = new Transactions.Transaction();
            //simpleUtxoWith1000000Satoshis.SpendFrom(fromAddress,)


            //var simpleUtxoWith1000000Satoshis = {
            //    "address": fromAddress,
            //    "txId": 'a477af6b2667c29670467e4e0728b685ee07b240235771862318e29ddbe58458',
            //    "outputIndex": 0,
            //    "scriptPubKey": P2PKHLockBuilder(fromAddress).getScriptPubkey().toString(),
            //    "satoshis": BigInt.from(1000000)
            //};
        }
    }
}