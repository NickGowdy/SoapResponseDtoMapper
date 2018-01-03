# SoapResponseDtoMapper

.Net framework class library that takes XML response and maps it to DTO.

## Getting Started

All you have to do is clone or download the solution. It's just one class library which can be referenced in your project.


### Example

This is how you use the mapper in your code:

```
    // use reflection to get list of properties to map to object
    var listOfProperties = typeof(DtoResponseObject).GetProperties().Select(expr => expr.Name);

    // map xml data to DTO
    var result = _xmlToDomainObject.MapXmlStringToObject<DtoResponseObject>(listOfProperties, xmlResponse);
```

You can also work with collections

```
	// use reflection to get list of properties to map to object
    var listOfProperties= typeof(DtoResponseObject).GetProperties().Select(expr => expr.Name);

    // map xml data to DTO
    var result =
        _xmlToDomainObject.MapXmlStringToListObject<List<DtoResponseObject>, DtoResponseObject>(
			listOfProperties, soapResponse);

```


## Authors

* **Nick Gowdy** - *Initial work* - [Institute of Civil Engineers](https://github.com/NickGowdy)(https://www.linkedin.com/in/nick-gowdy-29443917/)




