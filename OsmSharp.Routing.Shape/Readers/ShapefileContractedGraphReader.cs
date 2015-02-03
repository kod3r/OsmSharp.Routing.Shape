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

using OsmSharp.Collections.Tags;
using OsmSharp.Collections.Tags.Index;
using OsmSharp.Math.Geo.Simple;
using OsmSharp.Routing.CH.PreProcessing;
using OsmSharp.Routing.Graph;
using System.Collections.Generic;

namespace OsmSharp.Routing.Shape.Readers
{
    /// <summary>
    /// A reader to read a shapefile network directly into a live data source.
    /// </summary>
    public class ShapefileContractedGraphReader : ShapefileGraphReader<CHEdgeData>
    {
        /// <summary>
        /// Holds the vehicle profile.
        /// </summary>
        private Vehicle _vehicle;

        /// <summary>
        /// Creates a new shapefile graph reader.
        /// </summary>
        /// <param name="vehicle">The vehicle profile to build the graph for.</param>
        /// <param name="nodeFromColumn">The column containing the from node. ex: "JTE_ID_BEG"</param>
        /// <param name="nodeToColumn">The column containing the to node. ex: "JTE_ID_END"</param>
        public ShapefileContractedGraphReader(Vehicle vehicle, string nodeFromColumn, string nodeToColumn)
            : base(nodeFromColumn, nodeToColumn, false)
        {
            _vehicle = vehicle;
        }

        /// <summary>
        /// Creates a new shapefile graph reader.
        /// </summary>
        /// <param name="vehicle">The vehicle profile to build the graph for.</param>
        /// <param name="nodeFromColumn">The column containing the from node. ex: "JTE_ID_BEG"</param>
        /// <param name="nodeToColumn">The column containing the to node. ex: "JTE_ID_END"</param>
        /// <param name="distanceColumn">The column containg the distance. ex: "METERS"</param>
        /// <param name="distanceFactor">The column containing the factor to convert the distance to meter (1=meter, 0.001=km).</param>
        public ShapefileContractedGraphReader(Vehicle vehicle, string nodeFromColumn, string nodeToColumn, string distanceColumn, float distanceFactor)
            : base(nodeFromColumn, nodeToColumn, distanceColumn, distanceFactor, false)
        {
            _vehicle = vehicle;
        }

        /// <summary>
        /// Creates a new graph.
        /// </summary>
        /// <param name="tagsCollection"></param>
        /// <returns></returns>
        protected override DynamicGraphRouterDataSource<CHEdgeData> CreateGraph(ITagsCollectionIndexReadonly tagsCollection)
        {
            return new DynamicGraphRouterDataSource<CHEdgeData>(tagsCollection);
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
        /// <param name="weight"></param>
        protected override void AddEdge(DynamicGraphRouterDataSource<CHEdgeData> graph, ITagsCollectionIndex tagsIndex, uint vertex1, uint vertex2,
            List<GeoCoordinateSimple> intermediates, TagsCollectionBase tags, double weight)
        {
            bool? direction = _vehicle.IsOneWay(tags);
            bool forward = false;
            bool backward = false;
            if (!direction.HasValue)
            { // both directions.
                forward = true;
                backward = true;
            }
            else
            { // define back/forward.
                forward = direction.Value;
                backward = !direction.Value;
            }

            // add tags.
            var tagsId = tagsIndex.Add(tags);

            // initialize the edge data.
            var edgeData = new CHEdgeData()
            {
                TagsForward = true,
                Tags = tagsId,
                BackwardWeight = backward ? (float)weight : float.MaxValue,
                BackwardContractedId = 0,
                ForwardWeight = forward ? (float)weight : float.MaxValue,
                ForwardContractedId = 0
            };
            edgeData.SetContractedDirection(false, false);
            graph.AddEdge(vertex1, vertex2, edgeData);
        }
    }
}
