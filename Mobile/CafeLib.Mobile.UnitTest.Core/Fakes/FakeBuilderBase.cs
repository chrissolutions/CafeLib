using System;

// ReSharper disable UnusedMember.Global

namespace CafeLib.Mobile.UnitTest.Core.Fakes
{
    public abstract class FakeBuilderBase<T> where T : class
    {
        protected MobileUnitTest UnitTest { get; }

        protected FakeBuilderBase(MobileUnitTest test)
        {
            UnitTest = test ?? throw new ArgumentNullException(nameof(test));
        }

        public Func<T> OnCreate { protected get; set; }

        public abstract T Build();

        protected virtual T Create()
        {
            return Activator.CreateInstance<T>();
        }
    }
}