using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CafeLib.Core.Support;
using Newtonsoft.Json;

namespace CafeLib.Bitcoin.Api.WhatsOnChain
{
    public class VectorOutput : IEnumerable<Output>
    {
        private readonly IList<Output> _outputs;

        public VectorOutput()
        {
            _outputs = new List<Output>();
        }

        public VectorOutput(VectorOutput vout)
        {
            
        }

        public void Add(NonNullable<Output> output)
        {
            _outputs.Add(output);
        }

        public void Add(NonNullable<VectorOutput> vector)
        {
            ((List<Output>)_outputs).AddRange(vector.Value);
        }

        public bool Contains(string txId)
        {
            return _outputs.Any(x => x.TxId == txId)
        }


        public IEnumerator<Output> GetEnumerator()
        {
            throw new System.NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }




    }
}