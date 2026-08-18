export interface CreatePaymentOrderResponse {
  paymentOrderId: string;
  payPalOrderId: string;
  approveUrl?: string | null;
}
