export interface SetupTwoFactorAuthenticationResponse {
  showSetup: boolean;
  sharedKey: string;
  qrCodeUri: string;
}

export interface RecoverCodesResponse {
  data: string[];
  showRecoverCodes: boolean;
}
