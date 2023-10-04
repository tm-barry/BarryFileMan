using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;

namespace BarryFileMan.Helpers
{
    public static class ColorHelper
    {
        public static ReadOnlyCollection<Color> KellysMaxContrastSet
        {
            get { return _kellysMaxContrastSet.AsReadOnly(); }
        }

        private static readonly List<Color> _kellysMaxContrastSet = new()
        {
            UIntToColor(0xFFFFB300), //Vivid Yellow
            UIntToColor(0xFF803E75), //Strong Purple
            UIntToColor(0xFFFF6800), //Vivid Orange
            UIntToColor(0xFFA6BDD7), //Very Light Blue
            UIntToColor(0xFFC10020), //Vivid Red
            UIntToColor(0xFFCEA262), //Grayish Yellow
            UIntToColor(0xFF817066), //Medium Gray

            //The following will not be good for people with defective color vision
            UIntToColor(0xFF007D34), //Vivid Green
            UIntToColor(0xFFF6768E), //Strong Purplish Pink
            UIntToColor(0xFF00538A), //Strong Blue
            UIntToColor(0xFFFF7A5C), //Strong Yellowish Pink
            UIntToColor(0xFF53377A), //Strong Violet
            UIntToColor(0xFFFF8E00), //Vivid Orange Yellow
            UIntToColor(0xFFB32851), //Strong Purplish Red
            UIntToColor(0xFFF4C800), //Vivid Greenish Yellow
            UIntToColor(0xFF7F180D), //Strong Reddish Brown
            UIntToColor(0xFF93AA00), //Vivid Yellowish Green
            UIntToColor(0xFF593315), //Deep Yellowish Brown
            UIntToColor(0xFFF13A13), //Vivid Reddish Orange
            UIntToColor(0xFF232C16), //Dark Olive Green
        };

        public static ReadOnlyCollection<Color> BoyntonOptimized
        {
            get { return _boyntonOptimized.AsReadOnly(); }
        }

        private static readonly List<Color> _boyntonOptimized = new()
        {
            Color.FromArgb(0, 0, 255),      //Blue
            Color.FromArgb(255, 0, 0),      //Red
            Color.FromArgb(0, 255, 0),      //Green
            Color.FromArgb(255, 255, 0),    //Yellow
            Color.FromArgb(255, 0, 255),    //Magenta
            Color.FromArgb(255, 128, 128),  //Pink
            Color.FromArgb(128, 128, 128),  //Gray
            Color.FromArgb(128, 0, 0),      //Brown
            Color.FromArgb(255, 128, 0),    //Orange
        };

        public static Color UIntToColor(uint color)
        {
            var a = (byte)(color >> 24);
            var r = (byte)(color >> 16);
            var g = (byte)(color >> 8);
            var b = (byte)(color >> 0);
            return Color.FromArgb(a, r, g, b);
        }
    }
}
