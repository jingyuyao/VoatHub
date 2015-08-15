using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace VoatHub.Ui
{
    public static class WebViewExtensions
    {
        public static async void ResizeToContent(this WebView webView)
        {
            var heightString = await webView.InvokeScriptAsync("eval", new[] { "document.body.scrollHeight.toString()" });
            int height;
            if (int.TryParse(heightString, out height))
            {
                webView.Height = height;
            }

            var widthString = await webView.InvokeScriptAsync("eval", new[] { "document.body.scrollWidth.toString()" });
            int width;
            if (int.TryParse(widthString, out width))
            {
                webView.Width = width;
            }
        }
    }
}
