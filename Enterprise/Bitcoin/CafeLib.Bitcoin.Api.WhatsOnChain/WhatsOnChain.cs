#region Copyright
// Copyright (c) 2020 TonesNotes
// Distributed under the Open BSV software license, see the accompanying file LICENSE.
#endregion

using System.Buffers;
using System.Collections.Generic;
using System.Threading.Tasks;
using CafeLib.Bitcoin.Chain;
using CafeLib.Bitcoin.Extensions;
using CafeLib.Bitcoin.Global;
using CafeLib.Bitcoin.Services;
using CafeLib.Bitcoin.Utility;
using CafeLib.Web.Request;
using Newtonsoft.Json;

namespace CafeLib.Bitcoin.Api.WhatsOnChain {

    public class KzApiWhatsOnChain : BasicApiRequest
    {
        public KzApiWhatsOnChain()
        {
            Headers.Add("User-Agent", "KzApiWhatsOnChain");
        }

        public async Task<List<ByAddressUnspent>> GetUnspentTransactionsByAddress(string address)
        {
            var url = $"https://api.whatsonchain.com/v1/bsv/{Kz.Params.NetworkId}/address/{address}/unspent";

            var json = await GetAsync(url);

            var unspent = JsonConvert.DeserializeObject<List<ByAddressUnspent>>(json);

            return unspent;
        }

        public class ScriptSig
        {
            public string asm;
            public string hex;
        }

        public class Vin
        {
            public string txid;
            public int vout;
            public ScriptSig scriptSig;
            public uint sequence;
            public string coinbase;
        }

        public class ScriptPubKey
        {
            public string asm;
            public string hex;
            public int reqSigs;
            public int type;
            public string[] addresses;
            public string opReturn;
        }
        public class Vout
        {
            public decimal value;
            public int n;
            public string txid;
            public int vout;
            public ScriptSig scriptSig;
            public uint sequence;
            public string coinbase;
        }

        public class Transaction
        {
            public string hex;
            public string txid;
            public string hash;
            public int size;
            public int version;
            public uint locktime;
            public Vin[] vin;
            public Vout[] vout;
            public string blockhash;
            public int confirmations;
            public long time;
            public long blocktime;
        }

        public class ExchangeRate
        {
            public string currency;
            public decimal rate;
        }

        public async Task<KzTransaction> GetTransactionsByHash(KzUInt256 txId)
        {
            var url = $"https://api.whatsonchain.com/v1/bsv/{Kz.Params.NetworkId}/tx/hash/{txId}";

            var json = await GetAsync(url);

            var woctx = JsonConvert.DeserializeObject<Transaction>(json);

            var tx = new KzTransaction();
            var ros = new ReadOnlySequence<byte>(woctx.hex.HexToBytes());
            if (!tx.TryReadTransaction(ref ros))
                tx = null;
            return tx;
        }

        public async Task<decimal> GetExchangeRate()
        {
            var url = $"https://api.whatsonchain.com/v1/bsv/{Kz.Params.NetworkId}/exchangerate";

            var json = await GetAsync(url);

            // json == {"currency":"USD","rate":"174.04999999999998"}

            var er = JsonConvert.DeserializeObject<ExchangeRate>(json);

            return er.rate;
        }
    }
}
