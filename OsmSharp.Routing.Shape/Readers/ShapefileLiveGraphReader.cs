// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2013 Abelshausen Ben
// 
// This file is part of OsmSharp.
// 
// OsmSharp is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// OsmSharp is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with OsmSharp. If not, see <http://www.gnu.org/licenses/>.

using NetTopologySuite;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using OsmSharp.Collections.Tags.Index;
using OsmSharp.Logging;
using OsmSharp.Math.Geo;
using OsmSharp.Math.Geo.Simple;
using OsmSharp.Routing.Graph;
using OsmSharp.Routing.Osm.Graphs;
using System;
using System.Collections.Generic;
using System.IO;

namespace OsmSharp.Routing.Shape.Readers
{
    /// <summary>
    /// A reader to read a shapefile network directly into a live data source.
    /// </summary>
    public class ShapefileLiveGraphReader : ShapefileGraphReader<LiveEdge>
    {
        /// <summary>
        /// Creates a new shapefile graph reader.
        /// </summary>
        /// <param name="nodeFromColumn">The column containing the from node. ex: "JTE_ID_BEG"</param>
        /// <param name="nodeToColumn">The column containing the to node. ex: "JTE_ID_END"</param>
        public ShapefileLiveGraphReader(string nodeFromColumn, string nodeToColumn)
            : base(nodeFromColumn, nodeToColumn)
        {

        }

        /// <summary>
        /// Creates a new shapefile graph reader.
        /// </summary>
        /// <param name="nodeFromColumn">The column containing the from node. ex: "JTE_ID_BEG"</param>
        /// <param name="nodeToColumn">The column containing the to node. ex: "JTE_ID_END"</param>
        /// <param name="distanceColumn">The column containg the distance. ex: "METERS"</param>
        /// <param name="distanceFactor">The column containing the factor to convert the distance to meter (1=meter, 0.001=km).</param>
        public ShapefileLiveGraphReader(string nodeFromColumn, string nodeToColumn, string distanceColumn, float distanceFactor)
            : base(nodeFromColumn, nodeToColumn, distanceColumn, distanceFactor)
        {

        }

        /// <summary>
        /// Reads a routing network into a live edge graph.
        /// </summary>
        /// <param name="path">The root path to the shapefile(s).</param>
        /// <param name="searchPattern">The search string to identify the shapefile(s) to read.  ex: "*nw.shp"</param>
        /// <param name="interpreter">The shapefile interpreter telling the reader what to import or interpret.</param>
        /// <returns></returns>
        public override DynamicGraphRouterDataSource<LiveEdge> Read(string path, string searchPattern, ShapefileRoutingInterpreter interpreter)
        {
            // build a list of nw-files.
            var directoryInfo = new DirectoryInfo(path);
            var networkFiles = directoryInfo.EnumerateFiles(searchPattern, SearchOption.AllDirectories);

            // create target data structures.
            var nodeToVertex = new Dictionary<long, uint>();
            var tagsIndex = new TagsTableCollectionIndex();
            var graph = new DynamicGraphRouterDataSource<LiveEdge>(tagsIndex);

            // create all readers.
            var readers = new List<ShapefileDataReader>();
            var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory();
            foreach(var networkFile in networkFiles)
            {
                readers.Add(new ShapefileDataReader(networkFile.FullName, geometryFactory));
            }
            
            // read all vertices.
            for(int readerIdx = 0; readerIdx < readers.Count; readerIdx++)
            {
                var reader = readers[readerIdx];
                var header = new Dictionary<string, int>();
                // make sure the header is loaded.
                if (header.Count == 0)
                { // build header.
                    for (int idx = 0; idx < reader.DbaseHeader.Fields.Length; idx++)
                    {
                        header.Add(reader.DbaseHeader.Fields[idx].Name, idx);
                    }

                    // check if all columns are in the header.
                    if(!header.ContainsKey(this.NodeFromColumn))
                    { // no node from column.
                        throw new InvalidOperationException(string.Format("No column with name {0} found.", this.NodeFromColumn));
                    }
                    if (!header.ContainsKey(this.NodeToColumn))
                    { // no node to column.
                        throw new InvalidOperationException(string.Format("No column with name {0} found.", this.NodeToColumn));
                    }
                    if(this.HasDistanceColumn && !header.ContainsKey(this.DistanceColumn))
                    { // no distance column found.
                        throw new InvalidOperationException(string.Format("No column with name {0} found.", this.DistanceColumn));
                    }
                }

                // read all vertices.
                double latestProgress = 0;
                int current = 0;
                while (reader.Read())
                {
                    // get the geometry.
                    var lineString = reader.Geometry as LineString;

                    // read nodes
                    long fromId = reader.GetInt64(header[this.NodeFromColumn]);
                    if (!nodeToVertex.ContainsKey(fromId))
                    { // the node has not been processed yet.
                        nodeToVertex.Add(fromId,
                            graph.AddVertex(
                                (float)lineString.Coordinates[0].Y,
                                (float)lineString.Coordinates[0].X));
                    }

                    long toId = reader.GetInt64(header[this.NodeToColumn]);
                    if (!nodeToVertex.ContainsKey(toId))
                    { // the node has not been processed yet.
                        nodeToVertex.Add(toId,
                            graph.AddVertex(
                                (float)lineString.Coordinates[lineString.Coordinates.Length - 1].Y,
                                (float)lineString.Coordinates[lineString.Coordinates.Length - 1].X));
                    }

                    // report progress.
                    float progress = (float)System.Math.Round((((double)current / (double)reader.RecordCount) * 100));
                    current++;
                    if (progress != latestProgress)
                    {
                        OsmSharp.Logging.Log.TraceEvent("ShapefileLiveGraphReader", TraceEventType.Information,
                            "Reading vertices from file {1}/{2}... {0}%", progress, readerIdx + 1, readers.Count);
                        latestProgress = progress;
                    }
                }
            }

            // read all edges.
            for (int readerIdx = 0; readerIdx < readers.Count; readerIdx++)
            {
                var reader = readers[readerIdx];
                var header = new Dictionary<string, int>();
                // make sure the header is loaded.
                if (header.Count == 0)
                { // build header.
                    for (int idx = 0; idx < reader.DbaseHeader.Fields.Length; idx++)
                    {
                        header.Add(reader.DbaseHeader.Fields[idx].Name, idx);
                    }
                }

                // reset reader and read all edges/arcs.
                double latestProgress = 0;
                int current = 0;
                reader.Reset();
                while (reader.Read())
                {
                    // get the geometry.
                    var lineString = reader.Geometry as LineString;

                    // read nodes
                    long fromId = reader.GetInt64(header[this.NodeFromColumn]);
                    long toId = reader.GetInt64(header[this.NodeToColumn]);
                    uint fromVertexId, toVertexId;
                    if (nodeToVertex.TryGetValue(fromId, out fromVertexId) &&
                        nodeToVertex.TryGetValue(toId, out toVertexId))
                    { // the node has not been processed yet.
                        double distance = 0;
                        if (this.HasDistanceColumn)
                        { // use the distance column to read the distance.
                            distance = reader.GetDouble(header[this.DistanceColumn]) * this.DistanceFactor;
                        }
                        else
                        { // use the coordinates to calculate the distance.
                            float latitudeFrom, latitudeTo, longitudeFrom, longitudeTo;
                            if (graph.GetVertex(fromVertexId, out latitudeFrom, out longitudeFrom) &&
                                graph.GetVertex(toVertexId, out latitudeTo, out longitudeTo))
                            { // calculate distance.
                                distance = (new GeoCoordinate(latitudeFrom, longitudeFrom)).DistanceReal(
                                    new GeoCoordinate(latitudeTo, longitudeTo)).Value;
                            }
                        }

                        // add intermediates.
                        var intermediates = new List<GeoCoordinateSimple>();
                        for (int idx = 1; idx < lineString.Coordinates.Length - 1; idx++)
                        {
                            intermediates.Add(new GeoCoordinateSimple()
                                {
                                    Latitude = (float)lineString.Coordinates[idx].Y,
                                    Longitude = (float)lineString.Coordinates[idx].X
                                });
                        }

                        // get meta-tags.
                        var tags = reader.GetMetaTags();

                        tags.RemoveAll(x => !interpreter.IsRelevant(x.Key));
                        // TODO: augment tags?

                        // add the edge.
                        graph.AddArc(fromVertexId, toVertexId, new LiveEdge()
                            {
                                Coordinates = intermediates.ToArray(),
                                Distance = (float)distance,
                                Forward = true,
                                Tags = tagsIndex.Add(tags)
                            }, null);
                        graph.AddArc(toVertexId, fromVertexId, new LiveEdge()
                            {
                                Coordinates = intermediates.ToArray(),
                                Distance = (float)distance,
                                Forward = false,
                                Tags = tagsIndex.Add(tags)
                            }, null);
                    }

                    // report progress.
                    float progress = (float)System.Math.Round((((double)current / (double)reader.RecordCount) * 100));
                    current++;
                    if (progress != latestProgress)
                    {
                        OsmSharp.Logging.Log.TraceEvent("ShapefileLiveGraphReader", TraceEventType.Information,
                            "Reading edges {1}/{2}... {0}%", progress, readerIdx + 1, readers.Count);
                        latestProgress = progress;
                    }
                }
            }
            return graph;
        }
    }
}