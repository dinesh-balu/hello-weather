from fastapi import FastAPI, Depends, HTTPException
from fastapi.middleware.cors import CORSMiddleware
from sqlalchemy.orm import Session
from typing import List
from .models import WeatherRequest, WeatherResponse, WeatherData, CityPreference, UserCityPreference
from .database import get_db, create_tables
from .weather_service import WeatherService
from datetime import datetime

app = FastAPI(title="Hello Weather API", description="Weather API for US and India cities")

# Disable CORS. Do not remove this for full-stack development.
app.add_middleware(
    CORSMiddleware,
    allow_origins=["*"],  # Allows all origins
    allow_credentials=True,
    allow_methods=["*"],  # Allows all methods
    allow_headers=["*"],  # Allows all headers
)

create_tables()

weather_service = WeatherService()

@app.get("/healthz")
async def healthz():
    return {"status": "ok"}

@app.post("/api/weather", response_model=WeatherResponse)
async def get_weather(request: WeatherRequest, db: Session = Depends(get_db)):
    """Get weather data for cities or coordinates"""
    try:
        weather_data = []
        
        if request.use_current_location and request.latitude and request.longitude:
            weather = weather_service.get_weather_by_coordinates(request.latitude, request.longitude)
            if weather:
                weather_data.append(weather)
        
        if request.cities:
            city_weather = weather_service.get_weather_for_multiple_cities(request.cities)
            weather_data.extend(city_weather)
        
        if not weather_data:
            return WeatherResponse(
                weather_data=[],
                success=False,
                error_message="No weather data found for the provided locations"
            )
        
        return WeatherResponse(
            weather_data=weather_data,
            success=True
        )
    
    except Exception as e:
        return WeatherResponse(
            weather_data=[],
            success=False,
            error_message=f"Error fetching weather data: {str(e)}"
        )

@app.get("/api/cities/preferences")
async def get_city_preferences(db: Session = Depends(get_db)):
    """Get user's saved city preferences"""
    preferences = db.query(UserCityPreference).order_by(UserCityPreference.last_accessed.desc()).all()
    return [
        {
            "id": pref.id,
            "city_name": pref.city_name,
            "country_code": pref.country_code,
            "latitude": pref.latitude,
            "longitude": pref.longitude,
            "is_favorite": pref.is_favorite,
            "last_accessed": pref.last_accessed
        }
        for pref in preferences
    ]

@app.post("/api/cities/preferences")
async def save_city_preference(preference: CityPreference, db: Session = Depends(get_db)):
    """Save a city preference"""
    existing = db.query(UserCityPreference).filter(
        UserCityPreference.city_name == preference.city_name,
        UserCityPreference.country_code == preference.country_code
    ).first()
    
    if existing:
        existing.is_favorite = preference.is_favorite
        existing.last_accessed = datetime.utcnow()
    else:
        new_preference = UserCityPreference(
            city_name=preference.city_name,
            country_code=preference.country_code,
            latitude=preference.latitude,
            longitude=preference.longitude,
            is_favorite=preference.is_favorite,
            last_accessed=datetime.utcnow()
        )
        db.add(new_preference)
    
    db.commit()
    return {"message": "City preference saved successfully"}

@app.get("/api/cities/search")
async def search_cities(q: str):
    """Search for cities (simplified implementation)"""
    us_cities = [
        {"name": "New York", "country": "US", "lat": 40.7128, "lon": -74.0060},
        {"name": "Los Angeles", "country": "US", "lat": 34.0522, "lon": -118.2437},
        {"name": "Chicago", "country": "US", "lat": 41.8781, "lon": -87.6298},
        {"name": "Houston", "country": "US", "lat": 29.7604, "lon": -95.3698},
        {"name": "Phoenix", "country": "US", "lat": 33.4484, "lon": -112.0740},
        {"name": "Philadelphia", "country": "US", "lat": 39.9526, "lon": -75.1652},
        {"name": "San Antonio", "country": "US", "lat": 29.4241, "lon": -98.4936},
        {"name": "San Diego", "country": "US", "lat": 32.7157, "lon": -117.1611},
        {"name": "Dallas", "country": "US", "lat": 32.7767, "lon": -96.7970},
        {"name": "San Jose", "country": "US", "lat": 37.3382, "lon": -121.8863},
    ]
    
    indian_cities = [
        {"name": "Mumbai", "country": "IN", "lat": 19.0760, "lon": 72.8777},
        {"name": "Delhi", "country": "IN", "lat": 28.7041, "lon": 77.1025},
        {"name": "Bangalore", "country": "IN", "lat": 12.9716, "lon": 77.5946},
        {"name": "Hyderabad", "country": "IN", "lat": 17.3850, "lon": 78.4867},
        {"name": "Ahmedabad", "country": "IN", "lat": 23.0225, "lon": 72.5714},
        {"name": "Chennai", "country": "IN", "lat": 13.0827, "lon": 80.2707},
        {"name": "Kolkata", "country": "IN", "lat": 22.5726, "lon": 88.3639},
        {"name": "Surat", "country": "IN", "lat": 21.1702, "lon": 72.8311},
        {"name": "Pune", "country": "IN", "lat": 18.5204, "lon": 73.8567},
        {"name": "Jaipur", "country": "IN", "lat": 26.9124, "lon": 75.7873},
    ]
    
    all_cities = us_cities + indian_cities
    query_lower = q.lower()
    
    matching_cities = [
        city for city in all_cities 
        if query_lower in city["name"].lower()
    ]
    
    return matching_cities[:10]  # Return top 10 matches
