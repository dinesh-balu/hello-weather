# Hello Weather .NET Core 8 API

This is the .NET Core 8 backend API for the Hello Weather application, providing weather data for US and India cities.

## Features

- Weather data retrieval by coordinates or city names
- User city preferences management
- City search functionality
- SQLite database for storing user preferences
- OpenWeatherMap API integration
- CORS support for frontend applications

## Prerequisites

- .NET 8.0 SDK
- SQLite

## Setup

1. Clone the repository and navigate to the backend directory:
   ```bash
   cd backend/hello-weather-dotnet-api
   ```

2. Set up environment variables:
   ```bash
   export OPENWEATHER_API_KEY=your_actual_api_key_here
   ```

3. Restore dependencies:
   ```bash
   dotnet restore
   ```

4. Run the application:
   ```bash
   dotnet run
   ```

The API will be available at `https://localhost:5001` (HTTPS) or `http://localhost:5000` (HTTP).

## API Endpoints

### Health Check
- `GET /healthz` - Health check endpoint

### Weather
- `POST /api/weather` - Get weather data for cities or coordinates

### Cities
- `GET /api/cities/preferences` - Get user's saved city preferences
- `POST /api/cities/preferences` - Save a city preference
- `GET /api/cities/search?q={query}` - Search for cities

## Configuration

The application uses the following configuration:

- **Database**: SQLite database file (`weather.db`)
- **Weather API**: OpenWeatherMap API
- **CORS**: Configured to allow all origins for development

Configuration can be modified in `appsettings.json` and `appsettings.Development.json`.

## Database

The application uses Entity Framework Core with SQLite. The database is automatically created when the application starts.

## Environment Variables

- `OPENWEATHER_API_KEY`: Your OpenWeatherMap API key (required for weather data)

## Development

To run in development mode:

```bash
dotnet run --environment Development
```

This will enable Swagger UI at `/swagger` for API documentation and testing.
