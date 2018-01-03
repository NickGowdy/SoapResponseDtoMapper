using SoapResponseDtoMapper.Deserializer;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace SoapResponseDtoMapper.Mapper
{
    public class XmlToDomainObject : IXmlToDomainObject
    {

        private readonly IXmlDeserializer _xmlDeserializer;

        public XmlToDomainObject(IXmlDeserializer xmlDeserializer)
        {
            _xmlDeserializer = xmlDeserializer;
        }

        /// <summary>
        /// Map valid XML to domain object using IEnumerable of string properties
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerableStringDomainObjProperties"></param>
        /// <param name="xmlData"></param>
        /// <returns></returns>
        public T MapXmlStringToObject<T>(IEnumerable<string> enumerableStringDomainObjProperties, string xmlData) where T : new()
        {
            // get type using reflection
            var type = typeof(T);
            var ret = new T();

            // Map XML String to dictionary
            var dictionaryData = _xmlDeserializer.DeserializeXmlStringToDictionary(enumerableStringDomainObjProperties, xmlData);
            // set values to domain object based on dictionary value
            GetObjectFromDictionary(dictionaryData, type, ret);

            return ret;
        }

        public List<Type> MapXmlStringToListObject<List, Type>(IEnumerable<string> enumerableStringDomainObjProperties, string xmlData)
            where Type : new()
        {
            var returnValue = new List<Type>();


            // get type using reflection


            // Map XML String to dictionary
            var dictionaryData =
                _xmlDeserializer.DeserializeXmlStringToDictionaryList(enumerableStringDomainObjProperties, xmlData);

            foreach (var dictionary in dictionaryData)
            {
                var type = typeof(Type);
                var ret = new Type();
                returnValue.Add(GetObjectFromDictionary(dictionary, type, ret));
            }


            return returnValue;
        }

        private static T GetObjectFromDictionary<T>(Dictionary<string, string> dictionaryData, Type type, T ret) where T : new()
        {
            if (dictionaryData == null)
            {
                return ret;
            }
            foreach (var keyValue in dictionaryData)
            {
                // null reference check
                if (type.GetProperty(keyValue.Key, BindingFlags.SetProperty |
                                                   BindingFlags.IgnoreCase |
                                                   BindingFlags.Public |
                                                   BindingFlags.Instance) == null) continue;
                // Ignore case - very important because it was amateur hour at ANFP and data returned is in different cases
                // i.e. FUNTIMES, fuNTimes, FunTimes 
                PropertyInfo propertyInfo = type.GetProperty(keyValue.Key, BindingFlags.SetProperty |
                                                                  BindingFlags.IgnoreCase |
                                                                  BindingFlags.Public |
                                                                  BindingFlags.Instance);
                if (propertyInfo == null) continue;

                // convert to correct type and assign it to TYPE T
                switch (propertyInfo.PropertyType.Name)
                {
                    case "Int32":
                        {

                            var value = Convert.ToInt32(keyValue.Value);

                            propertyInfo
                                .SetValue(ret, value, null);
                        }
                        break;
                    case "DateTime":
                        {
                            // add one hour because of datetime when hosting on azure - might have to change this for a better solution later.
                            var value = Convert.ToDateTime(keyValue.Value).AddHours(2);

                            propertyInfo
                            .SetValue(ret, value, null);
                        }
                        break;
                    case "Nullable`1": // nullable datetime
                        {

                            if (keyValue.Value != null)
                            {
                                var value = Convert.ToDateTime(keyValue.Value).AddHours(2);

                                propertyInfo
                                    .SetValue(ret, value, null);
                            }
                            else
                            {
                                propertyInfo
                                    .SetValue(ret, null, null);
                            }

                        }
                        break;

                    default:
                        propertyInfo
                            .SetValue(ret, keyValue.Value, null);
                        break;
                }

            }
            return ret;
        }
    }
}
