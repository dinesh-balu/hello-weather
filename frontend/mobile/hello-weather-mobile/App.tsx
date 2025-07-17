import React, { useState, useEffect } from 'react';
import { StatusBar } from 'expo-status-bar';
import {
  StyleSheet,
  Text,
  View,
  ScrollView,
  TextInput,
  TouchableOpacity,
  Alert,
  ActivityIndicator,
  SafeAreaView,
} from 'react-native';
import * as Location from 'expo-location';

interface WeatherData {
  city: string;
  country: string;
  temperature: number;
  feels_like: number;
  description: string;
  humidity: number;
  wind_speed: number;
  pressure: number;
  visibility: number;
  last_updated: string;
}

interface City {
  name: string;
  country: string;
  lat: number;
  lon: number;
}

export default function App() {
  const [weatherData, setWeatherData] = useState<WeatherData[]>([]);
  const [loading, setLoading] = useState(false);
  const [searchQuery, setSearchQuery] = useState('');
  const [searchResults, setSearchResults] = useState<City[]>([]);
  const [selectedCities, setSelectedCities] = useState<string[]>(['New York', 'Mumbai']);

  const API_BASE_URL = process.env.EXPO_PUBLIC_API_BASE_URL || 'http://localhost:8000';

  useEffect(() => {
    if (selectedCities.length > 0) {
      fetchWeatherData();
    }
  }, [selectedCities]);

  const fetchWeatherData = async () => {
    setLoading(true);
    
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
      });
      
      const data = await response.json();
      
      if (data.success) {
        setWeatherData(data.weather_data);
      } else {
        Alert.alert('Error', data.error_message || 'Failed to fetch weather data');
      }
    } catch (err) {
      Alert.alert('Error', 'Network error: Unable to fetch weather data');
    } finally {
      setLoading(false);
    }
  };

  const searchCities = async (query: string) => {
    if (query.length < 2) {
      setSearchResults([]);
      return;
    }
    
    try {
      const response = await fetch(`${API_BASE_URL}/api/cities/search?q=${encodeURIComponent(query)}`);
      const cities = await response.json();
      setSearchResults(cities);
    } catch (err) {
      console.error('Error searching cities:', err);
    }
  };

  const addCity = (city: City) => {
    const cityName = city.name;
    if (!selectedCities.includes(cityName)) {
      setSelectedCities([...selectedCities, cityName]);
    }
    setSearchQuery('');
    setSearchResults([]);
  };

  const removeCity = (cityName: string) => {
    setSelectedCities(selectedCities.filter(city => city !== cityName));
  };

  const getCurrentLocation = async () => {
    try {
      setLoading(true);
      
      let { status } = await Location.requestForegroundPermissionsAsync();
      if (status !== 'granted') {
        Alert.alert('Permission denied', 'Permission to access location was denied');
        setLoading(false);
        return;
      }

      let location = await Location.getCurrentPositionAsync({});
      
      const response = await fetch(`${API_BASE_URL}/api/weather`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({
          latitude: location.coords.latitude,
          longitude: location.coords.longitude,
          use_current_location: true
        })
      });
      
      const data = await response.json();
      
      if (data.success && data.weather_data.length > 0) {
        const currentLocationWeather = data.weather_data[0];
        const currentCityName = currentLocationWeather.city;
        
        if (!selectedCities.includes(currentCityName)) {
          setSelectedCities([currentCityName, ...selectedCities]);
        }
      }
    } catch (err) {
      Alert.alert('Error', 'Failed to get weather for current location');
    } finally {
      setLoading(false);
    }
  };

  return (
    <SafeAreaView style={styles.container}>
      <StatusBar style="light" />
      
      <View style={styles.header}>
        <Text style={styles.title}>Hello Weather</Text>
        <Text style={styles.subtitle}>Weather for US and India cities</Text>
      </View>

      <View style={styles.searchContainer}>
        <TextInput
          style={styles.searchInput}
          placeholder="Search for cities..."
          value={searchQuery}
          onChangeText={(text) => {
            setSearchQuery(text);
            searchCities(text);
          }}
        />
        
        <TouchableOpacity style={styles.locationButton} onPress={getCurrentLocation}>
          <Text style={styles.locationButtonText}>📍 Current Location</Text>
        </TouchableOpacity>
      </View>

      {searchResults.length > 0 && (
        <View style={styles.searchResults}>
          {searchResults.map((city, index) => (
            <TouchableOpacity
              key={index}
              style={styles.searchResultItem}
              onPress={() => addCity(city)}
            >
              <Text>{city.name}, {city.country}</Text>
            </TouchableOpacity>
          ))}
        </View>
      )}

      {selectedCities.length > 0 && (
        <View style={styles.selectedCitiesContainer}>
          <Text style={styles.selectedCitiesTitle}>Selected Cities:</Text>
          <ScrollView horizontal showsHorizontalScrollIndicator={false}>
            {selectedCities.map((city) => (
              <View key={city} style={styles.cityTag}>
                <Text style={styles.cityTagText}>{city}</Text>
                <TouchableOpacity onPress={() => removeCity(city)}>
                  <Text style={styles.removeButton}>×</Text>
                </TouchableOpacity>
              </View>
            ))}
          </ScrollView>
        </View>
      )}

      {loading && (
        <View style={styles.loadingContainer}>
          <ActivityIndicator size="large" color="#ffffff" />
          <Text style={styles.loadingText}>Loading weather data...</Text>
        </View>
      )}

      <ScrollView style={styles.weatherContainer}>
        {weatherData.map((weather, index) => (
          <View key={index} style={styles.weatherCard}>
            <View style={styles.weatherHeader}>
              <Text style={styles.cityName}>{weather.city}, {weather.country}</Text>
              <Text style={styles.weatherDescription}>{weather.description}</Text>
            </View>
            
            <View style={styles.temperatureContainer}>
              <Text style={styles.temperature}>{Math.round(weather.temperature)}°C</Text>
              <Text style={styles.feelsLike}>Feels like {Math.round(weather.feels_like)}°C</Text>
            </View>
            
            <View style={styles.weatherDetails}>
              <View style={styles.detailRow}>
                <Text style={styles.detailLabel}>💧 Humidity:</Text>
                <Text style={styles.detailValue}>{weather.humidity}%</Text>
              </View>
              <View style={styles.detailRow}>
                <Text style={styles.detailLabel}>💨 Wind:</Text>
                <Text style={styles.detailValue}>{weather.wind_speed} m/s</Text>
              </View>
              <View style={styles.detailRow}>
                <Text style={styles.detailLabel}>👁 Visibility:</Text>
                <Text style={styles.detailValue}>{weather.visibility} km</Text>
              </View>
              <View style={styles.detailRow}>
                <Text style={styles.detailLabel}>📊 Pressure:</Text>
                <Text style={styles.detailValue}>{weather.pressure} hPa</Text>
              </View>
            </View>
            
            <Text style={styles.lastUpdated}>
              Last updated: {new Date(weather.last_updated).toLocaleTimeString()}
            </Text>
          </View>
        ))}
      </ScrollView>

      {weatherData.length === 0 && !loading && (
        <View style={styles.emptyState}>
          <Text style={styles.emptyStateText}>Welcome to Hello Weather!</Text>
          <Text style={styles.emptyStateSubtext}>Search for cities or use your current location to get started.</Text>
        </View>
      )}
    </SafeAreaView>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: '#3B82F6',
  },
  header: {
    alignItems: 'center',
    paddingVertical: 20,
    paddingHorizontal: 16,
  },
  title: {
    fontSize: 32,
    fontWeight: 'bold',
    color: 'white',
    marginBottom: 8,
  },
  subtitle: {
    fontSize: 16,
    color: '#DBEAFE',
  },
  searchContainer: {
    paddingHorizontal: 16,
    marginBottom: 16,
  },
  searchInput: {
    backgroundColor: 'white',
    borderRadius: 8,
    paddingHorizontal: 16,
    paddingVertical: 12,
    fontSize: 16,
    marginBottom: 12,
  },
  locationButton: {
    backgroundColor: 'rgba(255, 255, 255, 0.2)',
    borderRadius: 8,
    paddingVertical: 12,
    alignItems: 'center',
  },
  locationButtonText: {
    color: 'white',
    fontSize: 16,
    fontWeight: '500',
  },
  searchResults: {
    backgroundColor: 'white',
    marginHorizontal: 16,
    borderRadius: 8,
    marginBottom: 16,
  },
  searchResultItem: {
    paddingHorizontal: 16,
    paddingVertical: 12,
    borderBottomWidth: 1,
    borderBottomColor: '#E5E7EB',
  },
  selectedCitiesContainer: {
    paddingHorizontal: 16,
    marginBottom: 16,
  },
  selectedCitiesTitle: {
    color: 'white',
    fontSize: 18,
    fontWeight: '600',
    marginBottom: 8,
  },
  cityTag: {
    backgroundColor: 'rgba(255, 255, 255, 0.2)',
    borderRadius: 20,
    paddingHorizontal: 12,
    paddingVertical: 6,
    marginRight: 8,
    flexDirection: 'row',
    alignItems: 'center',
  },
  cityTagText: {
    color: 'white',
    marginRight: 8,
  },
  removeButton: {
    color: 'white',
    fontSize: 18,
    fontWeight: 'bold',
  },
  loadingContainer: {
    alignItems: 'center',
    paddingVertical: 20,
  },
  loadingText: {
    color: 'white',
    marginTop: 8,
  },
  weatherContainer: {
    flex: 1,
    paddingHorizontal: 16,
  },
  weatherCard: {
    backgroundColor: 'rgba(255, 255, 255, 0.95)',
    borderRadius: 12,
    padding: 16,
    marginBottom: 16,
  },
  weatherHeader: {
    alignItems: 'center',
    marginBottom: 16,
  },
  cityName: {
    fontSize: 20,
    fontWeight: 'bold',
    color: '#1F2937',
    marginBottom: 4,
  },
  weatherDescription: {
    fontSize: 16,
    color: '#6B7280',
  },
  temperatureContainer: {
    alignItems: 'center',
    marginBottom: 16,
  },
  temperature: {
    fontSize: 48,
    fontWeight: 'bold',
    color: '#1F2937',
  },
  feelsLike: {
    fontSize: 16,
    color: '#6B7280',
  },
  weatherDetails: {
    marginBottom: 12,
  },
  detailRow: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    marginBottom: 8,
  },
  detailLabel: {
    fontSize: 14,
    color: '#6B7280',
  },
  detailValue: {
    fontSize: 14,
    fontWeight: '500',
    color: '#1F2937',
  },
  lastUpdated: {
    fontSize: 12,
    color: '#9CA3AF',
    textAlign: 'center',
  },
  emptyState: {
    flex: 1,
    justifyContent: 'center',
    alignItems: 'center',
    paddingHorizontal: 32,
  },
  emptyStateText: {
    fontSize: 24,
    fontWeight: 'bold',
    color: 'white',
    marginBottom: 8,
    textAlign: 'center',
  },
  emptyStateSubtext: {
    fontSize: 16,
    color: '#DBEAFE',
    textAlign: 'center',
  },
});
