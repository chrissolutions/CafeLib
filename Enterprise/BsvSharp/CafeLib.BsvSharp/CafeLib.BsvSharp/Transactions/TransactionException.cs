using System;

namespace CafeLib.BsvSharp.Transactions
{
    public class TransactionException : Exception
    {
        public TransactionException(string message)
            : base(message)
        {
        }
    }
}