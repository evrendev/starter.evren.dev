import axios from "axios";
import { useAppStore } from "@/stores";
import NotificationService from "./notification";

const baseURL = import.meta.env.VITE_API_URL;

const apiClient = axios.create({ baseURL });

// Add request interceptor for auth token
apiClient.interceptors.request.use(
  function (config) {
    const auth = JSON.parse(localStorage.getItem("auth"));
    const token = auth?.token;

    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }

    config.headers.Accept = "application/json";
    config.headers["Accept-Language"] = localStorage.getItem("lang") ?? config.defaultLocale;

    return config;
  },
  function (error) {
    return Promise.reject(error);
  }
);

// Add response interceptor for auth token
apiClient.interceptors.response.use(
  function (config) {
    const auth = JSON.parse(localStorage.getItem("auth"));
    const token = auth?.token;

    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }

    config.headers.Accept = "application/json";
    config.headers["Accept-Language"] = localStorage.getItem("lang") ?? config.defaultLocale;

    return config;
  },
  function (error) {
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
