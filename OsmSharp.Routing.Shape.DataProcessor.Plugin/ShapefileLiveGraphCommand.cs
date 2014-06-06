using OsmSharpDataProcessor;
using OsmSharpDataProcessor.Commands;
using OsmSharpDataProcessor.Commands.Processors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsmSharp.Routing.Shape.DataProcessor.Plugin
{
    /// <summary>
    /// The write-redis command.
    /// </summary>
    public class ShapefileLiveGraphCommand : Command
    {
        /// <summary>
        /// Returns the switches for this command.
        /// </summary>
        /// <returns></returns>
        public override string[] GetSwitch()
        {
            return new string[] { "--create-graph-shape" };
        }

        /// <summary>
        /// Creates a new write-redis command.
        /// </summary>
        public ShapefileLiveGraphCommand()
        {

        }

        /// <summary>
        /// The path to the shapefiles.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// The search pattern.
        /// </summary>
        public string SearchPattern { get; set; }

        /// <summary>
        /// The filename of the output graph.
        /// </summary>
        public string Graph { get; set; }

        /// <summary>
        /// The node from column.
        /// </summary>
        public string NodeFromColumn { get; set; }

        /// <summary>
        /// The node to column.
        /// </summary>
        public string NodeToColumn { get; set; }

        /// <summary>
        /// The distance column.
        /// </summary>
        public string DistanceColumn { get; set; }

        /// <summary>
        /// The distance factor.
        /// </summary>
        public float DistanceFactor { get; set; }
        
        /// <summary>
        /// Parse the command arguments for the write-xml command.
        /// </summary>
        /// <param name="args"></param>
        /// <param name="idx"></param>
        /// <param name="command"></param>
        /// <returns></returns>
        public override int Parse(string[] args, int idx, out Command command)
        {
            // check next argument.
            if (args.Length < idx)
            {
                throw new CommandLineParserException("None", "Invalid file name for write-xml command!");
            }

            // everything ok, take the next argument as the filename.
            var routingShapeCommand = new ShapefileLiveGraphCommand();

            // parse arguments and keep parsing until the next switch.
            int startIdx = idx;
            while (args.Length > idx &&
                !CommandParser.IsSwitch(args[idx]))
            {
                string[] keyValue;
                if (CommandParser.SplitKeyValue(args[idx], out keyValue))
                { // the command splitting succeeded.
                    // set defaults.
                    routingShapeCommand.SearchPattern = "*nw.shp";
                    routingShapeCommand.DistanceFactor = 1;
                    routingShapeCommand.DistanceColumn = "METER";
                    routingShapeCommand.NodeFromColumn = "F_JNCTID";
                    routingShapeCommand.NodeToColumn = "T_JNCTID";
                    
                    keyValue[0] = CommandParser.RemoveQuotes(keyValue[0]);
                    keyValue[1] = CommandParser.RemoveQuotes(keyValue[1]);
                    switch (keyValue[0].ToLower())
                    {
                        case "graph":
                            routingShapeCommand.Graph = keyValue[1];
                            break;
                        case "path":
                            routingShapeCommand.Path = keyValue[1];
                            break;
                        default:
                            // the command splitting succeed but one of the arguments is unknown.
                            throw new CommandLineParserException("--create-graph-shape",
                                string.Format("Invalid parameter for command --create-graph-shape: {0} not recognized.", keyValue[0]));
                    }
                }
                else
                { // the command splitting failed and this is not a switch.
                    throw new CommandLineParserException("--create-graph-shape", string.Format("Invalid parameter for command --create-graph-shape: {0}", args[idx]));
                }

                idx++; // increase the index.
            }

            // everything ok, take the next argument as the filename.
            command = routingShapeCommand;
            return idx - startIdx;
        }

        /// <summary>
        /// Creates a processor that corresponds to this command.
        /// </summary>
        /// <returns></returns>
        public override ProcessorBase CreateProcessor()
        {
            return new ShapefileLiveGraphProcessor(this.Path, this.Graph, this.SearchPattern, this.NodeFromColumn, this.NodeToColumn, this.DistanceColumn, this.DistanceFactor);
        }

        /// <summary>
        /// Returns a description of this command.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("--create-graph-shape path={0} graph={1} search={2} nodefcol={3} nodetcol={4} distcol={5} distfact={6}",
                this.Path, this.Graph, this.SearchPattern, this.NodeFromColumn, this.NodeToColumn, this.DistanceColumn, this.DistanceFactor);
        }
    }
}