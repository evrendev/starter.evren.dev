import DateFnsAdapter from "@date-io/date-fns";
import { enUS, tr, de } from "date-fns/locale";

export const date = {
  adapter: DateFnsAdapter,
  locale: {
    en: enUS,
    tr: tr,
    de: de,
  },
};

export default date;
