namespace CafeLib.Bitcoin.Shared.Units
{
    public interface IAmountExchangeRate 
    {
        public decimal ConvertFromAmount(Amount value);
        public Amount ConvertToAmount(decimal value);
        //public DateTime AsOfWhen { get; }
        //public void UpdateExchangeRate();
    }
}