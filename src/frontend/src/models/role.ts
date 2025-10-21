export interface BasicRole {
  id: string;
  name: string;
  description: string;
}

export interface Role extends BasicRole {
  permissions: string[];
}
