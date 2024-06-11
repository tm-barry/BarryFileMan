using Avalonia.Media;
using Avalonia.Media.Immutable;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace BarryFileMan.Helpers
{
    public static class ColorHelper
    {
        // Colors
        public static readonly Color VividYellow = UIntToColor(0xFFFFB300);
        public static readonly Color StrongPurple = UIntToColor(0xFF803E75);
        public static readonly Color VividOrange = UIntToColor(0xFFFF6800);
        public static readonly Color VeryLightBlue = UIntToColor(0xFFA6BDD7);
        public static readonly Color VividRed = UIntToColor(0xFFC10020);
        public static readonly Color GreyishYellow = UIntToColor(0xFFCEA262);
        public static readonly Color MediumGrey = UIntToColor(0xFF817066);
        public static readonly Color VividGreen = UIntToColor(0xFF007D34);
        public static readonly Color StrongPurplishPink = UIntToColor(0xFFF6768E);
        public static readonly Color StrongBlue = UIntToColor(0xFF00538A);
        public static readonly Color StrongYellowishPink = UIntToColor(0xFFFF7A5C);
        public static readonly Color StrongViolet = UIntToColor(0xFF53377A);
        public static readonly Color VividOrangeYellow = UIntToColor(0xFFFF8E00);
        public static readonly Color StrongPurplishRed = UIntToColor(0xFFB32851);
        public static readonly Color VividGreenishYellow = UIntToColor(0xFFF4C800);
        public static readonly Color StrongReddishBrown = UIntToColor(0xFF7F180D);
        public static readonly Color VividYellowishGreen = UIntToColor(0xFF93AA00);
        public static readonly Color DeepYellowishBrown = UIntToColor(0xFF593315);
        public static readonly Color VividReddishOrange = UIntToColor(0xFFF13A13);
        public static readonly Color DarkOliveGreen = UIntToColor(0xFF232C16);

        // Brushes
        public static readonly IBrush VividYellowBrush = ColorToBrush(VividYellow);
        public static readonly IBrush StrongPurpleBrush = ColorToBrush(StrongPurple);
        public static readonly IBrush VividOrangeBrush = ColorToBrush(VividOrange);
        public static readonly IBrush VeryLightBlueBrush = ColorToBrush(VeryLightBlue);
        public static readonly IBrush VividRedBrush = ColorToBrush(VividRed);
        public static readonly IBrush GreyishYellowBrush = ColorToBrush(GreyishYellow);
        public static readonly IBrush MediumGreyBrush = ColorToBrush(MediumGrey);
        public static readonly IBrush VividGreenBrush = ColorToBrush(VividGreen);
        public static readonly IBrush StrongPurplishPinkBrush = ColorToBrush(StrongPurplishPink);
        public static readonly IBrush StrongBlueBrush = ColorToBrush(StrongBlue);
        public static readonly IBrush StrongYellowishPinkBrush = ColorToBrush(StrongYellowishPink);
        public static readonly IBrush StrongVioletBrush = ColorToBrush(StrongViolet);
        public static readonly IBrush VividOrangeYellowBrush = ColorToBrush(VividOrangeYellow);
        public static readonly IBrush StrongPurplishRedBrush = ColorToBrush(StrongPurplishRed);
        public static readonly IBrush VividGreenishYellowBrush = ColorToBrush(VividGreenishYellow);
        public static readonly IBrush StrongReddishBrownBrush = ColorToBrush(StrongReddishBrown);
        public static readonly IBrush VividYellowishGreenBrush = ColorToBrush(VividYellowishGreen);
        public static readonly IBrush DeepYellowishBrownBrush = ColorToBrush(DeepYellowishBrown);
        public static readonly IBrush VividReddishOrangeBrush = ColorToBrush(VividReddishOrange);
        public static readonly IBrush DarkOliveGreenBrush = ColorToBrush(DarkOliveGreen);

        public static ReadOnlyCollection<Color> KellysMaxContrastSet
        {
            get { return _kellysMaxContrastSet.AsReadOnly(); }
        }

        private static readonly List<Color> _kellysMaxContrastSet = new()
        {
            VividYellow,
            StrongPurple,
            VividOrange,
            VeryLightBlue,
            VividRed,
            GreyishYellow,
            MediumGrey,

            //The following will not be good for people with defective color vision
            VividGreen,
            StrongPurplishPink,
            StrongBlue,
            StrongYellowishPink,
            StrongViolet,
            VividOrangeYellow,
            StrongPurplishRed,
            VividGreenishYellow,
            StrongReddishBrown,
            VividYellowishGreen,
            DeepYellowishBrown,
            VividReddishOrange,
            DarkOliveGreen
        };

        public static Color UIntToColor(uint color)
        {
            var a = (byte)(color >> 24);
            var r = (byte)(color >> 16);
            var g = (byte)(color >> 8);
            var b = (byte)(color >> 0);
            return Color.FromArgb(a, r, g, b);
        }

        public static IBrush ColorToBrush(Color color)
        {
            return new ImmutableSolidColorBrush(color);
        }
    }
}
