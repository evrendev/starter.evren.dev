import { onMounted } from "vue";
import { NotificationService } from "@/utils/helpers";
import * as signalR from "@microsoft/signalr";

const baseURL = import.meta.env.VITE_BASE_URL;

export default function useSignalR() {
  onMounted(async () => {
    try {
      const connection = new signalR.HubConnectionBuilder()
        .withUrl(`${baseURL}/notificationhub`) // Hub URL
        .withAutomaticReconnect()
        .build();

      connection.on("ReceiveNotification", (data) => {
        console.log("Received notification:", data);
        NotificationService.success(data);
      });

      connection
        .start()
        .then(() => console.log("Connected to SignalR hub"))
        .catch((err) => {
          console.error("SignalR connection error:", err);
        });
    } catch (e) {
      console.error("Versiyon bilgisi alınamadı", e);
    }
  });
}
