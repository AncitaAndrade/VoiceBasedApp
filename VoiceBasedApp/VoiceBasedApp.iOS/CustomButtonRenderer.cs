using UIKit;
using Xamarin.Forms.Platform.iOS;

namespace VoiceBasedApp.iOS
{
    public class CustomButtonRenderer : ButtonRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Button> e)
        {
            base.OnElementChanged(e);

            var customButton = e.NewElement as CustomButton;

            var thisButton = Control as UIButton;
            thisButton.TouchDown += delegate
            {
                customButton.OnPressed();
            };
            thisButton.TouchUpInside += delegate
            {
                customButton.OnReleased();
            };
        }
    }
}