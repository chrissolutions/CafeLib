using Android.Content;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms;
using Android.Graphics;
using DroneLander.Droid.Renderers;

[assembly: ExportRenderer(typeof(ProgressBar), typeof(FuelControlRenderer))]
namespace DroneLander.Droid.Renderers
{
    public class FuelControlRenderer : ProgressBarRenderer
    {
        public FuelControlRenderer(Context context)
            : base(context)
        {

        }

        protected override void OnElementChanged(ElementChangedEventArgs<ProgressBar> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                Control.ScaleY = 4.0f;
                Control.ProgressDrawable.SetColorFilter(Android.Graphics.Color.Rgb(217, 0, 0), PorterDuff.Mode.SrcIn);
            }
        }
    }
}