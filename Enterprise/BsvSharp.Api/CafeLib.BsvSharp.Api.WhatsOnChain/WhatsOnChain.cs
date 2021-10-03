#region Copyright
// Copyright (c) 2020 TonesNotes
// Distributed under the Open BSV software license, see the accompanying file LICENSE.
#endregion

using System.Collections.Generic;
using System.Threading.Tasks;
using CafeLib.BsvSharp.Api.WhatsOnChain.Models;
using CafeLib.BsvSharp.Network;
using CafeLib.Core.Extensions;
using CafeLib.Web.Request;
using Newtonsoft.Json;

namespace CafeLib.BsvSharp.Api.WhatsOnChain 
{
    public class WhatsOnChain : BasicApiRequest
    {
        public string Network { get; }

        public WhatsOnChain(NetworkType networkType = NetworkType.Main)
        {
            Network = networkType.GetDescriptor();
            Headers.Add("Content-Type", WebContentType.Json);
            Headers.Add("User-Agent", "KzApiWhatsOnChain");
        }

        public async Task<List<ByAddressUnspent>> GetUnspentTransactionsByAddress(string address)
        {
            var url = $"https://api.whatsonchain.com/v1/bsv/{Network}/address/{address}/unspent";
            var json = await GetAsync(url);
            var unspent = JsonConvert.DeserializeObject<List<ByAddressUnspent>>(json);
            return unspent;
        }

        public async Task<Transaction> GetTransactionsByHash(string txid)
        {
            var url = $"https://api.whatsonchain.com/v1/bsv/{Network}/tx/hash/{txid}";
            var json = await GetAsync(url);
            var tx = JsonConvert.DeserializeObject<Transaction>(json);
            return tx;
        }

        public async Task<decimal> GetExchangeRate()
        {
            var url = $"https://api.whatsonchain.com/v1/bsv/{Network}/exchangerate";
            var json = await GetAsync(url);

            var er = JsonConvert.DeserializeObject<ExchangeRate>(json);
            return er.Rate;
        }

        public async Task<Balance> GetAddressBalance(string address)
        {
            var url = $"https://api.whatsonchain.com/v1/bsv/main/address/{address}/balance";
            var json = await GetAsync(url);
            var balance = JsonConvert.DeserializeObject<Balance>(json);
            return balance;
        }
    }
}
