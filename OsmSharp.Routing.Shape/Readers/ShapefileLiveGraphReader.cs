// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2015 Abelshausen Ben
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
using OsmSharp.Collections.Coordinates.Collections;
using OsmSharp.Collections.Tags.Index;
using OsmSharp.Logging;
using OsmSharp.Math.Geo;
using OsmSharp.Math.Geo.Simple;
using OsmSharp.Routing.Graph;
using OsmSharp.Routing.Osm.Graphs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Collections;
using OsmSharp.Collections.Tags;

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
        /// Creates a new graph.
        /// </summary>
        /// <param name="tagsCollection"></param>
        /// <returns></returns>
        protected override DynamicGraphRouterDataSource<LiveEdge> CreateGraph(ITagsCollectionIndexReadonly tagsCollection)
        {
            return new DynamicGraphRouterDataSource<LiveEdge>(tagsCollection);
        }

        /// <summary>
        /// Adds a new edge.
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="tagsIndex"></param>
        /// <param name="vertex1"></param>
        /// <param name="vertex2"></param>
        /// <param name="intermediates"></param>
        /// <param name="tags"></param>
        /// <param name="distance"></param>
        protected override void AddEdge(DynamicGraphRouterDataSource<LiveEdge> graph, ITagsCollectionIndex tagsIndex, uint vertex1, uint vertex2,
            List<GeoCoordinateSimple> intermediates, TagsCollectionBase tags, double distance)
        {
            // add the edge.
            var intermediatesCollection = new CoordinateArrayCollection<GeoCoordinateSimple>(intermediates.ToArray());
            graph.AddEdge(vertex1, vertex2, new LiveEdge()
            {
                Distance = (float)distance,
                Forward = true,
                Tags = tagsIndex.AddObject(tags)
            }, intermediatesCollection);
        }
    }
}