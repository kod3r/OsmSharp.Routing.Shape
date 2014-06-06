using OsmSharp.Collections.Tags;
using OsmSharp.Routing.Graph.Router;
using OsmSharp.Routing.Osm.Graphs;
using OsmSharp.Service.Routing;
using System.Configuration;
using System.IO;

namespace OsmSharp.Routing.Shape.Service.Routing.Plugin
{
    /// <summary>
    /// A router factory to create a custom router.
    /// </summary>
    public class RouterFactory : RouterFactoryBase
    {
        /// <summary>
        /// Holds the routing graph.
        /// </summary>
        private IBasicRouterDataSource<LiveEdge> _data;

        /// <summary>
        /// Creates a new router.
        /// </summary>
        /// <returns></returns>
        public override Router CreateRouter()
        {
            return Router.CreateLiveFrom(_data, new OsmSharp.Routing.Shape.ShapefileRoutingInterpreter());
        }

        /// <summary>
        /// Initializes this router factory.
        /// </summary>
        public override void Initialize()
        {
            new OsmSharp.Routing.Shape.Vehicles.Car(
                ConfigurationManager.AppSettings["onewaykey"].ToString(),
                ConfigurationManager.AppSettings["forwardkey"].ToString(),
                ConfigurationManager.AppSettings["backwardkey"].ToString(),
                ConfigurationManager.AppSettings["speedkey"].ToString()).Register();

            TagsCollectionBase metaTags;
            var serializer = new OsmSharp.Routing.Osm.Graphs.Serialization.LiveEdgeFlatfileSerializer();
            _data = serializer.Deserialize(
                new FileInfo(ConfigurationManager.AppSettings["graph.simple.flat"].ToString()).OpenRead(), out metaTags, false);
        }

        /// <summary>
        /// Returns true if this router factory is ready.
        /// </summary>
        public override bool IsReady
        {
            get
            {
                return _data != null;
            }
        }
    }
}