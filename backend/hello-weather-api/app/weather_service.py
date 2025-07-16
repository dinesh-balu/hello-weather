import requests
import os
from typing import List, Optional, Dict, Any
from .models import WeatherData, WeatherRequest
from datetime import datetime
from dotenv import load_dotenv

load_dotenv()

class WeatherService:
    def __init__(self):
        self.openweather_api_key = os.getenv("OPENWEATHER_API_KEY", "demo_key")
        self.openweather_base_url = "https://api.openweathermap.org/data/2.5"
    
    def get_weather_by_coordinates(self, lat: float, lon: float) -> Optional[WeatherData]:
        """Get weather data by coordinates using OpenWeatherMap API"""
        try:
            url = f"{self.openweather_base_url}/weather"
            params = {
                "lat": lat,
                "lon": lon,
                "appid": self.openweather_api_key,
                "units": "metric"
            }
            
            response = requests.get(url, params=params, timeout=10)
            response.raise_for_status()
            data = response.json()
            
            return self._parse_openweather_response(data)
        except Exception as e:
            print(f"Error fetching weather by coordinates: {e}")
            return None
    
    def get_weather_by_city(self, city_name: str, country_code: Optional[str] = None) -> Optional[WeatherData]:
        """Get weather data by city name"""
        try:
            url = f"{self.openweather_base_url}/weather"
            
            query = city_name
            if country_code:
                query = f"{city_name},{country_code}"
            
            params = {
                "q": query,
                "appid": self.openweather_api_key,
                "units": "metric"
            }
            
            response = requests.get(url, params=params, timeout=10)
            response.raise_for_status()
            data = response.json()
            
            return self._parse_openweather_response(data)
        except Exception as e:
            print(f"Error fetching weather for city {city_name}: {e}")
            return None
    
    def get_weather_for_multiple_cities(self, cities: List[str]) -> List[WeatherData]:
        """Get weather data for multiple cities"""
        weather_data = []
        
        for city in cities:
            country_code = self._determine_country_code(city)
            weather = self.get_weather_by_city(city, country_code)
            if weather:
                weather_data.append(weather)
        
        return weather_data
    
    def _determine_country_code(self, city: str) -> Optional[str]:
        """Determine country code based on city name for US and India optimization"""
        us_cities = {
            "new york", "los angeles", "chicago", "houston", "phoenix", "philadelphia",
            "san antonio", "san diego", "dallas", "san jose", "austin", "jacksonville",
            "fort worth", "columbus", "charlotte", "san francisco", "indianapolis",
            "seattle", "denver", "washington", "boston", "el paso", "detroit", "nashville",
            "portland", "memphis", "oklahoma city", "las vegas", "louisville", "baltimore",
            "milwaukee", "albuquerque", "tucson", "fresno", "mesa", "sacramento",
            "atlanta", "kansas city", "colorado springs", "miami", "raleigh", "omaha",
            "long beach", "virginia beach", "oakland", "minneapolis", "tulsa", "arlington"
        }
        
        indian_cities = {
            "mumbai", "delhi", "bangalore", "hyderabad", "ahmedabad", "chennai",
            "kolkata", "surat", "pune", "jaipur", "lucknow", "kanpur", "nagpur",
            "indore", "thane", "bhopal", "visakhapatnam", "pimpri-chinchwad",
            "patna", "vadodara", "ghaziabad", "ludhiana", "agra", "nashik",
            "faridabad", "meerut", "rajkot", "kalyan-dombivli", "vasai-virar",
            "varanasi", "srinagar", "aurangabad", "dhanbad", "amritsar",
            "navi mumbai", "allahabad", "ranchi", "howrah", "coimbatore",
            "jabalpur", "gwalior", "vijayawada", "jodhpur", "madurai", "raipur",
            "kota", "guwahati", "chandigarh", "solapur", "hubli-dharwad"
        }
        
        city_lower = city.lower().strip()
        
        if city_lower in us_cities:
            return "US"
        elif city_lower in indian_cities:
            return "IN"
        
        return None
    
    def _parse_openweather_response(self, data: Dict[str, Any]) -> WeatherData:
        """Parse OpenWeatherMap API response into WeatherData model"""
        return WeatherData(
            city=data["name"],
            country=data["sys"]["country"],
            temperature=data["main"]["temp"],
            feels_like=data["main"]["feels_like"],
            description=data["weather"][0]["description"].title(),
            humidity=data["main"]["humidity"],
            wind_speed=data["wind"]["speed"],
            pressure=data["main"]["pressure"],
            visibility=data.get("visibility", 0) / 1000,  # Convert to km
            last_updated=datetime.utcnow()
        )
