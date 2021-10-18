using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace CafeLib.BsvSharp.Api.WhatsOnChain.Models
{
    public class MerkleTree
    {
        public MerkleNode[] Nodes { get; set; }
    }
}
