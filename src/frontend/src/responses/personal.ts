export interface SetupTwoFactorAuthenticationResponse {
  showSetup: boolean;
  sharedKey: string;
  qrCodeUri: string;
}
