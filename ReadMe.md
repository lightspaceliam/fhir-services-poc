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