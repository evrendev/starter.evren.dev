import { DataTableHeader } from "vuetify";

export interface Props<T> {
  itemsPerPage?: number;
  itemValue?: string;
  items?: Array<T>;
  total?: number;
  loading?: boolean;
  headers?: Array<DataTableHeader>;
}
