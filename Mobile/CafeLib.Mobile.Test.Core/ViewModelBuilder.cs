using System.Windows.Input;
using CafeLib.Mobile.Test.Core.Fakes;
using CafeLib.Mobile.ViewModels;
using Moq;

// ReSharper disable UnusedMember.Global

namespace CafeLib.Mobile.Test.Core
{
    public class ViewModelBuilder<T> : FakeBuilderBase<T> where T : BaseViewModel
    {
        public Mock<ICommand> CloseCommandMock { get; set; }

        public ViewModelBuilder(MobileUnitTest test) 
            : base(test)
        {
            CloseCommandMock = new Mock<ICommand>();
            OnCreate = Create;
        }

        public override T Build()
        {
            var vm = OnCreate();
            vm.CloseCommand = CloseCommandMock.Object;
            return vm;
        }
    }
}