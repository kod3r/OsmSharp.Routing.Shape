using OsmSharp.Routing.Shape.Readers;
using System;
using System.Collections.Generic;
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

            var graphReader = new ShapefileLiveGraphReader("F_JNCTID", "T_JNCTID", "METERS", 1);
            var graph = graphReader.Read(@"C:\temp\tomtom\bel10000", "*nw.shp");


        }
    }
}
