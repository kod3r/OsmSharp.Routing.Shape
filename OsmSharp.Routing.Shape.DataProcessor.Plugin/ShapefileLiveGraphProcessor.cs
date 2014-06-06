using OsmSharp.Collections.Tags;
using OsmSharp.Routing.Osm.Graphs.Serialization;
using OsmSharpDataProcessor.Commands.Processors;
using OsmSharp.Routing.Shape.Readers;
using System;
using System.Collections.Generic;
using System.IO;

namespace OsmSharp.Routing.Shape.DataProcessor.Plugin
{
    /// <summary>
    /// Represents a live graph processor that serializes a routing graph directly from shapefiles.
    /// </summary>
    public class ShapefileLiveGraphProcessor : ProcessorBase
    {
        /// <summary>
        /// Holds the path of the shapefiles.
        /// </summary>
        private string _path;

        /// <summary>
        /// Holds the search pattern for the shapefiles.
        /// </summary>
        private string _searchPattern;

        /// <summary>
        /// Holds the path to the resulting graph.
        /// </summary>
        private string _graph;

        /// <summary>
        /// Holds the node from column.
        /// </summary>
        private string _nodeFromColumn;

        /// <summary>
        /// Holds the node to column.
        /// </summary>
        private string _nodeToColumn;

        /// <summary>
        /// Holds the column with the distance.
        /// </summary>
        private string _distanceColumn;

        /// <summary>
        /// Holds the distance factor relative to meters.
        /// </summary>
        private float _distanceFactor;

        /// <summary>
        /// Creates a new shapefile graph processor.
        /// </summary>
        /// <param name="path">The root path to the shapefile(s).</param>
        /// <param name="searchPattern">The search string to identify the shapefile(s) to read.  ex: "*nw.shp"</param>
        /// <param name="graph">The output file to write the resuting graph to.</param>
        /// <param name="nodeFromColumn">The column containing the from node. ex: "F_JNCTID"</param>
        /// <param name="nodeToColumn">The column containing the to node. ex: "T_JNCTID"</param>
        public ShapefileLiveGraphProcessor(string graph, string path, string searchPattern, string nodeFromColumn, string nodeToColumn)
        {
            _graph = graph;
            _path = path;
            _searchPattern = searchPattern;

            _nodeFromColumn = nodeFromColumn;
            _nodeToColumn = nodeToColumn;
            _distanceColumn = string.Empty;
            _distanceFactor = 0;
        }
    
        /// <summary>
        /// Creates a new shapefile graph processor.
        /// </summary>
        /// <param name="path">The root path to the shapefile(s).</param>
        /// <param name="searchPattern">The search string to identify the shapefile(s) to read.  ex: "*nw.shp"</param>
        /// <param name="graph">The output file to write the resuting graph to.</param>
        /// <param name="nodeFromColumn">The column containing the from node. ex: "F_JNCTID"</param>
        /// <param name="nodeToColumn">The column containing the to node. ex: "T_JNCTID"</param>
        /// <param name="distanceColumn">The column containg the distance. ex: "METERS"</param>
        /// <param name="distanceFactor">The column containing the factor to convert the distance to meter (1=meter, 0.001=km).</param>
        public ShapefileLiveGraphProcessor(string graph, string path, string searchPattern, string nodeFromColumn, string nodeToColumn, string distanceColumn, float distanceFactor)
        {
            _graph = graph;
            _path = path;
            _searchPattern = searchPattern;

            _nodeFromColumn = nodeFromColumn;
            _nodeToColumn = nodeToColumn;
            _distanceColumn = distanceColumn;
            _distanceFactor = distanceFactor;
        }

        /// <summary>
        /// Returns true if this processor can be executed.
        /// </summary>
        public override bool CanExecute
        {
            get { return true; }
        }

        /// <summary>
        /// Collapses the given list of processors by adding this one to it.
        /// </summary>
        /// <param name="processors"></param>
        public override void Collapse(List<ProcessorBase> processors)
        {
            // just add this processor, it's a standalone thing.
            processors.Add(this);
        }

        /// <summary>
        /// Executes the tasks or commands in this processor.
        /// </summary>
        public override void Execute()
        {
            if (_path == null) { throw new InvalidOperationException("Path not set."); }

            var graphReader = new ShapefileLiveGraphReader(_nodeFromColumn, _nodeToColumn, _distanceColumn, _distanceFactor);
            var graph = graphReader.Read(_path, _searchPattern);

            var stream = new FileInfo(_graph).OpenWrite();
            var serializer = new LiveEdgeFlatfileSerializer();
            serializer.Serialize(stream, graph, new TagsCollection());
            stream.Flush();
        }

        /// <summary>
        /// Returns true if this processor is ready.
        /// </summary>
        public override bool IsReady
        {
            get { return true; }
        }
    }
}
