from sqlalchemy import Column, Integer, String, DateTime, Float, Boolean
from sqlalchemy.ext.declarative import declarative_base
from sqlalchemy.orm import sessionmaker
from sqlalchemy import create_engine
from datetime import datetime
from typing import List, Optional
from pydantic import BaseModel

Base = declarative_base()

class UserCityPreference(Base):
    __tablename__ = "user_city_preferences"
    
    id = Column(Integer, primary_key=True, index=True)
    city_name = Column(String, index=True)
    country_code = Column(String)
    latitude = Column(Float)
    longitude = Column(Float)
    is_favorite = Column(Boolean, default=False)
    created_at = Column(DateTime, default=datetime.utcnow)
    last_accessed = Column(DateTime, default=datetime.utcnow)

class WeatherData(BaseModel):
    city: str
    country: str
    temperature: float
    feels_like: float
    description: str
    humidity: float
    wind_speed: float
    pressure: float
    visibility: float
    uv_index: Optional[float] = None
    last_updated: datetime

class WeatherRequest(BaseModel):
    cities: List[str] = []
    latitude: Optional[float] = None
    longitude: Optional[float] = None
    use_current_location: bool = False

class WeatherResponse(BaseModel):
    weather_data: List[WeatherData]
    success: bool
    error_message: Optional[str] = None

class CityPreference(BaseModel):
    city_name: str
    country_code: str
    latitude: float
    longitude: float
    is_favorite: bool = False
