using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CafeLib.Core.Collections;
using CafeLib.Core.Extensions;
using CafeLib.Core.Runnable;
using Xunit;

namespace CafeLib.Core.UnitTests
{
    public class RecurrentTests
    {
        private DateTime _checkTime;
        private TaskCompletionSource<bool> _taskCompletionSource;

        private Task RecurrentCallback()
        {
            Assert.Equal((_checkTime + TimeSpan.FromSeconds(5)).TruncateMilliseconds(), DateTime.Now.TruncateMilliseconds());
            _taskCompletionSource.SetResult(true);
            return Task.CompletedTask;
        }

        [Fact]
        public async Task BasicRecurrentTest()
        {
            _taskCompletionSource = new TaskCompletionSource<bool>();
            _checkTime = DateTime.Now;
            var recurrentTask = new RecurrentTask(RecurrentCallback, TimeSpan.FromSeconds(5), _checkTime);
            await recurrentTask.Start();
            await _taskCompletionSource.Task;
            await recurrentTask.Stop();
        }
    }
}
