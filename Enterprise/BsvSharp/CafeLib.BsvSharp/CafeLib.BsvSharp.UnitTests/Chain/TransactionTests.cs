﻿#region Copyright
// Copyright (c) 2020 TonesNotes
// Distributed under the Open BSV software license, see the accompanying file LICENSE.
#endregion

using CafeLib.BsvSharp.Builders;
using CafeLib.BsvSharp.Chain;
using CafeLib.BsvSharp.Scripting;
using Xunit;

namespace CafeLib.BsvSharp.UnitTests.Chain
{
    public class TransactionTests
    {
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

        }
    }
}