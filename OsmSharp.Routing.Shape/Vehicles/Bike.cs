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

using OsmSharp.Collections.Tags;
using OsmSharp.Units.Speed;

namespace OsmSharp.Routing.Shape.Vehicles
{
    /// <summary>
    /// Represents a custom bicycle profile.
    /// </summary>
    public class Bike : Vehicle
    {
        /// <summary>
        /// Creates a new shapefile bike.
        /// </summary>
        /// <param name="onewayKey">The key containing the oneway information. ex: "ONEWAY"</param>
        /// <param name="forwardValue">The value of a forward-only oneway edge. ex: "ft"</param>
        /// <param name="backwardValue">The value of a backward-only oneway edge. ex: "tf"</param>
        public Bike(string onewayKey, string forwardValue, string backwardValue)
            : base(onewayKey, forwardValue, backwardValue)
        {

        }

        /// <summary>
        /// Returns true if the edge with given properties can be traversed by this vehicle.
        /// </summary>
        /// <param name="tags"></param>
        /// <returns></returns>
        public override bool CanTraverse(TagsCollectionBase tags)
        {
            return true;
        }

        /// <summary>
        /// Returns true if the vehicle is allowed on the given edge.
        /// </summary>
        /// <param name="tags"></param>
        /// <param name="highwayType"></param>
        /// <returns></returns>
        protected override bool IsVehicleAllowed(TagsCollectionBase tags, string highwayType)
        {
            return true;
        }

        /// <summary>
        /// Returns the maximum speed.
        /// </summary>
        /// <returns></returns>
        protected override KilometerPerHour MaxSpeed()
        {
            return 30;
        }

        /// <summary>
        /// Returns the maximum speed for given highway-type.
        /// </summary>
        /// <param name="highwayType"></param>
        /// <returns></returns>
        protected override KilometerPerHour MaxSpeedAllowed(string highwayType)
        {
            return 30;
        }

        /// <summary>
        /// Returns the unique name of this profile.
        /// </summary>
        public override string UniqueName
        {
            get { return "Shape.Bike"; }
        }
    }
}