using OsmSharp.Collections.Tags;
using OsmSharp.Units.Speed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
