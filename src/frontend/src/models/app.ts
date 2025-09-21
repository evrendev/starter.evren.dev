export interface PredefinedValues {
  genders: Gender[];
  languages: Language[];
}

export interface Language {
  name: string;
  value: number;
}

export interface Gender {
  name: string;
  value: number;
}

export interface FileUploadRequest {
  name: string;
  extension: string;
  data: string;
}
