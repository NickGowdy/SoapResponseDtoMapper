using System.Collections.Generic;

namespace SoapResponseDtoMapper.Mapper
{
    public interface IXmlToDomainObject
    {
        T MapXmlStringToObject<T>(IEnumerable<string> enumerableStringDomainObjProperties, string xmlData) where T : new();

        List<T> MapXmlStringToListObject<L, T>(IEnumerable<string> enumerableStringDomainObjProperties, string xmlData)
            where T : new();
    }
}