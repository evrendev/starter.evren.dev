import { toast } from "vuetify-sonner";

const NOTIFICATION_TYPES = {
  SUCCESS: {
    color: "success",
    icon: "$checkCircle",
    duration: 3000
  },
  ERROR: {
    color: "error",
    icon: "$alertCircle",
    duration: 5000
  },
  WARNING: {
    color: "warning",
    icon: "$alert",
    duration: 4000
  },
  INFO: {
    color: "info",
    icon: "$information",
    duration: 3000
  }
};

const defaultOptions = {
  progressBar: true,
  cardProps: {
    elevation: 2
  }
};

class NotificationService {
  static showNotification(type, message, description = "", customOptions = {}) {
    const typeConfig = NOTIFICATION_TYPES[type];

    toast(message, {
      ...defaultOptions,
      description,
      duration: typeConfig.duration,
      cardProps: {
        ...defaultOptions.cardProps,
        color: typeConfig.color
      },
      prependIcon: typeConfig.icon,
      ...customOptions
    });
  }

  static success(message, description = "", options = {}) {
    this.showNotification("SUCCESS", message, description, options);
  }

  static error(message, description = "", options = {}) {
    this.showNotification("ERROR", message, description, options);
  }

  static warning(message, description = "", options = {}) {
    this.showNotification("WARNING", message, description, options);
  }

  static info(message, description = "", options = {}) {
    this.showNotification("INFO", message, description, options);
  }

  static handleApiResponse(response) {
    const { status, data } = response;
    if (status >= 200 && status < 300) {
      this.success(data.message || "Operation successful");
    } else if (status >= 400 && status < 500) {
      const errorMessage = data ? Object.values(data).join("\n") : data;
      this.error(errorMessage || "Request failed");
    } else if (status >= 500) {
      this.error(data || "Server error occurred");
    }
  }
}

export default NotificationService;
