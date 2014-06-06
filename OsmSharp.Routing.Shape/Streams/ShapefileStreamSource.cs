using NetTopologySuite;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using OsmSharp.Collections.Tags;
using OsmSharp.Osm;
using OsmSharp.Routing.Shape;
using System;
using System.Collections.Generic;
using System.IO;

namespace OsmSharp.Routing.Shape.Streams
{
    /// <summary>
    /// A stream source that converts the shapefile source data into the OSM data model.
    /// </summary>
    public class ShapefileStreamSource : OsmSharp.Osm.Streams.OsmStreamSource
    {
        /// <summary>
        /// Holds the shapefile datareader.
        /// </summary>
        private List<ShapefileDataReader> _nwReaders;

        /// <summary>
        /// Holds the current reader.
        /// </summary>
        private int _currentReader = -1;

        /// <summary>
        /// Holds a set of already processed nodes.
        /// </summary>
        private HashSet<long> _processedNodes = null;

        /// <summary>
        /// Holds a set of intermediates.
        /// </summary>
        private Dictionary<long, List<long>> _intermediates;

        /// <summary>
        /// Holds the next intermediate id.
        /// </summary>
        private long _nextIntermediateId = -1;

        /// <summary>
        /// Holds the current type that is being processed.
        /// </summary>
        private OsmGeoType _currentType;

        /// <summary>
        /// Holds already known next objects.
        /// </summary>
        private Queue<OsmGeo> _queue;

        /// <summary>
        /// Holds the current geometry.
        /// </summary>
        private OsmGeo _current;

        /// <summary>
        /// Holds a dictionary of the nwHeader definition.
        /// </summary>
        private Dictionary<string, int> _nwHeader;

        /// <summary>
        /// Holds the path.
        /// </summary>
        private string _path;

        /// <summary>
        /// Creates a new shapefile stream source.
        /// </summary>
        /// <param name="path">The path to the folder containing the NW (Network Features) Shapefile.</param>
        public ShapefileStreamSource(string path)
        {
            _processedNodes = new HashSet<long>();
            _queue = new Queue<OsmGeo>();
            _intermediates = new Dictionary<long, List<long>>();

            _nwHeader = new Dictionary<string, int>();
            _nwReaders = new List<ShapefileDataReader>();
            _path = path;
        }

        /// <summary>
        /// Returns true if this source can be reset.
        /// </summary>
        public override bool CanReset
        {
            get { return true; }
        }

        /// <summary>
        /// Returns the current object.
        /// </summary>
        /// <returns></returns>
        public override OsmGeo Current()
        {
            return _current;
        }

        /// <summary>
        /// Initializes this source.
        /// </summary>
        public override void Initialize()
        { // in this case resetting is the same as initializing!
            this.Reset();
        }

        /// <summary>
        /// Move to the next item in the stream.
        /// </summary>
        /// <param name="ignoreNodes">Makes this source skip all nodes.</param>
        /// <param name="ignoreWays">Makes this source skip all ways.</param>
        /// <param name="ignoreRelations">Makes this source skip all relations.</param>
        /// <returns></returns>
        public override bool MoveNext(bool ignoreNodes, bool ignoreWays, bool ignoreRelations)
        {
            while (this.DoMoveNext())
            {
                if (this.Current().Type == OsmGeoType.Node &&
                    !ignoreNodes)
                { // there is a node and it is not to be ignored.
                    return true;
                }
                else if (this.Current().Type == OsmGeoType.Way &&
                        !ignoreWays)
                { // there is a way and it is not to be ignored.
                    return true;
                }
                else if (this.Current().Type == OsmGeoType.Relation &&
                        !ignoreRelations)
                { // there is a relation and it is not to be ignored.
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Executes the actual move to the next object without ignoring objects.
        /// </summary>
        /// <returns></returns>
        private bool DoMoveNext()
        {
            if (_queue.Count > 0)
            {  // make sure the next object is used first.
                _current = _queue.Dequeue();
                return true;
            }

            if (_currentType == OsmGeoType.Node)
            { // keep on searching for nodes.
                while (_queue.Count == 0)
                { // keep trying until some non-processed node was found.
                    if (!_nwReaders[0].Read())
                    { // stream has ended, no more data, move to ways.
                        _currentType = OsmGeoType.Way;
                        _nwReaders[_currentReader].Reset();
                        return this.DoMoveNext();
                    }

                    // make sure the header is loaded.
                    if (_nwHeader.Count == 0)
                    { // build header.
                        for (int idx = 0; idx < _nwReaders[_currentReader].DbaseHeader.Fields.Length; idx++)
                        {
                            _nwHeader.Add(_nwReaders[_currentReader].DbaseHeader.Fields[idx].Name, idx);
                        }
                    }

                    // get the geometry.
                    var lineString = _nwReaders[_currentReader].Geometry as LineString;

                    // read the edge id.
                    long edgeId = _nwReaders[_currentReader].GetInt64(_nwHeader["ID"]);

                    // read nodes
                    long fromId = _nwReaders[_currentReader].GetInt64(_nwHeader["F_JNCTID"]);
                    if (!_processedNodes.Contains(fromId))
                    { // the node has not been processed yet.
                        _processedNodes.Add(fromId);

                        var node = new Node();
                        node.Id = fromId;
                        node.Latitude = lineString.Coordinates[0].Y;
                        node.Longitude = lineString.Coordinates[0].X;
                        node.TimeStamp = DateTime.Now;
                        node.Tags = null;
                        node.UserId = null;
                        node.UserName = "shapefile";
                        node.Version = 1;
                        node.Visible = true;
                        _queue.Enqueue(node);
                    }

                    // add the intermediates to the queue.
                    if (lineString.Coordinates.Length > 2)
                    { // there are intermediates.
                        var intermediates = new List<long>();
                        for (int idx = 1; idx < lineString.Coordinates.Length - 1; idx++)
                        {
                            var node = new Node();
                            node.Id = _nextIntermediateId;
                            node.Latitude = lineString.Coordinates[idx].Y;
                            node.Longitude = lineString.Coordinates[idx].X;
                            node.TimeStamp = DateTime.Now;
                            node.Tags = null;
                            node.UserId = null;
                            node.UserName = "shapefile";
                            node.Version = 1;
                            node.Visible = true;
                            _queue.Enqueue(node);

                            intermediates.Add(_nextIntermediateId);
                            _nextIntermediateId--;
                        }

                        _intermediates.Add(edgeId, intermediates);
                    }

                    long toId = _nwReaders[_currentReader].GetInt64(_nwHeader["T_JNCTID"]);
                    if (!_processedNodes.Contains(toId))
                    { // the node has not been processed yet.
                        _processedNodes.Add(toId);

                        var node = new Node();
                        node.Id = toId;
                        node.Latitude = lineString.Coordinates[lineString.Coordinates.Length - 1].Y;
                        node.Longitude = lineString.Coordinates[lineString.Coordinates.Length - 1].X;
                        node.TimeStamp = DateTime.Now;
                        node.Tags = null;
                        node.UserId = null;
                        node.UserName = "shapefile";
                        node.Version = 1;
                        node.Visible = true;
                        _queue.Enqueue(node);
                    }
                }
                _current = _queue.Dequeue();
                return true;
            }
            else
            { // process ways.
                if (!_nwReaders[_currentReader].Read())
                { // stream has ended, no more data.
                    // move to next reader.
                    _currentReader++;
                    if(_currentReader < _nwReaders.Count)
                    { // still readers left.
                        _currentType = OsmGeoType.Node;
                        _nwHeader.Clear();
                    }
                    return false;
                }

                // get the geometry.
                var lineString = _nwReaders[_currentReader].Geometry as LineString;

                // build nodes list.
                long edgeId = _nwReaders[_currentReader].GetInt64(_nwHeader["ID"]);
                var nodesList = new List<long>();
                nodesList.Add(_nwReaders[_currentReader].GetInt64(_nwHeader["F_JNCTID"]));
                var intermediates = new List<long>();
                if(_intermediates.TryGetValue(edgeId, out intermediates))
                { // add intermediate nodes.
                    nodesList.AddRange(intermediates);
                    _intermediates.Remove(edgeId);
                }
                nodesList.Add(_nwReaders[_currentReader].GetInt64(_nwHeader["T_JNCTID"]));

                // create the way.
                var way = new Way();
                way.Id = edgeId;
                way.Nodes = nodesList;
                way.Tags = _nwReaders[_currentReader].GetMetaTags();
                way.TimeStamp = DateTime.Now;
                way.UserId = null;
                way.UserName = "shapefile";
                way.Version = 1;
                way.Visible = true;

                _current = way;
                return true;
            }
        }

        /// <summary>
        /// Returns true if this source is sorted.
        /// </summary>
        public override bool IsSorted
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Resets this source.
        /// </summary>
        public override void Reset()
        {
            // reset shapefile reader.
            foreach (var nwReader in _nwReaders)
            {
                nwReader.Close();
            }
            _nwReaders.Clear();
            _nwHeader.Clear();
            _intermediates.Clear();

            // reset all status info.
            _currentReader = -1;
            _current = null;
            _currentType = OsmGeoType.Node;
            _processedNodes.Clear();
            _queue.Clear();

            // create readers.
            var directoryInfo = new DirectoryInfo(_path);
            var nwFiles = directoryInfo.EnumerateFiles("*nw.shp", SearchOption.AllDirectories);
            var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory();
            foreach (var nwFile in nwFiles)
            {
                _nwReaders.Add(new ShapefileDataReader(nwFile.FullName, geometryFactory));
            }
            _currentReader = 0;
        }
    }
}