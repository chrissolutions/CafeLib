using System;
using System.Collections.Generic;
using CafeLib.Core.IoC;

namespace CafeLib.Core.UnitTests.Services
{
    public interface IBarService : IServiceProvider
    {
        void DoSomeRealWork();
    }

    public class BarService : ServiceBase, IBarService
    {
        private readonly IFooService _fooService;
        private readonly IServiceProvider _serviceProvider;

        public BarService(IFooService fooService, IServiceProvider serviceProvider)
        {
            _fooService = fooService;
            _serviceProvider = serviceProvider;
        }

        public void DoSomeRealWork()
        {
            var list = new List<int>();
            for (int i = 0; i < 10; i++)
            {
                _fooService.DoThing(i);
                list.Add(i);
            }

            var array = list.ToArray();
            TestArgs(100, array);
        }

        public void TestArgs(params object[] args)
        {
            var a = args;
        }
    }
}