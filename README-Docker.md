# Docker Setup for Hello Weather

This document provides instructions for running the Hello Weather application using Docker containers.

## Prerequisites

- Docker and Docker Compose installed on your system
- OpenWeatherMap API key

## Quick Start

1. **Clone the repository and navigate to the project root:**
   ```bash
   git clone https://github.com/dinesh-balu/hello-weather.git
   cd hello-weather
   ```

2. **Create environment file:**
   ```bash
   cp .env.example .env
   ```

3. **Edit the `.env` file and add your OpenWeatherMap API key:**
   ```
   OPENWEATHER_API_KEY=your_actual_api_key_here
   ```

4. **Build and start all services:**
   ```bash
   docker-compose up --build
   ```

## Services

The application consists of three services:

### Backend (.NET Core 8 API)
- **Port:** 8000
- **URL:** http://localhost:8000
- **Health Check:** http://localhost:8000/healthz
- **API Documentation:** http://localhost:8000/swagger (in development mode)

### Web Frontend (React + Vite)
- **Port:** 3000
- **URL:** http://localhost:3000
- **Technology:** React with Vite, served by Nginx

### Mobile Frontend (React Native + Expo)
- **Port:** 8081 (Metro bundler)
- **Expo DevTools:** http://localhost:19000
- **Technology:** React Native with Expo development server

## Individual Service Commands

### Build individual services:
```bash
# Backend
docker build -t hello-weather-backend ./backend/hello-weather-dotnet-api

# Web Frontend
docker build -t hello-weather-web ./frontend/web/hello-weather-web

# Mobile Frontend
docker build -t hello-weather-mobile ./frontend/mobile/hello-weather-mobile
```

### Run individual services:
```bash
# Backend only
docker-compose up backend

# Web frontend only (requires backend)
docker-compose up backend web-frontend

# Mobile frontend only (requires backend)
docker-compose up backend mobile-frontend
```

## Development Workflow

1. **Start all services in detached mode:**
   ```bash
   docker-compose up -d
   ```

2. **View logs:**
   ```bash
   # All services
   docker-compose logs -f

   # Specific service
   docker-compose logs -f backend
   docker-compose logs -f web-frontend
   docker-compose logs -f mobile-frontend
   ```

3. **Stop all services:**
   ```bash
   docker-compose down
   ```

4. **Rebuild after code changes:**
   ```bash
   docker-compose up --build
   ```

## Environment Variables

### Backend
- `OPENWEATHER_API_KEY`: Your OpenWeatherMap API key (required)
- `ASPNETCORE_ENVIRONMENT`: Set to `Production` or `Development`
- `ASPNETCORE_URLS`: URL binding configuration

### Web Frontend
- `VITE_API_BASE_URL`: Backend API URL (default: http://localhost:8000)

### Mobile Frontend
- `EXPO_PUBLIC_API_BASE_URL`: Backend API URL (default: http://localhost:8000)
- `EXPO_DEVTOOLS_LISTEN_ADDRESS`: Set to `0.0.0.0` for Docker networking

## Troubleshooting

### Common Issues

1. **Port conflicts:**
   - Ensure ports 3000, 8000, 8081, 19000-19002 are not in use
   - Modify port mappings in `docker-compose.yml` if needed

2. **API key not working:**
   - Verify your OpenWeatherMap API key is valid
   - Check the `.env` file is in the project root
   - Ensure no extra spaces around the API key

3. **Mobile app can't connect to backend:**
   - Verify backend is running and accessible at http://localhost:8000/healthz
   - Check mobile app environment variable configuration

4. **Build failures:**
   - Clear Docker cache: `docker system prune -a`
   - Rebuild without cache: `docker-compose build --no-cache`

### Logs and Debugging

- View container logs: `docker-compose logs [service-name]`
- Access container shell: `docker-compose exec [service-name] sh`
- Check container status: `docker-compose ps`

## Database

The backend uses SQLite database which is persisted in a Docker volume named `weather-db`. The database file will be maintained across container restarts.

To reset the database:
```bash
docker-compose down
docker volume rm hello-weather_weather-db
docker-compose up
```

## Production Considerations

For production deployment, consider:
- Using environment-specific Docker Compose files
- Implementing proper secrets management
- Setting up reverse proxy (nginx/traefik)
- Configuring SSL/TLS certificates
- Using production-ready database (PostgreSQL/SQL Server)
