using System.Collections.Generic;
using CafeLib.BsvSharp.Numerics;
using CafeLib.BsvSharp.Scripting;

namespace CafeLib.BsvSharp.Builders
{
    /// <summary>
    ///    A map from(transaction hash, output number) to(script chunk index, pubKey).
    ///    When signing a bitcoin transaction, we need to be able to sign an input
    ///    with the correct key and also we need to know where to put signature when we
    ///    get it.This mapping allows us to find the key for an associated input(which
    ///    is identified by tx output hash and number) with which to sign the
    ///    transaction and then also to know where to insert the signature into the
    ///     input script.This gets us the public key, and we need a different method to
    ///     get the private key.That is because we often know the public key to be used
    ///     but may not have access to the private key until the entire tx is sent to
    ///     where the private keys are.
    /// </summary>
    public class SigOperations
    {
        private IDictionary<string, (int, string, string, SignatureHashEnum)[]> _map = new Dictionary<string, (int, string, string, SignatureHashEnum)[]>();

        public SigOperations AddOne(UInt256 txHash, int txOutIndex, int scriptChunkIndex, string type, string addressStr, SignatureHashEnum sigHashType = SignatureHashEnum.All | SignatureHashEnum.ForkId)
        {

            //const arr = this.get(txHashBuf, txOutNum) || []
            //arr.push({
            //    nScriptChunk,
            //    type,
            //    addressStr,
            //    nHashType
            //})
            //this.setMany(txHashBuf, txOutNum, arr)
            return this;
        }
    }
}
