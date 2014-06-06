using OsmSharp.Collections.Tags;

namespace OsmSharp.Routing.Shape.Vehicles
{
    /// <summary>
    /// Represents a custom-vehicle.
    /// </summary>
    public abstract class Vehicle : OsmSharp.Routing.Vehicle
    {
        /// <summary>
        /// Holds the key containing the oneway information.
        /// </summary>
        private string _onewayKey;

        /// <summary>
        /// Holds the value of a forward-only oneway edge.
        /// </summary>
        private string _forwardValue;

        /// <summary>
        /// Holds the value of a backward-only oneway edge.
        /// </summary>
        private string _backwardValue;

        /// <summary>
        /// Creates a new shapefile vehicle but without any oneway restrictions.
        /// </summary>
        public Vehicle()
        {
            _onewayKey = string.Empty;
        }

        /// <summary>
        /// Creates a new shapefile vehicle.
        /// </summary>
        /// <param name="onewayKey">The key containing the oneway information. ex: "ONEWAY"</param>
        /// <param name="forwardValue">The value of a forward-only oneway edge. ex: "ft"</param>
        /// <param name="backwardValue">The value of a backward-only oneway edge. ex: "tf"</param>
        public Vehicle(string onewayKey, string forwardValue, string backwardValue)
        {
            _onewayKey = onewayKey;
            _forwardValue = forwardValue;
            _backwardValue = backwardValue;
        }

        /// <summary>
        /// Returns true if the edge with given properties can be traversed by this vehicle.
        /// </summary>
        /// <param name="tags"></param>
        /// <returns></returns>
        public abstract override bool CanTraverse(TagsCollectionBase tags);

        /// <summary>
        /// Returns true if the edge with given properties is one way, null if both, false if one way reverse.
        /// </summary>
        /// <param name="tags"></param>
        /// <returns></returns>
        public override bool? IsOneWay(TagsCollectionBase tags)
        {
            if (_onewayKey != null && _onewayKey.Length > 0)
            {
                string value;
                if (tags.TryGetValue(_onewayKey, out value))
                { // there is a oneway tags.
                    if (value != null)
                    {
                        if (value.Equals(_forwardValue))
                        {
                            return true;
                        }
                        if (value.Equals(_backwardValue))
                        {
                            return false;
                        }
                    }
                }
            }
            return null;
        }
    }
}