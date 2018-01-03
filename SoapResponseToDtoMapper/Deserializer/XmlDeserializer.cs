using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace SoapResponseDtoMapper.Deserializer
{
    public class XmlDeserializer : IXmlDeserializer
    {
        /// <summary>
        /// Get a dictionary of key/value pairs based on XML string returned from SOAP service
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="stringXml"></param>
        /// <returns></returns>
        public Dictionary<string, string> DeserializeXmlStringToDictionary(IEnumerable<string> filter, string stringXml)
        {
            // Don't bother running code if there is no properties
            if (string.IsNullOrEmpty(stringXml)) return null;
            try
            {
                var iEnumerableOfXelements = GetIEnumerableOfXmlData(filter, stringXml);
                var listOfXelements = iEnumerableOfXelements.Distinct();

                return listOfXelements.ToDictionary(expr => expr.Name.LocalName, expr => expr.Value);
            }
            catch (XmlException ex)
            {
                throw new Exception("string is not well formed XML", ex);
            }
        }


        /// <summary>
        /// Algorithm for building a list of dictionary values based on valid XML and string filter of properties
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="stringXml"></param>
        /// <returns></returns>
        public List<Dictionary<string, string>> DeserializeXmlStringToDictionaryList(IEnumerable<string> filter, string stringXml)
        {
            // Don't bother running code if there is no properties
            if (stringXml.Length <= 0)
                return null;

            var enumerableFilter = filter as string[] ?? filter.ToArray();

            // get list of data to sort into list of dictionaries
            var listOfXElements = GetIEnumerableOfXmlData(enumerableFilter, stringXml).ToList();

            var dictionaryList = new List<Dictionary<string, string>>();
            var dictionary = new Dictionary<string, string>();

            foreach (var xElement in listOfXElements)
            {
                // if key doesn't exist add it
                if (!dictionary.ContainsKey(xElement.Name.LocalName))
                {
                    dictionary.Add(xElement.Name.LocalName, xElement.Value);
                }
                else
                {
                    // now key exists we know to add dictionary to list and start again
                    var record = dictionary.Take(dictionary.Count)
                        .ToDictionary(expr => expr.Key, expr => expr.Value);

                    // add dupe value to the new dictionary so it doesn't get lost when starting the loop again :D
                    dictionary = new Dictionary<string, string> { { xElement.Name.LocalName, xElement.Value } };


                    // extra check for dupes
                    if (!dictionaryList.Contains(record))
                        dictionaryList.Add(record);
                }
            }

            // if dictionary has values that haven't been added to list - do it now
            if (!dictionary.Any()) return dictionaryList;
            {
                // if list only contains 1 - add that one result
                var record = dictionary.Take(dictionary.Count)
                    .ToDictionary(expr => expr.Key, expr => expr.Value);
                dictionaryList.Add(record);
            }

            return dictionaryList;

        }


        private static IEnumerable<XElement> GetIEnumerableOfXmlData(IEnumerable<string> filter, string stringXml)
        {
            try
            {
                // check if valid document and parse it at same times
                var xDocumentResult = XDocument.Parse(stringXml);
                // get list of elements
                var xElementList = xDocumentResult.Descendants().ToList();

                // remove unwanted property characters so mapper works
                var cleanElementList = (from xElement in xElementList
                                        let replace = xElement.Name.LocalName.Replace("_", "").Replace("-", "")
                                        // regex might be better for this
                                        select new XElement(replace) { Value = xElement.Value })
                    .ToList();


                // map elements to dictionary - ignore case
                return cleanElementList
                    .Where(expr => filter.Contains(expr.Name.LocalName, StringComparer.InvariantCultureIgnoreCase));
            }
            catch (XmlException ex)
            {
                throw new Exception("string is not well formed XML", ex);
            }


        }
    }
}
