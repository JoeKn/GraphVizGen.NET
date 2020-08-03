// ====================================================================================
// File: HTML.cs / GraphViz C# .NET Generator
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
using GraphViz.Values;

namespace GraphViz.HTML
{
    /// <summary>
    /// Implements a label <para/>
    /// label 	    : text
    ///		        | fonttable <para/>
    /// text 	    : textitem
    ///		        | text textitem <para/>
    /// textitem 	: string
    /// 	    	| &lt;BR/&gt;
    /// 		    | &lt;FONT&gt; text &lt;/FONT&gt;
    /// 		    | &lt;I&gt; text &lt;/I&gt;
    /// 		    | &lt;B&gt; text &lt;/B&gt;
    /// 		    | &lt;U&gt; text &lt;/U&gt;
    /// 		    | &lt;O&gt; text &lt;/O&gt;
    /// 		    | &lt;SUB&gt; text &lt;/SUB&gt;
    /// 		    | &lt;SUP&gt; text &lt;/SUP&gt;
    /// 		    | &lt;S&gt; text &lt;/S&gt; <para/>
    /// fonttable 	: table
    /// 		    | &lt;FONT&gt; table &lt;/FONT&gt;
    /// 		    | &lt;I&gt; table &lt;/I&gt;
    /// 		    | &lt;B&gt; table &lt;/B&gt;
    /// 	    	| &lt;U&gt; table &lt;/U&gt;
    /// 		    | &lt;O&gt; table &lt;/O&gt; <para/>
    /// table 	    : &lt;TABLE&gt; rows &lt;/TABLE&gt; <para/>
    /// rows 	    : row
    /// 		    | rows row
    /// 		    | rows &lt;HR/&gt; row <para/>
    /// row 	    : &lt;TR&gt; cells &lt;/TR&gt; <para/>
    /// cells 	    : cell
    ///	    	    | cells cell
    /// 		    | cells &lt;VR/&gt; cell <para/>
    /// cell 	    : &lt;TD&gt; label &lt;/TD&gt;
    /// 		    | &lt;TD&gt; &lt;IMG/&gt; &lt;/TD&gt; <para/>
    /// </summary>
    /// See <see href="https://graphviz.org/doc/info/shapes.html#html"/>
    public class Label : GVHTML 
    {
        private readonly Text text;
        private readonly FontTable table;

        /// <summary>
        /// Constructs a label containing text.
        /// </summary>
        /// <param name="text">The label's inner text</param>
        /// See <see cref="Text"/> for the content entity.
        public Label(Text text)
        {
            this.text = text;
            table = null;
        }

        /// <summary>
        /// Constructs a label containing a table.
        /// </summary>
        /// <param name="table">The label's inner table</param>
        /// See <see cref="FontTable"/> for the content entity.
        public Label(FontTable table)
        {
            text = null;
            this.table = table;
        }

        /// <summary>
        /// Builds the text representation of the label
        /// </summary>
        /// <returns>The text representation of the label</returns>
        public override string ToString()
        {
            if (text != null) return "<" + text.ToString() + ">";
            if (table != null) return "<" + table.ToString() + ">";
            return "";
        }
    }

    /// <summary>
    /// Implements text <para/>
    /// text 	    : textitem
    ///		        | text textitem <para/>
    /// </summary>
    /// See <see cref="Label"/>
    public class Text : GVHTML
    {
        private readonly Queue<TextItem> items;

        /// <summary>
        /// Constructs a Text with an empty textitem-queue
        /// </summary>
        public Text()
        {
            items = new Queue<TextItem>();
        }

        /// <summary>
        /// Adds tzextitems to the Text
        /// </summary>
        /// <param name="item">the textitem to add</param>
        /// See <see cref="TextItem"/> for the content entity.
        public void Add(TextItem item)
        {
            items.Enqueue(item);
        }

        /// <summary>
        /// Builds the text representation of all contained textitems
        /// </summary>
        /// <returns>The text representation of all textitems</returns>
        public override string ToString()
        {
            string result = "";

            while (items.Count > 0)
            {
                result += items.Dequeue().ToString();
            }
            return result;
        }
    }

    /// <summary>
    /// Implements a abstract textitem <para/>
    /// textitem 	: string
    /// 	    	| &lt;BR/&gt;
    /// 		    | &lt;FONT&gt; text &lt;/FONT&gt;
    /// 		    | &lt;I&gt; text &lt;/I&gt;
    /// 		    | &lt;B&gt; text &lt;/B&gt;
    /// 		    | &lt;U&gt; text &lt;/U&gt;
    /// 		    | &lt;O&gt; text &lt;/O&gt;
    /// 		    | &lt;SUB&gt; text &lt;/SUB&gt;
    /// 		    | &lt;SUP&gt; text &lt;/SUP&gt;
    /// 		    | &lt;S&gt; text &lt;/S&gt; <para/>
    /// </summary>
    /// See <see cref="Text"/>
    public abstract class TextItem : GVHTML { }

    /// <summary>
    /// Implements a abstract fonttabe <para/>
    /// fonttable 	: table
    /// 		    | &lt;FONT&gt; table &lt;/FONT&gt;
    /// 		    | &lt;I&gt; table &lt;/I&gt;
    /// 		    | &lt;B&gt; table &lt;/B&gt;
    /// 	    	| &lt;U&gt; table &lt;/U&gt;
    /// 		    | &lt;O&gt; table &lt;/O&gt; <para/>
    /// </summary>
    /// See <see cref="Label"/>
    public abstract class FontTable : GVHTML { }

    /// <summary>
    /// Inplemets a HTML string 
    /// </summary>
    /// See <see cref="TextItem"/>
    public class String : TextItem
    {
        private readonly char[] html;

        /// <summary>
        /// Constructs a HTML String
        /// </summary>
        /// <param name="s">An UTF-16 string that is transformed into a HTML string</param>
        public String(string s)
        {
            char[] sArray = s.ToCharArray();
            int i;
            int j;
            for (i=0, j=0; i < sArray.Length; i++)
            {
                // Drop all control-characters below 0x20 (space)
                if (sArray[i] <= 0x001F) continue;
                if ((sArray[i] > 0x001F) && (sArray[i] < 0x007F))
                {
                    // Replace all amersand, less-than and greater-than by corresponding esc-sequences
                    switch (sArray[i])
                    {
                        case '&': j = ToHex(j, '&'); break;
                        case '<': j = ToHex(j, '<'); break;
                        case '>': j = ToHex(j, '>'); break;
                        default: html[j++] = sArray[i]; break;
                    }
                }
                // We Implicitly drop every control-character beween 0x7F and 0x9F
                // Replace everything above 0x9F by a corresponding esc-sequence
                if (sArray[i] > 0x009F) ToHex(j, sArray[i]);
            }
        }

        private char GetHexChar(int x)
        {
            if (x < 10) return (char)(x + '0');
            return (char)((x - 10) + 'A');
        }

        private int ToHex(int j, char c)
        {
            html[j++] = '&';
            html[j++] = '#';
            html[j++] = 'x';
            html[j++] = GetHexChar((c & 0xF000) >> 12);
            html[j++] = GetHexChar((c & 0x0F00) >> 8);
            html[j++] = GetHexChar((c & 0x00F0) >> 4);
            html[j++] = GetHexChar((c & 0x000F));
            html[j++] = ';';
            return j;
        }

        // TODO: implement ToString()
    }

    /// <summary>
    /// The super class for all tagged text entities within a GraphViz's HTML-Label
    /// </summary>
    public class TaggedText : TextItem
    {
        /// <summary>
        /// These are all available tags within GraphViz's HTML-Text-Tags.
        /// </summary>
        public enum Tags
        {
            /// <summary>The enumeration value for: &lt;BR/&gt;.</summary>
            LineBreak = 0,
            /// <summary>The enumeration Value for: &lt;FONT&gt; some text &lt;/FONT&gt;.</summary>
            FontSpec,
            /// <summary>The enumeration Value for: &lt;I&gt; some text &lt;/I&gt;.</summary>
            Italic,
            /// <summary>The enumeration Value for: &lt;B&gt; some text &lt;/B&gt;.</summary>
            Bold,
            /// <summary>The enumeration Value for: &lt;U&gt; some text &lt;/U&gt;.</summary>
            Underline,
            /// <summary>The enumeration Value for: &lt;O&gt; some text &lt;/O&gt;.</summary>
            Overline,
            /// <summary>The enumeration Value for: &lt;SUB&gt; some text &lt;/SUB&gt;.</summary>
            Subscript,
            /// <summary>The enumeration Value for: &lt;SUP&gt; some text &lt;/SUP&gt;.</summary>
            Superscript,
            /// <summary>The enumeration Value for: &lt;S&gt; some text &lt;/S&gt;.</summary>
            StrikeThrough,
        }

        /// <summary>
        /// These are the text-representations of all GraphViz's HTML-Text-Tags.
        /// </summary>
        private static readonly string[] name =
        {
            "BR",
            "FONT",
            "I",
            "B",
            "U",
            "O",
            "SUB",
            "SUP",
            "S",
        };

        /// <summary>
        /// Holds the tag for a specific tagged entity. 
        /// </summary>
        protected Tags tag;

        /// <summary>
        /// Keeps information about whether an tagged entity contains text or not.
        /// </summary>
        protected bool hasContent;

        /// <summary>
        /// Keeps the content if any.
        /// </summary>
        protected Text text;

        /// <summary>
        /// Handles an tagged entity's supported attributes if any
        /// </summary>
        /// See <see cref="HTML.Attributes">for a description on how to add attributes.</see>
        protected Attributes attributes;

        /// <summary>
        /// Allows access to <value>attributes</value>
        /// </summary>
        public Attributes Attributes { get => attributes; }

        /// <summary>
        /// Initializes the tagged entity's base.
        /// </summary>
        /// <param name="tag"></param> The specific entity's tag
        /// <param name="hasContent"></param> Whether or not the entity has a content
        public TaggedText(Tags tag, bool hasContent)
        {
            this.tag = tag;
            this.hasContent = hasContent;
        }

        /// <summary>
        /// Builds the text representation of the specific tagged entity
        /// </summary>
        /// <returns>The text representation of the specific tagged entity</returns>
        public override string ToString()
        {
            string result;

            result = "<" + name[(int)tag];
            result += attributes.ToString();
            result += (hasContent) ? ">" + ContentToString() + "</" + name[(int)tag] + ">" : "/>";
            return result;
        }

        /// <summary>
        /// Builds the inner text representation of the specific tagged entity if any
        /// </summary>
        /// <returns>The inner text representation of the specific tagged entity if any</returns>
        public string ContentToString()
        {
            return (hasContent)? text.ToString() : "";
        }
    }

    /// <summary>
    /// Implements line break: &lt;BR/&gt;
    /// </summary>
    /// <inheritdoc/>
    public class LineBreak : TaggedText
    {
        /// <summary>
        /// Holds all attributes supported by a line break entity.
        /// </summary>
        /// See <see cref="Attributes"/> for all available attributes and how say are handled.
        protected static Attributes.ID[] supported =
        {
            Attributes.ID.Align,
        };


        /// <summary>
        /// Constructs a LineBreak
        /// </summary>
        public LineBreak() : base(Tags.LineBreak, false)
        {
            attributes = new Attributes(supported);
        }
    }

    /// <summary>
    /// Implements text with a given font: &lt;FONT&gt; some text &lt;/FONT&gt;
    /// </summary>
    /// <inheritdoc/>
    public class TextFont : TaggedText
    {
        /// <summary>
        /// Holds all attributes supported by a text font entity.
        /// </summary>
        /// See <see cref="Attributes"/> for all available attributes and how say are handled.
        protected static Attributes.ID[] supported =
        {
            Attributes.ID.Color,
            Attributes.ID.Face,
            Attributes.ID.PointSize,
        };

        /// <summary>
        /// Constructs a text with a given font
        /// </summary>
        /// <param name="text">the text that should be rendered with the given font</param>
        /// See <see cref="Text"/> for the content entity.
        public TextFont(Text text) : base(Tags.FontSpec, true)
        {
            attributes = new Attributes(supported);
            this.text = text;
        }
    }

    /// <summary>
    /// Implements Italic text: &lt;I&gt; some text &lt;/I&gt;
    /// </summary>
    /// <inheritdoc/>
    public class TextItalic : TaggedText
    {
        /// <summary>
        /// Constructs italic text
        /// </summary>
        /// <param name="text">the text that should be rendered italic</param>
        /// See <see cref="Text"/> for the content entity.
        public TextItalic(Text text) : base(Tags.Italic, true)
        {
            attributes = new Attributes(null);
            this.text = text;
        }
    }

    /// <summary>
    /// Implements bold text: &lt;B&gt; some text &lt;/B&gt;
    /// </summary>
    /// <inheritdoc/>
    public class TextBold : TaggedText
    {
        /// <summary>
        /// Constructs bold text
        /// </summary>
        /// <param name="text">the text that should be rendered bold</param>
        /// See <see cref="Text"/> for the content entity.
        public TextBold(Text text) : base(Tags.Bold, true)
        {
            attributes = new Attributes(null);
            this.text = text;
        }
    }

    /// <summary>
    /// Implements underlined text: &lt;U&gt; some text &lt;/U&gt;
    /// </summary>
    /// <inheritdoc/>
    public class TextUnderline : TaggedText
    {
        /// <summary>
        /// Constructs underlined text
        /// </summary>
        /// <param name="text">the text that should be rendered underlined</param>
        /// See <see cref="Text"/> for the content entity.
        public TextUnderline(Text text) : base(Tags.Underline, true)
        {
            attributes = new Attributes(null);
            this.text = text;
        }
    }

    /// <summary>
    /// Implements overlined text: &lt;O&gt; some text &lt;/O&gt;
    /// </summary>
    /// <inheritdoc/>
    public class TextOverline : TaggedText
    {
        /// <summary>
        /// Constructs overlined text
        /// </summary>
        /// <param name="text">the text that should be rendered overlined</param>
        /// See <see cref="Text"/> for the content entity.
        public TextOverline(Text text) : base(Tags.Overline, true)
        {
            attributes = new Attributes(null);
            this.text = text;
        }
    }

    /// <summary>
    /// Implements subscripted text: &lt;SUB&gt; some text &lt;/SUB&gt;
    /// </summary>
    /// <inheritdoc/>
    public class Subscript : TaggedText
    {
        /// <summary>
        /// Constructs subscripted text
        /// </summary>
        /// <param name="text">the text that should be rendered subscripted</param>
        /// See <see cref="Text"/> for the content entity.
        public Subscript(Text text) : base(Tags.Subscript, true)
        {
            attributes = new Attributes(null);
            this.text = text;
        }
    }

    /// <summary>
    /// Implements superscripted text: &lt;SUP&gt; some text &lt;/SUP&gt;
    /// </summary>
    /// <inheritdoc/>
    public class Superscript : TaggedText
    {
        /// <summary>
        /// Constructs superscripted text
        /// </summary>
        /// <param name="text">the text that should be rendered superscripted</param>
        /// See <see cref="Text"/> for the content entity.
        public Superscript(Text text) : base(Tags.Superscript, true)
        {
            attributes = new Attributes(null);
            this.text = text;
        }
    }

    /// <summary>
    /// Implements striked through text: &lt;S&gt; some text &lt;/S&gt;
    /// </summary>
    /// <inheritdoc/>
    public class StrikeThrough : TaggedText
    {
        /// <summary>
        /// Constructs striked through text
        /// </summary>
        /// <param name="text">the text that should be rendered striked through</param>
        /// See <see cref="Text"/> for the content entity.
        public StrikeThrough(Text text) : base(Tags.StrikeThrough, true)
        {
            attributes = new Attributes(null);
            this.text = text;
        }
    }

    /// <summary>
    /// The super class for all tagged table entities within a GraphViz's HTML-Label
    /// </summary>
    public class TaggedTable : FontTable
    {
        /// <summary>
        /// These are all available tags within GraphViz's HTML-Table-Tags
        /// </summary>
        public enum Tags
        {
            /// <summary>The enumeration Value for: &lt;TABLE&gt; rows &lt;/TABLE&gt;.</summary>
            Table = 0,
            /// <summary>The enumeration Value for: &lt;TR&gt; cells &lt;/TR&gt;.</summary>
            TableRow,
            /// <summary>The enumeration Value for: &lt;TD&gt; label or image &lt;/TD&gt;.</summary>
            TableCell,
            /// <summary>The enumeration Value for: &lt;FONT&gt; table &lt;/FONT&gt;.</summary>
            FontSpec,
            /// <summary>The enumeration Value for: &lt;IMG/&gt;.</summary>
            Image,
            /// <summary>The enumeration Value for: &lt;I&gt; table &lt;/I&gt;.</summary>
            Italic,
            /// <summary>The enumeration Value for: &lt;B&gt; table &lt;/B&gt;.</summary>
            Bold,
            /// <summary>The enumeration Value for: &lt;U&gt; table &lt;/U&gt;.</summary>
            Underline,
            /// <summary>The enumeration Value for: &lt;O&gt; table &lt;/O&gt;.</summary>
            Overline,
            /// <summary>The enumeration Value for: &lt;HR/&gt;.</summary>
            HorizontalRule,
            /// <summary>The enumeration Value for: &lt;VR/&gt;.</summary>
            VerticalRule,
        }

        /// <summary>
        /// These are the text-representations of all GraphViz's HTML-Table-Tags
        /// </summary>
        private static readonly string[] name =
        {
            "TABLE",
            "TR",
            "TD",
            "FONT",
            "IMG",
            "I",
            "B",
            "U",
            "O",
            "HR",
            "VR",
        };

        /// <summary>
        /// Holds the tag for a specific tagged entity. 
        /// </summary>
        protected Tags tag;

        /// <summary>
        /// Keeps information about whether an tagged entity contains a table or not.
        /// </summary>
        protected bool hasContent;

        /// <summary>
        /// Keeps the table if any.
        /// </summary>
        protected Table table;

        /// <summary>
        /// Handles an tagged entity's supported attributes if any
        /// </summary>
        /// See <see cref="HTML.Attributes"/> for a description on how to add attributes.
        protected Attributes attributes;

        /// <summary>
        /// Allows access to <value>attributes</value>
        /// </summary>
        public Attributes Attributes { get => attributes; }

        /// <summary>
        /// Initializes the tagged entity's base. <value>attributes</value> is initialized in the specific constructor
        /// </summary>
        /// <param name="tag">The specific entity's tag</param> 
        /// <param name="hasContent">Whether or not the entity has a content</param> 
        public TaggedTable(Tags tag, bool hasContent)
        {
            this.tag = tag;
            this.hasContent = hasContent;
        }

        /// <summary>
        /// Builds the text representation of the specific tagged entity
        /// </summary>
        /// <returns>The text representation of the specific tagged entity</returns>
        public override string ToString()
        {
            string result;

            result = "<" + name[(int)tag];
            result += attributes.ToString();
            result += (hasContent) ? ">" + ContentToString() + "</" + name[(int)tag] + ">" : "/>";
            return result;
        }

        /// <summary>
        /// Builds the text representation of the table
        /// </summary>
        /// <returns>The text representation of the table</returns>
        public virtual string ContentToString()
        {
            return (hasContent)? table.ToString() : "";
        }
    }

    /// <summary>
    /// Implements a Table: &lt;TABLE&gt; some rows &lt;/TABLE&gt;
    /// </summary>
    /// <inheritdoc/>
    public class Table : TaggedTable
    {
        /// <summary>
        /// Holds all attributes supported by a table entity.
        /// </summary>
        /// See <see cref="Attributes"/> for all available attributes and how say are handled.
        protected static Attributes.ID[] supported =
        {
            Attributes.ID.Align,
            Attributes.ID.BGColor,
            Attributes.ID.Border,
            Attributes.ID.CellPadding,
            Attributes.ID.CellSpacing,
            Attributes.ID.Color,
            Attributes.ID.Columns,
            Attributes.ID.FixedSize,
            Attributes.ID.GradientAngle,
            Attributes.ID.Height,
            Attributes.ID.HRef,
            Attributes.ID.ID,
            Attributes.ID.Port,
            Attributes.ID.Rows,
            Attributes.ID.Sides,
            Attributes.ID.Style,
            Attributes.ID.Target,
            Attributes.ID.Title,
            Attributes.ID.ToolTip,
            Attributes.ID.VAlign,
            Attributes.ID.Width,
        };

        /// <summary>
        /// Stores the content of a table.
        /// </summary>
        /// See <see cref="TableRow"/> for the content entities.
        /// See <see cref="HorRule"/> for the content entities.
        private readonly Queue<TaggedTable> rows;

        /// <summary>
        /// Constructs a table entity.
        /// It also initializes its supported attributes.
        /// </summary>
        public Table() : base(Tags.Table, true)
        {
            attributes = new Attributes(supported);
            rows = new Queue<TaggedTable>();
        }

        /// <summary>
        /// Adds a row to the table <para/>
        /// rows 	    : <b>row</b>
        /// 		    | <b>rows row</b>
        /// 		    | <i>rows &lt;HR/&gt; row </i><para/>
        /// </summary>
        /// <param name="row"></param> the row to be added to the table.
        /// See <see cref="TableRow"/> for the content entities.
        public void Add(TableRow row)
        {
            rows.Enqueue(row);
        }

        /// <summary>
        /// Adds a horizontal rule to the table <para/>
        /// rows 	    : <i>row</i>
        /// 		    | <i>rows row</i>
        /// 		    | <b>rows &lt;HR/&gt; row</b> <para/>
        /// </summary>
        /// See <see cref="HorRule"/> for the content entities.
        public void AddVertRule()
        {
            rows.Enqueue(new HorRule());
        }

        /// <summary>
        /// Builds the text representation of all contained rows and horizontal rules.
        /// </summary>
        /// <returns>The text representation of all contained rows and horizontal rules</returns>
        public override string ContentToString()
        {
            string result = "";
            while (rows.Count > 0)
            {
                result += rows.Dequeue().ContentToString();
            }
            return result;
        }
    }

    /// <summary>
    /// Implements a Table Row &lt;TR&gt; some cells &lt;/TR&gt;
    /// </summary>
    /// <inheritdoc/>
    public class TableRow : TaggedTable
    {
        /// <summary>
        /// Stores the content of a table.
        /// </summary>
        /// See <see cref="TableCell"/> for the content entities.
        /// See <see cref="HorRule"/> for the content entities.
        private readonly Queue<TaggedTable> cells;

        /// <summary>
        /// Constructs a table row entity.
        /// </summary>
        public TableRow() : base(Tags.TableRow, true)
        {
            attributes = new Attributes(null);
            cells = new Queue<TaggedTable>();
        }

        /// <summary>
        /// Adds a cell to the row <para/>
        /// cells 	    : <b>cell</b>
        /// 		    | <b>cells cell</b>
        /// 		    | <i>cells &lt;VR/&gt; cell </i><para/>
        /// </summary>
        /// <param name="cell">The cell that should be added to the row</param>
        /// See <see cref="TableCell"/> for the content entities.
        public void Add(TableCell cell)
        {
            cells.Enqueue(cell);
        }

        /// <summary>
        /// Adds a vertical rule to the row <para/>
        /// cells 	    : <i>cell</i>
        /// 		    | <i>cells cell</i>
        /// 		    | <b>cells &lt;VR/&gt; cell</b> <para/>
        /// </summary>
        /// See <see cref="VertRule"/> for the content entities.
        public void AddVertRule()
        {
            cells.Enqueue(new VertRule());
        }

        /// <summary>
        /// Builds the text representation of all contained cells and vertical rules.
        /// </summary>
        /// <returns>The text representation of all contained cells and vertical rules</returns>
        public override string ContentToString()
        {
            string result = "";
            while (cells.Count > 0)
            {
                result += cells.Dequeue().ContentToString();
            }
            return result;
        }
    }

    /// <summary>
    /// Implements a Table Cell &lt;TD&gt; label or image &lt;/TD&gt;
    /// </summary>
    /// <inheritdoc/>
    public class TableCell : TaggedTable
    {
        /// <summary>
        /// Holds all attributes supported by a cell entity.
        /// </summary>
        /// See <see cref="Attributes"/> for all available attributes and how say are handled.
        protected static Attributes.ID[] supported =
        {
            Attributes.ID.Align,
            Attributes.ID.BAlign,
            Attributes.ID.BGColor,
            Attributes.ID.Border,
            Attributes.ID.CellPadding,
            Attributes.ID.CellSpacing,
            Attributes.ID.Color,
            Attributes.ID.ColSpan,
            Attributes.ID.FixedSize,
            Attributes.ID.GradientAngle,
            Attributes.ID.Height,
            Attributes.ID.HRef,
            Attributes.ID.ID,
            Attributes.ID.Port,
            Attributes.ID.RowSpan,
            Attributes.ID.Sides,
            Attributes.ID.Style,
            Attributes.ID.Target,
            Attributes.ID.Title,
            Attributes.ID.ToolTip,
            Attributes.ID.VAlign,
            Attributes.ID.Width,
        };

        private readonly Label label;
        private readonly Image image;

        /// <summary>
        /// Constructs a table cell entity containing a label.
        /// </summary>
        /// <param name="label">The label that is contained in the cell</param>
        /// See <see cref="Label"/> for the content entities.
        public TableCell(Label label) : base(Tags.TableCell, true)
        {
            attributes = new Attributes(supported);
            this.label = label;
            image = null;
        }

        /// <summary>
        /// Constructs a table cell entitycontaining an image.
        /// </summary>
        /// See <see cref="Image"/> for the content entities.
        public TableCell(Image image) : base(Tags.TableCell, true)
        {
            attributes = new Attributes(supported);
            label = null;
            this.image = image;
        }

        /// <summary>
        /// Builds the text representation of the contained label or image.
        /// </summary>
        /// <returns>The text representation of the contained label or image</returns>
        public override string ContentToString()
        {
            if (label != null) return label.ToString();
            if (image != null) return image.ToString();
            return "";
        }
    }

    /// <summary>
    /// Implements a Table Foont &lt;FONT&gt; table &lt;/FONT&gt;
    /// </summary>
    /// <inheritdoc/>
    public class TableFont : TaggedTable
    {
        /// <summary>
        /// Holds all attributes supported by a table font entity.
        /// </summary>
        /// See <see cref="Attributes"/> for all available attributes and how say are handled.
        protected static Attributes.ID[] supported =
        {
            Attributes.ID.Color,
            Attributes.ID.Face,
            Attributes.ID.PointSize,
        };

        /// <summary>
        /// Constructs a tablefont entity containing a table.
        /// </summary>
        /// <param name="table">The table with that font definition</param>
        /// See <see cref="Table"/> for the content entities.
        public TableFont(Table table) : base(Tags.FontSpec, true)
        {
            attributes = new Attributes(supported);
            this.table = table;
        }
    }


    /// <summary>
    /// Implements an image: &lt;IMG/&gt;
    /// </summary>
    /// <inheritdoc/>
    public class Image : TaggedTable
    {
        /// <summary>
        /// Holds all attributes supported by a image entity.
        /// </summary>
        /// See <see cref="Attributes"/> for all available attributes and how say are handled.
        protected static Attributes.ID[] supported =
        {
            Attributes.ID.Scale,
            Attributes.ID.Src,
        };

        /// <summary>
        /// Constructs a image entity.
        /// </summary>
        public Image() : base(Tags.Image, false)
        {
            attributes = new Attributes(supported);
        }
    }

    /// <summary>
    /// Implements a Italic table: &lt;I&gt; table &lt;/I&gt;
    /// </summary>
    /// <inheritdoc/>
    public class TableItalic : TaggedTable
    {
        /// <summary>
        /// Constructs a Italic table entity containing a table.
        /// </summary>
        /// <param name="table">The table with italic rendering</param>
        /// See <see cref="Table"/> for the content entities.
        public TableItalic(Table table) : base(Tags.Italic, true)
        {
            attributes = new Attributes(null);
            this.table = table;
        }
    }

    /// <summary>
    /// Implements a Bold table: &lt;B&gt; table &lt;/B&gt;
    /// </summary>
    /// <inheritdoc/>
    public class TableBold : TaggedTable
    {
        /// <summary>
        /// Constructs a Bold table entity containing a table.
        /// </summary>
        /// <param name="table">The table with bold rendering</param>
        /// See <see cref="Table"/> for the content entities.
        public TableBold(Table table) : base(Tags.Bold, true)
        {
            attributes = new Attributes(null);
            this.table = table;
        }
    }

    /// <summary>
    /// Implements Underlined table: &lt;U&gt; table &lt;/U&gt;
    /// </summary>
    /// <inheritdoc/>
    public class TableUnderline : TaggedTable
    {
        /// <summary>
        /// Constructs a Underlined table entity containing a table.
        /// </summary>
        /// <param name="table">The table with underlined rendering</param>
        /// See <see cref="Table"/> for the content entities.
        public TableUnderline(Table table) : base(Tags.Underline, true)
        {
            attributes = new Attributes(null);
            this.table = table;
        }
    }

    /// <summary>
    /// Implements Overlined table: &lt;O&gt; table &lt;/O&gt;
    /// </summary>
    /// <inheritdoc/>
    public class TableOverline : TaggedTable
    {
        /// <summary>
        /// Constructs a Overlined table entity containing a table.
        /// </summary>
        /// <param name="table">The table with overlined rendering</param>
        /// See <see cref="Table"/> for the content entities.
        public TableOverline(Table table) : base(Tags.Overline, true)
        {
            attributes = new Attributes(null);
            this.table = table;
        }
    }

    /// <summary>
    /// Implements a horizontal rule: &lt;HR/&gt;
    /// </summary>
    /// <inheritdoc/>
    public class HorRule : TaggedTable
    {
        /// <summary>
        /// Constructs a horizontal rule.
        /// </summary>
        public HorRule() : base(Tags.HorizontalRule, false)
        {
            attributes = new Attributes(null);
        }
    }

    /// <summary>
    /// Implements a vertical rule: &lt;VR/&gt;
    /// </summary>
    /// <inheritdoc/>
    public class VertRule : TaggedTable
    {
        /// <summary>
        /// Constructs a horizontal rule.
        /// </summary>
        public VertRule() : base(Tags.VerticalRule, false)
        {
            attributes = new Attributes(null);
        }
    }

    /// <summary>
    /// Handles attributes for the HTML entities.
    /// </summary>
    public class Attributes
    {
        /// <summary>
        /// Enumerates all available attributes for the HTML entities.
        /// </summary>
        public enum ID
        {
            /// <summary>The enumeration Value for: <b>ALIGN</b>="CENTER|LEFT|RIGHT|TEXT".</summary>
            /// See <see cref="Alignment"/> for correspondig values.
            Align = 0,
            /// <summary>The enumeration Value for: <b>BALIGN</b>="CENTER|LEFT|RIGHT".</summary>
            /// See <see cref="Alignment"/> for correspondig values.
            BAlign,
            /// <summary>The enumeration Value for: <b>BGCOLOR</b>="color".</summary>
            /// See <see cref="GVColor"/> for corresponing values.
            BGColor,
            /// <summary>The enumeration Value for: <b>BORDER</b>="value" where value is an integer between 0 and 255.</summary>
            /// See <see cref="GVInt"/> for information on integers.
            Border,
            /// <summary>The enumeration Value for: <b>CELLPADDING</b>="value" where value is an integer between 0 and 255.</summary>
            /// See <see cref="GVInt"/> for information on integers.
            CellPadding,
            /// <summary>The enumeration Value for: <b>CELLSPACING</b>="value" where value is an integer between 0 and 127.</summary>
            /// See <see cref="GVInt"/> for information on integers.
            CellSpacing,
            /// <summary>The enumeration Value for: <b>COLOR</b>="color".</summary>
            /// See <see cref="GVColor"/> for corresponing values.
            Color,
            /// <summary>The enumeration Value for: <b>COLSPAN</b>="value" where value is an integer between 0 and 65535.</summary>
            /// See <see cref="GVInt"/> for information on integers.
            ColSpan,
            /// <summary>The enumeration Value for: <b>COLUMNS</b>="value" where the only legal value is "*".</summary>
            Columns,
            /// <summary>The enumeration Value for: <b>FACE</b>="fontname".</summary>
            Face,
            /// <summary>The enumeration Value for: <b>FIXEDSIZE</b>="FALSE|TRUE".</summary>
            FixedSize,
            /// <summary>The enumeration Value for: <b>GRADIENTANGLE</b>="value" where value is an integer between 0 and 360.</summary>
            /// See <see cref="GVInt"/> for information on integers.
            GradientAngle,
            /// <summary>The enumeration Value for: <b>HEIGHT</b>="value" where value is an integer between 0 and 65535.</summary>
            /// See <see cref="GVInt"/> for information on integers.
            Height,
            /// <summary>The enumeration Value for: <b>HREF</b>="URL". The URL is treated like an escString.</summary>
            /// See <see cref="GVEscString"/> for more information.
            HRef,
            /// <summary>The enumeration Value for: <b>ID</b>="identifier".</summary>
            ID,
            /// <summary>The enumeration Value for: <b>POINT-SIZE</b>="value" where value is an integer > 0.</summary>
            /// See <see cref="GVInt"/> for information on integers.
            PointSize,
            /// <summary>The enumeration Value for: <b>PORT</b>="portName".</summary>
            Port,
            /// <summary>The enumeration Value for: <b>ROWS</b>="value" where the only legal value is "*".</summary>
            Rows,
            /// <summary>The enumeration Value for: <b>COLSPAN</b>="value" where value is an integer between 0 and 65535.</summary>
            /// See <see cref="GVInt"/> for information on integers.
            RowSpan,
            /// <summary>The enumeration Value for: <b>SCALE</b>="FALSE|TRUE|WIDTH|HEIGHT|BOTH".</summary>
            /// See <see cref="Attributes.Scale"/> for corresponding values.
            Scale,
            /// <summary>The enumeration Value for: <b>SIDES</b>="L?R?B?T?".</summary>
            /// See <see cref="Attributes.Sides"/> for corresponding values.
            Sides,
            /// <summary>The enumeration Value for: <b>SRC</b>="filepath".</summary>
            Src,
            /// <summary>The enumeration Value for: <b>STYLE</b>="ROUNDED|RADIAL".</summary>
            /// See <see cref="Attributes.Style"/> for corresponding values.
            /// See <seealso href="https://graphviz.org/doc/info/shapes.html#html"/> for more information.
            Style,
            /// <summary>The enumeration Value for: <b>TARGET</b>="value". The value is treated like an escString.</summary>
            /// See <see href="https://graphviz.org/doc/info/shapes.html#html"/> for more information.
            Target,
            /// <summary>The enumeration Value for: <b>TITLE</b>="value". The value is treated like an escString.</summary>
            /// See <see href="https://graphviz.org/doc/info/shapes.html#html"/> for more information.
            Title,
            /// <summary>The enumeration Value for: <b>TOOLTIP</b>="value". The value is treated like an escString.</summary>
            /// See <see href="https://graphviz.org/doc/info/shapes.html#html"/> for more information.
            ToolTip,
            /// <summary>The enumeration Value for: <b>VALIGN</b>="MIDDLE|BOTTOM|TOP".</summary>
            /// See <see cref="Alignment"/> for corresponding values.
            VAlign,
            /// <summary>The enumeration Value for: <b>WIDTH</b>="value" where value is an integer between 0 and 65535.</summary>
            /// See <see cref="GVInt"/> for information on integers.
            Width,
        }

        private static readonly string[] Text =
        {
            "ALIGN",
            "BALIGN",
            "BGCOLOR",
            "BORDER",
            "CELLPADDING",
            "CELLSPACING",
            "COLOR",
            "COLSPAN",
            "COLUMNS",
            "FACE",
            "FIXEDSIZE",
            "GRADIENTANGLE",
            "HEIGHT",
            "HREF",
            "ID",
            "POINT-SIZE",
            "PORT",
            "ROWS",
            "ROWSPAN",
            "SCALE",
            "SIDES",
            "SRC",
            "STYLE",
            "TARGET",
            "TITLE",
            "TOOLTIP",
            "VALIGN",
            "WIDTH",
        };

        /// <summary>
        /// Enumerates all available alignment variants.
        /// </summary>
        public enum Alignment
        {
            /// <summary>The enumeration Value for: ALIGN|BALIGN="<b>CENTER</b>|LEFT|RIGHT|TEXT".</summary>
            Center = 0,
            /// <summary>The enumeration Value for: ALIGN|BALIGN="CENTER|<b>LEFT</b>|RIGHT|TEXT".</summary>
            Left,
            /// <summary>The enumeration Value for: ALIGN|BALIGN="CENTER|LEFT|<b>RIGHT</b>|TEXT".</summary>
            Right,
            /// <summary>The enumeration Value for: ALIGN="CENTER|LEFT|RIGHT|<b>TEXT</b>".</summary>
            Text,
            /// <summary>The enumeration Value for: VALIGN="<b>MIDDLE</b>|BOTTOM|TOP".</summary>
            Middle,
            /// <summary>The enumeration Value for: VALIGN="MIDDLE|<b>BOTTOM</b>|TOP".</summary>
            Bottom,
            /// <summary>The enumeration Value for: VALIGN="MIDDLE|BOTTOM|<b>TOP</b>".</summary>
            Top
        }

        private static readonly string[] alignmentText =
        {
            "CENTER",
            "LEFT",
            "RIGHT",
            "TEXT",
            "MIDDLE",
            "BOTTOM",
            "TOP",
        };

        /// <summary>
        /// Enumerates all available scaling variants.
        /// </summary>
        public enum Scale
        {
            /// <summary>The enumeration Value for: SCALE="<b>TRUE</b>|FALSE|WIDTH|HEIGHT|BOTH".</summary>
            True,
            /// <summary>The enumeration Value for: SCALE="TRUE|<b>FALSE</b>|WIDTH|HEIGHT|BOTH".</summary>
            False,
            /// <summary>The enumeration Value for: SCALE="TRUE|FALSE|<b>WIDTH</b>|HEIGHT|BOTH".</summary>
            Width,
            /// <summary>The enumeration Value for: SCALE="TRUE|FALSE|WIDTH|<b>HEIGHT</b>|BOTH".</summary>
            Height,
            /// <summary>The enumeration Value for: SCALE="TRUE|FALSE|WIDTH|HEIGHT|<b>BOTH</b>".</summary>
            Both,
        }

        private static readonly string[] scaleName =
        {
            "TRUE",
            "FALSE",
            "WIDTH",
            "HEIGHT",
            "BOTH",
        };

        /// <summary>
        /// Describes wether a cell should have a border or not.
        /// </summary>
        [Flags]
        public enum Sides
        {
            /// <summary>If set the cell has has left border: SIDES="<b>L</b>R?B?T?". if false: SIDES="R?B?T?"</summary>
            Left = 1,
            /// <summary>If set the cell has has right border: SIDES="L?<b>R</b>B?T?". if false: SIDES="L?B?T?"</summary>
            Right = 2,
            /// <summary>If set the cell has has bottom border: SIDES="L?R?<b>B</b>T?". if false: SIDES="L?R?T?"</summary>
            Bottom = 4,
            /// <summary>If set the cell has has top border: SIDES="L?R?B?<b>T</b>". if false: SIDES="L?R?B?"</summary>
            Top = 8,
        }

        /// <summary>
        /// Enumerates all available styles for Tables and Cells. 
        /// </summary>
        public enum Style
        {
            /// <summary>The enumeration Value for: STYLE="<b>ROUNDED</b>|RADIAL".</summary>
            Rounded,
            /// <summary>The enumeration Value for: SCALE="ROUNDED|<b>RADIAL</b>".</summary>
            Radial,
        }

        private static readonly string[] styleName =
        {
            "ROUNDED",
            "RADIAL",
        };

        /// <summary>
        /// Collects all supported attributes of a specific HTML entity.
        /// </summary>
        protected Queue<ID> supported;

        /// <summary>
        /// Collects all added attributes and their values of a specific HTML entity.
        /// </summary>
        protected Dictionary<ID, string> attributes;

        /// <summary>
        /// Constructs the attributes handler of a specific HTML entity.
        /// </summary>
        /// <param name="supported">An array of all supported atrributs of a specific HTML entity</param>
        public Attributes(ID[] supported)
        {
            this.supported = new Queue<ID>();
            if (supported != null)
            {
                for (int i = 0; i < supported.Length; i++) this.supported.Enqueue(supported[i]);
                attributes = new Dictionary<ID, string>();
            }
        }

        /// <summary>
        /// Builds the text representation of all contained attributes.
        /// </summary>
        /// <returns>The text representation of all contained attributes</returns>
        public override string ToString()
        {
            string result = "";

            if (supported.Count == 0) return result;

            foreach (KeyValuePair<ID, string> attrValue in attributes)
            {
                result += " " + Text[(int)(attrValue.Key)] + "=\"" + attrValue.Value + "\"";
            }

            return result;
        }

        /// <summary>
        /// Sets an attribute for a specific HTML entity.
        /// </summary>
        /// <param name="attr">The attribute i</param>
        /// <param name="value">The value the given attribute should be set to</param>
        /// <exception cref="GVException">Thrown when the attribute is not supported oe the value is of wrong type</exception>
        public void Add(ID attr, object value)
        {
            if (!supported.Contains(attr)) throw new GVException("Attribute " + attr + "\"" + Text[(int)attr] + "\" is not supported in this context");

            Alignment a;
            Sides s;
            GVInt i;

            switch (attr)
            {
                case ID.Align:
                    if (value.GetType() != typeof(Alignment)) throw new GVException("Attribute " + attr + " expects an " + typeof(Alignment));
                    a = (Alignment)value;
                    if (attributes.ContainsKey(ID.Align)) attributes.Remove(ID.Align);
                    switch (a)
                    {
                        case Alignment.Center:
                        case Alignment.Left:
                        case Alignment.Right:
                        case Alignment.Text: attributes.Add(attr, alignmentText[(int)a]); break;
                        default: throw new GVException("" + a + " not allowed as " + attr);
                    }
                    break;
                case ID.BAlign:
                    if (value.GetType() != typeof(Alignment)) throw new GVException("Attribute " + attr + " expects an " + typeof(Alignment));
                    a = (Alignment)value;
                    if (attributes.ContainsKey(ID.Align)) attributes.Remove(ID.Align);
                    switch (a)
                    {
                        case Alignment.Center:
                        case Alignment.Left:
                        case Alignment.Right: attributes.Add(attr, alignmentText[(int)a]); break;
                        default: throw new GVException("" + a + " not allowed as " + attr);
                    }
                    break;
                case ID.BGColor:
                    if (value.GetType() != typeof(GVColor)) throw new GVException("Attribute " + attr + " expects an " + typeof(GVColor));
                    attributes.Add(attr, ((GVColor)value).ToString());
                    break;
                case ID.Border:
                    if (value.GetType() != typeof(GVInt)) throw new GVException("Attribute " + attr + " expects an" + typeof(GVInt));
                    i = (GVInt)value;
                    if ((i < GVInt.Zero) || (i > GVInt.UInt8Max)) throw new GVException("Attribute " + attr + " expects an " + typeof(GVInt) + " between 0 and 255");
                    attributes.Add(attr, i.ToString());
                    break;
                case ID.CellPadding:
                    if (value.GetType() != typeof(GVInt)) throw new GVException("Attribute " + attr + " expects an" + typeof(GVInt));
                    i = (GVInt)value;
                    if ((i < GVInt.Zero) || (i > GVInt.UInt8Max)) throw new GVException("Attribute " + attr + " expects an " + typeof(GVInt) + " between 0 and 255");
                    attributes.Add(attr, i.ToString());
                    break;
                case ID.CellSpacing:
                    if (value.GetType() != typeof(GVInt)) throw new GVException("Attribute " + attr + " expects an" + typeof(GVInt));
                    i = (GVInt)value;
                    if ((i < GVInt.Zero) || (i > GVInt.Int8Max)) throw new GVException("Attribute " + attr + " expects an " + typeof(GVInt) + " between 0 and 127");
                    attributes.Add(attr, i.ToString());
                    break;
                case ID.Color:
                    if (value.GetType() != typeof(GVColor)) throw new GVException("Attribute " + attr + " expects an " + typeof(GVColor));
                    attributes.Add(attr, ((GVColor)value).ToString());
                    break;
                case ID.ColSpan:
                    if (value.GetType() != typeof(GVInt)) throw new GVException("Attribute " + attr + " expects an" + typeof(GVInt));
                    i = (GVInt)value;
                    if ((i < GVInt.Zero) || (i > GVInt.UInt16Max)) throw new GVException("Attribute " + attr + " expects an " + typeof(GVInt) + " between 0 and 65535");
                    attributes.Add(attr, i.ToString());
                    break;
                case ID.Columns:
                    if (value.GetType() != typeof(string)) throw new GVException("Attribute " + attr + " expects an" + typeof(string));
                    if (((string)value) != "*") throw new GVException("Attribute " + attr + " supports only \"*\" at present");
                    attributes.Add(attr, (string)value);
                    break;
                case ID.Face:
                    // TODO: Font face must be defined
                    throw new GVException("Attribute " + attr + " unsupported at present");
                    break;
                case ID.FixedSize:
                    if (value.GetType() != typeof(GVBool)) throw new GVException("Attribute " + attr + " expects an" + typeof(GVBool));
                    attributes.Add(attr, ((GVBool)value).ToString());
                    break;
                case ID.GradientAngle:
                    if (value.GetType() != typeof(GVInt)) throw new GVException("Attribute " + attr + " expects an" + typeof(GVInt));
                    i = (GVInt)value;
                    if ((i < GVInt.Zero) || (i > GVInt.AngleMax)) throw new GVException("Attribute " + attr + " expects an " + typeof(GVInt) + " between 0 and 360");
                    attributes.Add(attr, i.ToString());
                    break;
                case ID.Height:
                    if (value.GetType() != typeof(GVInt)) throw new GVException("Attribute " + attr + " expects an" + typeof(GVInt));
                    i = (GVInt)value;
                    if ((i < GVInt.Zero) || (i > GVInt.UInt16Max)) throw new GVException("Attribute " + attr + " expects an " + typeof(GVInt) + " between 0 and 65535");
                    attributes.Add(attr, i.ToString());
                    break;
                case ID.HRef:
                    if (value.GetType() != typeof(GVEscString)) throw new GVException("Attribute " + attr + " expects an" + typeof(GVEscString));
                    attributes.Add(attr, ((GVEscString)value).ToString());
                    break;
                case ID.ID:
                    if (value.GetType() != typeof(GVEscString)) throw new GVException("Attribute " + attr + " expects an" + typeof(GVEscString));
                    attributes.Add(attr, ((GVEscString)value).ToString());
                    break;
                case ID.PointSize:
                    if (value.GetType() != typeof(GVInt)) throw new GVException("Attribute " + attr + " expects an" + typeof(GVInt));
                    attributes.Add(attr, ((GVInt)value).ToString());
                    break;
                case ID.Port:
                    // TODO: Port and PortName must be defined
                    throw new GVException("Attribute " + attr + " unsupported at present");
                    break;
                case ID.Rows:
                    if (value.GetType() != typeof(string)) throw new GVException("Attribute " + attr + " expects an" + typeof(string));
                    if (((string)value) != "*") throw new GVException("Attribute " + attr + " supports only \"*\" at present");
                    attributes.Add(attr, (string)value);
                    break;
                case ID.RowSpan:
                    if (value.GetType() != typeof(GVInt)) throw new GVException("Attribute " + attr + " expects an" + typeof(GVInt));
                    i = (GVInt)value;
                    if ((i < GVInt.Zero) || (i > GVInt.UInt16Max)) throw new GVException("Attribute " + attr + " expects an " + typeof(GVInt) + " between 0 and 65535");
                    attributes.Add(attr, i.ToString());
                    break;
                case ID.Scale:
                    if (value.GetType() != typeof(Scale)) throw new GVException("Attribute " + attr + " expects an " + typeof(Scale));
                    attributes.Add(attr, scaleName[(int)((Scale)value)]);
                    break;
                case ID.Sides:
                    if (value.GetType() != typeof(Sides)) throw new GVException("Attribute " + attr + " expects an " + typeof(Sides));
                    s = (Sides)value;
                    string r = "";
                    if ((s & Sides.Left) > 0) r += "L";
                    if ((s & Sides.Right) > 0) r += "R";
                    if ((s & Sides.Bottom) > 0) r += "B";
                    if ((s & Sides.Top) > 0) r += "T";
                    attributes.Add(attr, r);
                    break;
                case ID.Src:
                    // TODO: File Source must be defined
                    throw new GVException("Attribute " + attr + " unsupported at present");
                    break;
                case ID.Style:
                    if (value.GetType() != typeof(Style)) throw new GVException("Attribute " + attr + " expects an " + typeof(Style));
                    attributes.Add(attr, styleName[(int)((Style)value)]);
                    break;
                case ID.Target:
                    if (value.GetType() != typeof(GVEscString)) throw new GVException("Attribute " + attr + " expects an" + typeof(GVEscString));
                    attributes.Add(attr, ((GVEscString)value).ToString());
                    break;
                case ID.Title:
                    if (value.GetType() != typeof(GVEscString)) throw new GVException("Attribute " + attr + " expects an" + typeof(GVEscString));
                    attributes.Add(attr, ((GVEscString)value).ToString());
                    break;
                case ID.ToolTip:
                    if (value.GetType() != typeof(GVEscString)) throw new GVException("Attribute " + attr + " expects an" + typeof(GVEscString));
                    attributes.Add(attr, ((GVEscString)value).ToString());
                    break;
                case ID.VAlign:
                    if (value.GetType() != typeof(Alignment)) throw new GVException("Attribute " + attr + " expects an " + typeof(Alignment));
                    a = (Alignment)value;
                    if (attributes.ContainsKey(ID.Align)) attributes.Remove(ID.Align);
                    switch (a)
                    {
                        case Alignment.Middle:
                        case Alignment.Bottom:
                        case Alignment.Top: attributes.Add(attr, alignmentText[(int)a]); break;
                        default: throw new GVException("" + a + " not allowed as " + attr);
                    }
                    break;
                case ID.Width:
                    if (value.GetType() != typeof(GVInt)) throw new GVException("Attribute " + attr + " expects an" + typeof(GVInt));
                    i = (GVInt)value;
                    if ((i < GVInt.Zero) || (i > GVInt.UInt16Max)) throw new GVException("Attribute " + attr + " expects an " + typeof(GVInt) + " between 0 and 65535");
                    attributes.Add(attr, i.ToString());
                    break;
            }
        }
    }
}
