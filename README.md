# url-shortener
A simple url shortener based on Sha256 and 62 alpha numerical latin characters. It uses sqlite database. It contains a wpf test application.

# starting web api
to run web api (linux or win compatible): 
dotnet run --project "UrlShortenerApi\UrlShortener.Application.csproj"

OpenApi Specification:
http://localhost:5001/swagger/v1/swagger.json

Swagger Homepage
https://localhost:5001/swagger/index.html


# starting test application
to run test client : 
dotnet run --project "UrlShortenerTestApp/UrlShortenerTestApp.csproj"
