import http from "@/utils/http";
import { ApiResponse } from "@/models/common";
import {
  UserDetailsDto,
  UpdateUserRequest,
  ChangePasswordRequest,
  AuditDto,
} from "@/models/auth"; // veya user.ts

export const getProfile = (): Promise<ApiResponse<UserDetailsDto>> => {
  return http.get("/personal/profile");
};

export const updateProfile = (request: UpdateUserRequest): Promise<void> => {
  return http.put(`/personal/profile`, request);
};

export const changePassword = (
  request: ChangePasswordRequest
): Promise<void> => {
  return http.put(`/personal/change-password`, request);
};

export const getPermissions = (): Promise<ApiResponse<string[]>> => {
  return http.get("/personal/permissions");
};

export const getAuditLogs = (): Promise<ApiResponse<AuditDto[]>> => {
  return http.get("/personal/logs");
};
