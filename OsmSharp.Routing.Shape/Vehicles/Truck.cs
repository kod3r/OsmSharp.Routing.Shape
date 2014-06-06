using OsmSharp.Collections.Tags;
using OsmSharp.Units.Speed;
using System;

namespace OsmSharp.Routing.Shape.Vehicles
{
    /// <summary>
    /// Represents a truck vehicle profile.
    /// </summary>
    public class Truck : Vehicle
    {
        /// <summary>
        /// Creates a new shapefile truck.
        /// </summary>
        /// <param name="onewayKey">The key containing the oneway information. ex: "ONEWAY"</param>
        /// <param name="forwardValue">The value of a forward-only oneway edge. ex: "ft"</param>
        /// <param name="backwardValue">The value of a backward-only oneway edge. ex: "tf"</param>
        public Truck(string onewayKey, string forwardValue, string backwardValue)
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
        /// Returns true if the given vehicle is allowed for the given highway type.
        /// </summary>
        /// <param name="tags"></param>
        /// <param name="highwayType"></param>
        /// <returns></returns>
        protected override bool IsVehicleAllowed(TagsCollectionBase tags, string highwayType)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the maximum speed for this vehicle.
        /// </summary>
        /// <returns></returns>
        protected override KilometerPerHour MaxSpeed()
        {
            return 120;
        }

        /// <summary>
        /// Returns the maximum speed for given highway-type.
        /// </summary>
        /// <param name="highwayType"></param>
        /// <returns></returns>
        protected override KilometerPerHour MaxSpeedAllowed(string highwayType)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the unique name of this profile.
        /// </summary>
        public override string UniqueName
        {
            get { return "Shape.Truck"; }
        }
    }
}