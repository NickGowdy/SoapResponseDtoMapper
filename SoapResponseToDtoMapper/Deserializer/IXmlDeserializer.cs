using System.Collections.Generic;

namespace SoapResponseDtoMapper.Deserializer
{
    public interface IXmlDeserializer
    {
        Dictionary<string, string> DeserializeXmlStringToDictionary(IEnumerable<string> filter, string stringXml);

        List<Dictionary<string, string>> DeserializeXmlStringToDictionaryList(IEnumerable<string> filter,
            string stringXml);
    }
}