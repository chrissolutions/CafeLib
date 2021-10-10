using System.Threading.Tasks;
using CafeLib.BsvSharp.Mapi.MatterPool;
using Xunit;

namespace CafeLib.BsvSharp.Api.UnitTests 
{
    public class MerchantClientTests
    {
        private readonly MatterPoolClient _matterPool = new MatterPoolClient();

        #region Mapi

        [Fact]
        public async Task GetFeeQuotes_Test()
        {
            var response = await _matterPool.GetFeeQuote();
            Assert.NotNull(response);
            Assert.Equal("matterpool", response.Result.ProviderName);
        }

        [Theory]
        [InlineData("3ea6bb35923dbff216aa11084280e0d6d477d78ed8010edac92c3253b3d79024")]
        public async Task GetTransactionStatus_Test(string txHash)
        {
            var response = await _matterPool.GetTransactionStatus(txHash);
            Assert.NotNull(response.Result.Payload);
            Assert.Equal("failure", response.Result.ReturnResult);
        }

        //[Theory]
        //[InlineData("010000000200010000000000000000000000000000000000000000000000000000000000000000000049483045022100d180fd2eb9140aeb4210c9204d3f358766eb53842b2a9473db687fa24b12a3cc022079781799cd4f038b85135bbe49ec2b57f306b2bb17101b17f71f000fcab2b6fb01ffffffff0002000000000000000000000000000000000000000000000000000000000000000000004847304402205f7530653eea9b38699e476320ab135b74771e1c48b81a5d041e2ca84b9be7a802200ac8d1f40fb026674fe5a5edd3dea715c27baa9baca51ed45ea750ac9dc0a55e81ffffffff010100000000000000015100000000")]
        //public async Task Broadcast_Test(string txRaw)
        //{
        //    var response = await Api.BroadcastTransaction(txRaw);
        //    Assert.False(response.IsSuccessful);
        //    Assert.Contains("dust", response.GetException<WebRequestException>().Response.Content);
        //}

        #endregion
    }
}
