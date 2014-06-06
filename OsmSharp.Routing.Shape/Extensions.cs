using NetTopologySuite.IO;
using OsmSharp.Collections.Tags;

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
                tagsCollection.Add(field.Name.ToLowerInvariant(), valueString);
            }
            return tagsCollection;
        }
    }
}
