﻿using System;
using Android.Content;
using AndroidX.AppCompat.Widget;
using AndroidX.Fragment.App;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using NavigationPageRenderer = CafeLib.Mobile.Android.Renderers.NavigationPageRenderer;

[assembly: ExportRenderer(typeof(NavigationPage), typeof(NavigationPageRenderer))]

namespace CafeLib.Mobile.Android.Renderers
{
    public class NavigationPageRenderer : Xamarin.Forms.Platform.Android.AppCompat.NavigationPageRenderer
    {
        public NavigationPageRenderer(Context context)
            : base(context)
        {
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                base.Dispose(disposing);
            }
            catch (Exception ex)
            {
                // If you have taken a photo then cancel the inspection screen this throws an exception
                // so handle it and not crash the app. This happens every time so no need to log to
                // insights as it will just be noise.
                System.Diagnostics.Debug.WriteLine($"Exception when disposing navigation page - {ex.Message}");
            }
        }

        protected override void OnAttachedToWindow()
        {
            base.OnAttachedToWindow();

            // See if the Element is really a master-detail page.
            var page = FindFlyoutPage();

            var clickListener = page != null
                ? new FlyoutPageMenuClickListener(Element, Platform.GetRenderer(page) as FlyoutPageRenderer)
                : (IOnClickListener) new NavigationClickListener(Element);

            var toolbar = FindToolbar();
            toolbar?.SetNavigationOnClickListener(clickListener);
        }

        protected override void SetupPageTransition(FragmentTransaction transaction, bool isPush)
        {
            if (isPush)
                transaction.SetCustomAnimations(global::Android.Resource.Animation.SlideInLeft, global::Android.Resource.Animation.SlideInLeft, 0, 0);
            else
                transaction.SetCustomAnimations(0, global::Android.Resource.Animation.SlideOutRight, 0, 0);
        }

        private Page FindFlyoutPage()
        {
            for (var element = Element.Parent; element != null; element = element.Parent)
            {
                if (element is FlyoutPage page)
                {
                    return page;
                }
            }

            return null;
        }

        private Toolbar FindToolbar()
        {
            Toolbar toolbar = null;

            for (var ii = 0; ii < ChildCount; ++ii)
            {
                var child = GetChildAt(ii);
                toolbar = child as Toolbar;
                if (toolbar != null)
                {
                    break;
                }
            }

            return toolbar;
        }
    }
}