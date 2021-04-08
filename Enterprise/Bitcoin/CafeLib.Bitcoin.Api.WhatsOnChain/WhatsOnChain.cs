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
using CafeLib.Bitcoin.Utility;
using CafeLib.Web.Request;
using Newtonsoft.Json;

namespace CafeLib.Bitcoin.Api.WhatsOnChain 
{
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
