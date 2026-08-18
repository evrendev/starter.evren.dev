import { defineStore } from "pinia";

// Local Types
import { CreatePaymentOrderResponse } from "@/types/responses/payment";

// Refactored Architecture Imports
import http, { handleRequest } from "@/utils/http";
import { AppError } from "@/primitives/error";
import { Result } from "@/primitives/result";

export const usePaymentStore = defineStore("payment", {
  state: () => ({
    loading: false as boolean,
    error: null as AppError | null,
  }),
  actions: {
    async createOrder(courseId: string): Promise<Result<CreatePaymentOrderResponse>> {
      this.loading = true;
      this.error = null;

      try {
        const result = await handleRequest<CreatePaymentOrderResponse>(
          http.post("/v1/payments/orders", { courseId }),
        );

        if (!result.succeeded) {
          this.error = result.errors!;
        }

        return result;
      } catch (error) {
        this.error = error as AppError;
        return error as Result<CreatePaymentOrderResponse>;
      } finally {
        this.loading = false;
      }
    },

    async captureOrder(paymentOrderId: string): Promise<Result<boolean>> {
      this.loading = true;
      this.error = null;

      try {
        const result = await handleRequest<boolean>(
          http.post(`/v1/payments/orders/${paymentOrderId}/capture`),
        );

        if (!result.succeeded) {
          this.error = result.errors!;
        }

        return result;
      } catch (error) {
        this.error = error as AppError;
        return error as Result<boolean>;
      } finally {
        this.loading = false;
      }
    },
  },
});
