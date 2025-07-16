import { useState, useEffect } from 'react'
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { MapPin, Search, Star, Thermometer, Droplets, Wind, Eye } from 'lucide-react'
import './App.css'

interface WeatherData {
  city: string
  country: string
  temperature: number
  feels_like: number
  description: string
  humidity: number
  wind_speed: number
  pressure: number
  visibility: number
  last_updated: string
}

interface City {
  name: string
  country: string
  lat: number
  lon: number
}

function App() {
  const [weatherData, setWeatherData] = useState<WeatherData[]>([])
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState<string | null>(null)
  const [searchQuery, setSearchQuery] = useState('')
  const [searchResults, setSearchResults] = useState<City[]>([])
  const [selectedCities, setSelectedCities] = useState<string[]>(['New York', 'Mumbai'])

  const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || (window.location.hostname === 'localhost' ? 'http://localhost:8000' : `${window.location.protocol}//${window.location.host.replace(/^user:[^@]+@/, '')}`)

  useEffect(() => {
    if (selectedCities.length > 0) {
      fetchWeatherData()
    }
  }, [selectedCities])

  const fetchWeatherData = async () => {
    setLoading(true)
    setError(null)
    
    try {
      const response = await fetch(`${API_BASE_URL}/api/weather`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({
          cities: selectedCities,
          use_current_location: false
        })
      })
      
      const data = await response.json()
      
      if (data.success) {
        setWeatherData(data.weather_data)
      } else {
        setError(data.error_message || 'Failed to fetch weather data')
      }
    } catch (err) {
      setError('Network error: Unable to fetch weather data')
    } finally {
      setLoading(false)
    }
  }

  const searchCities = async (query: string) => {
    if (query.length < 2) {
      setSearchResults([])
      return
    }
    
    try {
      const response = await fetch(`${API_BASE_URL}/api/cities/search?q=${encodeURIComponent(query)}`)
      const cities = await response.json()
      setSearchResults(cities)
    } catch (err) {
      console.error('Error searching cities:', err)
    }
  }

  const addCity = (city: City) => {
    const cityName = city.name
    if (!selectedCities.includes(cityName)) {
      setSelectedCities([...selectedCities, cityName])
    }
    setSearchQuery('')
    setSearchResults([])
  }

  const removeCity = (cityName: string) => {
    setSelectedCities(selectedCities.filter(city => city !== cityName))
  }

  const getCurrentLocation = () => {
    if (navigator.geolocation) {
      setLoading(true)
      navigator.geolocation.getCurrentPosition(
        async (position) => {
          try {
            const response = await fetch(`${API_BASE_URL}/api/weather`, {
              method: 'POST',
              headers: {
                'Content-Type': 'application/json',
              },
              body: JSON.stringify({
                latitude: position.coords.latitude,
                longitude: position.coords.longitude,
                use_current_location: true
              })
            })
            
            const data = await response.json()
            
            if (data.success && data.weather_data.length > 0) {
              const currentLocationWeather = data.weather_data[0]
              const currentCityName = currentLocationWeather.city
              
              if (!selectedCities.includes(currentCityName)) {
                setSelectedCities([currentCityName, ...selectedCities])
              }
            }
          } catch (err) {
            setError('Failed to get weather for current location')
          } finally {
            setLoading(false)
          }
        },
        () => {
          setError('Unable to get your location. Please enable location services.')
          setLoading(false)
        }
      )
    } else {
      setError('Geolocation is not supported by this browser.')
    }
  }

  return (
    <div className="min-h-screen bg-gradient-to-br from-blue-400 via-blue-500 to-blue-600 p-4">
      <div className="max-w-6xl mx-auto">
        <header className="text-center mb-8">
          <h1 className="text-4xl font-bold text-white mb-2">Hello Weather</h1>
          <p className="text-blue-100">Get weather details for cities in US and India</p>
        </header>

        <div className="mb-6 flex flex-col sm:flex-row gap-4 items-center justify-center">
          <div className="relative flex-1 max-w-md">
            <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400 h-4 w-4" />
            <Input
              type="text"
              placeholder="Search for cities..."
              value={searchQuery}
              onChange={(e) => {
                setSearchQuery(e.target.value)
                searchCities(e.target.value)
              }}
              className="pl-10"
            />
            {searchResults.length > 0 && (
              <div className="absolute top-full left-0 right-0 bg-white border border-gray-200 rounded-md shadow-lg z-10 mt-1">
                {searchResults.map((city, index) => (
                  <button
                    key={index}
                    onClick={() => addCity(city)}
                    className="w-full text-left px-4 py-2 hover:bg-gray-100 border-b border-gray-100 last:border-b-0"
                  >
                    {city.name}, {city.country}
                  </button>
                ))}
              </div>
            )}
          </div>
          
          <Button onClick={getCurrentLocation} variant="outline" className="bg-white">
            <MapPin className="h-4 w-4 mr-2" />
            Use Current Location
          </Button>
        </div>

        {selectedCities.length > 0 && (
          <div className="mb-6">
            <h3 className="text-white text-lg font-semibold mb-3">Selected Cities:</h3>
            <div className="flex flex-wrap gap-2">
              {selectedCities.map((city) => (
                <div key={city} className="bg-white/20 text-white px-3 py-1 rounded-full flex items-center gap-2">
                  <span>{city}</span>
                  <button
                    onClick={() => removeCity(city)}
                    className="text-white/80 hover:text-white"
                  >
                    ×
                  </button>
                </div>
              ))}
            </div>
          </div>
        )}

        {error && (
          <div className="mb-6 p-4 bg-red-100 border border-red-400 text-red-700 rounded-md">
            {error}
          </div>
        )}

        {loading && (
          <div className="text-center text-white mb-6">
            <div className="inline-block animate-spin rounded-full h-8 w-8 border-b-2 border-white"></div>
            <p className="mt-2">Loading weather data...</p>
          </div>
        )}

        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
          {weatherData.map((weather, index) => (
            <Card key={index} className="bg-white/95 backdrop-blur-sm">
              <CardHeader>
                <CardTitle className="flex items-center justify-between">
                  <span>{weather.city}, {weather.country}</span>
                  <Button variant="ghost" size="icon">
                    <Star className="h-4 w-4" />
                  </Button>
                </CardTitle>
                <CardDescription>{weather.description}</CardDescription>
              </CardHeader>
              <CardContent>
                <div className="space-y-4">
                  <div className="flex items-center justify-center">
                    <div className="text-center">
                      <div className="flex items-center justify-center mb-2">
                        <Thermometer className="h-8 w-8 text-orange-500 mr-2" />
                        <span className="text-4xl font-bold">{Math.round(weather.temperature)}°C</span>
                      </div>
                      <p className="text-gray-600">Feels like {Math.round(weather.feels_like)}°C</p>
                    </div>
                  </div>
                  
                  <div className="grid grid-cols-2 gap-4 text-sm">
                    <div className="flex items-center">
                      <Droplets className="h-4 w-4 text-blue-500 mr-2" />
                      <span>Humidity: {weather.humidity}%</span>
                    </div>
                    <div className="flex items-center">
                      <Wind className="h-4 w-4 text-gray-500 mr-2" />
                      <span>Wind: {weather.wind_speed} m/s</span>
                    </div>
                    <div className="flex items-center">
                      <Eye className="h-4 w-4 text-purple-500 mr-2" />
                      <span>Visibility: {weather.visibility} km</span>
                    </div>
                    <div className="flex items-center">
                      <span className="text-gray-500">Pressure: {weather.pressure} hPa</span>
                    </div>
                  </div>
                  
                  <div className="text-xs text-gray-500 text-center">
                    Last updated: {new Date(weather.last_updated).toLocaleTimeString()}
                  </div>
                </div>
              </CardContent>
            </Card>
          ))}
        </div>

        {weatherData.length === 0 && !loading && !error && (
          <div className="text-center text-white">
            <p className="text-xl mb-4">Welcome to Hello Weather!</p>
            <p>Search for cities or use your current location to get started.</p>
          </div>
        )}
      </div>
    </div>
  )
}

export default App
