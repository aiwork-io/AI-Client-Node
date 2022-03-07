import moment from "moment";

export const DEFAULT_DATE_TIME_FORMAT = "DD-MM-YYYY HH:mm";

export function formatTime(
  date: string,
  format = DEFAULT_DATE_TIME_FORMAT
): string {
  return moment(date).local().format(format);
}
