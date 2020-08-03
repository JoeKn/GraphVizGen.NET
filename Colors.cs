// ====================================================================================
// File: Colors.cs / GraphViz C# .NET Generator
//
// Copyright 2020 Jörg Knura
// Copyright 2020 Qubes GmbH, Frankfurt(Main), Germany
// 
// Redistribution and use in source and binary forms, with or without modification, are
// permitted provided that the following conditions are met:
// 
// 1. Redistributions of source code must retain the above copyright notice, this list
//    of conditions and the following disclaimer.
// 
// 2. Redistributions in binary form must reproduce the above copyright notice, this
//    list of conditions and the following disclaimer in the documentation and/or
//    other materials provided with the distribution.
// 
// 3. Neither the names of the copyright holders nor the names of its contributors may
//    be used to endorse or promote products derived from this software without
//    specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY
// EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES
// OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT
// SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT,
// INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED
// TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR
// BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
// CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH
// DAMAGE.
// ====================================================================================

using System;
using GraphViz.Values;

namespace GraphViz.Colors
{
    /// <summary>
    /// A class representing RGB (red, green, blue) colors
    /// </summary>
    public class RGB : GVColor
    {
        /// <summary>The red color component of the RGB color.</summary>
        protected byte red;
        /// <summary>The green color component of the RGB color.</summary>
        protected byte green;
        /// <summary>The blue color component of the RGB color.</summary>
        protected byte blue;

        /// <summary>The accessor for the red color component.</summary>
        public byte Red { get => red; }
        /// <summary>The accessor for the green color component.</summary>
        public byte Green { get => green; }
        /// <summary>The accessor for the blue color component.</summary>
        public byte Blue { get => blue; }


        /// <summary>
        /// Constructs a RGB color from single color values.
        /// </summary>
        /// <param name="red">The value of the red color component.</param>
        /// <param name="green">The value of the green color component.</param>
        /// <param name="blue">The value of the blue color component.</param>
        public RGB( byte red, byte green, byte blue)
        {
            this.red = red;
            this.green = green;
            this.blue = blue;
        }

        /// <summary>
        /// Builds the text representation of the RGB color
        /// </summary>
        /// <returns>A text representation of the RGB color in the form "#XXXXXX"</returns>
        public override string ToString()
        {
            return String.Format("#{0,2:X}{1,2:X}{2,2:X}", red, green, blue);
        }

    }

    /// <summary>
    /// A class that extens an RGB color with an alpha channel for transparency
    /// </summary>
    /// <inheritdoc/>
    public class RGBA : RGB
    {
        /// <summary>
        /// The tarnsparency value
        /// </summary>
        protected byte Alpha;

        /// <summary>
        /// Constructs a RGBA color from single color and transparency values.
        /// </summary>
        /// <param name="red">The value of the red color component.</param>
        /// <param name="green">The value of the green color component.</param>
        /// <param name="blue">The value of the blue color component.</param>
        /// <param name="alpha">The transparency value.</param>
        public RGBA(byte red, byte green, byte blue, byte alpha) : base(red, green, blue)
        {
            Alpha = alpha;
        }

        /// <summary>
        /// Constructs a RGBA color from a RGB color (its single color values) and transparency value.
        /// </summary>
        /// <param name="color">A RGB color.</param>
        /// <param name="alpha">The transparency value.</param>
        public RGBA(RGB color, byte alpha) : base(color.Red, color.Green, color.Blue)
        {
            Alpha = alpha;
        }

        /// <summary>
        /// Builds the text representation of the RGBA color
        /// </summary>
        /// <returns>A text representation of the RGB color in the form "#XXXXXXXX"</returns>
        public override string ToString()
        {
            return base.ToString() + String.Format("{0,2:X}", Alpha);
        }
    }

    /// <summary>
    /// A class that represents a HSV (Hue, Saturation, Value) color
    /// </summary>
    public class HSV : RGB
    {
        /// <summary>
        /// The Hue component of the color. The value range is 0.0 to 1.0.
        /// </summary>
        protected double Hue;
        /// <summary>
        /// The Saturation component of the color. The value range is 0.0 to 1.0.
        /// </summary>
        protected double Saturation;
        /// <summary>
        /// The Value component of the color. The value range is 0.0 to 1.0.
        /// </summary>
        protected double Value;

        /// <summary>
        /// Constructs a HSV color that is internaly transformed to a RGB color.
        /// </summary>
        /// <see href="https://www.rapidtables.com/convert/color/hsv-to-rgb.html"/>
        /// <param name="hue">The Hue component of the color. The value range is 0.0 to 1.0.</param>
        /// <param name="saturation">The Saturation component of the color. The value range is 0.0 to 1.0.</param>
        /// <param name="value">The Value component of the color. The value range is 0.0 to 1.0.</param>
        /// <exception cref="GVException">Thrown when outside the value ranges</exception>
        public HSV(double hue, double saturation, double value) : base (0, 0, 0)
        {
            if ((hue < 0.0) || (hue > 1.0)) throw new GVException("HSV Color: Hue out of range (0.0 - 1.0)");
            if ((saturation < 0.0) || (saturation > 1.0)) throw new GVException("HSV Color: Satruration out of range (0.0 - 1.0)");
            if ((value < 0.0) || (value > 1.0)) throw new GVException("HSV Color: Value out of range (0.0 - 1.0)");

            if (hue == 1.0) hue = 0.0;
            Hue = hue;
            Saturation = saturation;
            Value = value;

            double h = hue * 360.0;
            double c = value * saturation;
            double x = c * (1.0 - Math.Abs(((h / 60.0) % 2.0) - 1.0));
            double m = value - c;
            double r = 0.0;
            double g = 0.0;
            double b = 0.0;
            if ((h >=   0.0) && (h <  60.0)) (r, g, b) = (  c,   x, 0.0);
            if ((h >=  60.0) && (h < 120.0)) (r, g, b) = (  x,   c, 0.0);
            if ((h >= 120.0) && (h < 180.0)) (r, g, b) = (0.0,   c,   x);
            if ((h >= 180.0) && (h < 240.0)) (r, g, b) = (0.0,   x,   c);
            if ((h >= 240.0) && (h < 300.0)) (r, g, b) = (  x, 0.0,   c);
            if ((h >= 300.0) && (h < 360.0)) (r, g, b) = (  c, 0.0,   x);
            (red, green, blue) = ((byte)((r + m) * 255.0), (byte)((g + m) * 255.0), (byte)((b + m) * 255.0));
        }
    }

   /// <summary>
   /// This class only holds prdefined RGB color objects for each color defined
   /// foir the X11 windows system.
   /// </summary>
    public static class X11
    {
        ///<summary>RGB representation of X11 color named Snow</summary>
        public static readonly RGB Snow = new RGB(255, 250, 250);
        ///<summary>RGB representation of X11 color named GhostWhite</summary>
        public static readonly RGB GhostWhite = new RGB(248, 248, 255);
        ///<summary>RGB representation of X11 color named WhiteSmoke</summary>
        public static readonly RGB WhiteSmoke = new RGB(245, 245, 245);
        ///<summary>RGB representation of X11 color named Gainsboro</summary>
        public static readonly RGB Gainsboro = new RGB(220, 220, 220);
        ///<summary>RGB representation of X11 color named FloralWhite</summary>
        public static readonly RGB FloralWhite = new RGB(255, 250, 240);
        ///<summary>RGB representation of X11 color named OldLace</summary>
        public static readonly RGB OldLace = new RGB(253, 245, 230);
        ///<summary>RGB representation of X11 color named Linen</summary>
        public static readonly RGB Linen = new RGB(250, 240, 230);
        ///<summary>RGB representation of X11 color named AntiqueWhite</summary>
        public static readonly RGB AntiqueWhite = new RGB(250, 235, 215);
        ///<summary>RGB representation of X11 color named PapayaWhip</summary>
        public static readonly RGB PapayaWhip = new RGB(255, 239, 213);
        ///<summary>RGB representation of X11 color named BlanchedAlmond</summary>
        public static readonly RGB BlanchedAlmond = new RGB(255, 235, 205);
        ///<summary>RGB representation of X11 color named Bisque</summary>
        public static readonly RGB Bisque = new RGB(255, 228, 196);
        ///<summary>RGB representation of X11 color named PeachPuff</summary>
        public static readonly RGB PeachPuff = new RGB(255, 218, 185);
        ///<summary>RGB representation of X11 color named NavajoWhite</summary>
        public static readonly RGB NavajoWhite = new RGB(255, 222, 173);
        ///<summary>RGB representation of X11 color named Moccasin</summary>
        public static readonly RGB Moccasin = new RGB(255, 228, 181);
        ///<summary>RGB representation of X11 color named Cornsilk</summary>
        public static readonly RGB Cornsilk = new RGB(255, 248, 220);
        ///<summary>RGB representation of X11 color named Ivory</summary>
        public static readonly RGB Ivory = new RGB(255, 255, 240);
        ///<summary>RGB representation of X11 color named LemonChiffon</summary>
        public static readonly RGB LemonChiffon = new RGB(255, 250, 205);
        ///<summary>RGB representation of X11 color named Seashell</summary>
        public static readonly RGB Seashell = new RGB(255, 245, 238);
        ///<summary>RGB representation of X11 color named Honeydew</summary>
        public static readonly RGB Honeydew = new RGB(240, 255, 240);
        ///<summary>RGB representation of X11 color named MintCream</summary>
        public static readonly RGB MintCream = new RGB(245, 255, 250);
        ///<summary>RGB representation of X11 color named Azure</summary>
        public static readonly RGB Azure = new RGB(240, 255, 255);
        ///<summary>RGB representation of X11 color named AliceBlue</summary>
        public static readonly RGB AliceBlue = new RGB(240, 248, 255);
        ///<summary>RGB representation of X11 color named Lavender</summary>
        public static readonly RGB Lavender = new RGB(230, 230, 250);
        ///<summary>RGB representation of X11 color named LavenderBlush</summary>
        public static readonly RGB LavenderBlush = new RGB(255, 240, 245);
        ///<summary>RGB representation of X11 color named MistyRose</summary>
        public static readonly RGB MistyRose = new RGB(255, 228, 225);
        ///<summary>RGB representation of X11 color named White</summary>
        public static readonly RGB White = new RGB(255, 255, 255);
        ///<summary>RGB representation of X11 color named DimGray</summary>
        public static readonly RGB DimGray = new RGB(105, 105, 105);
        ///<summary>RGB representation of X11 color named DimGrey</summary>
        public static readonly RGB DimGrey = new RGB(105, 105, 105);
        ///<summary>RGB representation of X11 color named SlateGray</summary>
        public static readonly RGB SlateGray = new RGB(112, 128, 144);
        ///<summary>RGB representation of X11 color named SlateGrey</summary>
        public static readonly RGB SlateGrey = new RGB(112, 128, 144);
        ///<summary>RGB representation of X11 color named LightSlateGray</summary>
        public static readonly RGB LightSlateGray = new RGB(119, 136, 153);
        ///<summary>RGB representation of X11 color named LightSlateGrey</summary>
        public static readonly RGB LightSlateGrey = new RGB(119, 136, 153);
        ///<summary>RGB representation of X11 color named Gray</summary>
        public static readonly RGB Gray = new RGB(190, 190, 190);
        ///<summary>RGB representation of X11 color named Grey</summary>
        public static readonly RGB Grey = new RGB(190, 190, 190);
        ///<summary>RGB representation of X11 color named LightGrey</summary>
        public static readonly RGB LightGrey = new RGB(211, 211, 211);
        ///<summary>RGB representation of X11 color named LightGray</summary>
        public static readonly RGB LightGray = new RGB(211, 211, 211);
        ///<summary>RGB representation of X11 color named CornflowerBlue</summary>
        public static readonly RGB CornflowerBlue = new RGB(100, 149, 237);
        ///<summary>RGB representation of X11 color named SlateBlue</summary>
        public static readonly RGB SlateBlue = new RGB(106, 90, 205);
        ///<summary>RGB representation of X11 color named MediumSlateBlue</summary>
        public static readonly RGB MediumSlateBlue = new RGB(123, 104, 238);
        ///<summary>RGB representation of X11 color named LightSlateBlue</summary>
        public static readonly RGB LightSlateBlue = new RGB(132, 112, 255);
        ///<summary>RGB representation of X11 color named SkyBlue</summary>
        public static readonly RGB SkyBlue = new RGB(135, 206, 235);
        ///<summary>RGB representation of X11 color named LightSkyBlue</summary>
        public static readonly RGB LightSkyBlue = new RGB(135, 206, 250);
        ///<summary>RGB representation of X11 color named LightSteelBlue</summary>
        public static readonly RGB LightSteelBlue = new RGB(176, 196, 222);
        ///<summary>RGB representation of X11 color named LightBlue</summary>
        public static readonly RGB LightBlue = new RGB(173, 216, 230);
        ///<summary>RGB representation of X11 color named PowderBlue</summary>
        public static readonly RGB PowderBlue = new RGB(176, 224, 230);
        ///<summary>RGB representation of X11 color named PaleTurquoise</summary>
        public static readonly RGB PaleTurquoise = new RGB(175, 238, 238);
        ///<summary>RGB representation of X11 color named LightCyan</summary>
        public static readonly RGB LightCyan = new RGB(224, 255, 255);
        ///<summary>RGB representation of X11 color named MediumAquamarine</summary>
        public static readonly RGB MediumAquamarine = new RGB(102, 205, 170);
        ///<summary>RGB representation of X11 color named Aquamarine</summary>
        public static readonly RGB Aquamarine = new RGB(127, 255, 212);
        ///<summary>RGB representation of X11 color named DarkSeaGreen</summary>
        public static readonly RGB DarkSeaGreen = new RGB(143, 188, 143);
        ///<summary>RGB representation of X11 color named PaleGreen</summary>
        public static readonly RGB PaleGreen = new RGB(152, 251, 152);
        ///<summary>RGB representation of X11 color named LawnGreen</summary>
        public static readonly RGB LawnGreen = new RGB(124, 252, 0);
        ///<summary>RGB representation of X11 color named Chartreuse</summary>
        public static readonly RGB Chartreuse = new RGB(127, 255, 0);
        ///<summary>RGB representation of X11 color named GreenYellow</summary>
        public static readonly RGB GreenYellow = new RGB(173, 255, 47);
        ///<summary>RGB representation of X11 color named YellowGreen</summary>
        public static readonly RGB YellowGreen = new RGB(154, 205, 50);
        ///<summary>RGB representation of X11 color named OliveDrab</summary>
        public static readonly RGB OliveDrab = new RGB(107, 142, 35);
        ///<summary>RGB representation of X11 color named DarkKhaki</summary>
        public static readonly RGB DarkKhaki = new RGB(189, 183, 107);
        ///<summary>RGB representation of X11 color named Khaki</summary>
        public static readonly RGB Khaki = new RGB(240, 230, 140);
        ///<summary>RGB representation of X11 color named PaleGoldenrod</summary>
        public static readonly RGB PaleGoldenrod = new RGB(238, 232, 170);
        ///<summary>RGB representation of X11 color named LightGoldenrodYellow</summary>
        public static readonly RGB LightGoldenrodYellow = new RGB(250, 250, 210);
        ///<summary>RGB representation of X11 color named LightYellow</summary>
        public static readonly RGB LightYellow = new RGB(255, 255, 224);
        ///<summary>RGB representation of X11 color named Yellow</summary>
        public static readonly RGB Yellow = new RGB(255, 255, 0);
        ///<summary>RGB representation of X11 color named Gold</summary>
        public static readonly RGB Gold = new RGB(255, 215, 0);
        ///<summary>RGB representation of X11 color named LightGoldenrod</summary>
        public static readonly RGB LightGoldenrod = new RGB(238, 221, 130);
        ///<summary>RGB representation of X11 color named Goldenrod</summary>
        public static readonly RGB Goldenrod = new RGB(218, 165, 32);
        ///<summary>RGB representation of X11 color named DarkGoldenrod</summary>
        public static readonly RGB DarkGoldenrod = new RGB(184, 134, 11);
        ///<summary>RGB representation of X11 color named RosyBrown</summary>
        public static readonly RGB RosyBrown = new RGB(188, 143, 143);
        ///<summary>RGB representation of X11 color named IndianRed</summary>
        public static readonly RGB IndianRed = new RGB(205, 92, 92);
        ///<summary>RGB representation of X11 color named SaddleBrown</summary>
        public static readonly RGB SaddleBrown = new RGB(139, 69, 19);
        ///<summary>RGB representation of X11 color named Sienna</summary>
        public static readonly RGB Sienna = new RGB(160, 82, 45);
        ///<summary>RGB representation of X11 color named Peru</summary>
        public static readonly RGB Peru = new RGB(205, 133, 63);
        ///<summary>RGB representation of X11 color named Burlywood</summary>
        public static readonly RGB Burlywood = new RGB(222, 184, 135);
        ///<summary>RGB representation of X11 color named Beige</summary>
        public static readonly RGB Beige = new RGB(245, 245, 220);
        ///<summary>RGB representation of X11 color named Wheat</summary>
        public static readonly RGB Wheat = new RGB(245, 222, 179);
        ///<summary>RGB representation of X11 color named SandyBrown</summary>
        public static readonly RGB SandyBrown = new RGB(244, 164, 96);
        ///<summary>RGB representation of X11 color named Tan</summary>
        public static readonly RGB Tan = new RGB(210, 180, 140);
        ///<summary>RGB representation of X11 color named Chocolate</summary>
        public static readonly RGB Chocolate = new RGB(210, 105, 30);
        ///<summary>RGB representation of X11 color named Firebrick</summary>
        public static readonly RGB Firebrick = new RGB(178, 34, 34);
        ///<summary>RGB representation of X11 color named Brown</summary>
        public static readonly RGB Brown = new RGB(165, 42, 42);
        ///<summary>RGB representation of X11 color named DarkSalmon</summary>
        public static readonly RGB DarkSalmon = new RGB(233, 150, 122);
        ///<summary>RGB representation of X11 color named Salmon</summary>
        public static readonly RGB Salmon = new RGB(250, 128, 114);
        ///<summary>RGB representation of X11 color named LightSalmon</summary>
        public static readonly RGB LightSalmon = new RGB(255, 160, 122);
        ///<summary>RGB representation of X11 color named Orange</summary>
        public static readonly RGB Orange = new RGB(255, 165, 0);
        ///<summary>RGB representation of X11 color named DarkOrange</summary>
        public static readonly RGB DarkOrange = new RGB(255, 140, 0);
        ///<summary>RGB representation of X11 color named Coral</summary>
        public static readonly RGB Coral = new RGB(255, 127, 80);
        ///<summary>RGB representation of X11 color named LightCoral</summary>
        public static readonly RGB LightCoral = new RGB(240, 128, 128);
        ///<summary>RGB representation of X11 color named Tomato</summary>
        public static readonly RGB Tomato = new RGB(255, 99, 71);
        ///<summary>RGB representation of X11 color named OrangeRed</summary>
        public static readonly RGB OrangeRed = new RGB(255, 69, 0);
        ///<summary>RGB representation of X11 color named Red</summary>
        public static readonly RGB Red = new RGB(255, 0, 0);
        ///<summary>RGB representation of X11 color named HotPink</summary>
        public static readonly RGB HotPink = new RGB(255, 105, 180);
        ///<summary>RGB representation of X11 color named DeepPink</summary>
        public static readonly RGB DeepPink = new RGB(255, 20, 147);
        ///<summary>RGB representation of X11 color named Pink</summary>
        public static readonly RGB Pink = new RGB(255, 192, 203);
        ///<summary>RGB representation of X11 color named LightPink</summary>
        public static readonly RGB LightPink = new RGB(255, 182, 193);
        ///<summary>RGB representation of X11 color named PaleVioletRed</summary>
        public static readonly RGB PaleVioletRed = new RGB(219, 112, 147);
        ///<summary>RGB representation of X11 color named Maroon</summary>
        public static readonly RGB Maroon = new RGB(176, 48, 96);
        ///<summary>RGB representation of X11 color named MediumVioletRed</summary>
        public static readonly RGB MediumVioletRed = new RGB(199, 21, 133);
        ///<summary>RGB representation of X11 color named VioletRed</summary>
        public static readonly RGB VioletRed = new RGB(208, 32, 144);
        ///<summary>RGB representation of X11 color named Magenta</summary>
        public static readonly RGB Magenta = new RGB(255, 0, 255);
        ///<summary>RGB representation of X11 color named Violet</summary>
        public static readonly RGB Violet = new RGB(238, 130, 238);
        ///<summary>RGB representation of X11 color named Plum</summary>
        public static readonly RGB Plum = new RGB(221, 160, 221);
        ///<summary>RGB representation of X11 color named Orchid</summary>
        public static readonly RGB Orchid = new RGB(218, 112, 214);
        ///<summary>RGB representation of X11 color named MediumOrchid</summary>
        public static readonly RGB MediumOrchid = new RGB(186, 85, 211);
        ///<summary>RGB representation of X11 color named DarkOrchid</summary>
        public static readonly RGB DarkOrchid = new RGB(153, 50, 204);
        ///<summary>RGB representation of X11 color named DarkViolet</summary>
        public static readonly RGB DarkViolet = new RGB(148, 0, 211);
        ///<summary>RGB representation of X11 color named BlueViolet</summary>
        public static readonly RGB BlueViolet = new RGB(138, 43, 226);
        ///<summary>RGB representation of X11 color named Purple</summary>
        public static readonly RGB Purple = new RGB(160, 32, 240);
        ///<summary>RGB representation of X11 color named MediumPurple</summary>
        public static readonly RGB MediumPurple = new RGB(147, 112, 219);
        ///<summary>RGB representation of X11 color named Thistle</summary>
        public static readonly RGB Thistle = new RGB(216, 191, 216);
        ///<summary>RGB representation of X11 color named Snow1</summary>
        public static readonly RGB Snow1 = new RGB(255, 250, 250);
        ///<summary>RGB representation of X11 color named Snow2</summary>
        public static readonly RGB Snow2 = new RGB(238, 233, 233);
        ///<summary>RGB representation of X11 color named Snow3</summary>
        public static readonly RGB Snow3 = new RGB(205, 201, 201);
        ///<summary>RGB representation of X11 color named Snow4</summary>
        public static readonly RGB Snow4 = new RGB(139, 137, 137);
        ///<summary>RGB representation of X11 color named Seashell1</summary>
        public static readonly RGB Seashell1 = new RGB(255, 245, 238);
        ///<summary>RGB representation of X11 color named Seashell2</summary>
        public static readonly RGB Seashell2 = new RGB(238, 229, 222);
        ///<summary>RGB representation of X11 color named Seashell3</summary>
        public static readonly RGB Seashell3 = new RGB(205, 197, 191);
        ///<summary>RGB representation of X11 color named Seashell4</summary>
        public static readonly RGB Seashell4 = new RGB(139, 134, 130);
        ///<summary>RGB representation of X11 color named AntiqueWhite1</summary>
        public static readonly RGB AntiqueWhite1 = new RGB(255, 239, 219);
        ///<summary>RGB representation of X11 color named AntiqueWhite2</summary>
        public static readonly RGB AntiqueWhite2 = new RGB(238, 223, 204);
        ///<summary>RGB representation of X11 color named AntiqueWhite3</summary>
        public static readonly RGB AntiqueWhite3 = new RGB(205, 192, 176);
        ///<summary>RGB representation of X11 color named AntiqueWhite4</summary>
        public static readonly RGB AntiqueWhite4 = new RGB(139, 131, 120);
        ///<summary>RGB representation of X11 color named Bisque1</summary>
        public static readonly RGB Bisque1 = new RGB(255, 228, 196);
        ///<summary>RGB representation of X11 color named Bisque2</summary>
        public static readonly RGB Bisque2 = new RGB(238, 213, 183);
        ///<summary>RGB representation of X11 color named Bisque3</summary>
        public static readonly RGB Bisque3 = new RGB(205, 183, 158);
        ///<summary>RGB representation of X11 color named Bisque4</summary>
        public static readonly RGB Bisque4 = new RGB(139, 125, 107);
        ///<summary>RGB representation of X11 color named PeachPuff1</summary>
        public static readonly RGB PeachPuff1 = new RGB(255, 218, 185);
        ///<summary>RGB representation of X11 color named PeachPuff2</summary>
        public static readonly RGB PeachPuff2 = new RGB(238, 203, 173);
        ///<summary>RGB representation of X11 color named PeachPuff3</summary>
        public static readonly RGB PeachPuff3 = new RGB(205, 175, 149);
        ///<summary>RGB representation of X11 color named PeachPuff4</summary>
        public static readonly RGB PeachPuff4 = new RGB(139, 119, 101);
        ///<summary>RGB representation of X11 color named NavajoWhite1</summary>
        public static readonly RGB NavajoWhite1 = new RGB(255, 222, 173);
        ///<summary>RGB representation of X11 color named NavajoWhite2</summary>
        public static readonly RGB NavajoWhite2 = new RGB(238, 207, 161);
        ///<summary>RGB representation of X11 color named NavajoWhite3</summary>
        public static readonly RGB NavajoWhite3 = new RGB(205, 179, 139);
        ///<summary>RGB representation of X11 color named NavajoWhite4</summary>
        public static readonly RGB NavajoWhite4 = new RGB(139, 121, 94);
        ///<summary>RGB representation of X11 color named LemonChiffon1</summary>
        public static readonly RGB LemonChiffon1 = new RGB(255, 250, 205);
        ///<summary>RGB representation of X11 color named LemonChiffon2</summary>
        public static readonly RGB LemonChiffon2 = new RGB(238, 233, 191);
        ///<summary>RGB representation of X11 color named LemonChiffon3</summary>
        public static readonly RGB LemonChiffon3 = new RGB(205, 201, 165);
        ///<summary>RGB representation of X11 color named LemonChiffon4</summary>
        public static readonly RGB LemonChiffon4 = new RGB(139, 137, 112);
        ///<summary>RGB representation of X11 color named Cornsilk1</summary>
        public static readonly RGB Cornsilk1 = new RGB(255, 248, 220);
        ///<summary>RGB representation of X11 color named Cornsilk2</summary>
        public static readonly RGB Cornsilk2 = new RGB(238, 232, 205);
        ///<summary>RGB representation of X11 color named Cornsilk3</summary>
        public static readonly RGB Cornsilk3 = new RGB(205, 200, 177);
        ///<summary>RGB representation of X11 color named Cornsilk4</summary>
        public static readonly RGB Cornsilk4 = new RGB(139, 136, 120);
        ///<summary>RGB representation of X11 color named Ivory1</summary>
        public static readonly RGB Ivory1 = new RGB(255, 255, 240);
        ///<summary>RGB representation of X11 color named Ivory2</summary>
        public static readonly RGB Ivory2 = new RGB(238, 238, 224);
        ///<summary>RGB representation of X11 color named Ivory3</summary>
        public static readonly RGB Ivory3 = new RGB(205, 205, 193);
        ///<summary>RGB representation of X11 color named Ivory4</summary>
        public static readonly RGB Ivory4 = new RGB(139, 139, 131);
        ///<summary>RGB representation of X11 color named Honeydew1</summary>
        public static readonly RGB Honeydew1 = new RGB(240, 255, 240);
        ///<summary>RGB representation of X11 color named Honeydew2</summary>
        public static readonly RGB Honeydew2 = new RGB(224, 238, 224);
        ///<summary>RGB representation of X11 color named Honeydew3</summary>
        public static readonly RGB Honeydew3 = new RGB(193, 205, 193);
        ///<summary>RGB representation of X11 color named Honeydew4</summary>
        public static readonly RGB Honeydew4 = new RGB(131, 139, 131);
        ///<summary>RGB representation of X11 color named LavenderBlush1</summary>
        public static readonly RGB LavenderBlush1 = new RGB(255, 240, 245);
        ///<summary>RGB representation of X11 color named LavenderBlush2</summary>
        public static readonly RGB LavenderBlush2 = new RGB(238, 224, 229);
        ///<summary>RGB representation of X11 color named LavenderBlush3</summary>
        public static readonly RGB LavenderBlush3 = new RGB(205, 193, 197);
        ///<summary>RGB representation of X11 color named LavenderBlush4</summary>
        public static readonly RGB LavenderBlush4 = new RGB(139, 131, 134);
        ///<summary>RGB representation of X11 color named MistyRose1</summary>
        public static readonly RGB MistyRose1 = new RGB(255, 228, 225);
        ///<summary>RGB representation of X11 color named MistyRose2</summary>
        public static readonly RGB MistyRose2 = new RGB(238, 213, 210);
        ///<summary>RGB representation of X11 color named MistyRose3</summary>
        public static readonly RGB MistyRose3 = new RGB(205, 183, 181);
        ///<summary>RGB representation of X11 color named MistyRose4</summary>
        public static readonly RGB MistyRose4 = new RGB(139, 125, 123);
        ///<summary>RGB representation of X11 color named Azure1</summary>
        public static readonly RGB Azure1 = new RGB(240, 255, 255);
        ///<summary>RGB representation of X11 color named Azure2</summary>
        public static readonly RGB Azure2 = new RGB(224, 238, 238);
        ///<summary>RGB representation of X11 color named Azure3</summary>
        public static readonly RGB Azure3 = new RGB(193, 205, 205);
        ///<summary>RGB representation of X11 color named Azure4</summary>
        public static readonly RGB Azure4 = new RGB(131, 139, 139);
        ///<summary>RGB representation of X11 color named SlateBlue1</summary>
        public static readonly RGB SlateBlue1 = new RGB(131, 111, 255);
        ///<summary>RGB representation of X11 color named SlateBlue2</summary>
        public static readonly RGB SlateBlue2 = new RGB(122, 103, 238);
        ///<summary>RGB representation of X11 color named SlateBlue3</summary>
        public static readonly RGB SlateBlue3 = new RGB(105, 89, 205);
        ///<summary>RGB representation of X11 color named SkyBlue1</summary>
        public static readonly RGB SkyBlue1 = new RGB(135, 206, 255);
        ///<summary>RGB representation of X11 color named SkyBlue2</summary>
        public static readonly RGB SkyBlue2 = new RGB(126, 192, 238);
        ///<summary>RGB representation of X11 color named SkyBlue3</summary>
        public static readonly RGB SkyBlue3 = new RGB(108, 166, 205);
        ///<summary>RGB representation of X11 color named LightSkyBlue1</summary>
        public static readonly RGB LightSkyBlue1 = new RGB(176, 226, 255);
        ///<summary>RGB representation of X11 color named LightSkyBlue2</summary>
        public static readonly RGB LightSkyBlue2 = new RGB(164, 211, 238);
        ///<summary>RGB representation of X11 color named LightSkyBlue3</summary>
        public static readonly RGB LightSkyBlue3 = new RGB(141, 182, 205);
        ///<summary>RGB representation of X11 color named SlateGray1</summary>
        public static readonly RGB SlateGray1 = new RGB(198, 226, 255);
        ///<summary>RGB representation of X11 color named SlateGray2</summary>
        public static readonly RGB SlateGray2 = new RGB(185, 211, 238);
        ///<summary>RGB representation of X11 color named SlateGray3</summary>
        public static readonly RGB SlateGray3 = new RGB(159, 182, 205);
        ///<summary>RGB representation of X11 color named SlateGray4</summary>
        public static readonly RGB SlateGray4 = new RGB(108, 123, 139);
        ///<summary>RGB representation of X11 color named LightSteelBlue1</summary>
        public static readonly RGB LightSteelBlue1 = new RGB(202, 225, 255);
        ///<summary>RGB representation of X11 color named LightSteelBlue2</summary>
        public static readonly RGB LightSteelBlue2 = new RGB(188, 210, 238);
        ///<summary>RGB representation of X11 color named LightSteelBlue3</summary>
        public static readonly RGB LightSteelBlue3 = new RGB(162, 181, 205);
        ///<summary>RGB representation of X11 color named LightSteelBlue4</summary>
        public static readonly RGB LightSteelBlue4 = new RGB(110, 123, 139);
        ///<summary>RGB representation of X11 color named LightBlue1</summary>
        public static readonly RGB LightBlue1 = new RGB(191, 239, 255);
        ///<summary>RGB representation of X11 color named LightBlue2</summary>
        public static readonly RGB LightBlue2 = new RGB(178, 223, 238);
        ///<summary>RGB representation of X11 color named LightBlue3</summary>
        public static readonly RGB LightBlue3 = new RGB(154, 192, 205);
        ///<summary>RGB representation of X11 color named LightBlue4</summary>
        public static readonly RGB LightBlue4 = new RGB(104, 131, 139);
        ///<summary>RGB representation of X11 color named LightCyan1</summary>
        public static readonly RGB LightCyan1 = new RGB(224, 255, 255);
        ///<summary>RGB representation of X11 color named LightCyan2</summary>
        public static readonly RGB LightCyan2 = new RGB(209, 238, 238);
        ///<summary>RGB representation of X11 color named LightCyan3</summary>
        public static readonly RGB LightCyan3 = new RGB(180, 205, 205);
        ///<summary>RGB representation of X11 color named LightCyan4</summary>
        public static readonly RGB LightCyan4 = new RGB(122, 139, 139);
        ///<summary>RGB representation of X11 color named PaleTurquoise1</summary>
        public static readonly RGB PaleTurquoise1 = new RGB(187, 255, 255);
        ///<summary>RGB representation of X11 color named PaleTurquoise2</summary>
        public static readonly RGB PaleTurquoise2 = new RGB(174, 238, 238);
        ///<summary>RGB representation of X11 color named PaleTurquoise3</summary>
        public static readonly RGB PaleTurquoise3 = new RGB(150, 205, 205);
        ///<summary>RGB representation of X11 color named PaleTurquoise4</summary>
        public static readonly RGB PaleTurquoise4 = new RGB(102, 139, 139);
        ///<summary>RGB representation of X11 color named CadetBlue1</summary>
        public static readonly RGB CadetBlue1 = new RGB(152, 245, 255);
        ///<summary>RGB representation of X11 color named CadetBlue2</summary>
        public static readonly RGB CadetBlue2 = new RGB(142, 229, 238);
        ///<summary>RGB representation of X11 color named CadetBlue3</summary>
        public static readonly RGB CadetBlue3 = new RGB(122, 197, 205);
        ///<summary>RGB representation of X11 color named DarkSlateGray1</summary>
        public static readonly RGB DarkSlateGray1 = new RGB(151, 255, 255);
        ///<summary>RGB representation of X11 color named DarkSlateGray2</summary>
        public static readonly RGB DarkSlateGray2 = new RGB(141, 238, 238);
        ///<summary>RGB representation of X11 color named DarkSlateGray3</summary>
        public static readonly RGB DarkSlateGray3 = new RGB(121, 205, 205);
        ///<summary>RGB representation of X11 color named Aquamarine1</summary>
        public static readonly RGB Aquamarine1 = new RGB(127, 255, 212);
        ///<summary>RGB representation of X11 color named Aquamarine2</summary>
        public static readonly RGB Aquamarine2 = new RGB(118, 238, 198);
        ///<summary>RGB representation of X11 color named Aquamarine3</summary>
        public static readonly RGB Aquamarine3 = new RGB(102, 205, 170);
        ///<summary>RGB representation of X11 color named DarkSeaGreen1</summary>
        public static readonly RGB DarkSeaGreen1 = new RGB(193, 255, 193);
        ///<summary>RGB representation of X11 color named DarkSeaGreen2</summary>
        public static readonly RGB DarkSeaGreen2 = new RGB(180, 238, 180);
        ///<summary>RGB representation of X11 color named DarkSeaGreen3</summary>
        public static readonly RGB DarkSeaGreen3 = new RGB(155, 205, 155);
        ///<summary>RGB representation of X11 color named DarkSeaGreen4</summary>
        public static readonly RGB DarkSeaGreen4 = new RGB(105, 139, 105);
        ///<summary>RGB representation of X11 color named PaleGreen1</summary>
        public static readonly RGB PaleGreen1 = new RGB(154, 255, 154);
        ///<summary>RGB representation of X11 color named PaleGreen2</summary>
        public static readonly RGB PaleGreen2 = new RGB(144, 238, 144);
        ///<summary>RGB representation of X11 color named PaleGreen3</summary>
        public static readonly RGB PaleGreen3 = new RGB(124, 205, 124);
        ///<summary>RGB representation of X11 color named Chartreuse1</summary>
        public static readonly RGB Chartreuse1 = new RGB(127, 255, 0);
        ///<summary>RGB representation of X11 color named Chartreuse2</summary>
        public static readonly RGB Chartreuse2 = new RGB(118, 238, 0);
        ///<summary>RGB representation of X11 color named Chartreuse3</summary>
        public static readonly RGB Chartreuse3 = new RGB(102, 205, 0);
        ///<summary>RGB representation of X11 color named OliveDrab1</summary>
        public static readonly RGB OliveDrab1 = new RGB(192, 255, 62);
        ///<summary>RGB representation of X11 color named OliveDrab2</summary>
        public static readonly RGB OliveDrab2 = new RGB(179, 238, 58);
        ///<summary>RGB representation of X11 color named OliveDrab3</summary>
        public static readonly RGB OliveDrab3 = new RGB(154, 205, 50);
        ///<summary>RGB representation of X11 color named OliveDrab4</summary>
        public static readonly RGB OliveDrab4 = new RGB(105, 139, 34);
        ///<summary>RGB representation of X11 color named DarkOliveGreen1</summary>
        public static readonly RGB DarkOliveGreen1 = new RGB(202, 255, 112);
        ///<summary>RGB representation of X11 color named DarkOliveGreen2</summary>
        public static readonly RGB DarkOliveGreen2 = new RGB(188, 238, 104);
        ///<summary>RGB representation of X11 color named DarkOliveGreen3</summary>
        public static readonly RGB DarkOliveGreen3 = new RGB(162, 205, 90);
        ///<summary>RGB representation of X11 color named DarkOliveGreen4</summary>
        public static readonly RGB DarkOliveGreen4 = new RGB(110, 139, 61);
        ///<summary>RGB representation of X11 color named Khaki1</summary>
        public static readonly RGB Khaki1 = new RGB(255, 246, 143);
        ///<summary>RGB representation of X11 color named Khaki2</summary>
        public static readonly RGB Khaki2 = new RGB(238, 230, 133);
        ///<summary>RGB representation of X11 color named Khaki3</summary>
        public static readonly RGB Khaki3 = new RGB(205, 198, 115);
        ///<summary>RGB representation of X11 color named Khaki4</summary>
        public static readonly RGB Khaki4 = new RGB(139, 134, 78);
        ///<summary>RGB representation of X11 color named LightGoldenrod1</summary>
        public static readonly RGB LightGoldenrod1 = new RGB(255, 236, 139);
        ///<summary>RGB representation of X11 color named LightGoldenrod2</summary>
        public static readonly RGB LightGoldenrod2 = new RGB(238, 220, 130);
        ///<summary>RGB representation of X11 color named LightGoldenrod3</summary>
        public static readonly RGB LightGoldenrod3 = new RGB(205, 190, 112);
        ///<summary>RGB representation of X11 color named LightGoldenrod4</summary>
        public static readonly RGB LightGoldenrod4 = new RGB(139, 129, 76);
        ///<summary>RGB representation of X11 color named LightYellow1</summary>
        public static readonly RGB LightYellow1 = new RGB(255, 255, 224);
        ///<summary>RGB representation of X11 color named LightYellow2</summary>
        public static readonly RGB LightYellow2 = new RGB(238, 238, 209);
        ///<summary>RGB representation of X11 color named LightYellow3</summary>
        public static readonly RGB LightYellow3 = new RGB(205, 205, 180);
        ///<summary>RGB representation of X11 color named LightYellow4</summary>
        public static readonly RGB LightYellow4 = new RGB(139, 139, 122);
        ///<summary>RGB representation of X11 color named Yellow1</summary>
        public static readonly RGB Yellow1 = new RGB(255, 255, 0);
        ///<summary>RGB representation of X11 color named Yellow2</summary>
        public static readonly RGB Yellow2 = new RGB(238, 238, 0);
        ///<summary>RGB representation of X11 color named Yellow3</summary>
        public static readonly RGB Yellow3 = new RGB(205, 205, 0);
        ///<summary>RGB representation of X11 color named Yellow4</summary>
        public static readonly RGB Yellow4 = new RGB(139, 139, 0);
        ///<summary>RGB representation of X11 color named Gold1</summary>
        public static readonly RGB Gold1 = new RGB(255, 215, 0);
        ///<summary>RGB representation of X11 color named Gold2</summary>
        public static readonly RGB Gold2 = new RGB(238, 201, 0);
        ///<summary>RGB representation of X11 color named Gold3</summary>
        public static readonly RGB Gold3 = new RGB(205, 173, 0);
        ///<summary>RGB representation of X11 color named Gold4</summary>
        public static readonly RGB Gold4 = new RGB(139, 117, 0);
        ///<summary>RGB representation of X11 color named Goldenrod1</summary>
        public static readonly RGB Goldenrod1 = new RGB(255, 193, 37);
        ///<summary>RGB representation of X11 color named Goldenrod2</summary>
        public static readonly RGB Goldenrod2 = new RGB(238, 180, 34);
        ///<summary>RGB representation of X11 color named Goldenrod3</summary>
        public static readonly RGB Goldenrod3 = new RGB(205, 155, 29);
        ///<summary>RGB representation of X11 color named Goldenrod4</summary>
        public static readonly RGB Goldenrod4 = new RGB(139, 105, 20);
        ///<summary>RGB representation of X11 color named DarkGoldenrod1</summary>
        public static readonly RGB DarkGoldenrod1 = new RGB(255, 185, 15);
        ///<summary>RGB representation of X11 color named DarkGoldenrod2</summary>
        public static readonly RGB DarkGoldenrod2 = new RGB(238, 173, 14);
        ///<summary>RGB representation of X11 color named DarkGoldenrod3</summary>
        public static readonly RGB DarkGoldenrod3 = new RGB(205, 149, 12);
        ///<summary>RGB representation of X11 color named DarkGoldenrod4</summary>
        public static readonly RGB DarkGoldenrod4 = new RGB(139, 101, 8);
        ///<summary>RGB representation of X11 color named RosyBrown1</summary>
        public static readonly RGB RosyBrown1 = new RGB(255, 193, 193);
        ///<summary>RGB representation of X11 color named RosyBrown2</summary>
        public static readonly RGB RosyBrown2 = new RGB(238, 180, 180);
        ///<summary>RGB representation of X11 color named RosyBrown3</summary>
        public static readonly RGB RosyBrown3 = new RGB(205, 155, 155);
        ///<summary>RGB representation of X11 color named RosyBrown4</summary>
        public static readonly RGB RosyBrown4 = new RGB(139, 105, 105);
        ///<summary>RGB representation of X11 color named IndianRed1</summary>
        public static readonly RGB IndianRed1 = new RGB(255, 106, 106);
        ///<summary>RGB representation of X11 color named IndianRed2</summary>
        public static readonly RGB IndianRed2 = new RGB(238, 99, 99);
        ///<summary>RGB representation of X11 color named IndianRed3</summary>
        public static readonly RGB IndianRed3 = new RGB(205, 85, 85);
        ///<summary>RGB representation of X11 color named IndianRed4</summary>
        public static readonly RGB IndianRed4 = new RGB(139, 58, 58);
        ///<summary>RGB representation of X11 color named Sienna1</summary>
        public static readonly RGB Sienna1 = new RGB(255, 130, 71);
        ///<summary>RGB representation of X11 color named Sienna2</summary>
        public static readonly RGB Sienna2 = new RGB(238, 121, 66);
        ///<summary>RGB representation of X11 color named Sienna3</summary>
        public static readonly RGB Sienna3 = new RGB(205, 104, 57);
        ///<summary>RGB representation of X11 color named Sienna4</summary>
        public static readonly RGB Sienna4 = new RGB(139, 71, 38);
        ///<summary>RGB representation of X11 color named Burlywood1</summary>
        public static readonly RGB Burlywood1 = new RGB(255, 211, 155);
        ///<summary>RGB representation of X11 color named Burlywood2</summary>
        public static readonly RGB Burlywood2 = new RGB(238, 197, 145);
        ///<summary>RGB representation of X11 color named Burlywood3</summary>
        public static readonly RGB Burlywood3 = new RGB(205, 170, 125);
        ///<summary>RGB representation of X11 color named Burlywood4</summary>
        public static readonly RGB Burlywood4 = new RGB(139, 115, 85);
        ///<summary>RGB representation of X11 color named Wheat1</summary>
        public static readonly RGB Wheat1 = new RGB(255, 231, 186);
        ///<summary>RGB representation of X11 color named Wheat2</summary>
        public static readonly RGB Wheat2 = new RGB(238, 216, 174);
        ///<summary>RGB representation of X11 color named Wheat3</summary>
        public static readonly RGB Wheat3 = new RGB(205, 186, 150);
        ///<summary>RGB representation of X11 color named Wheat4</summary>
        public static readonly RGB Wheat4 = new RGB(139, 126, 102);
        ///<summary>RGB representation of X11 color named Tan1</summary>
        public static readonly RGB Tan1 = new RGB(255, 165, 79);
        ///<summary>RGB representation of X11 color named Tan2</summary>
        public static readonly RGB Tan2 = new RGB(238, 154, 73);
        ///<summary>RGB representation of X11 color named Tan3</summary>
        public static readonly RGB Tan3 = new RGB(205, 133, 63);
        ///<summary>RGB representation of X11 color named Tan4</summary>
        public static readonly RGB Tan4 = new RGB(139, 90, 43);
        ///<summary>RGB representation of X11 color named Chocolate1</summary>
        public static readonly RGB Chocolate1 = new RGB(255, 127, 36);
        ///<summary>RGB representation of X11 color named Chocolate2</summary>
        public static readonly RGB Chocolate2 = new RGB(238, 118, 33);
        ///<summary>RGB representation of X11 color named Chocolate3</summary>
        public static readonly RGB Chocolate3 = new RGB(205, 102, 29);
        ///<summary>RGB representation of X11 color named Chocolate4</summary>
        public static readonly RGB Chocolate4 = new RGB(139, 69, 19);
        ///<summary>RGB representation of X11 color named Firebrick1</summary>
        public static readonly RGB Firebrick1 = new RGB(255, 48, 48);
        ///<summary>RGB representation of X11 color named Firebrick2</summary>
        public static readonly RGB Firebrick2 = new RGB(238, 44, 44);
        ///<summary>RGB representation of X11 color named Firebrick3</summary>
        public static readonly RGB Firebrick3 = new RGB(205, 38, 38);
        ///<summary>RGB representation of X11 color named Firebrick4</summary>
        public static readonly RGB Firebrick4 = new RGB(139, 26, 26);
        ///<summary>RGB representation of X11 color named Brown1</summary>
        public static readonly RGB Brown1 = new RGB(255, 64, 64);
        ///<summary>RGB representation of X11 color named Brown2</summary>
        public static readonly RGB Brown2 = new RGB(238, 59, 59);
        ///<summary>RGB representation of X11 color named Brown3</summary>
        public static readonly RGB Brown3 = new RGB(205, 51, 51);
        ///<summary>RGB representation of X11 color named Brown4</summary>
        public static readonly RGB Brown4 = new RGB(139, 35, 35);
        ///<summary>RGB representation of X11 color named Salmon1</summary>
        public static readonly RGB Salmon1 = new RGB(255, 140, 105);
        ///<summary>RGB representation of X11 color named Salmon2</summary>
        public static readonly RGB Salmon2 = new RGB(238, 130, 98);
        ///<summary>RGB representation of X11 color named Salmon3</summary>
        public static readonly RGB Salmon3 = new RGB(205, 112, 84);
        ///<summary>RGB representation of X11 color named Salmon4</summary>
        public static readonly RGB Salmon4 = new RGB(139, 76, 57);
        ///<summary>RGB representation of X11 color named LightSalmon1</summary>
        public static readonly RGB LightSalmon1 = new RGB(255, 160, 122);
        ///<summary>RGB representation of X11 color named LightSalmon2</summary>
        public static readonly RGB LightSalmon2 = new RGB(238, 149, 114);
        ///<summary>RGB representation of X11 color named LightSalmon3</summary>
        public static readonly RGB LightSalmon3 = new RGB(205, 129, 98);
        ///<summary>RGB representation of X11 color named LightSalmon4</summary>
        public static readonly RGB LightSalmon4 = new RGB(139, 87, 66);
        ///<summary>RGB representation of X11 color named Orange1</summary>
        public static readonly RGB Orange1 = new RGB(255, 165, 0);
        ///<summary>RGB representation of X11 color named Orange2</summary>
        public static readonly RGB Orange2 = new RGB(238, 154, 0);
        ///<summary>RGB representation of X11 color named Orange3</summary>
        public static readonly RGB Orange3 = new RGB(205, 133, 0);
        ///<summary>RGB representation of X11 color named Orange4</summary>
        public static readonly RGB Orange4 = new RGB(139, 90, 0);
        ///<summary>RGB representation of X11 color named DarkOrange1</summary>
        public static readonly RGB DarkOrange1 = new RGB(255, 127, 0);
        ///<summary>RGB representation of X11 color named DarkOrange2</summary>
        public static readonly RGB DarkOrange2 = new RGB(238, 118, 0);
        ///<summary>RGB representation of X11 color named DarkOrange3</summary>
        public static readonly RGB DarkOrange3 = new RGB(205, 102, 0);
        ///<summary>RGB representation of X11 color named DarkOrange4</summary>
        public static readonly RGB DarkOrange4 = new RGB(139, 69, 0);
        ///<summary>RGB representation of X11 color named Coral1</summary>
        public static readonly RGB Coral1 = new RGB(255, 114, 86);
        ///<summary>RGB representation of X11 color named Coral2</summary>
        public static readonly RGB Coral2 = new RGB(238, 106, 80);
        ///<summary>RGB representation of X11 color named Coral3</summary>
        public static readonly RGB Coral3 = new RGB(205, 91, 69);
        ///<summary>RGB representation of X11 color named Coral4</summary>
        public static readonly RGB Coral4 = new RGB(139, 62, 47);
        ///<summary>RGB representation of X11 color named Tomato1</summary>
        public static readonly RGB Tomato1 = new RGB(255, 99, 71);
        ///<summary>RGB representation of X11 color named Tomato2</summary>
        public static readonly RGB Tomato2 = new RGB(238, 92, 66);
        ///<summary>RGB representation of X11 color named Tomato3</summary>
        public static readonly RGB Tomato3 = new RGB(205, 79, 57);
        ///<summary>RGB representation of X11 color named Tomato4</summary>
        public static readonly RGB Tomato4 = new RGB(139, 54, 38);
        ///<summary>RGB representation of X11 color named OrangeRed1</summary>
        public static readonly RGB OrangeRed1 = new RGB(255, 69, 0);
        ///<summary>RGB representation of X11 color named OrangeRed2</summary>
        public static readonly RGB OrangeRed2 = new RGB(238, 64, 0);
        ///<summary>RGB representation of X11 color named OrangeRed3</summary>
        public static readonly RGB OrangeRed3 = new RGB(205, 55, 0);
        ///<summary>RGB representation of X11 color named OrangeRed4</summary>
        public static readonly RGB OrangeRed4 = new RGB(139, 37, 0);
        ///<summary>RGB representation of X11 color named Red1</summary>
        public static readonly RGB Red1 = new RGB(255, 0, 0);
        ///<summary>RGB representation of X11 color named Red2</summary>
        public static readonly RGB Red2 = new RGB(238, 0, 0);
        ///<summary>RGB representation of X11 color named Red3</summary>
        public static readonly RGB Red3 = new RGB(205, 0, 0);
        ///<summary>RGB representation of X11 color named Red4</summary>
        public static readonly RGB Red4 = new RGB(139, 0, 0);
        ///<summary>RGB representation of X11 color named DebianRed</summary>
        public static readonly RGB DebianRed = new RGB(215, 7, 81);
        ///<summary>RGB representation of X11 color named DeepPink1</summary>
        public static readonly RGB DeepPink1 = new RGB(255, 20, 147);
        ///<summary>RGB representation of X11 color named DeepPink2</summary>
        public static readonly RGB DeepPink2 = new RGB(238, 18, 137);
        ///<summary>RGB representation of X11 color named DeepPink3</summary>
        public static readonly RGB DeepPink3 = new RGB(205, 16, 118);
        ///<summary>RGB representation of X11 color named DeepPink4</summary>
        public static readonly RGB DeepPink4 = new RGB(139, 10, 80);
        ///<summary>RGB representation of X11 color named HotPink1</summary>
        public static readonly RGB HotPink1 = new RGB(255, 110, 180);
        ///<summary>RGB representation of X11 color named HotPink2</summary>
        public static readonly RGB HotPink2 = new RGB(238, 106, 167);
        ///<summary>RGB representation of X11 color named HotPink3</summary>
        public static readonly RGB HotPink3 = new RGB(205, 96, 144);
        ///<summary>RGB representation of X11 color named HotPink4</summary>
        public static readonly RGB HotPink4 = new RGB(139, 58, 98);
        ///<summary>RGB representation of X11 color named Pink1</summary>
        public static readonly RGB Pink1 = new RGB(255, 181, 197);
        ///<summary>RGB representation of X11 color named Pink2</summary>
        public static readonly RGB Pink2 = new RGB(238, 169, 184);
        ///<summary>RGB representation of X11 color named Pink3</summary>
        public static readonly RGB Pink3 = new RGB(205, 145, 158);
        ///<summary>RGB representation of X11 color named Pink4</summary>
        public static readonly RGB Pink4 = new RGB(139, 99, 108);
        ///<summary>RGB representation of X11 color named LightPink1</summary>
        public static readonly RGB LightPink1 = new RGB(255, 174, 185);
        ///<summary>RGB representation of X11 color named LightPink2</summary>
        public static readonly RGB LightPink2 = new RGB(238, 162, 173);
        ///<summary>RGB representation of X11 color named LightPink3</summary>
        public static readonly RGB LightPink3 = new RGB(205, 140, 149);
        ///<summary>RGB representation of X11 color named LightPink4</summary>
        public static readonly RGB LightPink4 = new RGB(139, 95, 101);
        ///<summary>RGB representation of X11 color named PaleVioletRed1</summary>
        public static readonly RGB PaleVioletRed1 = new RGB(255, 130, 171);
        ///<summary>RGB representation of X11 color named PaleVioletRed2</summary>
        public static readonly RGB PaleVioletRed2 = new RGB(238, 121, 159);
        ///<summary>RGB representation of X11 color named PaleVioletRed3</summary>
        public static readonly RGB PaleVioletRed3 = new RGB(205, 104, 137);
        ///<summary>RGB representation of X11 color named PaleVioletRed4</summary>
        public static readonly RGB PaleVioletRed4 = new RGB(139, 71, 93);
        ///<summary>RGB representation of X11 color named Maroon1</summary>
        public static readonly RGB Maroon1 = new RGB(255, 52, 179);
        ///<summary>RGB representation of X11 color named Maroon2</summary>
        public static readonly RGB Maroon2 = new RGB(238, 48, 167);
        ///<summary>RGB representation of X11 color named Maroon3</summary>
        public static readonly RGB Maroon3 = new RGB(205, 41, 144);
        ///<summary>RGB representation of X11 color named Maroon4</summary>
        public static readonly RGB Maroon4 = new RGB(139, 28, 98);
        ///<summary>RGB representation of X11 color named VioletRed1</summary>
        public static readonly RGB VioletRed1 = new RGB(255, 62, 150);
        ///<summary>RGB representation of X11 color named VioletRed2</summary>
        public static readonly RGB VioletRed2 = new RGB(238, 58, 140);
        ///<summary>RGB representation of X11 color named VioletRed3</summary>
        public static readonly RGB VioletRed3 = new RGB(205, 50, 120);
        ///<summary>RGB representation of X11 color named VioletRed4</summary>
        public static readonly RGB VioletRed4 = new RGB(139, 34, 82);
        ///<summary>RGB representation of X11 color named Magenta1</summary>
        public static readonly RGB Magenta1 = new RGB(255, 0, 255);
        ///<summary>RGB representation of X11 color named Magenta2</summary>
        public static readonly RGB Magenta2 = new RGB(238, 0, 238);
        ///<summary>RGB representation of X11 color named Magenta3</summary>
        public static readonly RGB Magenta3 = new RGB(205, 0, 205);
        ///<summary>RGB representation of X11 color named Magenta4</summary>
        public static readonly RGB Magenta4 = new RGB(139, 0, 139);
        ///<summary>RGB representation of X11 color named Orchid1</summary>
        public static readonly RGB Orchid1 = new RGB(255, 131, 250);
        ///<summary>RGB representation of X11 color named Orchid2</summary>
        public static readonly RGB Orchid2 = new RGB(238, 122, 233);
        ///<summary>RGB representation of X11 color named Orchid3</summary>
        public static readonly RGB Orchid3 = new RGB(205, 105, 201);
        ///<summary>RGB representation of X11 color named Orchid4</summary>
        public static readonly RGB Orchid4 = new RGB(139, 71, 137);
        ///<summary>RGB representation of X11 color named Plum1</summary>
        public static readonly RGB Plum1 = new RGB(255, 187, 255);
        ///<summary>RGB representation of X11 color named Plum2</summary>
        public static readonly RGB Plum2 = new RGB(238, 174, 238);
        ///<summary>RGB representation of X11 color named Plum3</summary>
        public static readonly RGB Plum3 = new RGB(205, 150, 205);
        ///<summary>RGB representation of X11 color named Plum4</summary>
        public static readonly RGB Plum4 = new RGB(139, 102, 139);
        ///<summary>RGB representation of X11 color named MediumOrchid1</summary>
        public static readonly RGB MediumOrchid1 = new RGB(224, 102, 255);
        ///<summary>RGB representation of X11 color named MediumOrchid2</summary>
        public static readonly RGB MediumOrchid2 = new RGB(209, 95, 238);
        ///<summary>RGB representation of X11 color named MediumOrchid3</summary>
        public static readonly RGB MediumOrchid3 = new RGB(180, 82, 205);
        ///<summary>RGB representation of X11 color named MediumOrchid4</summary>
        public static readonly RGB MediumOrchid4 = new RGB(122, 55, 139);
        ///<summary>RGB representation of X11 color named DarkOrchid1</summary>
        public static readonly RGB DarkOrchid1 = new RGB(191, 62, 255);
        ///<summary>RGB representation of X11 color named DarkOrchid2</summary>
        public static readonly RGB DarkOrchid2 = new RGB(178, 58, 238);
        ///<summary>RGB representation of X11 color named DarkOrchid3</summary>
        public static readonly RGB DarkOrchid3 = new RGB(154, 50, 205);
        ///<summary>RGB representation of X11 color named DarkOrchid4</summary>
        public static readonly RGB DarkOrchid4 = new RGB(104, 34, 139);
        ///<summary>RGB representation of X11 color named Purple1</summary>
        public static readonly RGB Purple1 = new RGB(155, 48, 255);
        ///<summary>RGB representation of X11 color named Purple2</summary>
        public static readonly RGB Purple2 = new RGB(145, 44, 238);
        ///<summary>RGB representation of X11 color named Purple3</summary>
        public static readonly RGB Purple3 = new RGB(125, 38, 205);
        ///<summary>RGB representation of X11 color named MediumPurple1</summary>
        public static readonly RGB MediumPurple1 = new RGB(171, 130, 255);
        ///<summary>RGB representation of X11 color named MediumPurple2</summary>
        public static readonly RGB MediumPurple2 = new RGB(159, 121, 238);
        ///<summary>RGB representation of X11 color named MediumPurple3</summary>
        public static readonly RGB MediumPurple3 = new RGB(137, 104, 205);
        ///<summary>RGB representation of X11 color named Thistle1</summary>
        public static readonly RGB Thistle1 = new RGB(255, 225, 255);
        ///<summary>RGB representation of X11 color named Thistle2</summary>
        public static readonly RGB Thistle2 = new RGB(238, 210, 238);
        ///<summary>RGB representation of X11 color named Thistle3</summary>
        public static readonly RGB Thistle3 = new RGB(205, 181, 205);
        ///<summary>RGB representation of X11 color named Thistle4</summary>
        public static readonly RGB Thistle4 = new RGB(139, 123, 139);
        ///<summary>RGB representation of X11 color named Gray40</summary>
        public static readonly RGB Gray40 = new RGB(102, 102, 102);
        ///<summary>RGB representation of X11 color named Grey40</summary>
        public static readonly RGB Grey40 = new RGB(102, 102, 102);
        ///<summary>RGB representation of X11 color named Gray41</summary>
        public static readonly RGB Gray41 = new RGB(105, 105, 105);
        ///<summary>RGB representation of X11 color named Grey41</summary>
        public static readonly RGB Grey41 = new RGB(105, 105, 105);
        ///<summary>RGB representation of X11 color named Gray42</summary>
        public static readonly RGB Gray42 = new RGB(107, 107, 107);
        ///<summary>RGB representation of X11 color named Grey42</summary>
        public static readonly RGB Grey42 = new RGB(107, 107, 107);
        ///<summary>RGB representation of X11 color named Gray43</summary>
        public static readonly RGB Gray43 = new RGB(110, 110, 110);
        ///<summary>RGB representation of X11 color named Grey43</summary>
        public static readonly RGB Grey43 = new RGB(110, 110, 110);
        ///<summary>RGB representation of X11 color named Gray44</summary>
        public static readonly RGB Gray44 = new RGB(112, 112, 112);
        ///<summary>RGB representation of X11 color named Grey44</summary>
        public static readonly RGB Grey44 = new RGB(112, 112, 112);
        ///<summary>RGB representation of X11 color named Gray45</summary>
        public static readonly RGB Gray45 = new RGB(115, 115, 115);
        ///<summary>RGB representation of X11 color named Grey45</summary>
        public static readonly RGB Grey45 = new RGB(115, 115, 115);
        ///<summary>RGB representation of X11 color named Gray46</summary>
        public static readonly RGB Gray46 = new RGB(117, 117, 117);
        ///<summary>RGB representation of X11 color named Grey46</summary>
        public static readonly RGB Grey46 = new RGB(117, 117, 117);
        ///<summary>RGB representation of X11 color named Gray47</summary>
        public static readonly RGB Gray47 = new RGB(120, 120, 120);
        ///<summary>RGB representation of X11 color named Grey47</summary>
        public static readonly RGB Grey47 = new RGB(120, 120, 120);
        ///<summary>RGB representation of X11 color named Gray48</summary>
        public static readonly RGB Gray48 = new RGB(122, 122, 122);
        ///<summary>RGB representation of X11 color named Grey48</summary>
        public static readonly RGB Grey48 = new RGB(122, 122, 122);
        ///<summary>RGB representation of X11 color named Gray49</summary>
        public static readonly RGB Gray49 = new RGB(125, 125, 125);
        ///<summary>RGB representation of X11 color named Grey49</summary>
        public static readonly RGB Grey49 = new RGB(125, 125, 125);
        ///<summary>RGB representation of X11 color named Gray50</summary>
        public static readonly RGB Gray50 = new RGB(127, 127, 127);
        ///<summary>RGB representation of X11 color named Grey50</summary>
        public static readonly RGB Grey50 = new RGB(127, 127, 127);
        ///<summary>RGB representation of X11 color named Gray51</summary>
        public static readonly RGB Gray51 = new RGB(130, 130, 130);
        ///<summary>RGB representation of X11 color named Grey51</summary>
        public static readonly RGB Grey51 = new RGB(130, 130, 130);
        ///<summary>RGB representation of X11 color named Gray52</summary>
        public static readonly RGB Gray52 = new RGB(133, 133, 133);
        ///<summary>RGB representation of X11 color named Grey52</summary>
        public static readonly RGB Grey52 = new RGB(133, 133, 133);
        ///<summary>RGB representation of X11 color named Gray53</summary>
        public static readonly RGB Gray53 = new RGB(135, 135, 135);
        ///<summary>RGB representation of X11 color named Grey53</summary>
        public static readonly RGB Grey53 = new RGB(135, 135, 135);
        ///<summary>RGB representation of X11 color named Gray54</summary>
        public static readonly RGB Gray54 = new RGB(138, 138, 138);
        ///<summary>RGB representation of X11 color named Grey54</summary>
        public static readonly RGB Grey54 = new RGB(138, 138, 138);
        ///<summary>RGB representation of X11 color named Gray55</summary>
        public static readonly RGB Gray55 = new RGB(140, 140, 140);
        ///<summary>RGB representation of X11 color named Grey55</summary>
        public static readonly RGB Grey55 = new RGB(140, 140, 140);
        ///<summary>RGB representation of X11 color named Gray56</summary>
        public static readonly RGB Gray56 = new RGB(143, 143, 143);
        ///<summary>RGB representation of X11 color named Grey56</summary>
        public static readonly RGB Grey56 = new RGB(143, 143, 143);
        ///<summary>RGB representation of X11 color named Gray57</summary>
        public static readonly RGB Gray57 = new RGB(145, 145, 145);
        ///<summary>RGB representation of X11 color named Grey57</summary>
        public static readonly RGB Grey57 = new RGB(145, 145, 145);
        ///<summary>RGB representation of X11 color named Gray58</summary>
        public static readonly RGB Gray58 = new RGB(148, 148, 148);
        ///<summary>RGB representation of X11 color named Grey58</summary>
        public static readonly RGB Grey58 = new RGB(148, 148, 148);
        ///<summary>RGB representation of X11 color named Gray59</summary>
        public static readonly RGB Gray59 = new RGB(150, 150, 150);
        ///<summary>RGB representation of X11 color named Grey59</summary>
        public static readonly RGB Grey59 = new RGB(150, 150, 150);
        ///<summary>RGB representation of X11 color named Gray60</summary>
        public static readonly RGB Gray60 = new RGB(153, 153, 153);
        ///<summary>RGB representation of X11 color named Grey60</summary>
        public static readonly RGB Grey60 = new RGB(153, 153, 153);
        ///<summary>RGB representation of X11 color named Gray61</summary>
        public static readonly RGB Gray61 = new RGB(156, 156, 156);
        ///<summary>RGB representation of X11 color named Grey61</summary>
        public static readonly RGB Grey61 = new RGB(156, 156, 156);
        ///<summary>RGB representation of X11 color named Gray62</summary>
        public static readonly RGB Gray62 = new RGB(158, 158, 158);
        ///<summary>RGB representation of X11 color named Grey62</summary>
        public static readonly RGB Grey62 = new RGB(158, 158, 158);
        ///<summary>RGB representation of X11 color named Gray63</summary>
        public static readonly RGB Gray63 = new RGB(161, 161, 161);
        ///<summary>RGB representation of X11 color named Grey63</summary>
        public static readonly RGB Grey63 = new RGB(161, 161, 161);
        ///<summary>RGB representation of X11 color named Gray64</summary>
        public static readonly RGB Gray64 = new RGB(163, 163, 163);
        ///<summary>RGB representation of X11 color named Grey64</summary>
        public static readonly RGB Grey64 = new RGB(163, 163, 163);
        ///<summary>RGB representation of X11 color named Gray65</summary>
        public static readonly RGB Gray65 = new RGB(166, 166, 166);
        ///<summary>RGB representation of X11 color named Grey65</summary>
        public static readonly RGB Grey65 = new RGB(166, 166, 166);
        ///<summary>RGB representation of X11 color named Gray66</summary>
        public static readonly RGB Gray66 = new RGB(168, 168, 168);
        ///<summary>RGB representation of X11 color named Grey66</summary>
        public static readonly RGB Grey66 = new RGB(168, 168, 168);
        ///<summary>RGB representation of X11 color named Gray67</summary>
        public static readonly RGB Gray67 = new RGB(171, 171, 171);
        ///<summary>RGB representation of X11 color named Grey67</summary>
        public static readonly RGB Grey67 = new RGB(171, 171, 171);
        ///<summary>RGB representation of X11 color named Gray68</summary>
        public static readonly RGB Gray68 = new RGB(173, 173, 173);
        ///<summary>RGB representation of X11 color named Grey68</summary>
        public static readonly RGB Grey68 = new RGB(173, 173, 173);
        ///<summary>RGB representation of X11 color named Gray69</summary>
        public static readonly RGB Gray69 = new RGB(176, 176, 176);
        ///<summary>RGB representation of X11 color named Grey69</summary>
        public static readonly RGB Grey69 = new RGB(176, 176, 176);
        ///<summary>RGB representation of X11 color named Gray70</summary>
        public static readonly RGB Gray70 = new RGB(179, 179, 179);
        ///<summary>RGB representation of X11 color named Grey70</summary>
        public static readonly RGB Grey70 = new RGB(179, 179, 179);
        ///<summary>RGB representation of X11 color named Gray71</summary>
        public static readonly RGB Gray71 = new RGB(181, 181, 181);
        ///<summary>RGB representation of X11 color named Grey71</summary>
        public static readonly RGB Grey71 = new RGB(181, 181, 181);
        ///<summary>RGB representation of X11 color named Gray72</summary>
        public static readonly RGB Gray72 = new RGB(184, 184, 184);
        ///<summary>RGB representation of X11 color named Grey72</summary>
        public static readonly RGB Grey72 = new RGB(184, 184, 184);
        ///<summary>RGB representation of X11 color named Gray73</summary>
        public static readonly RGB Gray73 = new RGB(186, 186, 186);
        ///<summary>RGB representation of X11 color named Grey73</summary>
        public static readonly RGB Grey73 = new RGB(186, 186, 186);
        ///<summary>RGB representation of X11 color named Gray74</summary>
        public static readonly RGB Gray74 = new RGB(189, 189, 189);
        ///<summary>RGB representation of X11 color named Grey74</summary>
        public static readonly RGB Grey74 = new RGB(189, 189, 189);
        ///<summary>RGB representation of X11 color named Gray75</summary>
        public static readonly RGB Gray75 = new RGB(191, 191, 191);
        ///<summary>RGB representation of X11 color named Grey75</summary>
        public static readonly RGB Grey75 = new RGB(191, 191, 191);
        ///<summary>RGB representation of X11 color named Gray76</summary>
        public static readonly RGB Gray76 = new RGB(194, 194, 194);
        ///<summary>RGB representation of X11 color named Grey76</summary>
        public static readonly RGB Grey76 = new RGB(194, 194, 194);
        ///<summary>RGB representation of X11 color named Gray77</summary>
        public static readonly RGB Gray77 = new RGB(196, 196, 196);
        ///<summary>RGB representation of X11 color named Grey77</summary>
        public static readonly RGB Grey77 = new RGB(196, 196, 196);
        ///<summary>RGB representation of X11 color named Gray78</summary>
        public static readonly RGB Gray78 = new RGB(199, 199, 199);
        ///<summary>RGB representation of X11 color named Grey78</summary>
        public static readonly RGB Grey78 = new RGB(199, 199, 199);
        ///<summary>RGB representation of X11 color named Gray79</summary>
        public static readonly RGB Gray79 = new RGB(201, 201, 201);
        ///<summary>RGB representation of X11 color named Grey79</summary>
        public static readonly RGB Grey79 = new RGB(201, 201, 201);
        ///<summary>RGB representation of X11 color named Gray80</summary>
        public static readonly RGB Gray80 = new RGB(204, 204, 204);
        ///<summary>RGB representation of X11 color named Grey80</summary>
        public static readonly RGB Grey80 = new RGB(204, 204, 204);
        ///<summary>RGB representation of X11 color named Gray81</summary>
        public static readonly RGB Gray81 = new RGB(207, 207, 207);
        ///<summary>RGB representation of X11 color named Grey81</summary>
        public static readonly RGB Grey81 = new RGB(207, 207, 207);
        ///<summary>RGB representation of X11 color named Gray82</summary>
        public static readonly RGB Gray82 = new RGB(209, 209, 209);
        ///<summary>RGB representation of X11 color named Grey82</summary>
        public static readonly RGB Grey82 = new RGB(209, 209, 209);
        ///<summary>RGB representation of X11 color named Gray83</summary>
        public static readonly RGB Gray83 = new RGB(212, 212, 212);
        ///<summary>RGB representation of X11 color named Grey83</summary>
        public static readonly RGB Grey83 = new RGB(212, 212, 212);
        ///<summary>RGB representation of X11 color named Gray84</summary>
        public static readonly RGB Gray84 = new RGB(214, 214, 214);
        ///<summary>RGB representation of X11 color named Grey84</summary>
        public static readonly RGB Grey84 = new RGB(214, 214, 214);
        ///<summary>RGB representation of X11 color named Gray85</summary>
        public static readonly RGB Gray85 = new RGB(217, 217, 217);
        ///<summary>RGB representation of X11 color named Grey85</summary>
        public static readonly RGB Grey85 = new RGB(217, 217, 217);
        ///<summary>RGB representation of X11 color named Gray86</summary>
        public static readonly RGB Gray86 = new RGB(219, 219, 219);
        ///<summary>RGB representation of X11 color named Grey86</summary>
        public static readonly RGB Grey86 = new RGB(219, 219, 219);
        ///<summary>RGB representation of X11 color named Gray87</summary>
        public static readonly RGB Gray87 = new RGB(222, 222, 222);
        ///<summary>RGB representation of X11 color named Grey87</summary>
        public static readonly RGB Grey87 = new RGB(222, 222, 222);
        ///<summary>RGB representation of X11 color named Gray88</summary>
        public static readonly RGB Gray88 = new RGB(224, 224, 224);
        ///<summary>RGB representation of X11 color named Grey88</summary>
        public static readonly RGB Grey88 = new RGB(224, 224, 224);
        ///<summary>RGB representation of X11 color named Gray89</summary>
        public static readonly RGB Gray89 = new RGB(227, 227, 227);
        ///<summary>RGB representation of X11 color named Grey89</summary>
        public static readonly RGB Grey89 = new RGB(227, 227, 227);
        ///<summary>RGB representation of X11 color named Gray90</summary>
        public static readonly RGB Gray90 = new RGB(229, 229, 229);
        ///<summary>RGB representation of X11 color named Grey90</summary>
        public static readonly RGB Grey90 = new RGB(229, 229, 229);
        ///<summary>RGB representation of X11 color named Gray91</summary>
        public static readonly RGB Gray91 = new RGB(232, 232, 232);
        ///<summary>RGB representation of X11 color named Grey91</summary>
        public static readonly RGB Grey91 = new RGB(232, 232, 232);
        ///<summary>RGB representation of X11 color named Gray92</summary>
        public static readonly RGB Gray92 = new RGB(235, 235, 235);
        ///<summary>RGB representation of X11 color named Grey92</summary>
        public static readonly RGB Grey92 = new RGB(235, 235, 235);
        ///<summary>RGB representation of X11 color named Gray93</summary>
        public static readonly RGB Gray93 = new RGB(237, 237, 237);
        ///<summary>RGB representation of X11 color named Grey93</summary>
        public static readonly RGB Grey93 = new RGB(237, 237, 237);
        ///<summary>RGB representation of X11 color named Gray94</summary>
        public static readonly RGB Gray94 = new RGB(240, 240, 240);
        ///<summary>RGB representation of X11 color named Grey94</summary>
        public static readonly RGB Grey94 = new RGB(240, 240, 240);
        ///<summary>RGB representation of X11 color named Gray95</summary>
        public static readonly RGB Gray95 = new RGB(242, 242, 242);
        ///<summary>RGB representation of X11 color named Grey95</summary>
        public static readonly RGB Grey95 = new RGB(242, 242, 242);
        ///<summary>RGB representation of X11 color named Gray96</summary>
        public static readonly RGB Gray96 = new RGB(245, 245, 245);
        ///<summary>RGB representation of X11 color named Grey96</summary>
        public static readonly RGB Grey96 = new RGB(245, 245, 245);
        ///<summary>RGB representation of X11 color named Gray97</summary>
        public static readonly RGB Gray97 = new RGB(247, 247, 247);
        ///<summary>RGB representation of X11 color named Grey97</summary>
        public static readonly RGB Grey97 = new RGB(247, 247, 247);
        ///<summary>RGB representation of X11 color named Gray98</summary>
        public static readonly RGB Gray98 = new RGB(250, 250, 250);
        ///<summary>RGB representation of X11 color named Grey98</summary>
        public static readonly RGB Grey98 = new RGB(250, 250, 250);
        ///<summary>RGB representation of X11 color named Gray99</summary>
        public static readonly RGB Gray99 = new RGB(252, 252, 252);
        ///<summary>RGB representation of X11 color named Grey99</summary>
        public static readonly RGB Grey99 = new RGB(252, 252, 252);
        ///<summary>RGB representation of X11 color named Gray100</summary>
        public static readonly RGB Gray100 = new RGB(255, 255, 255);
        ///<summary>RGB representation of X11 color named Grey100</summary>
        public static readonly RGB Grey100 = new RGB(255, 255, 255);
        ///<summary>RGB representation of X11 color named DarkGrey</summary>
        public static readonly RGB DarkGrey = new RGB(169, 169, 169);
        ///<summary>RGB representation of X11 color named DarkGray</summary>
        public static readonly RGB DarkGray = new RGB(169, 169, 169);
        ///<summary>RGB representation of X11 color named DarkBlue</summary>
        public static readonly RGB DarkBlue = new RGB(0, 0, 139);
        ///<summary>RGB representation of X11 color named DarkCyan</summary>
        public static readonly RGB DarkCyan = new RGB(0, 139, 139);
        ///<summary>RGB representation of X11 color named DarkMagenta</summary>
        public static readonly RGB DarkMagenta = new RGB(139, 0, 139);
        ///<summary>RGB representation of X11 color named DarkRed</summary>
        public static readonly RGB DarkRed = new RGB(139, 0, 0);
        ///<summary>RGB representation of X11 color named LightGreen</summary>
        public static readonly RGB LightGreen = new RGB(144, 238, 144);
    }
}