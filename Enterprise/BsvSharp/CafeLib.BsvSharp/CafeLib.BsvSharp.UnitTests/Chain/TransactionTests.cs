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
using CafeLib.BsvSharp.Crypto;
using CafeLib.BsvSharp.Encoding;
using CafeLib.BsvSharp.Numerics;
using CafeLib.BsvSharp.Scripting;
using CafeLib.Core.Extensions;
using Microsoft.VisualBasic.CompilerServices;
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
                Hash = new UInt256(Encoders.Hex.Decode(hash)),
                Index = int.Parse(index),
                ScriptPubKey = scriptPubKey
            };
        }

        [Fact]
        public void Basic()
        {
            GetValidTransactions()
                .Where(x => (x.VerifyFlags & ScriptFlags.VERIFY_P2SH) == 0)
                .ForEach(x =>
                {
                    var decode = Encoders.Hex.Decode(x.Serialized);
                    var hash = Hashes.Hash256(decode);

                    var transaction = new Transactions.Transaction(Encoders.Hex.Decode(x.Serialized));
                    var count = transaction.Inputs.Count;
                });

            Console.WriteLine("kilroy");
        }
    }
}