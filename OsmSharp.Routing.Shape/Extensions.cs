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

using NetTopologySuite.IO;
using OsmSharp.Collections.Tags;
using System;

namespace OsmSharp.Routing.Shape
{
    /// <summary>
    /// Contains extension methods.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Converts the current fields and data into a tags collection.
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static TagsCollectionBase GetMetaTags(this ShapefileDataReader reader)
        {
            var tagsCollection = new TagsCollection();
            foreach (var field in reader.DbaseHeader.Fields)
            {
                string valueString = string.Empty;
                object value = reader[field.Name];
                if (value != null)
                { // TODO: make sure this is culture-invariant!
                    valueString = value.ToString();
                }
                tagsCollection.Add(field.Name, valueString);
            }
            return tagsCollection;
        }

        /// <summary>
        /// Converts the current fields and data into a tags collection.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="keepKeys"></param>
        /// <returns></returns>
        public static TagsCollectionBase GetMetaTags(this ShapefileDataReader reader, Func<string, bool> keepKeys)
        {
            var tagsCollection = new TagsCollection();
            foreach (var field in reader.DbaseHeader.Fields)
            {
                if (keepKeys.Invoke(field.Name))
                {
                    string valueString = string.Empty;
                    object value = reader[field.Name];
                    if (value != null)
                    { // TODO: make sure this is culture-invariant!
                        valueString = value.ToString();
                    }
                    tagsCollection.Add(field.Name, valueString);
                }
            }
            return tagsCollection;
        }
    }
}