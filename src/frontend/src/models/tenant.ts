export interface Tenant {
  id: string;
  name: string;
  connectionString: string;
  adminEmail: string;
  isActive: boolean;
  validUpto: string;
  issuer: string;
}
