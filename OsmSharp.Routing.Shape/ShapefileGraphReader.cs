using OsmSharp.Routing.Graph;
using OsmSharp.Routing.Shape.Vehicles;

namespace OsmSharp.Routing.Shape
{
    /// <summary>
    /// A reader to read a shapefile network directly into a router data source.
    /// </summary>
    public abstract class ShapefileGraphReader<TEdgeData>
        where TEdgeData : IDynamicGraphEdgeData
    {
        /// <summary>
        /// Holds the node from column.
        /// </summary>
        private string _nodeFromColumn;

        /// <summary>
        /// Holds the node to column.
        /// </summary>
        private string _nodeToColumn;

        /// <summary>
        /// Holds the column with the distance.
        /// </summary>
        private string _distanceColumn;

        /// <summary>
        /// Holds the distance factor relative to meters.
        /// </summary>
        private float _distanceFactor;

        /// <summary>
        /// Creates a new shapefile graph reader.
        /// </summary>
        /// <param name="nodeFromColumn">The column containing the from node. ex: "F_JNCTID"</param>
        /// <param name="nodeToColumn">The column containing the to node. ex: "T_JNCTID"</param>
        protected ShapefileGraphReader(string nodeFromColumn, string nodeToColumn)
        {
            _nodeFromColumn = nodeFromColumn;
            _nodeToColumn = nodeToColumn;

            _distanceColumn = string.Empty;
            _distanceFactor = 0;
        }

        /// <summary>
        /// Creates a new shapefile graph reader.
        /// </summary>
        /// <param name="nodeFromColumn">The column containing the from node. ex: "F_JNCTID"</param>
        /// <param name="nodeToColumn">The column containing the to node. ex: "T_JNCTID"</param>
        /// <param name="distanceColumn">The column containg the distance. ex: "METERS"</param>
        /// <param name="distanceFactor">The column containing the factor to convert the distance to meter (1=meter, 0.001=km).</param>
        protected ShapefileGraphReader(string nodeFromColumn, string nodeToColumn, string distanceColumn, float distanceFactor)
        {
            _nodeFromColumn = nodeFromColumn;
            _nodeToColumn = nodeToColumn;

            _distanceColumn = distanceColumn;
            _distanceFactor = distanceFactor;
        }

        /// <summary>
        /// Gets the node from column.
        /// </summary>
        public string NodeFromColumn
        {
            get
            {
                return _nodeFromColumn;
            }
        }

        /// <summary>
        /// Gets the node to column.
        /// </summary>
        public string NodeToColumn
        {
            get
            {
                return _nodeToColumn;
            }
        }

        /// <summary>
        /// Returns true if a distance column and factor are set.
        /// </summary>
        public bool HasDistanceColumn
        {
            get
            {
                return _distanceColumn != null && _distanceColumn.Length > 0 && _distanceFactor != 0;
            }
        }

        /// <summary>
        /// Gets the distance column.
        /// </summary>
        public string DistanceColumn
        {
            get
            {
                return _distanceColumn;
            }
        }

        /// <summary>
        /// Gets the distance factor.
        /// </summary>
        public float DistanceFactor
        {
            get
            {
                return _distanceFactor;
            }
        }

        /// <summary>
        /// Reads a routing network.
        /// </summary>
        /// <param name="path">The root path to the shapefile(s).</param>
        /// <param name="searchPattern">The search string to identify the shapefile(s) to read.  ex: "*nw.shp"</param>
        /// <returns>The graph that was created from the shapefile(s).</returns>
        public DynamicGraphRouterDataSource<TEdgeData> Read(string path, string searchPattern)
        {
            return this.Read(path, searchPattern, new ShapefileRoutingInterpreter());
        }

        /// <summary>
        /// Reads a routing network according to the given interpreter.
        /// </summary>
        /// <param name="path">The root path to the shapefile(s).</param>
        /// <param name="searchPattern">The search string to identify the shapefile(s) to read.  ex: "*nw.shp"</param>
        /// <param name="interpreter">The shapefile interpreter telling the reader what to import or interpret.</param>
        /// <returns>The graph that was created from the shapefile(s).</returns>
        public abstract DynamicGraphRouterDataSource<TEdgeData> Read(string path, string searchPattern, ShapefileRoutingInterpreter interpreter);
    }
}