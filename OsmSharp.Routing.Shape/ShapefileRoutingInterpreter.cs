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

using OsmSharp.Routing.Constraints;
using OsmSharp.Routing.Interpreter;
using OsmSharp.Routing.Interpreter.Roads;
using System.Collections.Generic;

namespace OsmSharp.Routing.Shape
{
    /// <summary>
    /// Represents a shapefile routing interpreter.
    /// </summary>
    public class ShapefileRoutingInterpreter : IRoutingInterpreter
    {
        /// <summary>
        /// Holds all useful keys.
        /// </summary>
        private HashSet<string> _usefulKeys;

        /// <summary>
        /// Creates a new shapefile routing interpreter.
        /// </summary>
        public ShapefileRoutingInterpreter()
        {
            _usefulKeys = new HashSet<string>();
        }
        
        /// <summary>
        /// Creates a new shapefile routing interpreter.
        /// </summary>
        /// <param name="usefulKeys"></param>
        public ShapefileRoutingInterpreter(HashSet<string> usefulKeys)
        {
            _usefulKeys = usefulKeys;
        }

        /// <summary>
        /// Returns true if the given sequence of vertices can be traversed.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="along"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public bool CanBeTraversed(long from, long along, long to)
        {
            return true;
        }

        /// <summary>
        /// Returns the contraints.
        /// </summary>
        public IRoutingConstraints Constraints
        {
            get { return null; }
        }

        /// <summary>
        /// Returns the edge interpreter.
        /// </summary>
        public IEdgeInterpreter EdgeInterpreter
        {
            get { return new ShapefileEdgeInterpreter(); }
        }

        /// <summary>
        /// Returns true if the given key-value pair is relevant.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool IsRelevant(string key, string value)
        {
            return this.IsRelevant(key);
        }

        /// <summary>
        /// Returns true if the given key is relevant.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool IsRelevant(string key)
        {
            if(_usefulKeys.Count > 0)
            { // keep only keys.
                return _usefulKeys.Contains(key);
            }
            return true; // keep all keys.
        }
    }
}