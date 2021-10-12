using Android.Content;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms;
using Android.Graphics;
using DroneLander.Droid.Renderers;
using Android.Graphics.Drawables;
using Android.Support.V4.Content;

[assembly: ExportRenderer(typeof(Slider), typeof(ThrottleControlRenderer))]
namespace DroneLander.Droid.Renderers
{
    public class ThrottleControlRenderer : SliderRenderer
    {
        public ThrottleControlRenderer(Context context)
            : base(context)
        {
            
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Slider> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                Control.ProgressDrawable.SetColorFilter(Android.Graphics.Color.Rgb(217, 0, 0), PorterDuff.Mode.SrcIn);
                Drawable myThumb = ContextCompat.GetDrawable(Context, Resource.Drawable.throttle_thumb);
                Control.SetThumb(myThumb);
            }
        }
    }
}