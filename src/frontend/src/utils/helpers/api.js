import axios from "axios";
import { useAppStore } from "@/stores";
import NotificationService from "./notification";
import config from "@/config";

const baseURL = import.meta.env.VITE_API_URL;

// Create axios instance
const apiClient = axios.create({
  baseURL
});

// Add request interceptor for auth token
apiClient.interceptors.request.use(
  (cfg) => {
    const auth = JSON.parse(localStorage.getItem("auth"));
    const token = auth?.token;

    if (token) {
      cfg.headers.Authorization = `Bearer ${token}`;
    }

    cfg.headers.Accept = "application/json";
    cfg.headers["Accept-Language"] = localStorage.getItem("lang") ?? config.defaultLocale;

    return cfg;
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
      window.location.href = "/auth/login";
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
    const store = this.getStore();
    store?.setLoading(true);

    try {
      const response = await apiClient.get(endpoint);
      if (showNotification) NotificationService.handleApiResponse(response);
      return response.data;
    } catch (error) {
      this.handleError(error);
      throw error;
    } finally {
      store?.setLoading(false);
    }
  }

  // POST request
  async post(endpoint, data) {
    const store = this.getStore();
    store?.setLoading(true);

    try {
      const response = await apiClient.post(endpoint, data);
      NotificationService.handleApiResponse(response);
      return response.data;
    } catch (error) {
      this.handleError(error);
      throw error;
    } finally {
      store?.setLoading(false);
    }
  }

  // PUT request
  async put(endpoint, data) {
    const store = this.getStore();
    store?.setLoading(true);

    try {
      const response = await apiClient.put(endpoint, data);
      NotificationService.handleApiResponse(response);
      return response.data;
    } catch (error) {
      this.handleError(error);
      throw error;
    } finally {
      store?.setLoading(false);
    }
  }

  // DELETE request
  async delete(endpoint) {
    const store = this.getStore();
    store?.setLoading(true);

    try {
      const response = await apiClient.delete(endpoint);
      NotificationService.handleApiResponse(response);
      return response.data;
    } catch (error) {
      this.handleError(error);
      throw error;
    } finally {
      store?.setLoading(false);
    }
  }

  handleError(error) {
    const errorMessage = error.response?.data?.message || "An error occurred";
    console.error("API Error:", errorMessage);
    NotificationService.handleApiResponse(error.response);
  }
}

export const apiService = new ApiService();
