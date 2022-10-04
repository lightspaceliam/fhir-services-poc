# Fhir Services POC

Discover how to integrate with a HL7 FHIR service and surface data via a .NET Core API.

1. [API](#API)
2. [Fhir.Service](#Fhir.Service)


## API

Where the subscribing client gets all data from.

### Endpoints

**Patient:**

- GET Pagination api/resources/patients?page={int}&count={int}
- GET Pagination api/resources/patients/{id:string}

## Fhir.Service

HL7 FHIR
- Fast
- Healthcare
- Interoperability
- Resources

Currently supports: 

- Patient
    - GET Pagination<T>
    - GET Resource<R>

## Filtering & Sorting

{base}/{version}/{Resource}?{optional-query-string}

**Patient Pagination with Sorting**

- base: https://hapi.fhir.org/baseR5/
- resource: Patient
- query string: _sort=name&_count={count}&_offset={offset}

Even though the property `family` exists in the schema like so:

```json
{
    "name": [
        {
            "text": "...",
            "family": "...",
            "given": [
                "...",
            ]
        }
    ]
}
```
It still filters appropriately.

```C#
// ASC
var queryString = $"{Resource}?_sort={property}&_count={count}&_offset={offset}";

// https://hapi.fhir.org/baseR5/Patient?_sort=family&_count=10&_offset=0

// DESC
var queryString = $"{Resource}?_sort=-{property}&_count={count}&_offset={offset}";

// https://hapi.fhir.org/baseR5/Patient?_sort=-family&_count=10&_offset=0
```

**Caveat:**

Both baseR4 & baseR5 do not provide: `total` as part of the 'search' schema in `bundle` object for total records.

## References

- [HL7 FHIR Search Sort](https://www.hl7.org/fhir/search.html#_sort)