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