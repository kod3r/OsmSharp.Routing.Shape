OsmSharp.Routing.Shape
======================

OsmSharp.Routing.Shape is a small module that supports importing a routing network from shapefiles into an OsmSharp instance.

Development was sponsored by via.nl:

VIA.nl
Rembrandterf 1
5261 XS Vught Nederland
http://www.via.nl
@Via_nl

This can used to read the TomTom MultiNet shapefiles and the Dutch Nationaal Wegenbestand (NWB) among others.

How to use?
-----------

This example is how for the Dutch national road registry (Nationaal Wegenbestand (NWB)). You can download the shapefile here:

http://www.jigsaw.nl/nwb/

Unzip the shapefile to \some\folder and convert it to WGS84 using any respectable GIS editing software out there (for example QGIS). Support for non-WGS84 projections is coming, see https://github.com/OsmSharp/OsmSharp.Routing.Shape/issues/4.

You can use the following code to create a routable graph from the shapefiles.

```csharp
// create an instance of the graph reader and define the columns that contain the 'node-ids'.
var graphReader = new ShapefileLiveGraphReader("JTE_ID_BEG", "JTE_ID_END");
// read the graph from the folder where the shapefiles are placed.
var graph = graphReader.Read(@"\some\folder", "WegvakkenWGS84.shp", new ShapefileRoutingInterpreter());

// use the graph here with the routing profiles from OsmSharp.Routing.Shape.
var vehicle = new OsmSharp.Routing.Shape.Vehicles.Car("RIJRICHTING", "H", "T", string.Empty); // define vehicle with the column and values that define the onway restrictions and (optional) the speed in KPH.
var router = Router.CreateLiveFrom(graph,
	new ShapefileRoutingInterpreter());
var routerPoint1 = router.Resolve(vehicle, new GeoCoordinate(52.16024, 4.48415)); // define some coordinate here, this is Leiden, NL.
var routerPoint2 = router.Resolve(vehicle, new GeoCoordinate(51.66028, 5.29190)); // define some coordinate here, this is Vught, NL.
var route = router.Calculate(vehicle, routerPoint1, routerPoint2);

// OPTIONAL: reading the shapefiles can take a while, this serializes the routing graph to disk to load again later in OsmSharp.
var stream = new FileInfo(@"C:\temp\nwb.routing.flatfile").OpenWrite();
var serializer = new LiveEdgeFlatfileSerializer();
serializer.Serialize(stream, graph, new TagsCollection());
stream.Flush();
```