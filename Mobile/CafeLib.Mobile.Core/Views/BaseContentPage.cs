using System;
using System.Runtime.CompilerServices;
using Xamarin.Forms;
using CafeLib.Mobile.Core.ViewModels;

namespace CafeLib.Mobile.Core.Views
{
    public abstract class BaseContentPage<T> : ContentPage where T : AbstractViewModel
    {
        public T ViewModel => BindingContext as T;

        protected BaseContentPage()
        {
        }

        protected BaseContentPage(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null) throw new ArgumentNullException(nameof(serviceProvider));
        }

        protected override void ChangeVisualState()
        {
            base.ChangeVisualState();
        }

        protected override void InvalidateMeasure()
        {
            base.InvalidateMeasure();
        }

        protected override void LayoutChildren(double x, double y, double width, double height)
        {
            base.LayoutChildren(x, y, width, height);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }

        protected override bool OnBackButtonPressed()
        {
            return base.OnBackButtonPressed();
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
        }

        protected override void OnChildAdded(Element child)
        {
            base.OnChildAdded(child);
        }

        protected override void OnChildMeasureInvalidated(object sender, EventArgs e)
        {
            base.OnChildMeasureInvalidated(sender, e);
        }

        protected override void OnChildRemoved(Element child)
        {
            base.OnChildRemoved(child);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }

        protected override SizeRequest OnMeasure(double widthConstraint, double heightConstraint)
        {
            return base.OnMeasure(widthConstraint, heightConstraint);
        }

        protected override void OnParentSet()
        {
            base.OnParentSet();
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
        }

        protected override void OnPropertyChanging([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanging(propertyName);
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
        }

        protected override void OnTabIndexPropertyChanged(int oldValue, int newValue)
        {
            base.OnTabIndexPropertyChanged(oldValue, newValue);
        }

        protected override void OnTabStopPropertyChanged(bool oldValue, bool newValue)
        {
            base.OnTabStopPropertyChanged(oldValue, newValue);
        }

        protected override int TabIndexDefaultValueCreator()
        {
            return base.TabIndexDefaultValueCreator();
        }

        protected override bool TabStopDefaultValueCreator()
        {
            return base.TabStopDefaultValueCreator();
        }
    }
}
