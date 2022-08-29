# Api

## Getting Started

1. Ensure you have .NET Core 6 installed
2. Right click the Api project and select `Manage User Secrets` and add the following:
```json
{
    "Fhir": {
        "BaseUrl": "your-chosen-fhir-service"
    }
}
```
3. All done. Run the app with the Api set as the `Startup project`.

## Sign In Credentials 

There is currently a Mock Authentication Service configured to provide a mock bearer token.

Sign in to: `/api/authentication/sign-in` with:
```json
{
    "username": "doogie@rmh.com.au",
    "password": "Password"
}
```
