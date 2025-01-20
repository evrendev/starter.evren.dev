import axios from "axios"

const acceptLanguage = document.documentElement.lang

const axiosInstance = axios.create({
  baseURL: `${import.meta.env.VITE_API_URL}/api`,
  headers: {
    "Accept-Language": acceptLanguage,
    "Content-Type": "application/json",
  },
})

// Add request interceptor (optional)
axiosInstance.interceptors.request.use(
  config => {
    // You can add auth tokens or other headers here
    return config
  },
  error => {
    return Promise.reject(error)
  },
)

// Add response interceptor (optional)
axiosInstance.interceptors.response.use(
  response => {
    return response
  },
  error => {
    return Promise.reject(error)
  },
)

export default axiosInstance
