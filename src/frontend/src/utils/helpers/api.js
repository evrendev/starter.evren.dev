import axios from "axios";
import NotificationService from "./notification";
import { useAppStore } from "@/stores";

const baseURL = import.meta.env.VITE_API_URL;

// Create axios instance
const apiClient = axios.create({
  baseURL
});

// Add request interceptor for auth token
apiClient.interceptors.request.use(
  (config) => {
    const auth = JSON.parse(localStorage.getItem("auth"));
    const token = auth?.token;

    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }

    config.headers.Accept = "application/json";
    config.headers["Accept-Language"] = localStorage.getItem("lang");

    return config;
  },
  (error) => {
    // Handle request errors here
    return Promise.reject(error);
  }
);

// Add response interceptor for handling 401
apiClient.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      localStorage.removeItem("auth");
    }
    return Promise.reject(error);
  }
);

class ApiService {
  // Method to get store instance
  getStore() {
    try {
      return useAppStore();
    } catch (error) {
      console.warn("Store access failed, might be outside of component context", error);
      return null;
    }
  }

  // GET request
  async get(endpoint, showNotification = true) {
    try {
      const response = await apiClient.get(endpoint);
      if (showNotification) NotificationService.handleApiResponse(response);
      return response.data;
    } catch (error) {
      this.handleError(error);
      throw error;
    }
  }

  // POST request
  async post(endpoint, data) {
    const store = this.getStore();
    store?.setPageLoader(true);

    try {
      const response = await apiClient.post(endpoint, data);
      NotificationService.handleApiResponse(response);
      return response.data;
    } catch (error) {
      this.handleError(error);
      throw error;
    } finally {
      store?.setPageLoader(false);
    }
  }

  // PUT request
  async put(endpoint, data) {
    const store = this.getStore();
    store?.setPageLoader(true);

    try {
      const response = await apiClient.put(endpoint, data);
      NotificationService.handleApiResponse(response);
      return response.data;
    } catch (error) {
      this.handleError(error);
      throw error;
    } finally {
      store?.setPageLoader(false);
    }
  }

  // DELETE request
  async delete(endpoint) {
    const store = this.getStore();
    store?.setPageLoader(true);

    try {
      const response = await apiClient.delete(endpoint);
      NotificationService.handleApiResponse(response);
      return response.data;
    } catch (error) {
      this.handleError(error);
      throw error;
    } finally {
      store?.setPageLoader(false);
    }
  }

  handleError(error) {
    const errorMessage = error.response?.data?.message || "An error occurred";
    console.error("API Error:", errorMessage);
    NotificationService.handleApiResponse(error.response);
  }
}

export const apiService = new ApiService();
