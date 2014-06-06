using OsmSharp.Routing.Constraints;
using OsmSharp.Routing.Interpreter;
using OsmSharp.Routing.Interpreter.Roads;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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