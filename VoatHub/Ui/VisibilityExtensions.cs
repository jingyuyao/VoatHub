using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace VoatHub.Ui
{
    public static class VisibilityExtensions
    {
        public static Visibility Inverse(this Visibility visibility)
        {
            if (visibility == Visibility.Collapsed)
                return Visibility.Visible;
            else
                return Visibility.Collapsed;
        }
    }
}
