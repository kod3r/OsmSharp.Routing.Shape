using OsmSharp.Collections.Tags;
using OsmSharp.Routing.Interpreter.Roads;
using System.Collections.Generic;

namespace OsmSharp.Routing.Shape
{
    /// <summary>
    /// Represents a custom edge interpreter.
    /// </summary>
    public class ShapefileEdgeInterpreter : IEdgeInterpreter
    {
        /// <summary>
        /// Returns true if the edge with the given tags is only accessible locally.
        /// </summary>
        /// <param name="tags"></param>
        /// <returns></returns>
        public bool IsOnlyLocalAccessible(TagsCollectionBase tags)
        {
            return false;
        }

        /// <summary>
        /// Returns true if the edge with the given tags is routable.
        /// </summary>
        /// <param name="tags"></param>
        /// <returns></returns>
        public bool IsRoutable(TagsCollectionBase tags)
        {
            return true;
        }

        /// <summary>
        /// Returns the name of a given way.
        /// </summary>
        /// <param name="tags"></param>
        /// <returns></returns>
        public string GetName(TagsCollectionBase tags)
        {
            var name = string.Empty;
            if (tags.ContainsKey("name"))
            {
                name = tags["name"];
            }
            else if (tags.ContainsKey("NAME"))
            {
                name = tags["NAME"];
            }
            return name;
        }

        /// <summary>
        ///     Returns all the names in all languages and alternatives.
        /// </summary>
        /// <param name="tags"></param>
        /// <returns></returns>
        public Dictionary<string, string> GetNamesInAllLanguages(TagsCollectionBase tags)
        {
            var names = new Dictionary<string, string>();
            return names;
        }

        /// <summary>
        ///     Returns true if the edge with the given properties represents a roundabout.
        /// </summary>
        /// <param name="tags"></param>
        /// <returns></returns>
        public bool IsRoundabout(TagsCollectionBase tags)
        { // TODO: figure out why this is here and find a way to remove this.
            return false;
        }

        /// <summary>
        /// Returns true if the edge with given tags can be traversed by the given vehicle.
        /// </summary>
        /// <param name="tags"></param>
        /// <param name="vehicle"></param>
        /// <returns></returns>
        public bool CanBeTraversedBy(TagsCollectionBase tags, OsmSharp.Routing.Vehicle vehicle)
        {
            return vehicle.CanTraverse(tags);
        }
    }
}