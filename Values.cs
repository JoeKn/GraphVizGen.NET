// ====================================================================================
// File: Values.cs / GraphViz C# .NET Generator
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
using System.Collections.Generic;
using System.Drawing;

namespace GraphViz.Values
{
    /// <summary>
    /// Implements a restricted version of an ID:<para/>
    ///     Any string of<para/>
    ///     - alphabetic ([a-zA-Z\200-\377]) characters<para/>
    ///     - underscores ('_')<para/> 
    ///     - digits ([0-9])<para/>
    ///     - not beginning with a digit<para/>
    ///     Note that this implemantaion does not allow none-letters between \200-\377 (U+0080-U-00FF).
    ///     Control characters, currency-symbols etc. and multiplication/division signs are not allowed.
    /// </summary>
    public class GVID : GVValue
    {
        private readonly string name;

        /// <summary>
        /// Constructs a named ID
        /// </summary>
        /// <param name="name">The name of this ID</param>
        public GVID(string name)
        {
            char[] nameChars = name.ToCharArray();

            if (!(IsLatin(nameChars[0]) || (nameChars[0] == '_'))) throw new GVException("IDs must start with a latin letter or underscore");

            for (int i = 1; i < nameChars.Length; i++)
            {
                if (!(IsLatin(nameChars[0]) || IsDigit(nameChars[0]) || (nameChars[0] == '_')))
                    throw new GVException("IDs must only contain latin letters, decimal digits or underscore");
            }
            this.name = name;
        }

        private bool IsLatin(char c)
        {
            return
                (
                ((c >= 'a') && (c <= 'z')) ||
                ((c >= 'A') && (c <= 'Z')) ||
                ((c >= 0x00C0) && (c <= 0x00D6)) ||
                ((c >= 0x00D8) && (c <= 0x00F6)) ||
                ((c >= 0x00F8) && (c <= 0x00FF))
                );
        }

        private bool IsDigit(char c)
        {
            return ((c >= '0') && (c <= '9'));
        }

        /// <summary>
        /// Builds a test-represenation af this ID
        /// </summary>
        /// <returns>The test-represenation af this ID</returns>
        public override string ToString()
        {
            return name;
        }
    }

    public class GVBool : GVValue
    {
        private readonly bool value;

        public GVBool(bool value)
        {
            this.value = value;
        }

        public override string ToString()
        {
            return (value) ? "true" : "false";
        }
    }

    public class GVDouble : GVValue
    {
        public static readonly GVDouble Zero = new GVDouble(0.0);
        public static readonly GVDouble One = new GVDouble(1.0);

        private readonly double value;

        public GVDouble(double value)
        {
            this.value = value;
        }

        public GVDouble(long value)
        {
            this.value = (double)value;
        }

        public override string ToString()
        {
            return value.ToString("F", EnUS);
        }

        public static bool operator ==(GVDouble a, GVDouble b)
        {
            return a.value == b.value;
        }

        public static bool operator !=(GVDouble a, GVDouble b)
        {
            return a.value != b.value;
        }

        public static bool operator <(GVDouble a, GVDouble b)
        {
            return a.value < b.value;
        }

        public static bool operator >(GVDouble a, GVDouble b)
        {
            return a.value > b.value;
        }

        public static GVDouble operator +(GVDouble a)
        {
            return a;
        }

        public static GVDouble operator -(GVDouble a)
        {
            return new GVDouble(-a.value);
        }

        public static GVDouble operator +(GVDouble a, GVDouble b)
        {
            return new GVDouble(a.value + b.value);
        }

        public static GVDouble operator -(GVDouble a, GVDouble b)
        {
            return new GVDouble(a.value - b.value);
        }

        public static GVDouble operator *(GVDouble a, GVDouble b)
        {
            return new GVDouble(a.value * b.value);
        }

        public static GVDouble operator /(GVDouble a, GVDouble b)
        {
            if (b.value == 0.0)
            {
                throw new DivideByZeroException();
            }
            return new GVDouble(a.value / b.value);
        }
    }

    public class GVDoubleList : GVValue
    {
        private readonly Queue<GVDouble> doubleList;

        public GVDoubleList()
        {
            doubleList = new Queue<GVDouble>();
        }

        public void Add(GVDouble d)
        {
            doubleList.Enqueue(d);
        }

        public override string ToString()
        {
            string result = "";
            string colon = "";

            while (doubleList.Count > 0)
            {
                GVDouble d = doubleList.Dequeue();
                result += colon + d.ToString();
                colon = ":";
            }
            return result;
        }
    }

    public class GVInt : GVValue
    {
        public static readonly GVInt Zero = new GVInt(0);
        public static readonly GVInt Int8Max = new GVInt(127);
        public static readonly GVInt UInt8Max = new GVInt(255);
        public static readonly GVInt AngleMax = new GVInt(360);
        public static readonly GVInt UInt16Max = new GVInt(65535);

        private readonly long value;

        public GVInt(long value)
        {
            this.value = value;
        }

        public override string ToString()
        {
            return value.ToString();
        }

        public static bool operator ==(GVInt a, GVInt b)
        {
            return a.value == b.value;
        }

        public static bool operator !=(GVInt a, GVInt b)
        {
            return a.value != b.value;
        }

        public static bool operator <(GVInt a, GVInt b)
        {
            return a.value < b.value;
        }

        public static bool operator >(GVInt a, GVInt b)
        {
            return a.value > b.value;
        }

        public static GVInt operator +(GVInt a)
        {
            return a;
        }

        public static GVInt operator -(GVInt a)
        {
            return new GVInt(-a.value);
        }

        public static GVInt operator +(GVInt a, GVInt b)
        {
            return new GVInt(a.value + b.value);
        }

        public static GVInt operator -(GVInt a, GVInt b)
        {
            return new GVInt(a.value - b.value);
        }

        public static GVInt operator *(GVInt a, GVInt b)
        {
            return new GVInt(a.value * b.value);
        }

        public static GVInt operator /(GVInt a, GVInt b)
        {
            if (b.value == 00)
            {
                throw new DivideByZeroException();
            }
            return new GVInt(a.value / b.value);
        }
    }

    public class GVString : GVValue
    {
        private readonly string value;

        public GVString(string value)
        {
            this.value = value;
        }

        public override string ToString()
        {
            return "\"" + value + "\"";
        }
    }

    public class GVEscString : GVString
    {
        public const string BackSlash = "\\\\";
        public const string HeadNodeName = "\\H";
        public const string NodeName = "\\N";
        public const string TailNodeName = "\\T";
        public const string GraphName = "\\G";
        public const string EdgeName = "\\E";
        public const string Label = "\\L";
        public const string HeadLabel = "\\l";
        public const string CenterLabel = "\\n";
        public const string TailLabel = "\\r";

        private readonly string value;

        public GVEscString(string value) : base(value) { }
    }

    /// <summary>
    /// The base class for all HTML content
    /// </summary>
    public abstract class GVHTML : GVValue { }

    /// <summary>
    /// The base class for all colors
    /// </summary>
    public abstract class GVColor : GVValue { }

    /// <summary>
    /// 
    /// </summary>
    public class GVColorList : GVColor {
        private struct WeightedColor {
            public GVDouble Weight;
            public GVColor Color;
        }

        private GVDouble weightSum;
        private readonly Queue<WeightedColor> colorList;

        public GVColorList()
        {
            colorList = new Queue<WeightedColor>();
            weightSum = new GVDouble(0.0);
        }

        public void Add(GVColor color, GVDouble weight)
        {
            if (weight < GVDouble.Zero) throw new GVException("Color-weight must be between 0.0 and 1.0");
            if ((weightSum + weight) > GVDouble.One) throw new GVException("Summed Color-weights exceed 1.0");

            weightSum += weight;
            colorList.Enqueue(new WeightedColor { Weight = weight, Color = color });
        }
        public void Add(GVColor color)
        {
            // we use Weight = -1.0 to state that this color has no explicit weight
            colorList.Enqueue(new WeightedColor { Weight = -GVDouble.One, Color = color });
        }

        public override string ToString()
        {
            string result = "";
            string colon = "";

            while (colorList.Count > 0)
            {
                WeightedColor wc = colorList.Dequeue();
                if (wc.Weight == -GVDouble.One)
                {
                    result += colon + wc.Color.ToString();
                } else
                {
                    result += colon + wc.Color.ToString() + ";" + wc.Weight.ToString();
                }
                colon = ":";
            }
            return result;
        }
    }

    public class GVArrowType : GVValue
    {
        private readonly string type;
        private GVArrowType(string type)
        {
            this.type = type;
        }

        public static GVArrowType Normal = new GVArrowType("normal");
        public static GVArrowType Inv = new GVArrowType("inv");
        public static GVArrowType Dot = new GVArrowType("dot");
        public static GVArrowType InvDot = new GVArrowType("invdot");
        public static GVArrowType ODot = new GVArrowType("odot");
        public static GVArrowType InvODot = new GVArrowType("invodot");
        public static GVArrowType None = new GVArrowType("none");
        public static GVArrowType Tee = new GVArrowType("tee");
        public static GVArrowType Empty = new GVArrowType("empty");
        public static GVArrowType InvEmpty = new GVArrowType("invempty");
        public static GVArrowType Diamond = new GVArrowType("diamond");
        public static GVArrowType ODiamond = new GVArrowType("odiamond");
        public static GVArrowType EDiamond = new GVArrowType("ediamond");
        public static GVArrowType Crow = new GVArrowType("crow");
        public static GVArrowType Box = new GVArrowType("box");
        public static GVArrowType OBox = new GVArrowType("obox");
        public static GVArrowType Open = new GVArrowType("open");
        public static GVArrowType HalfOpen = new GVArrowType("halfopen");
        public static GVArrowType Vee = new GVArrowType("vee");

        public override string ToString()
        {
            return type;
        }
    }

    public class GVPoint : GVValue
    {
        private readonly GVDouble x;
        private readonly GVDouble y;
        private readonly GVDouble z;

        public bool Is3D { get; }
        public bool IsMarked { get; }

        public GVPoint(double x, double y)
        {
            this.x = new GVDouble(x);
            this.y = new GVDouble(y);
            Is3D = false;
            IsMarked = false;
        }

        public GVPoint(double x, double y, bool marked)
        {
            this.x = new GVDouble(x);
            this.y = new GVDouble(y);
            Is3D = false;
            IsMarked = marked;
        }

        public GVPoint(double x, double y, double z)
        {
            this.x = new GVDouble(x);
            this.y = new GVDouble(y);
            this.z = new GVDouble(z);
            Is3D = false;
            IsMarked = false;
        }

        public GVPoint(double x, double y, double z, bool marked)
        {
            this.x = new GVDouble(x);
            this.y = new GVDouble(y);
            this.z = new GVDouble(z);
            Is3D = false;
            IsMarked = marked;
        }

        public override string ToString()
        {
            return x.ToString() + "," + y.ToString() + ((Is3D)? z.ToString() : "") + ((IsMarked)? "!" : "");
        }

    }

    /// <summary>
    /// 
    /// </summary>
    public class GVPointList : GVValue
    {
        private readonly Queue<GVPoint> pointList;

        public GVPointList()
        {
            pointList = new Queue<GVPoint>();
        }

        public void Add(GVPoint point)
        {
            pointList.Enqueue(point);
        }

        public override string ToString()
        {
            string result = "";
            string space = "";

            while (pointList.Count > 0)
            {
                GVPoint point = pointList.Dequeue();
                result += space + point.ToString();
                space = " ";
            }
            return result;
        }
    }

    public class GVRect : GVValue
    {
        protected GVPoint lowerLeft;
        protected GVPoint upperRight;
        public GVRect(GVPoint lowerLeft, GVPoint upperRight)
        {
            if (lowerLeft.Is3D || upperRight.Is3D) throw new GVException("2D GVPoints expected");
            if (lowerLeft.IsMarked || upperRight.IsMarked) throw new GVException("Unmarked GVPoints expected");
            this.lowerLeft = lowerLeft;
            this.upperRight = upperRight;
        }

        public override string ToString()
        {
            return lowerLeft.ToString() + "," + upperRight.ToString();
        }
    }

    public class GVClusterMode : GVValue
    {
        private readonly string mode;

        private GVClusterMode(string mode)
        {
            this.mode = mode;
        }

        public static GVClusterMode Local = new GVClusterMode("local");
        public static GVClusterMode GLobal = new GVClusterMode("global");
        public static GVClusterMode None = new GVClusterMode("none");

        public override string ToString()
        {
            return mode;
        }
    }

    public class GVDirType : GVValue
    {
        private readonly string type;

        private GVDirType(string type)
        {
            this.type = type;
        }

        public static GVDirType Forward = new GVDirType("forward");
        public static GVDirType Back = new GVDirType("back");
        public static GVDirType Both = new GVDirType("both");
        public static GVDirType None = new GVDirType("none");

        public override string ToString()
        {
            return type;
        }
    }

}
