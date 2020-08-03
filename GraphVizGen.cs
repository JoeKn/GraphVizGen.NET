// ====================================================================================
// File: GraphVizGen.cs / GraphViz C# .NET Generator
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
using System.Globalization;
using System.Linq;
using GraphViz.Values;

namespace GraphViz
{

    /// <summary>
    /// The root class only contains the cultural info for en-US localization.
    /// </summary>
    public abstract class GVRoot
    {
        /// <summary>
        /// The cultural info for en-US localization.
        /// DOT's number representation uses '.' and not like in Europe ','.
        /// </summary>
        public static CultureInfo EnUS = CultureInfo.CreateSpecificCulture("en-US");
    }

    /// <summary>
    /// The Exception that is thrown on errors.
    /// </summary>
    public class GVException : Exception
    {
        /// <summary>
        /// Constructs a new Exception
        /// </summary>
        /// <param name="message">The message of this exception</param>
        public GVException(string message) : base("GraphViz Generator: " + message) { }
    }


    /// <summary>
    /// The contexts allows the creation of more than one graph at the same time with
    /// each graph in its own context. It also checks for uniqueness of ID.
    /// </summary>
    public class GVContext
    {
        private readonly Dictionary<string, GVGraph> namedObjects;
        private GVGraph graph;
        
        /// <summary>
        /// Constructs a new context. A context contains exactly one graph.
        /// </summary>
        public GVContext()
        {
            namedObjects = new Dictionary<string, GVGraph>();
        }

        public GVGraph CreateGraph(GVID name, bool isDirected)
        {
            if (namedObjects.ContainsKey(name.ToString())) throw new GVException("Dulicated Object ID (name)");
            graph = new GVGraph(name);
            lock (namedObjects)
            {
                namedObjects.Add(name.ToString(), graph);
            }

            return graph;
        }
    }

    /// <summary>
    /// Implements an optional strict graph, digraph or subgraph<para/>
    /// <i>graph</i> 	    : 	[ <b>strict</b> ] (<b>graph</b> | <b>digraph</b>) [ <i>ID</i> ] '<b>{</b>' <i>stmt_list</i> '<b>}</b>'<para/>
    /// <i>stmt_list</i> 	: 	[ <i>stmt</i> [ '<b>;</b>' ] <i>stmt_list</i> ]<para/>
    /// <i>stmt</i> 	        : 	<i>node_stmt</i>
    ///	                    | 	<i>edge_stmt</i>
    ///	                    | 	<i>attr_stmt</i>
    ///	                    | 	<i>ID</i> '<b>=</b>' <i>ID</i>
    ///	                    | 	<i>subgraph</i><para/>
    /// <i>attr_stmt</i> 	: 	(<b>graph</b> | <b>node</b> | <b>edge</b>) <i>attr_list</i><para/>
    /// <i>attr_list</i> 	: 	'<b>[</b>' [ <i>a_list</i> ] '<b>]</b>' [ <i>attr_list</i> ]<para/>
    /// <i>a_list</i> 	    : 	<i>ID</i> '<b>=</b>' <i>ID</i> [ ('<b>;</b>' | '<b>,</b>') ] [ <i>a_list</i> ]<para/>
    /// <i>edge_stmt</i> 	: 	(<i>node_id</i> | <i>subgraph</i>) <i>edgeRHS</i> [ <i>attr_list</i> ]<para/>
    /// <i>edgeRHS</i> 	    : 	<i>edgeop</i> (<i>node_id</i> | <i>subgraph</i>) [ <i>edgeRHS</i> ]<para/>
    /// <i>node_stmt</i> 	: 	<i>node_id</i> [ <i>attr_list</i> ]<para/>
    /// <i>node_id</i> 	    : 	<i>ID</i> [ <i>port</i> ]<para/>
    /// <i>port</i>      	: 	'<b>:</b>' <i>ID</i> [ '<b>:</b>' <i>compass_pt</i> ]
    ///	                    | 	'<b>:</b>' <i>compass_pt</i><para/>
    /// <i>subgraph</i>  	: 	[ <b>subgraph</b> [ <i>ID</i> ] ] '<b>{</b>' <i>stmt_list</i> '<b>}</b>'<para/>
    /// <i>compass_pt</i> 	: 	(<b>n</b> | <b>ne</b> | <b>e</b> | <b>se</b> | <b>s</b> | <b>sw</b> | <b>w</b> | <b>nw</b> | <b>c</b> | <b>_</b>)<para/>
    /// </summary>
    public class GVGraph : GVRoot
    {
        /// <summary>
        /// Holds all available graph-types
        /// </summary>
        public enum Type
        {
            /// <summary>Enumariot for: <i>graph</i> : 	 <b>graph</b> <i>name</i> '<b>{</b>' <i>stmt_list</i> '<b>}</b>'</summary>
            Graph = 0,
            /// <summary>Enumariot for: <i>graph</i> : 	 <b>digraph</b> <i>name</i> '<b>{</b>' <i>stmt_list</i> '<b>}</b>'</summary>
            DiGraph,
            /// <summary>Enumariot for: <i>graph</i> : 	 <b>strict graph</b> <i>name</i> '<b>{</b>' <i>stmt_list</i> '<b>}</b>'</summary>
            StrictGraph,
            /// <summary>Enumariot for: <i>graph</i> : 	 <b>strict digraph</b> <i>name</i> '<b>{</b>' <i>stmt_list</i> '<b>}</b>'</summary>
            StrictDiGraph,
            /// <summary>Enumariot for: <i>subgraph</i> : 	<b>subgraph</b> <i>name</i> '<b>{</b>' <i>stmt_list</i> '<b>}</b>'</summary>
            SubGraph,
            /// <summary>Enumariot for: <i>graph</i> : 	 <b>graph</b> <b>cluster_</b><i>name</i> '<b>{</b>' <i>stmt_list</i> '<b>}</b>'</summary>
            ClusterGraph,
            /// <summary>Enumariot for: <i>graph</i> : 	 <b>digraph</b> <b>cluster_</b><i>name</i> '<b>{</b>' <i>stmt_list</i> '<b>}</b>'</summary>
            ClusterDiGraph,
            /// <summary>Enumariot for: <i>graph</i> : 	 <b>strict graph</b> <b>cluster_</b><i>name</i> '<b>{</b>' <i>stmt_list</i> '<b>}</b>'</summary>
            StrictClusterGraph,
            /// <summary>Enumariot for: <i>graph</i> : 	 <b>strict digraph</b> <b>cluster_</b><i>name</i> '<b>{</b>' <i>stmt_list</i> '<b>}</b>'</summary>
            StrictClusterDiGraph,
            /// <summary>Enumariot for: <i>subgraph</i> : 	<b>subgraph</b> <b>cluster_</b><i>name</i> '<b>{</b>' <i>stmt_list</i> '<b>}</b>'</summary>
            ClusterSubGraph,
        }

        private string name;
        private bool isStrict;
        private bool isDirected;
        private bool isSubgraph;
        private bool isCluster;

        private GVAttributes attributes;


        public GVAttributes.ID[] supportedByGraph = {
            GVAttributes.ID.Damping,
            GVAttributes.ID.K,
            GVAttributes.ID.URL,
            GVAttributes.ID._background,
            GVAttributes.ID.Bb,
            GVAttributes.ID.Bgcolor,
            GVAttributes.ID.Center,
            GVAttributes.ID.Charset,
            GVAttributes.ID.Class,
            GVAttributes.ID.Clusterrank,
            GVAttributes.ID.Colorscheme,
            GVAttributes.ID.Comment,
            GVAttributes.ID.Compound,
            GVAttributes.ID.Concentrate,
            GVAttributes.ID.Defaultdist,
            GVAttributes.ID.Dim,
            GVAttributes.ID.Dimen,
            GVAttributes.ID.Diredgeconstraints,
            GVAttributes.ID.Dpi,
            GVAttributes.ID.Epsilon,
            GVAttributes.ID.Esep,
            GVAttributes.ID.Fontcolor,
            GVAttributes.ID.Fontname,
            GVAttributes.ID.Fontnames,
            GVAttributes.ID.Fontpath,
            GVAttributes.ID.Fontsize,
            GVAttributes.ID.Forcelabels,
            GVAttributes.ID.Gradientangle,
            GVAttributes.ID.Href,
            GVAttributes.ID.Id,
            GVAttributes.ID.Imagepath,
            GVAttributes.ID.Inputscale,
            GVAttributes.ID.Label,
            GVAttributes.ID.Label_scheme,
            GVAttributes.ID.Labeljust,
            GVAttributes.ID.Labelloc,
            GVAttributes.ID.Landscape,
            GVAttributes.ID.Layerlistsep,
            GVAttributes.ID.Layers,
            GVAttributes.ID.Layerselect,
            GVAttributes.ID.Layersep,
            GVAttributes.ID.Layout,
            GVAttributes.ID.Levels,
            GVAttributes.ID.Levelsgap,
            GVAttributes.ID.Lheight,
            GVAttributes.ID.Lp,
            GVAttributes.ID.Lwidth,
            GVAttributes.ID.Margin,
            GVAttributes.ID.Maxiter,
            GVAttributes.ID.Mclimit,
            GVAttributes.ID.Mindist,
            GVAttributes.ID.Mode,
            GVAttributes.ID.Model,
            GVAttributes.ID.Mosek,
            GVAttributes.ID.Newrank,
            GVAttributes.ID.Nodesep,
            GVAttributes.ID.Nojustify,
            GVAttributes.ID.Normalize,
            GVAttributes.ID.Notranslate,
            GVAttributes.ID.Nslimit1,
            GVAttributes.ID.Ordering,
            // GVAttributes.ID.Orientation,
            GVAttributes.ID.Outputorder,
            GVAttributes.ID.Overlap,
            GVAttributes.ID.Overlap_scaling,
            GVAttributes.ID.Overlap_shrink,
            GVAttributes.ID.Pack,
            GVAttributes.ID.Packmode,
            GVAttributes.ID.Pad,
            GVAttributes.ID.Page,
            GVAttributes.ID.Pagedir,
            GVAttributes.ID.Quadtree,
            GVAttributes.ID.Quantum,
            GVAttributes.ID.Rankdir,
            GVAttributes.ID.Ranksep,
            GVAttributes.ID.Ratio,
            GVAttributes.ID.Remincross,
            GVAttributes.ID.Repulsiveforce,
            GVAttributes.ID.Resolution,
            GVAttributes.ID.Root,
            GVAttributes.ID.Rotate,
            GVAttributes.ID.Rotation,
            GVAttributes.ID.Scale,
            GVAttributes.ID.Searchsize,
            GVAttributes.ID.Sep,
            GVAttributes.ID.Showboxes,
            GVAttributes.ID.Size,
            GVAttributes.ID.Smoothing,
            GVAttributes.ID.Sortv,
            GVAttributes.ID.Splines,
            GVAttributes.ID.Start,
            GVAttributes.ID.Style,
            GVAttributes.ID.Stylesheet,
            GVAttributes.ID.Target,
            GVAttributes.ID.Truecolor,
            GVAttributes.ID.Viewport,
            GVAttributes.ID.Voro_margin,
            GVAttributes.ID.Xdotversion,
        };

        public GVAttributes.ID[] supportedByCluster = {
            GVAttributes.ID.K,
            GVAttributes.ID.URL,
            GVAttributes.ID.Area,
            GVAttributes.ID.Bgcolor,
            GVAttributes.ID.Class,
            GVAttributes.ID.Color,
            GVAttributes.ID.Colorscheme,
            GVAttributes.ID.Fillcolor,
            GVAttributes.ID.Fontcolor,
            GVAttributes.ID.Fontname,
            GVAttributes.ID.Fontsize,
            GVAttributes.ID.Gradientangle,
            GVAttributes.ID.Href,
            GVAttributes.ID.Id,
            GVAttributes.ID.Label,
            GVAttributes.ID.Labeljust,
            GVAttributes.ID.Labelloc,
            GVAttributes.ID.Layer,
            GVAttributes.ID.Lheight,
            GVAttributes.ID.Lp,
            GVAttributes.ID.Lwidth,
            GVAttributes.ID.Margin,
            GVAttributes.ID.Nojustify,
            GVAttributes.ID.Pencolor,
            GVAttributes.ID.Penwidth,
            GVAttributes.ID.Peripheries,
            GVAttributes.ID.Sortv,
            GVAttributes.ID.Style,
            GVAttributes.ID.Target,
            GVAttributes.ID.Tooltip,
        };

        public GVAttributes.ID[] supportedBySubGraph = {
            GVAttributes.ID.Rank,
        };


        private GVGraph(string name, Type type)
        {
            this.name = name;
            isStrict = false;
            isDirected = false;
            isSubgraph = false;
            isCluster = false;

            switch (type)
            {
                case Type.Graph:
                    break;
                case Type.DiGraph:
                    isDirected = true;
                    break;
                case Type.StrictGraph:
                    isStrict = true;
                    break;
                case Type.StrictDiGraph:
                    isStrict = true;
                    isDirected = true;
                    break;
                case Type.ClusterGraph:
                    isCluster = true;
                    break;
                case Type.ClusterDiGraph:
                    isDirected = true;
                    isCluster = true;
                    break;
                case Type.StrictClusterGraph:
                    isStrict = true;
                    isCluster = true;
                    break;
                case Type.StrictClusterDiGraph:
                    isStrict = true;
                    isDirected = true;
                    isCluster = true;
                    break;
                case Type.SubGraph:
                    isSubgraph = true;
                    break;
                case Type.ClusterSubGraph:
                    isSubgraph = true;
                    isCluster = true;
                    break;
            }
            switch (type)
            {
                case Type.Graph:
                case Type.DiGraph:
                case Type.StrictGraph:
                case Type.StrictDiGraph:
                    attributes = new GVAttributes(supportedByGraph);
                    break;
                case Type.ClusterGraph:
                case Type.ClusterDiGraph:
                case Type.StrictClusterGraph:
                case Type.StrictClusterDiGraph:
                    attributes = new GVAttributes(supportedByGraph.Union(supportedByCluster).ToArray());
                    break;
                case Type.SubGraph:
                    attributes = new GVAttributes(supportedBySubGraph);
                    break;
                case Type.ClusterSubGraph:
                    attributes = new GVAttributes(supportedBySubGraph.Union(supportedByCluster).ToArray());
                    break;
            }
        }

        public GVGraph(GVID name) : this(name.ToString(), Type.Graph) { }

        public GVGraph(GVDouble name) : this(name.ToString(), Type.Graph) { }

        public GVGraph(GVInt name) : this(name.ToString(), Type.Graph) { }

        public GVGraph(HTML.String name) : this(name.ToString(), Type.Graph) { }

        public GVGraph(GVID name, Type type) : this(name.ToString(), type) { }

        public GVGraph(GVDouble name, Type type) : this(name.ToString(), type) { }

        public GVGraph(GVInt name, Type type) : this(name.ToString(), type) { }

        public GVGraph(HTML.String name, Type type) : this(name.ToString(), type) { }
    }


    public abstract class GVValue : GVRoot { }




    public class GVNode : GVRoot
    {

    }

    public class GVEdge : GVRoot
    {

    }

    public class GVAttributes
    {
        public enum ID
        {
            ///<summary>Enumeration value of attribute "Damping"</summary>
            ///See <see cref="GVDouble"/>for corresponding values
            Damping,
            ///<summary>Enumeration value of attribute "K"</summary>
            ///See <see cref="GVDouble"/>for corresponding values
            K,
            ///<summary>Enumeration value of attribute "URL"</summary>
            ///See <see cref="GVEscString"/>for corresponding values
            URL,
            ///<summary>Enumeration value of attribute "_background"</summary>
            ///See <see cref="GVString"/>for corresponding values
            _background,
            ///<summary>Enumeration value of attribute "area"</summary>
            ///See <see cref="GVDouble"/>for corresponding values
            Area,
            ///<summary>Enumeration value of attribute "arrowhead"</summary>
            ///See <see cref="GVArrowType"/>for corresponding values
            Arrowhead,
            ///<summary>Enumeration value of attribute "arrowsize"</summary>
            ///See <see cref="GVDouble"/>for corresponding values
            Arrowsize,
            ///<summary>Enumeration value of attribute "arrowtail"</summary>
            ///See <see cref="GVArrowType"/>for corresponding values
            Arrowtail,
            ///<summary>Enumeration value of attribute "bb"</summary>
            ///See <see cref="GVRect"/>for corresponding values
            Bb,
            ///<summary>Enumeration value of attribute "bgcolor"</summary>
            ///See <see cref="GVColor"/>for corresponding values
            Bgcolor,
            // TODO: colorList 	<none>		
            ///<summary>Enumeration value of attribute "center"</summary>
            ///See <see cref="GVBool"/>for corresponding values
            Center,
            ///<summary>Enumeration value of attribute "charset"</summary>
            ///See <see cref="GVString"/>for corresponding values
            Charset,
            ///<summary>Enumeration value of attribute "class"</summary>
            ///See <see cref="GVString"/>for corresponding values
            Class,
            ///<summary>Enumeration value of attribute "clusterrank"</summary>
            ///See <see cref="GVClusterMode"/>for corresponding values
            Clusterrank,
            ///<summary>Enumeration value of attribute "color"</summary>
            ///See <see cref="GVColor"/>for corresponding values
            Color,
            // TODO: colorList 	black		
            ///<summary>Enumeration value of attribute "colorscheme"</summary>
            ///See <see cref="GVString"/>for corresponding values
            Colorscheme,
            ///<summary>Enumeration value of attribute "comment"</summary>
            ///See <see cref="GVString"/>for corresponding values
            Comment,
            ///<summary>Enumeration value of attribute "compound"</summary>
            ///See <see cref="GVBool"/>for corresponding values
            Compound,
            ///<summary>Enumeration value of attribute "concentrate"</summary>
            ///See <see cref="GVBool"/>for corresponding values
            Concentrate,
            ///<summary>Enumeration value of attribute "constraint"</summary>
            ///See <see cref="GVBool"/>for corresponding values
            Constraint,
            ///<summary>Enumeration value of attribute "decorate"</summary>
            ///See <see cref="GVBool"/>for corresponding values
            Decorate,
            ///<summary>Enumeration value of attribute "defaultdist"</summary>
            ///See <see cref="GVDouble"/>for corresponding values
            Defaultdist,
            ///<summary>Enumeration value of attribute "dim"</summary>
            ///See <see cref="GVInt"/>for corresponding values
            Dim,
            ///<summary>Enumeration value of attribute "dimen"</summary>
            ///See <see cref="GVInt"/>for corresponding values
            Dimen,
            ///<summary>Enumeration value of attribute "dir"</summary>
            ///See <see cref="GVDirType"/>for corresponding values
            Dir,
            // TODO: none(undirected)		
            ///<summary>Enumeration value of attribute "diredgeconstraints"</summary>
            ///See <see cref="GVString"/>for corresponding values
            Diredgeconstraints,
            // TODO: bool 	false		neato only
            ///<summary>Enumeration value of attribute "distortion"</summary>
            ///See <see cref="GVDouble"/>for corresponding values
            Distortion,
            ///<summary>Enumeration value of attribute "dpi"</summary>
            ///See <see cref="GVDouble"/>for corresponding values
            Dpi,
            // TODO: 0.0		svg, bitmap output only
            ///<summary>Enumeration value of attribute "edgeURL"</summary>
            ///See <see cref="GVEscString"/>for corresponding values
            EdgeURL,
            ///<summary>Enumeration value of attribute "edgehref"</summary>
            ///See <see cref="GVEscString"/>for corresponding values
            Edgehref,
            ///<summary>Enumeration value of attribute "edgetarget"</summary>
            ///See <see cref="GVEscString"/>for corresponding values
            Edgetarget,
            ///<summary>Enumeration value of attribute "edgetooltip"</summary>
            ///See <see cref="GVEscString"/>for corresponding values
            Edgetooltip,
            ///<summary>Enumeration value of attribute "epsilon"</summary>
            ///See <see cref="GVDouble"/>for corresponding values
            Epsilon,
            // TODO: .0001(mode == major)
            // TODO: .01(mode == sgd)		neato only
            ///<summary>Enumeration value of attribute "esep"</summary>
            ///See <see cref="GVAddDouble"/>for corresponding values
            Esep,
            // TODO: addPoint 	+3		not dot
            ///<summary>Enumeration value of attribute "fillcolor"</summary>
            ///See <see cref="GVColor"/>for corresponding values
            Fillcolor,
            // TODO: colorList 	lightgrey(nodes)
            // TODO: black(clusters)		
            ///<summary>Enumeration value of attribute "fixedsize"</summary>
            ///See <see cref="GVBool"/>for corresponding values
            Fixedsize,
            // TODO: string	false		
            ///<summary>Enumeration value of attribute "fontcolor"</summary>
            ///See <see cref="GVColor"/>for corresponding values
            Fontcolor,
            ///<summary>Enumeration value of attribute "fontname"</summary>
            ///See <see cref="GVString"/>for corresponding values
            Fontname,
            ///<summary>Enumeration value of attribute "fontnames"</summary>
            ///See <see cref="GVString"/>for corresponding values
            Fontnames,
            ///<summary>Enumeration value of attribute "fontpath"</summary>
            ///See <see cref="GVString"/>for corresponding values
            Fontpath,
            ///<summary>Enumeration value of attribute "fontsize"</summary>
            ///See <see cref="GVDouble"/>for corresponding values
            Fontsize,
            ///<summary>Enumeration value of attribute "forcelabels"</summary>
            ///See <see cref="GVBool"/>for corresponding values
            Forcelabels,
            ///<summary>Enumeration value of attribute "gradientangle"</summary>
            ///See <see cref="GVInt"/>for corresponding values
            Gradientangle,
            ///<summary>Enumeration value of attribute "group"</summary>
            ///See <see cref="GVString"/>for corresponding values
            Group,
            ///<summary>Enumeration value of attribute "headURL"</summary>
            ///See <see cref="GVEscString"/>for corresponding values
            HeadURL,
            ///<summary>Enumeration value of attribute "head_lp"</summary>
            ///See <see cref="GVPoint"/>for corresponding values
            Head_lp,
            ///<summary>Enumeration value of attribute "headclip"</summary>
            ///See <see cref="GVBool"/>for corresponding values
            Headclip,
            ///<summary>Enumeration value of attribute "headhref"</summary>
            ///See <see cref="GVEscString"/>for corresponding values
            Headhref,
            ///<summary>Enumeration value of attribute "headlabel"</summary>
            ///See <see cref="GVEscString"/>for corresponding values
            ///See <see cref="HTML.Label"/>for corresponding values
            Headlabel,
            ///<summary>Enumeration value of attribute "headport"</summary>
            ///See <see cref="GVPortPos"/>for corresponding values
            Headport,
            ///<summary>Enumeration value of attribute "headtarget"</summary>
            ///See <see cref="GVEscString"/>for corresponding values
            Headtarget,
            ///<summary>Enumeration value of attribute "headtooltip"</summary>
            ///See <see cref="GVEscString"/>for corresponding values
            Headtooltip,
            ///<summary>Enumeration value of attribute "height"</summary>
            ///See <see cref="GVDouble"/>for corresponding values
            Height,
            ///<summary>Enumeration value of attribute "href"</summary>
            ///See <see cref="GVEscString"/>for corresponding values
            Href,
            ///<summary>Enumeration value of attribute "id"</summary>
            ///See <see cref="GVEscString"/>for corresponding values
            Id,
            ///<summary>Enumeration value of attribute "image"</summary>
            ///See <see cref="GVString"/>for corresponding values
            Image,
            ///<summary>Enumeration value of attribute "imagepath"</summary>
            ///See <see cref="GVString"/>for corresponding values
            Imagepath,
            ///<summary>Enumeration value of attribute "imagepos"</summary>
            ///See <see cref="GVString"/>for corresponding values
            Imagepos,
            ///<summary>Enumeration value of attribute "imagescale"</summary>
            ///See <see cref="GVBool"/>for corresponding values
            Imagescale,
            // TODO: string	false		
            ///<summary>Enumeration value of attribute "inputscale"</summary>
            ///See <see cref="GVDouble"/>for corresponding values
            Inputscale,
            ///<summary>Enumeration value of attribute "label"</summary>
            ///See <see cref="GVEscString"/>for corresponding values
            ///See <see cref="HTML.Label"/>for corresponding values
            Label,
            // TODO: "" (otherwise)		
            ///<summary>Enumeration value of attribute "labelURL"</summary>
            ///See <see cref="GVEscString"/>for corresponding values
            LabelURL,
            ///<summary>Enumeration value of attribute "label_scheme"</summary>
            ///See <see cref="GVInt"/>for corresponding values
            Label_scheme,
            ///<summary>Enumeration value of attribute "labelangle"</summary>
            ///See <see cref="GVDouble"/>for corresponding values
            Labelangle,
            ///<summary>Enumeration value of attribute "labeldistance"</summary>
            ///See <see cref="GVDouble"/>for corresponding values
            Labeldistance,
            ///<summary>Enumeration value of attribute "labelfloat"</summary>
            ///See <see cref="GVBool"/>for corresponding values
            Labelfloat,
            ///<summary>Enumeration value of attribute "labelfontcolor"</summary>
            ///See <see cref="GVColor"/>for corresponding values
            Labelfontcolor,
            ///<summary>Enumeration value of attribute "labelfontname"</summary>
            ///See <see cref="GVString"/>for corresponding values
            Labelfontname,
            ///<summary>Enumeration value of attribute "labelfontsize"</summary>
            ///See <see cref="GVDouble"/>for corresponding values
            Labelfontsize,
            ///<summary>Enumeration value of attribute "labelhref"</summary>
            ///See <see cref="GVEscString"/>for corresponding values
            Labelhref,
            ///<summary>Enumeration value of attribute "labeljust"</summary>
            ///See <see cref="GVString"/>for corresponding values
            Labeljust,
            ///<summary>Enumeration value of attribute "labelloc"</summary>
            ///See <see cref="GVString"/>for corresponding values
            Labelloc,
            // TODO: "b"(root graphs)
            // TODO: "c"(nodes)		
            ///<summary>Enumeration value of attribute "labeltarget"</summary>
            ///See <see cref="GVEscString"/>for corresponding values
            Labeltarget,
            ///<summary>Enumeration value of attribute "labeltooltip"</summary>
            ///See <see cref="GVEscString"/>for corresponding values
            Labeltooltip,
            ///<summary>Enumeration value of attribute "landscape"</summary>
            ///See <see cref="GVBool"/>for corresponding values
            Landscape,
            ///<summary>Enumeration value of attribute "layer"</summary>
            ///See <see cref="GVLayerRange"/>for corresponding values
            Layer,
            ///<summary>Enumeration value of attribute "layerlistsep"</summary>
            ///See <see cref="GVString"/>for corresponding values
            Layerlistsep,
            ///<summary>Enumeration value of attribute "layers"</summary>
            ///See <see cref="GVLayerList"/>for corresponding values
            Layers,
            ///<summary>Enumeration value of attribute "layerselect"</summary>
            ///See <see cref="GVLayerRange"/>for corresponding values
            Layerselect,
            ///<summary>Enumeration value of attribute "layersep"</summary>
            ///See <see cref="GVString"/>for corresponding values
            Layersep,
            ///<summary>Enumeration value of attribute "layout"</summary>
            ///See <see cref="GVString"/>for corresponding values
            Layout,
            ///<summary>Enumeration value of attribute "len"</summary>
            ///See <see cref="GVDouble"/>for corresponding values
            Len,
            // TODO: 0.3(fdp)		fdp, neato only
            ///<summary>Enumeration value of attribute "levels"</summary>
            ///See <see cref="GVInt"/>for corresponding values
            Levels,
            ///<summary>Enumeration value of attribute "levelsgap"</summary>
            ///See <see cref="GVDouble"/>for corresponding values
            Levelsgap,
            ///<summary>Enumeration value of attribute "lhead"</summary>
            ///See <see cref="GVString"/>for corresponding values
            Lhead,
            ///<summary>Enumeration value of attribute "lheight"</summary>
            ///See <see cref="GVDouble"/>for corresponding values
            Lheight,
            ///<summary>Enumeration value of attribute "lp"</summary>
            ///See <see cref="GVPoint"/>for corresponding values
            Lp,
            ///<summary>Enumeration value of attribute "ltail"</summary>
            ///See <see cref="GVString"/>for corresponding values
            Ltail,
            ///<summary>Enumeration value of attribute "lwidth"</summary>
            ///See <see cref="GVDouble"/>for corresponding values
            Lwidth,
            ///<summary>Enumeration value of attribute "margin"</summary>
            ///See <see cref="GVDouble"/>for corresponding values
            Margin,
            // TODO: point 	<device-dependent>		
            ///<summary>Enumeration value of attribute "maxiter"</summary>
            ///See <see cref="GVInt"/>for corresponding values
            Maxiter,
            // TODO: 200(mode == major)
            // TODO: 30(mode == sgd)
            // TODO: 600(fdp)		fdp, neato only
            ///<summary>Enumeration value of attribute "mclimit"</summary>
            ///See <see cref="GVDouble"/>for corresponding values
            Mclimit,
            ///<summary>Enumeration value of attribute "mindist"</summary>
            ///See <see cref="GVDouble"/>for corresponding values
            Mindist,
            ///<summary>Enumeration value of attribute "minlen"</summary>
            ///See <see cref="GVInt"/>for corresponding values
            Minlen,
            ///<summary>Enumeration value of attribute "mode"</summary>
            ///See <see cref="GVString"/>for corresponding values
            Mode,
            ///<summary>Enumeration value of attribute "model"</summary>
            ///See <see cref="GVString"/>for corresponding values
            Model,
            ///<summary>Enumeration value of attribute "mosek"</summary>
            ///See <see cref="GVBool"/>for corresponding values
            Mosek,
            ///<summary>Enumeration value of attribute "newrank"</summary>
            ///See <see cref="GVBool"/>for corresponding values
            Newrank,
            ///<summary>Enumeration value of attribute "nodesep"</summary>
            ///See <see cref="GVDouble"/>for corresponding values
            Nodesep,
            ///<summary>Enumeration value of attribute "nojustify"</summary>
            ///See <see cref="GVBool"/>for corresponding values
            Nojustify,
            ///<summary>Enumeration value of attribute "normalize"</summary>
            ///See <see cref="GVDouble"/>for corresponding values
            Normalize,
            // TODO: bool 	false		not dot
            ///<summary>Enumeration value of attribute "notranslate"</summary>
            ///See <see cref="GVBool"/>for corresponding values
            Notranslate,
            // TODO: nslimit
            ///<summary>Enumeration value of attribute "nslimit1"</summary>
            ///See <see cref="GVDouble"/>for corresponding values
            Nslimit1,
            ///<summary>Enumeration value of attribute "ordering"</summary>
            ///See <see cref="GVString"/>for corresponding values
            Ordering,
            ///<summary>Enumeration value of attribute "orientation"</summary>
            ///See <see cref="GVDouble"/>for corresponding values
            Orientation,
            ///<summary>Enumeration value of attribute "orientation"</summary>
            ///See <see cref="GVString"/>for corresponding values
            // Orientation,
            ///<summary>Enumeration value of attribute "outputorder"</summary>
            ///See <see cref="GVOutputMode"/>for corresponding values
            Outputorder,
            ///<summary>Enumeration value of attribute "overlap"</summary>
            ///See <see cref="GVString"/>for corresponding values
            Overlap,
            // TODO: bool 	true		not dot
            ///<summary>Enumeration value of attribute "overlap_scaling"</summary>
            ///See <see cref="GVDouble"/>for corresponding values
            Overlap_scaling,
            ///<summary>Enumeration value of attribute "overlap_shrink"</summary>
            ///See <see cref="GVBool"/>for corresponding values
            Overlap_shrink,
            ///<summary>Enumeration value of attribute "pack"</summary>
            ///See <see cref="GVBool"/>for corresponding values
            Pack,
            // TODO: int	false		
            ///<summary>Enumeration value of attribute "packmode"</summary>
            ///See <see cref="GVPackMode"/>for corresponding values
            Packmode,
            ///<summary>Enumeration value of attribute "pad"</summary>
            ///See <see cref="GVDouble"/>for corresponding values
            Pad,
            // TODO: point 	0.0555 (4 points)		
            ///<summary>Enumeration value of attribute "page"</summary>
            ///See <see cref="GVDouble"/>for corresponding values
            Page,
            // TODO: point 			
            ///<summary>Enumeration value of attribute "pagedir"</summary>
            ///See <see cref="GVPagedir"/>for corresponding values
            Pagedir,
            ///<summary>Enumeration value of attribute "pencolor"</summary>
            ///See <see cref="GVColor"/>for corresponding values
            Pencolor,
            ///<summary>Enumeration value of attribute "penwidth"</summary>
            ///See <see cref="GVDouble"/>for corresponding values
            Penwidth,
            ///<summary>Enumeration value of attribute "peripheries"</summary>
            ///See <see cref="GVInt"/>for corresponding values
            Peripheries,
            // TODO: 1(clusters)	0	
            ///<summary>Enumeration value of attribute "pin"</summary>
            ///See <see cref="GVBool"/>for corresponding values
            Pin,
            ///<summary>Enumeration value of attribute "pos"</summary>
            ///See <see cref="GVPoint"/>for corresponding values
            Pos,
            // TODO: splineType 			
            ///<summary>Enumeration value of attribute "quadtree"</summary>
            ///See <see cref="GVQuadType"/>for corresponding values
            Quadtree,
            // TODO: bool 	normal		sfdp only
            ///<summary>Enumeration value of attribute "quantum"</summary>
            ///See <see cref="GVDouble"/>for corresponding values
            Quantum,
            ///<summary>Enumeration value of attribute "rank"</summary>
            ///See <see cref="GVRankType"/>for corresponding values
            Rank,
            ///<summary>Enumeration value of attribute "rankdir"</summary>
            ///See <see cref="GVRankdir"/>for corresponding values
            Rankdir,
            ///<summary>Enumeration value of attribute "ranksep"</summary>
            ///See <see cref="GVDouble"/>for corresponding values
            Ranksep,
            // TODO: doubleList 	0.5(dot)
            // TODO: 1.0(twopi)	0.02	twopi, dot only
            ///<summary>Enumeration value of attribute "ratio"</summary>
            ///See <see cref="GVDouble"/>for corresponding values
            Ratio,
            // TODO: string			
            ///<summary>Enumeration value of attribute "rects"</summary>
            ///See <see cref="GVRect"/>for corresponding values
            Rects,
            ///<summary>Enumeration value of attribute "regular"</summary>
            ///See <see cref="GVBool"/>for corresponding values
            Regular,
            ///<summary>Enumeration value of attribute "remincross"</summary>
            ///See <see cref="GVBool"/>for corresponding values
            Remincross,
            ///<summary>Enumeration value of attribute "repulsiveforce"</summary>
            ///See <see cref="GVDouble"/>for corresponding values
            Repulsiveforce,
            ///<summary>Enumeration value of attribute "resolution"</summary>
            ///See <see cref="GVDouble"/>for corresponding values
            Resolution,
            // TODO: 0.0		svg, bitmap output only
            ///<summary>Enumeration value of attribute "root"</summary>
            ///See <see cref="GVString"/>for corresponding values
            Root,
            // TODO: bool 	<none>(graphs)
            // TODO: false(nodes)		circo, twopi only
            ///<summary>Enumeration value of attribute "rotate"</summary>
            ///See <see cref="GVInt"/>for corresponding values
            Rotate,
            ///<summary>Enumeration value of attribute "rotation"</summary>
            ///See <see cref="GVDouble"/>for corresponding values
            Rotation,
            ///<summary>Enumeration value of attribute "samehead"</summary>
            ///See <see cref="GVString"/>for corresponding values
            Samehead,
            ///<summary>Enumeration value of attribute "sametail"</summary>
            ///See <see cref="GVString"/>for corresponding values
            Sametail,
            ///<summary>Enumeration value of attribute "samplepoints"</summary>
            ///See <see cref="GVInt"/>for corresponding values
            Samplepoints,
            // TODO: 20(overlap and image maps)		
            ///<summary>Enumeration value of attribute "scale"</summary>
            ///See <see cref="GVDouble"/>for corresponding values
            Scale,
            // TODO: point 			not dot
            ///<summary>Enumeration value of attribute "searchsize"</summary>
            ///See <see cref="GVInt"/>for corresponding values
            Searchsize,
            ///<summary>Enumeration value of attribute "sep"</summary>
            ///See <see cref="GVAddDouble"/>for corresponding values
            Sep,
            // TODO: addPoint 	+4		not dot
            ///<summary>Enumeration value of attribute "shape"</summary>
            ///See <see cref="GVShape"/>for corresponding values
            Shape,
            ///<summary>Enumeration value of attribute "shapefile"</summary>
            ///See <see cref="GVString"/>for corresponding values
            Shapefile,
            ///<summary>Enumeration value of attribute "showboxes"</summary>
            ///See <see cref="GVInt"/>for corresponding values
            Showboxes,
            ///<summary>Enumeration value of attribute "sides"</summary>
            ///See <see cref="GVInt"/>for corresponding values
            Sides,
            ///<summary>Enumeration value of attribute "size"</summary>
            ///See <see cref="GVDouble"/>for corresponding values
            Size,
            // TODO: point 			
            ///<summary>Enumeration value of attribute "skew"</summary>
            ///See <see cref="GVDouble"/>for corresponding values
            Skew,
            ///<summary>Enumeration value of attribute "smoothing"</summary>
            ///See <see cref="GVSmoothType"/>for corresponding values
            Smoothing,
            ///<summary>Enumeration value of attribute "sortv"</summary>
            ///See <see cref="GVInt"/>for corresponding values
            Sortv,
            ///<summary>Enumeration value of attribute "splines"</summary>
            ///See <see cref="GVBool"/>for corresponding values
            Splines,
            // TODO: string			
            ///<summary>Enumeration value of attribute "start"</summary>
            ///See <see cref="GVStartType"/>for corresponding values
            Start,
            ///<summary>Enumeration value of attribute "style"</summary>
            ///See <see cref="GVStyle"/>for corresponding values
            Style,
            ///<summary>Enumeration value of attribute "stylesheet"</summary>
            ///See <see cref="GVString"/>for corresponding values
            Stylesheet,
            ///<summary>Enumeration value of attribute "tailURL"</summary>
            ///See <see cref="GVEscString"/>for corresponding values
            TailURL,
            ///<summary>Enumeration value of attribute "tail_lp"</summary>
            ///See <see cref="GVPoint"/>for corresponding values
            Tail_lp,
            ///<summary>Enumeration value of attribute "tailclip"</summary>
            ///See <see cref="GVBool"/>for corresponding values
            Tailclip,
            ///<summary>Enumeration value of attribute "tailhref"</summary>
            ///See <see cref="GVEscString"/>for corresponding values
            Tailhref,
            ///<summary>Enumeration value of attribute "taillabel"</summary>
            ///See <see cref="GVEscString"/>for corresponding values
            ///See <see cref="HTML.Label"/>for corresponding values
            Taillabel,
            ///<summary>Enumeration value of attribute "tailport"</summary>
            ///See <see cref="GVPortPos"/>for corresponding values
            Tailport,
            ///<summary>Enumeration value of attribute "tailtarget"</summary>
            ///See <see cref="GVEscString"/>for corresponding values
            Tailtarget,
            ///<summary>Enumeration value of attribute "tailtooltip"</summary>
            ///See <see cref="GVEscString"/>for corresponding values
            Tailtooltip,
            ///<summary>Enumeration value of attribute "target"</summary>
            ///See <see cref="GVEscString"/>for corresponding values
            Target,
            // TODO: string	<none>		svg, map only
            ///<summary>Enumeration value of attribute "tooltip"</summary>
            ///See <see cref="GVEscString"/>for corresponding values
            Tooltip,
            ///<summary>Enumeration value of attribute "truecolor"</summary>
            ///See <see cref="GVBool"/>for corresponding values
            Truecolor,
            ///<summary>Enumeration value of attribute "vertices"</summary>
            ///See <see cref="GVPointList"/>for corresponding values
            Vertices,
            ///<summary>Enumeration value of attribute "viewport"</summary>
            ///See <see cref="GVViewPort"/>for corresponding values
            Viewport,
            ///<summary>Enumeration value of attribute "voro_margin"</summary>
            ///See <see cref="GVDouble"/>for corresponding values
            Voro_margin,
            ///<summary>Enumeration value of attribute "weight"</summary>
            ///See <see cref="GVInt"/>for corresponding values
            Weight,
            // TODO: double	1	0(dot,twopi)
            // TODO: 1(neato,fdp)	
            ///<summary>Enumeration value of attribute "width"</summary>
            ///See <see cref="GVDouble"/>for corresponding values
            Width,
            ///<summary>Enumeration value of attribute "xdotversion"</summary>
            ///See <see cref="GVString"/>for corresponding values
            Xdotversion,
            ///<summary>Enumeration value of attribute "xlabel"</summary>
            ///See <see cref="GVEscString"/>for corresponding values
            ///See <see cref="HTML.Label"/>for corresponding values
            Xlabel,
            ///<summary>Enumeration value of attribute "xlp"</summary>
            ///See <see cref="GVPoint"/>for corresponding values
            Xlp,
            ///<summary>Enumeration value of attribute "z"</summary>
            ///See <see cref="GVDouble"/>for corresponding values
            Z,
            // TODO: -1000	
        }

        private static readonly string[] Text = {
            "Damping",
            "K",
            "URL",
            "_background",
            "area",
            "arrowhead",
            "arrowsize",
            "arrowtail",
            "bb",
            "bgcolor",
            "center",
            "charset",
            "class",
            "clusterrank",
            "color",
            "colorscheme",
            "comment",
            "compound",
            "concentrate",
            "constraint",
            "decorate",
            "defaultdist",
            "dim",
            "dimen",
            "dir",
            "diredgeconstraints",
            "distortion",
            "dpi",
            "edgeURL",
            "edgehref",
            "edgetarget",
            "edgetooltip",
            "epsilon",
            "esep",
            "fillcolor",
            "fixedsize",
            "fontcolor",
            "fontname",
            "fontnames",
            "fontpath",
            "fontsize",
            "forcelabels",
            "gradientangle",
            "group",
            "headURL",
            "head_lp",
            "headclip",
            "headhref",
            "headlabel",
            "headport",
            "headtarget",
            "headtooltip",
            "height",
            "href",
            "id",
            "image",
            "imagepath",
            "imagepos",
            "imagescale",
            "inputscale",
            "label",
            "labelURL",
            "label_scheme",
            "labelangle",
            "labeldistance",
            "labelfloat",
            "labelfontcolor",
            "labelfontname",
            "labelfontsize",
            "labelhref",
            "labeljust",
            "labelloc",
            "labeltarget",
            "labeltooltip",
            "landscape",
            "layer",
            "layerlistsep",
            "layers",
            "layerselect",
            "layersep",
            "layout",
            "len",
            "levels",
            "levelsgap",
            "lhead",
            "lheight",
            "lp",
            "ltail",
            "lwidth",
            "margin",
            "maxiter",
            "mclimit",
            "mindist",
            "minlen",
            "mode",
            "model",
            "mosek",
            "newrank",
            "nodesep",
            "nojustify",
            "normalize",
            "notranslate",
            "nslimit1",
            "ordering",
            "orientation",
            //"orientation",
            "outputorder",
            "overlap",
            "overlap_scaling",
            "overlap_shrink",
            "pack",
            "packmode",
            "pad",
            "page",
            "pagedir",
            "pencolor",
            "penwidth",
            "peripheries",
            "pin",
            "pos",
            "quadtree",
            "quantum",
            "rank",
            "rankdir",
            "ranksep",
            "ratio",
            "rects",
            "regular",
            "remincross",
            "repulsiveforce",
            "resolution",
            "root",
            "rotate",
            "rotation",
            "samehead",
            "sametail",
            "samplepoints",
            "scale",
            "searchsize",
            "sep",
            "shape",
            "shapefile",
            "showboxes",
            "sides",
            "size",
            "skew",
            "smoothing",
            "sortv",
            "splines",
            "start",
            "style",
            "stylesheet",
            "tailURL",
            "tail_lp",
            "tailclip",
            "tailhref",
            "taillabel",
            "tailport",
            "tailtarget",
            "tailtooltip",
            "target",
            "tooltip",
            "truecolor",
            "vertices",
            "viewport",
            "voro_margin",
            "weight",
            "width",
            "xdotversion",
            "xlabel",
            "xlp",
            "z",
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
        /// <param name="supported">An array of all supported attributes of a specific HTML entity</param>
        public GVAttributes(ID[] supported)
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

        public void Add(ID attr, GVValue value)
        {
            switch (attr)
            {
                case ID.Damping:
                    if (value.GetType() != typeof(GVDouble)) throw new GVException("Attribute Damping expects a value of type GVDouble");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.K:
                    if (value.GetType() != typeof(GVDouble)) throw new GVException("Attribute K expects a value of type GVDouble");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.URL:
                    if (value.GetType() != typeof(GVEscString)) throw new GVException("Attribute URL expects a value of type GVEscString");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID._background:
                    if (value.GetType() != typeof(GVString)) throw new GVException("Attribute _background expects a value of type GVString");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Area:
                    if (value.GetType() != typeof(GVDouble)) throw new GVException("Attribute Area expects a value of type GVDouble");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Arrowhead:
                    if (value.GetType() != typeof(GVArrowType)) throw new GVException("Attribute Arrowhead expects a value of type GVArrowType");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Arrowsize:
                    if (value.GetType() != typeof(GVDouble)) throw new GVException("Attribute Arrowsize expects a value of type GVDouble");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Arrowtail:
                    if (value.GetType() != typeof(GVArrowType)) throw new GVException("Attribute Arrowtail expects a value of type GVArrowType");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Bb:
                    if (value.GetType() != typeof(GVRect)) throw new GVException("Attribute Bb expects a value of type GVRect");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Bgcolor:
                    if (value.GetType() != typeof(GVColor)) throw new GVException("Attribute Bgcolor expects a value of type GVColor");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Center:
                    if (value.GetType() != typeof(GVBool)) throw new GVException("Attribute Center expects a value of type GVBool");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Charset:
                    if (value.GetType() != typeof(GVString)) throw new GVException("Attribute Charset expects a value of type GVString");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Class:
                    if (value.GetType() != typeof(GVString)) throw new GVException("Attribute Class expects a value of type GVString");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Clusterrank:
                    if (value.GetType() != typeof(GVClusterMode)) throw new GVException("Attribute Clusterrank expects a value of type GVClusterMode");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Color:
                    if (value.GetType() != typeof(GVColor)) throw new GVException("Attribute Color expects a value of type GVColor");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Colorscheme:
                    if (value.GetType() != typeof(GVString)) throw new GVException("Attribute Colorscheme expects a value of type GVString");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Comment:
                    if (value.GetType() != typeof(GVString)) throw new GVException("Attribute Comment expects a value of type GVString");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Compound:
                    if (value.GetType() != typeof(GVBool)) throw new GVException("Attribute Compound expects a value of type GVBool");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Concentrate:
                    if (value.GetType() != typeof(GVBool)) throw new GVException("Attribute Concentrate expects a value of type GVBool");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Constraint:
                    if (value.GetType() != typeof(GVBool)) throw new GVException("Attribute Constraint expects a value of type GVBool");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Decorate:
                    if (value.GetType() != typeof(GVBool)) throw new GVException("Attribute Decorate expects a value of type GVBool");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Defaultdist:
                    if (value.GetType() != typeof(GVDouble)) throw new GVException("Attribute Defaultdist expects a value of type GVDouble");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Dim:
                    if (value.GetType() != typeof(GVInt)) throw new GVException("Attribute Dim expects a value of type GVInt");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Dimen:
                    if (value.GetType() != typeof(GVInt)) throw new GVException("Attribute Dimen expects a value of type GVInt");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Dir:
                    if (value.GetType() != typeof(GVDirType)) throw new GVException("Attribute Dir expects a value of type GVDirType");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Diredgeconstraints:
                    if (value.GetType() != typeof(GVString)) throw new GVException("Attribute Diredgeconstraints expects a value of type GVString");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Distortion:
                    if (value.GetType() != typeof(GVDouble)) throw new GVException("Attribute Distortion expects a value of type GVDouble");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Dpi:
                    if (value.GetType() != typeof(GVDouble)) throw new GVException("Attribute Dpi expects a value of type GVDouble");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.EdgeURL:
                    if (value.GetType() != typeof(GVEscString)) throw new GVException("Attribute EdgeURL expects a value of type GVEscString");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Edgehref:
                    if (value.GetType() != typeof(GVEscString)) throw new GVException("Attribute Edgehref expects a value of type GVEscString");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Edgetarget:
                    if (value.GetType() != typeof(GVEscString)) throw new GVException("Attribute Edgetarget expects a value of type GVEscString");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Edgetooltip:
                    if (value.GetType() != typeof(GVEscString)) throw new GVException("Attribute Edgetooltip expects a value of type GVEscString");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Epsilon:
                    if (value.GetType() != typeof(GVDouble)) throw new GVException("Attribute Epsilon expects a value of type GVDouble");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Esep:
                    if (value.GetType() != typeof(GVAddDouble)) throw new GVException("Attribute Esep expects a value of type GVAddDouble");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Fillcolor:
                    if (value.GetType() != typeof(GVColor)) throw new GVException("Attribute Fillcolor expects a value of type GVColor");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Fixedsize:
                    if (value.GetType() != typeof(GVBool)) throw new GVException("Attribute Fixedsize expects a value of type GVBool");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Fontcolor:
                    if (value.GetType() != typeof(GVColor)) throw new GVException("Attribute Fontcolor expects a value of type GVColor");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Fontname:
                    if (value.GetType() != typeof(GVString)) throw new GVException("Attribute Fontname expects a value of type GVString");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Fontnames:
                    if (value.GetType() != typeof(GVString)) throw new GVException("Attribute Fontnames expects a value of type GVString");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Fontpath:
                    if (value.GetType() != typeof(GVString)) throw new GVException("Attribute Fontpath expects a value of type GVString");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Fontsize:
                    if (value.GetType() != typeof(GVDouble)) throw new GVException("Attribute Fontsize expects a value of type GVDouble");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Forcelabels:
                    if (value.GetType() != typeof(GVBool)) throw new GVException("Attribute Forcelabels expects a value of type GVBool");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Gradientangle:
                    if (value.GetType() != typeof(GVInt)) throw new GVException("Attribute Gradientangle expects a value of type GVInt");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Group:
                    if (value.GetType() != typeof(GVString)) throw new GVException("Attribute Group expects a value of type GVString");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.HeadURL:
                    if (value.GetType() != typeof(GVEscString)) throw new GVException("Attribute HeadURL expects a value of type GVEscString");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Head_lp:
                    if (value.GetType() != typeof(GVPoint)) throw new GVException("Attribute Head_lp expects a value of type GVPoint");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Headclip:
                    if (value.GetType() != typeof(GVBool)) throw new GVException("Attribute Headclip expects a value of type GVBool");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Headhref:
                    if (value.GetType() != typeof(GVEscString)) throw new GVException("Attribute Headhref expects a value of type GVEscString");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Headlabel:
                    if ((value.GetType() != typeof(GVEscString)) && (value.GetType() != typeof(HTML.Label)))
                        throw new GVException("Attribute Headlabel expects a value of type GVEscString or HTML.Label");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Headport:
                    if (value.GetType() != typeof(GVPortPos)) throw new GVException("Attribute Headport expects a value of type GVPortPos");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Headtarget:
                    if (value.GetType() != typeof(GVEscString)) throw new GVException("Attribute Headtarget expects a value of type GVEscString");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Headtooltip:
                    if (value.GetType() != typeof(GVEscString)) throw new GVException("Attribute Headtooltip expects a value of type GVEscString");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Height:
                    if (value.GetType() != typeof(GVDouble)) throw new GVException("Attribute Height expects a value of type GVDouble");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Href:
                    if (value.GetType() != typeof(GVEscString)) throw new GVException("Attribute Href expects a value of type GVEscString");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Id:
                    if (value.GetType() != typeof(GVEscString)) throw new GVException("Attribute Id expects a value of type GVEscString");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Image:
                    if (value.GetType() != typeof(GVString)) throw new GVException("Attribute Image expects a value of type GVString");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Imagepath:
                    if (value.GetType() != typeof(GVString)) throw new GVException("Attribute Imagepath expects a value of type GVString");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Imagepos:
                    if (value.GetType() != typeof(GVString)) throw new GVException("Attribute Imagepos expects a value of type GVString");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Imagescale:
                    if (value.GetType() != typeof(GVBool)) throw new GVException("Attribute Imagescale expects a value of type GVBool");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Inputscale:
                    if (value.GetType() != typeof(GVDouble)) throw new GVException("Attribute Inputscale expects a value of type GVDouble");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Label:
                    if ((value.GetType() != typeof(GVEscString)) && (value.GetType() != typeof(HTML.Label)))
                        throw new GVException("Attribute Headlabel expects a value of type GVEscString or HTML.Label");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.LabelURL:
                    if (value.GetType() != typeof(GVEscString)) throw new GVException("Attribute LabelURL expects a value of type GVEscString");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Label_scheme:
                    if (value.GetType() != typeof(GVInt)) throw new GVException("Attribute Label_scheme expects a value of type GVInt");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Labelangle:
                    if (value.GetType() != typeof(GVDouble)) throw new GVException("Attribute Labelangle expects a value of type GVDouble");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Labeldistance:
                    if (value.GetType() != typeof(GVDouble)) throw new GVException("Attribute Labeldistance expects a value of type GVDouble");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Labelfloat:
                    if (value.GetType() != typeof(GVBool)) throw new GVException("Attribute Labelfloat expects a value of type GVBool");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Labelfontcolor:
                    if (value.GetType() != typeof(GVColor)) throw new GVException("Attribute Labelfontcolor expects a value of type GVColor");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Labelfontname:
                    if (value.GetType() != typeof(GVString)) throw new GVException("Attribute Labelfontname expects a value of type GVString");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Labelfontsize:
                    if (value.GetType() != typeof(GVDouble)) throw new GVException("Attribute Labelfontsize expects a value of type GVDouble");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Labelhref:
                    if (value.GetType() != typeof(GVEscString)) throw new GVException("Attribute Labelhref expects a value of type GVEscString");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Labeljust:
                    if (value.GetType() != typeof(GVString)) throw new GVException("Attribute Labeljust expects a value of type GVString");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Labelloc:
                    if (value.GetType() != typeof(GVString)) throw new GVException("Attribute Labelloc expects a value of type GVString");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Labeltarget:
                    if (value.GetType() != typeof(GVEscString)) throw new GVException("Attribute Labeltarget expects a value of type GVEscString");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Labeltooltip:
                    if (value.GetType() != typeof(GVEscString)) throw new GVException("Attribute Labeltooltip expects a value of type GVEscString");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Landscape:
                    if (value.GetType() != typeof(GVBool)) throw new GVException("Attribute Landscape expects a value of type GVBool");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Layer:
                    if (value.GetType() != typeof(GVLayerRange)) throw new GVException("Attribute Layer expects a value of type GVLayerRange");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Layerlistsep:
                    if (value.GetType() != typeof(GVString)) throw new GVException("Attribute Layerlistsep expects a value of type GVString");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Layers:
                    if (value.GetType() != typeof(GVLayerList)) throw new GVException("Attribute Layers expects a value of type GVLayerList");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Layerselect:
                    if (value.GetType() != typeof(GVLayerRange)) throw new GVException("Attribute Layerselect expects a value of type GVLayerRange");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Layersep:
                    if (value.GetType() != typeof(GVString)) throw new GVException("Attribute Layersep expects a value of type GVString");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Layout:
                    if (value.GetType() != typeof(GVString)) throw new GVException("Attribute Layout expects a value of type GVString");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Len:
                    if (value.GetType() != typeof(GVDouble)) throw new GVException("Attribute Len expects a value of type GVDouble");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Levels:
                    if (value.GetType() != typeof(GVInt)) throw new GVException("Attribute Levels expects a value of type GVInt");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Levelsgap:
                    if (value.GetType() != typeof(GVDouble)) throw new GVException("Attribute Levelsgap expects a value of type GVDouble");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Lhead:
                    if (value.GetType() != typeof(GVString)) throw new GVException("Attribute Lhead expects a value of type GVString");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Lheight:
                    if (value.GetType() != typeof(GVDouble)) throw new GVException("Attribute Lheight expects a value of type GVDouble");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Lp:
                    if (value.GetType() != typeof(GVPoint)) throw new GVException("Attribute Lp expects a value of type GVPoint");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Ltail:
                    if (value.GetType() != typeof(GVString)) throw new GVException("Attribute Ltail expects a value of type GVString");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Lwidth:
                    if (value.GetType() != typeof(GVDouble)) throw new GVException("Attribute Lwidth expects a value of type GVDouble");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Margin:
                    if (value.GetType() != typeof(GVDouble)) throw new GVException("Attribute Margin expects a value of type GVDouble");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Maxiter:
                    if (value.GetType() != typeof(GVInt)) throw new GVException("Attribute Maxiter expects a value of type GVInt");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Mclimit:
                    if (value.GetType() != typeof(GVDouble)) throw new GVException("Attribute Mclimit expects a value of type GVDouble");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Mindist:
                    if (value.GetType() != typeof(GVDouble)) throw new GVException("Attribute Mindist expects a value of type GVDouble");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Minlen:
                    if (value.GetType() != typeof(GVInt)) throw new GVException("Attribute Minlen expects a value of type GVInt");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Mode:
                    if (value.GetType() != typeof(GVString)) throw new GVException("Attribute Mode expects a value of type GVString");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Model:
                    if (value.GetType() != typeof(GVString)) throw new GVException("Attribute Model expects a value of type GVString");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Mosek:
                    if (value.GetType() != typeof(GVBool)) throw new GVException("Attribute Mosek expects a value of type GVBool");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Newrank:
                    if (value.GetType() != typeof(GVBool)) throw new GVException("Attribute Newrank expects a value of type GVBool");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Nodesep:
                    if (value.GetType() != typeof(GVDouble)) throw new GVException("Attribute Nodesep expects a value of type GVDouble");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Nojustify:
                    if (value.GetType() != typeof(GVBool)) throw new GVException("Attribute Nojustify expects a value of type GVBool");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Normalize:
                    if (value.GetType() != typeof(GVDouble)) throw new GVException("Attribute Normalize expects a value of type GVDouble");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Notranslate:
                    if (value.GetType() != typeof(GVBool)) throw new GVException("Attribute Notranslate expects a value of type GVBool");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Nslimit1:
                    if (value.GetType() != typeof(GVDouble)) throw new GVException("Attribute Nslimit1 expects a value of type GVDouble");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Ordering:
                    if (value.GetType() != typeof(GVString)) throw new GVException("Attribute Ordering expects a value of type GVString");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Orientation:
                    if (value.GetType() != typeof(GVString)) throw new GVException("Attribute Orientation expects a value of type GVString");
                    attributes.Add(attr, value.ToString());
                    break;
                //case ID.Orientation:
                //    if (value.GetType() != typeof(GVString)) throw new GVException("Attribute Orientation expects a value of type GVString");
                //    attributes.Add(attr, value.ToString());
                //    break;
                case ID.Outputorder:
                    if (value.GetType() != typeof(GVOutputMode)) throw new GVException("Attribute Outputorder expects a value of type GVOutputMode");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Overlap:
                    if (value.GetType() != typeof(GVString)) throw new GVException("Attribute Overlap expects a value of type GVString");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Overlap_scaling:
                    if (value.GetType() != typeof(GVDouble)) throw new GVException("Attribute Overlap_scaling expects a value of type GVDouble");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Overlap_shrink:
                    if (value.GetType() != typeof(GVBool)) throw new GVException("Attribute Overlap_shrink expects a value of type GVBool");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Pack:
                    if (value.GetType() != typeof(GVBool)) throw new GVException("Attribute Pack expects a value of type GVBool");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Packmode:
                    if (value.GetType() != typeof(GVPackMode)) throw new GVException("Attribute Packmode expects a value of type GVPackMode");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Pad:
                    if (value.GetType() != typeof(GVDouble)) throw new GVException("Attribute Pad expects a value of type GVDouble");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Page:
                    if (value.GetType() != typeof(GVDouble)) throw new GVException("Attribute Page expects a value of type GVDouble");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Pagedir:
                    if (value.GetType() != typeof(GVPagedir)) throw new GVException("Attribute Pagedir expects a value of type GVPagedir");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Pencolor:
                    if (value.GetType() != typeof(GVColor)) throw new GVException("Attribute Pencolor expects a value of type GVColor");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Penwidth:
                    if (value.GetType() != typeof(GVDouble)) throw new GVException("Attribute Penwidth expects a value of type GVDouble");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Peripheries:
                    if (value.GetType() != typeof(GVInt)) throw new GVException("Attribute Peripheries expects a value of type GVInt");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Pin:
                    if (value.GetType() != typeof(GVBool)) throw new GVException("Attribute Pin expects a value of type GVBool");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Pos:
                    if (value.GetType() != typeof(GVPoint)) throw new GVException("Attribute Pos expects a value of type GVPoint");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Quadtree:
                    if (value.GetType() != typeof(GVQuadType)) throw new GVException("Attribute Quadtree expects a value of type GVQuadType");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Quantum:
                    if (value.GetType() != typeof(GVDouble)) throw new GVException("Attribute Quantum expects a value of type GVDouble");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Rank:
                    if (value.GetType() != typeof(GVRankType)) throw new GVException("Attribute Rank expects a value of type GVRankType");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Rankdir:
                    if (value.GetType() != typeof(GVRankdir)) throw new GVException("Attribute Rankdir expects a value of type GVRankdir");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Ranksep:
                    if (value.GetType() != typeof(GVDouble)) throw new GVException("Attribute Ranksep expects a value of type GVDouble");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Ratio:
                    if (value.GetType() != typeof(GVDouble)) throw new GVException("Attribute Ratio expects a value of type GVDouble");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Rects:
                    if (value.GetType() != typeof(GVRect)) throw new GVException("Attribute Rects expects a value of type GVRect");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Regular:
                    if (value.GetType() != typeof(GVBool)) throw new GVException("Attribute Regular expects a value of type GVBool");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Remincross:
                    if (value.GetType() != typeof(GVBool)) throw new GVException("Attribute Remincross expects a value of type GVBool");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Repulsiveforce:
                    if (value.GetType() != typeof(GVDouble)) throw new GVException("Attribute Repulsiveforce expects a value of type GVDouble");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Resolution:
                    if (value.GetType() != typeof(GVDouble)) throw new GVException("Attribute Resolution expects a value of type GVDouble");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Root:
                    if (value.GetType() != typeof(GVString)) throw new GVException("Attribute Root expects a value of type GVString");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Rotate:
                    if (value.GetType() != typeof(GVInt)) throw new GVException("Attribute Rotate expects a value of type GVInt");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Rotation:
                    if (value.GetType() != typeof(GVDouble)) throw new GVException("Attribute Rotation expects a value of type GVDouble");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Samehead:
                    if (value.GetType() != typeof(GVString)) throw new GVException("Attribute Samehead expects a value of type GVString");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Sametail:
                    if (value.GetType() != typeof(GVString)) throw new GVException("Attribute Sametail expects a value of type GVString");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Samplepoints:
                    if (value.GetType() != typeof(GVInt)) throw new GVException("Attribute Samplepoints expects a value of type GVInt");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Scale:
                    if (value.GetType() != typeof(GVDouble)) throw new GVException("Attribute Scale expects a value of type GVDouble");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Searchsize:
                    if (value.GetType() != typeof(GVInt)) throw new GVException("Attribute Searchsize expects a value of type GVInt");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Sep:
                    if (value.GetType() != typeof(GVAddDouble)) throw new GVException("Attribute Sep expects a value of type GVAddDouble");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Shape:
                    if (value.GetType() != typeof(GVShape)) throw new GVException("Attribute Shape expects a value of type GVShape");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Shapefile:
                    if (value.GetType() != typeof(GVString)) throw new GVException("Attribute Shapefile expects a value of type GVString");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Showboxes:
                    if (value.GetType() != typeof(GVInt)) throw new GVException("Attribute Showboxes expects a value of type GVInt");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Sides:
                    if (value.GetType() != typeof(GVInt)) throw new GVException("Attribute Sides expects a value of type GVInt");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Size:
                    if (value.GetType() != typeof(GVDouble)) throw new GVException("Attribute Size expects a value of type GVDouble");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Skew:
                    if (value.GetType() != typeof(GVDouble)) throw new GVException("Attribute Skew expects a value of type GVDouble");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Smoothing:
                    if (value.GetType() != typeof(GVSmoothType)) throw new GVException("Attribute Smoothing expects a value of type GVSmoothType");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Sortv:
                    if (value.GetType() != typeof(GVInt)) throw new GVException("Attribute Sortv expects a value of type GVInt");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Splines:
                    if (value.GetType() != typeof(GVBool)) throw new GVException("Attribute Splines expects a value of type GVBool");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Start:
                    if (value.GetType() != typeof(GVStartType)) throw new GVException("Attribute Start expects a value of type GVStartType");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Style:
                    if (value.GetType() != typeof(GVStyle)) throw new GVException("Attribute Style expects a value of type GVStyle");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Stylesheet:
                    if (value.GetType() != typeof(GVString)) throw new GVException("Attribute Stylesheet expects a value of type GVString");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.TailURL:
                    if (value.GetType() != typeof(GVEscString)) throw new GVException("Attribute TailURL expects a value of type GVEscString");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Tail_lp:
                    if (value.GetType() != typeof(GVPoint)) throw new GVException("Attribute Tail_lp expects a value of type GVPoint");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Tailclip:
                    if (value.GetType() != typeof(GVBool)) throw new GVException("Attribute Tailclip expects a value of type GVBool");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Tailhref:
                    if (value.GetType() != typeof(GVEscString)) throw new GVException("Attribute Tailhref expects a value of type GVEscString");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Taillabel:
                    if ((value.GetType() != typeof(GVEscString)) && (value.GetType() != typeof(HTML.Label)))
                        throw new GVException("Attribute Headlabel expects a value of type GVEscString or HTML.Label");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Tailport:
                    if (value.GetType() != typeof(GVPortPos)) throw new GVException("Attribute Tailport expects a value of type GVPortPos");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Tailtarget:
                    if (value.GetType() != typeof(GVEscString)) throw new GVException("Attribute Tailtarget expects a value of type GVEscString");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Tailtooltip:
                    if (value.GetType() != typeof(GVEscString)) throw new GVException("Attribute Tailtooltip expects a value of type GVEscString");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Target:
                    if (value.GetType() != typeof(GVEscString)) throw new GVException("Attribute Target expects a value of type GVEscString");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Tooltip:
                    if (value.GetType() != typeof(GVEscString)) throw new GVException("Attribute Tooltip expects a value of type GVEscString");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Truecolor:
                    if (value.GetType() != typeof(GVBool)) throw new GVException("Attribute Truecolor expects a value of type GVBool");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Vertices:
                    if (value.GetType() != typeof(GVPointList)) throw new GVException("Attribute Vertices expects a value of type GVPointList");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Viewport:
                    if (value.GetType() != typeof(GVViewPort)) throw new GVException("Attribute Viewport expects a value of type GVViewPort");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Voro_margin:
                    if (value.GetType() != typeof(GVDouble)) throw new GVException("Attribute Voro_margin expects a value of type GVDouble");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Weight:
                    if (value.GetType() != typeof(GVInt)) throw new GVException("Attribute Weight expects a value of type GVInt");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Width:
                    if (value.GetType() != typeof(GVDouble)) throw new GVException("Attribute Width expects a value of type GVDouble");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Xdotversion:
                    if (value.GetType() != typeof(GVString)) throw new GVException("Attribute Xdotversion expects a value of type GVString");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Xlabel:
                    if ((value.GetType() != typeof(GVEscString)) && (value.GetType() != typeof(HTML.Label)))
                        throw new GVException("Attribute Headlabel expects a value of type GVEscString or HTML.Label");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Xlp:
                    if (value.GetType() != typeof(GVPoint)) throw new GVException("Attribute Xlp expects a value of type GVPoint");
                    attributes.Add(attr, value.ToString());
                    break;
                case ID.Z:
                    if (value.GetType() != typeof(GVDouble)) throw new GVException("Attribute Z expects a value of type GVDouble");
                    attributes.Add(attr, value.ToString());
                    break;
            }
        }
    }
}
