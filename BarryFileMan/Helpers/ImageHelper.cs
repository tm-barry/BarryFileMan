using Avalonia.Media.Imaging;
using Avalonia.Platform;
using System;

namespace BarryFileMan.Helpers
{
    public static class ImageHelper
    {
        public static Bitmap LoadFromResource(Uri resourceUri)
        {
            return new Bitmap(AssetLoader.Open(resourceUri));
        }

        public static Bitmap LoadFromResource(string resourceUriString)
        {
            return LoadFromResource(new Uri(resourceUriString));
        }
    }
}
