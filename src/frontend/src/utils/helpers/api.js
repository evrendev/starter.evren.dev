import axios from "axios";
import NotificationService from "./notification";

const baseURL = import.meta.env.VITE_API_URL;
const acceptLanguage = document.documentElement.lang;

// Create axios instance
const apiClient = axios.create({
  baseURL,
  headers: {
    "Accept-Language": acceptLanguage,
    "Content-Type": "application/json"
  }
});

// Add request interceptor for auth token
apiClient.interceptors.request.use((config) => {
  const token = localStorage.getItem("token");
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

class ApiService {
  // POST request
  async post(endpoint, data) {
    try {
      const response = await apiClient.post(endpoint, data);
      NotificationService.handleApiResponse(response);
      return response.data;
    } catch (error) {
      this.handleError(error);
      throw error;
    }
  }

  // PUT request
  async put(endpoint, data) {
    try {
      const response = await apiClient.put(endpoint, data);
      NotificationService.handleApiResponse(response);
      return response.data;
    } catch (error) {
      this.handleError(error);
      throw error;
    }
  }

  // DELETE request
  async delete(endpoint) {
    try {
      const response = await apiClient.delete(endpoint);
      NotificationService.handleApiResponse(response);
      return response.data;
    } catch (error) {
      this.handleError(error);
      throw error;
    }
  }

  handleError(error) {
    const errorMessage = error.response?.data?.message || "An error occurred";
    console.error("API Error:", errorMessage);
    NotificationService.handleApiResponse(error.response);
  }
}

export const apiService = new ApiService();
