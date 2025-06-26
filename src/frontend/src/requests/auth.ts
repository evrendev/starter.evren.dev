import http from "@/utils/http";
import { TokenRequest, TokenResponse, ApiResponse } from "@/models/auth";

export const login = (
  request: TokenRequest,
  tenantId: string
): Promise<ApiResponse<TokenResponse>> => {
  return http.post("/auth/login", request, {
    headers: {
      Tenant: tenantId,
    },
  });
};

export const logout = (tenantId: string): Promise<TokenResponse> => {
  return http.post(
    "/auth/logout",
    {},
    {
      headers: {
        Tenant: tenantId,
      },
    }
  );
};

export const refreshToken = (): Promise<ApiResponse<TokenResponse>> => {
  return http.get("/auth/refresh-token");
};
