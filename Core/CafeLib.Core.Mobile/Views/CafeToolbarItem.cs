using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace CafeLib.Core.Mobile.Views
{
    [Obsolete]
    public class CafeToolbarItem : ToolbarItem
    {
        public static BindableProperty IsVisibleProperty = BindableProperty.CreateAttached(propertyName: "IsVisible",
                                                                                           returnType: typeof(bool),
                                                                                           declaringType: typeof(CafeToolbarItem),
                                                                                           defaultValue: false,
                                                                                           propertyChanged: OnIsVisibleChanged);

        public CafeToolbarItem()
        {
            InitVisibility();
        }

        private async void InitVisibility()
        {
            await Task.Yield();
            //await Task.Delay(100);
            OnIsVisibleChanged(this, false, IsVisible);
        }

        public bool IsVisible
        {
            get { return (bool)GetValue(IsVisibleProperty); }
            set { SetValue(IsVisibleProperty, value); }
        }

        private static void OnIsVisibleChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var item = bindable as CafeToolbarItem;

            var page = item.Parent as ContentPage;

            if (page == null)
                return;

            var items = page.ToolbarItems;

            if (!(bool)newValue)
            {
                item.Text = string.Empty;
            }

            // DT: Note, we cannot remove the item from array, it would cause an exception (because we are in a loop of setting binding context for each item in the array).
            //if ((bool)newValue && !items.Contains(item))
            //{
            //    items.Add(item);
            //}
            //else if (!(bool)newValue && items.Contains(item))
            //{
            //    items.Remove(item);
            //}
        }
    }
}
