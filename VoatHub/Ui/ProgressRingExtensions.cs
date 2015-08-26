using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace VoatHub.Ui
{
    public static class ProgressRingExtensions
    {
        public static void Toggle(this ProgressRing ring)
        {
            ring.Visibility = ring.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
            ring.IsActive = ring.IsActive ? false : true;
        }
    }
}
