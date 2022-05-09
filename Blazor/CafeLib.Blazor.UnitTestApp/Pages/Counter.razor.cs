using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace CafeLib.Blazor.UnitTestApp.Pages
{
    public partial class Counter : ComponentBase
    {
        private int _currentCount = 0;

        private void IncrementCount()
        {
            _currentCount++;
        }
    }
}
