using System;
using System.Globalization;
using CafeLib.Data.UnitTest.ChequeAccess;
using Xunit;

namespace CafeLib.Data.UnitTest
{
    public class DataMappingTests
    {
        [Fact]
        public void Map_To_Entity_From_Model_Test()
        {
            var cheque = new Cheque { ChequeId = "123456", Atm = "ATM123", Status = ChequeStatus.Ready };
            var chequeDto = cheque.ToEntity();
            Assert.Equal("Ready", chequeDto.Status);
        }

        [Fact]
        public void Map_From_Entity_To_Model_Test()
        {
            var chequeDto = new ChequeDto
            {
                Id = 100,
                ChequeId = "123456",
                CreationDate = "3/15/2020",
                Atm = "Atm",
                Status = "Ready",
                IsDeleted = 1
            };

            var cheque = new Cheque();
            cheque.Populate(chequeDto);
            Assert.Equal(100, cheque.Id);
            Assert.Equal("123456", cheque.ChequeId);
            Assert.Equal("Atm", cheque.Atm);
            Assert.Equal(ChequeStatus.Ready, cheque.Status);
            Assert.Equal(DateTime.Parse(chequeDto.CreationDate, CultureInfo.InvariantCulture).Date, cheque.CreationDate.Date);
            Assert.True(cheque.IsDeleted);
        }
    }
}
