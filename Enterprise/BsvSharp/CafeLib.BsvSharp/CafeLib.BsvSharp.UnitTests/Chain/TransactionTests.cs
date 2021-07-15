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
using CafeLib.BsvSharp.Scripting;
using Microsoft.VisualBasic.CompilerServices;
using Newtonsoft.Json.Linq;
using Xunit;

namespace CafeLib.BsvSharp.UnitTests.Chain
{
    public class TransactionTests
    {
        private const string DataFolder = @"..\..\..\data";

        /// <summary>
        /// Test Vector
        /// </summary>
        class TxInfo
        {
            /// <summary>
            /// ScriptSig as hex string.
            /// </summary>
            public List<string> Transactions;
            public string Serialized;
            public string VerifyFlag;
        }




        private static IEnumerable<TxInfo> GetValidTransaction()
        {
            var values = new List<TxInfo>();
            var jarray = JArray.Parse(File.ReadAllText(Path.Combine(DataFolder, "tx_valid.json")));

            foreach (var json in jarray)
            {
                var jtoken = FindValueInArray(json);
                if (jtoken.Type == JTokenType.String && jtoken.Parent.Count >= 3)
                {
                    var txInfo = GetTransactionInfo(json);
                    values.Add(txInfo);
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

            static TxInfo GetTransactionInfo(JToken token)
            {
                var transactions = new List<string>();
                var parent = token.Parent;
                
                while (token != null)
                {
                    token = FindValueInArray(token);
                    parent = token.Parent;
                    var prevTxHash = parent?[0]?.Value<string>();
                    var prevTxIndex = parent?[1]?.Value<string>();
                    var prevScript = parent?[2]?.Value<string>();
                    transactions.Add(string.Join('|', prevTxHash, prevTxIndex, prevScript));
                    token = parent?.Next;
                }

                return new TxInfo
                {
                    Transactions = transactions,
                    Serialized = parent?.Parent?.Parent?[1]?.Value<string>(),
                    VerifyFlag = parent?.Parent?.Parent?[2]?.Value<string>()
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

        [Fact]
        public void Basic()
        {
            var lines = GetValidTransaction();
            var t = lines.Where(x => !x.VerifyFlag.Contains("P2SH"));
            Console.WriteLine("kilroy");
        }
    }
}