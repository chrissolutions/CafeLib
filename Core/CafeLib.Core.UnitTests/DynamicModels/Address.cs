namespace CafeLib.Core.UnitTests.DynamicModels
{
    public class Address
    {
        public string FullAddress { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }

        public Address()
        {
            FullAddress = "32 Kaiea";
            Phone = "808 132-3456";
            Email = "rick@whatsa.com";
        }
    }
}