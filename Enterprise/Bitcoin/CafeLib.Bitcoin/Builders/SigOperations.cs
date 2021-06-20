using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using CafeLib.Bitcoin.Numerics;

namespace CafeLib.Bitcoin.Builders
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
    internal class SigOperations
    {
        private IDictionary<UInt256, (UInt256, uint, uint, string)> _map = new Dictionary<UInt256, (UInt256, uint, uint, string)>();
    }
}
