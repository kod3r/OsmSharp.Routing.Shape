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
using OsmSharp.Routing.Osm.Graphs.Serialization;
using OsmSharp.Routing.Shape.Readers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsmSharp.Routing.Shape.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            OsmSharp.Logging.Log.Enable();
            OsmSharp.Logging.Log.RegisterListener(new OsmSharp.WinForms.UI.Logging.ConsoleTraceListener());

            var graphReader = new ShapefileLiveGraphReader("JTE_ID_BEG", "JTE_ID_END");
            var graph = graphReader.Read(@"C:\temp\nwb\Wegvakken", "*nw.shp", new ShapefileRoutingInterpreter());

            var stream = new FileInfo(@"C:\temp\nwb.routing.flatfile").OpenWrite();
            var serializer = new LiveEdgeFlatfileSerializer();
            serializer.Serialize(stream, graph, new TagsCollection());
            stream.Flush();
        }
    }
}
