using System;
using System.Threading;
using System.Threading.Tasks;
using CafeLib.Core.Runnable;
using Xunit;

namespace CafeLib.Core.UnitTests
{
    public class RecurrentTests
    {
        private DateTime _checkTime;
        private TaskCompletionSource<bool> _taskCompletionSource;

        private Task RecurrentCallback(CancellationToken token)
        {
            _taskCompletionSource.SetResult(true);
            return Task.CompletedTask;
        }

        [Fact]
        public async Task BasicRecurrentTest()
        {
            _taskCompletionSource = new TaskCompletionSource<bool>();
            _checkTime = DateTime.Now;
            using var recurrentTask = new RecurrentTask(RecurrentCallback, TimeSpan.FromSeconds(5), _checkTime);
            await recurrentTask.Start();
            var _ = await _taskCompletionSource.Task;
            await recurrentTask.Stop();
        }
    }
}
