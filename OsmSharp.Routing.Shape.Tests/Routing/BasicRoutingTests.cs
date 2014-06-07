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

using NUnit.Framework;
using OsmSharp.Collections.Tags;
using OsmSharp.Math.Geo;
using OsmSharp.Routing.Graph;
using OsmSharp.Routing.Osm.Graphs;

namespace OsmSharp.Routing.Shape.Tests.Routing
{
    /// <summary>
    /// Contains the most basic of routing tests.
    /// </summar>
    [TestFixture]
    public class BasicRoutingTests
    {
        /// <summary>
        /// Basic point-to-point routing test (no restrictions, vehicle type car).
        /// </summary>
        [Test]
        public void TestRoutingI()
        {
            var epsilon = 0.0001;

            // define basic graph with two points.
            var coord1 = new GeoCoordinate(51.26369, 4.78525);
            var coord2 = new GeoCoordinate(51.26495, 4.78522);

            // do route calculation.
            var route = this.DoCalculation(coord1, coord2, new TagsCollection(
                Tag.Create("FC", "5"),
                Tag.Create("KPH", "50")), true);

            // check route.
            Assert.IsNotNull(route);
            Assert.IsNotNull(route.Entries);
            Assert.AreEqual(2, route.Entries.Length);
            Assert.AreEqual(route.Entries[0].Latitude, coord1.Latitude, epsilon);
            Assert.AreEqual(route.Entries[0].Longitude, coord1.Longitude, epsilon);
            Assert.AreEqual(route.Entries[1].Latitude, coord2.Latitude, epsilon);
            Assert.AreEqual(route.Entries[1].Longitude, coord2.Longitude, epsilon);

            // do route calculation.
            route = this.DoCalculation(coord1, coord2, new TagsCollection(
                Tag.Create("FC", "5"),
                Tag.Create("KPH", "50")), false);

            // check route.
            Assert.IsNotNull(route);
            Assert.IsNotNull(route.Entries);
            Assert.AreEqual(2, route.Entries.Length);
            Assert.AreEqual(route.Entries[0].Latitude, coord1.Latitude, epsilon);
            Assert.AreEqual(route.Entries[0].Longitude, coord1.Longitude, epsilon);
            Assert.AreEqual(route.Entries[1].Latitude, coord2.Latitude, epsilon);
            Assert.AreEqual(route.Entries[1].Longitude, coord2.Longitude, epsilon);
        }

        /// <summary>
        /// Basic point-to-point routing test (oneway, vehicle type car).
        /// </summary>
        [Test]
        public void TestRoutingIOneway()
        {
            // test for:
            // - ft: meaning 'open in positive direction'.
            // - tf: meaning 'open in negative direction'.
            // - n: meaning 'closed in both directions'.

            var epsilon = 0.0001;

            // define basic graph with two points.
            var coord1 = new GeoCoordinate(51.26369, 4.78525);
            var coord2 = new GeoCoordinate(51.26495, 4.78522);

            // do route calculation with 'ft' forward.
            var route = this.DoCalculation(coord1, coord2, new TagsCollection(
                Tag.Create("FC", "5"),
                Tag.Create("KPH", "50"),
                Tag.Create("ONEWAY", "FT")), true);

            // check route.
            Assert.IsNotNull(route);
            Assert.IsNotNull(route.Entries);
            Assert.AreEqual(2, route.Entries.Length);
            Assert.AreEqual(route.Entries[0].Latitude, coord1.Latitude, epsilon);
            Assert.AreEqual(route.Entries[0].Longitude, coord1.Longitude, epsilon);
            Assert.AreEqual(route.Entries[1].Latitude, coord2.Latitude, epsilon);
            Assert.AreEqual(route.Entries[1].Longitude, coord2.Longitude, epsilon);

            // do route calculation with 'ft' backward.
            route = this.DoCalculation(coord1, coord2, new TagsCollection(
                Tag.Create("FC", "5"),
                Tag.Create("KPH", "50"),
                Tag.Create("ONEWAY", "FT")), false);

            // check route.
            Assert.IsNull(route);

            // do route calculation with 'tf' forward.
            route = this.DoCalculation(coord1, coord2, new TagsCollection(
                Tag.Create("FC", "5"),
                Tag.Create("KPH", "50"),
                Tag.Create("ONEWAY", "TF")), true);

            // check route.
            Assert.IsNull(route);

            // do route calculation with 'tf' backward.
            route = this.DoCalculation(coord1, coord2, new TagsCollection(
                Tag.Create("FC", "5"),
                Tag.Create("KPH", "50"),
                Tag.Create("ONEWAY", "TF")), false);

            // check route.
            Assert.IsNotNull(route);
            Assert.IsNotNull(route.Entries);
            Assert.AreEqual(2, route.Entries.Length);
            Assert.AreEqual(route.Entries[0].Latitude, coord1.Latitude, epsilon);
            Assert.AreEqual(route.Entries[0].Longitude, coord1.Longitude, epsilon);
            Assert.AreEqual(route.Entries[1].Latitude, coord2.Latitude, epsilon);
            Assert.AreEqual(route.Entries[1].Longitude, coord2.Longitude, epsilon);

            // do route calculation with 'n'.
            // edges with 'n' should not be added to the network! make sure of that by testing reader and preprocessor.
        }

        /// <summary>
        /// Does a point-to-point calculations for a given tags collection.
        /// </summary>
        /// <param name="coord1"></param>
        /// <param name="coord2"></param>
        /// <param name="tags"></param>
        /// <param name="forward"></param>
        private Route DoCalculation(GeoCoordinate coord1, GeoCoordinate coord2, TagsCollection tags, bool forward)
        {
            var tagsIndex = new OsmSharp.Collections.Tags.Index.TagsTableCollectionIndex();
            var graph = new DynamicGraphRouterDataSource<LiveEdge>(new MemoryDynamicGraph<LiveEdge>(), tagsIndex);

            var vertex1 = graph.AddVertex((float)coord1.Latitude, (float)coord1.Longitude);
            var vertex2 = graph.AddVertex((float)coord2.Latitude, (float)coord2.Longitude);
            var edge1 = new LiveEdge()
            {
                Coordinates = null,
                Distance = (float)coord1.DistanceReal(coord2).Value,
                Forward = forward,
                Tags = tagsIndex.Add(tags)
            };
            graph.AddArc(vertex1, vertex2, edge1, null);

            // do routing.
            var vehicle = new OsmSharp.Routing.Shape.Vehicles.Car("ONEWAY", "FT", "TF", "KPH");
            var router = Router.CreateLiveFrom(graph,
                new ShapefileRoutingInterpreter());
            var routerPoint1 = router.Resolve(vehicle, coord1);
            var routerPoint2 = router.Resolve(vehicle, coord2);
            return router.Calculate(vehicle, routerPoint1, routerPoint2);
        }
    }
}