using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Platform.UWP;
using YDB.Views;
using YDB.UWP;
using Windows.UI.Xaml.Media;

[assembly: ExportRenderer(typeof(CustomFrame), typeof(CustomFrameRenderer))]
namespace YDB.UWP
{
    public class CustomFrameRenderer : FrameRenderer
    {
        Brush defColor;

        protected override void OnElementChanged(ElementChangedEventArgs<Frame> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                defColor = Control.Background;
                Control.PointerEntered += Control_PointerEntered;
                Control.PointerExited += Control_PointerExited;
            }
        }

        private void Control_PointerExited(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            Windows.UI.Xaml.Controls.Border frame = (sender as Windows.UI.Xaml.Controls.Border);
            SolidColorBrush solidColorBrush = new SolidColorBrush(Windows.UI.Colors.White);
            frame.Background = defColor;
        }

        private void Control_PointerEntered(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            Windows.UI.Xaml.Controls.Border frame = (sender as Windows.UI.Xaml.Controls.Border);
            SolidColorBrush solidColorBrush = new SolidColorBrush(Windows.UI.Colors.LightGray);
            frame.Background = solidColorBrush;
        }
    }
}
