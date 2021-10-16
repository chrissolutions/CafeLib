using System;
using System.Threading.Tasks;
using CafeLib.Core.Support;
using Xunit;

namespace CafeLib.Core.UnitTests
{
    public class RetryTest
    {
        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async void ActionRetry(bool doException)
        {
            var trials = 0;

            try
            {
                await Retry.Run(3, 50, x =>
                {
                    trials = x;
                    if (doException)
                    {
                        throw new ArgumentException();
                    }
                });

                Assert.Equal(1, trials);
            }
            catch (Exception e)
            {
                Assert.IsType<AggregateException>(e);
                Assert.Equal(3, trials);
            }
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async void ActionWithDefaultParameters(bool doException)
        {
            var trials = 0;

            try
            {
                await Retry.Run(x =>
                {
                    trials = x;
                    if (doException)
                    {
                        throw new ArgumentException();
                    }
                });

                Assert.Equal(1, trials);
            }
            catch (Exception e)
            {
                Assert.IsType<AggregateException>(e);
                Assert.Equal(3, trials);
            }
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async void FunctionWithReturnRetry(bool doException)
        {
            var trials = 0;

            try
            {
                var result = await Retry.Run(3, 50, x =>
                {
                    trials = x;
                    if (doException)
                    {
                        throw new ArgumentException();
                    }

                    return 10;
                });

                Assert.Equal(1, trials);
                Assert.Equal(10, result);
            }
            catch (Exception e)
            {
                Assert.IsType<AggregateException>(e);
                Assert.Equal(3, trials);
            }
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async void FunctionWithReturnRetryWithDefaults(bool doException)
        {
            var trials = 0;

            try
            {
                var result = await Retry.Run(x =>
                {
                    trials = x;
                    if (doException)
                    {
                        throw new ArgumentException();
                    }

                    return 10;
                });

                Assert.Equal(1, trials);
                Assert.Equal(10, result);
            }
            catch (Exception e)
            {
                Assert.IsType<AggregateException>(e);
                Assert.Equal(3, trials);
            }
        }

        [Fact]
        public async void TaskMainPathRetry()
        {
            var trials = 0;

            await Retry.Run(3, 50, async x =>
            {
                trials = x;
                await Task.CompletedTask;
            });

            Assert.Equal(1, trials);
        }

        [Fact]
        public async void TaskExceptionPathRetry()
        {
            var trials = 0;
            await Assert.ThrowsAsync<AggregateException>(async () =>
            {
                await Retry.Run(3, 50, async x =>
                {
                    trials = x;
                    await Task.CompletedTask;
                    throw new ArgumentException();
                });
            });

            Assert.Equal(3, trials);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async void TaskRetryWithDefaults(bool doException)
        {
            var trials = 0;

            try
            {
                await Retry.Run(async x =>
                {
                    trials = x;
                    if (doException)
                    {
                        throw new ArgumentException();
                    }

                    return await Task.FromResult(10);
                });

                Assert.Equal(1, trials);
            }
            catch (Exception e)
            {
                Assert.IsType<AggregateException>(e);
                Assert.Equal(3, trials);
            }
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async void TaskWithReturnRetry(bool doException)
        {
            var trials = 0;

            try
            {
                var result = await Retry.Run(3, 50, async x =>
                {
                    trials = x;
                    if (doException)
                    {
                        throw new ArgumentException();
                    }

                    return await Task.FromResult(10);
                });

                Assert.Equal(1, trials);
                Assert.Equal(10, result);
            }
            catch (Exception e)
            {
                Assert.IsType<AggregateException>(e);
                Assert.Equal(3, trials);
            }
        }
    }
}
