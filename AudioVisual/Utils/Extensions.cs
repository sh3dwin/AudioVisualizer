using System.Windows.Controls;
using System.Windows.Media;

namespace AudioVisual.Utils
{
    public static class Extensions
    {
        public static void Clear(this Canvas c)
        {
            c.Background = new SolidColorBrush(Color.FromRgb(10, 10, 10));
            c.Children.Clear();
        }
    }
}
