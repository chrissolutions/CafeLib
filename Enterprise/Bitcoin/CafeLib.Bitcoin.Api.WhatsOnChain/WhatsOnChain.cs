#region Copyright
// Copyright (c) 2020 TonesNotes
// Distributed under the Open BSV software license, see the accompanying file LICENSE.
#endregion

using System.Collections.Generic;
using System.Threading.Tasks;
using CafeLib.Bitcoin.Api.WhatsOnChain.Models;
using CafeLib.Bitcoin.Global;
using CafeLib.Web.Request;
using Newtonsoft.Json;

namespace CafeLib.Bitcoin.Api.WhatsOnChain 
{
    public class WhatsOnChain : BasicApiRequest
    {
        public WhatsOnChain()
        {
            Headers.Add("Content-Type", WebContentType.Json);
            Headers.Add("User-Agent", "KzApiWhatsOnChain");
        }

        public async Task<List<ByAddressUnspent>> GetUnspentTransactionsByAddress(string address)
        {
            var url = $"https://api.whatsonchain.com/v1/bsv/{Kz.Params.NetworkId}/address/{address}/unspent";
            var json = await GetAsync(url);
            var unspent = JsonConvert.DeserializeObject<List<ByAddressUnspent>>(json);
            return unspent;
        }

        public async Task<Transaction> GetTransactionsByHash(string txId)
        {
            var url = $"https://api.whatsonchain.com/v1/bsv/{Kz.Params.NetworkId}/tx/hash/{txId}";
            var json = await GetAsync(url);
            var tx = JsonConvert.DeserializeObject<Transaction>(json);
            return tx;

            //var tx = new KzTransaction();
            ////var ros = new ReadOnlySequence<byte>(woctx.Hex.HexToBytes());
            //var ros = new ReadOnlySequence<byte>(woctx.VectorOutput[0].ScriptPubKey.Hex.HexToBytes());
            //if (!tx.TryReadTransaction(ref ros))
            //    tx = null;
            //return tx;
        }

        public async Task<decimal> GetExchangeRate()
        {
            var url = $"https://api.whatsonchain.com/v1/bsv/{Kz.Params.NetworkId}/exchangerate";
            var json = await GetAsync(url);

            // json == {"currency":"USD","rate":"174.04999999999998"}

            var er = JsonConvert.DeserializeObject<ExchangeRate>(json);
            return er.Rate;
        }
    }
}
