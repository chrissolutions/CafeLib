using System;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using CafeLib.Core.Logging;
using CafeLib.Core.Support;
using Xunit;

namespace CafeLib.Core.UnitTests
{
    public class RetryTest
    {
        [Fact]
        public async void MainPathRetry()
        {
            var trials = 0;

            await Retry2.Run(3, 50, async x =>
            {
                trials = x;
                await Task.CompletedTask;
            });

            Assert.Equal(1, trials);
        }

        [Fact]
        public async void ExceptionPathRetry()
        {
            var trials = 0;
            await Assert.ThrowsAsync<AggregateException>(async () =>
            {
                await Retry2.Run(3, 50, async x =>
                {
                    trials = x;
                    await Task.CompletedTask;
                    throw new ArgumentException();
                });
            });

            Assert.Equal(3, trials);
        }
    }
}
