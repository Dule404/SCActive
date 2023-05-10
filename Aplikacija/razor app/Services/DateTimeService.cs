using System;
using backend.Services.Interfaces;

namespace backend.Services
{

        public class DateTimeService : IDateTimeService
        {
        
            private const int Difference = +1;
            private DateTime GetDtNow(string zone = "") {
                DateTime now = DateTime.UtcNow;
                try {
                    TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById("Central European Summer Time");
                    if (!string.IsNullOrEmpty(zone)) {
                        timeZone = TimeZoneInfo.FindSystemTimeZoneById(zone);    
                    }
                    now = TimeZoneInfo.ConvertTimeFromUtc(now, timeZone);
                }
                catch (TimeZoneNotFoundException) {
                    now = now.AddHours(Difference);
                }
                catch (InvalidTimeZoneException) {
                    now = now.AddHours(Difference);
                }
                return now;
            }
        
            public DateTime GetDateTimeNow(string zone = "")
            {
                DateTime now = GetDtNow(zone);
                return now.AddMinutes(-10);
            }
            public string GetDateTimeFormat()
            {
                return "MM/dd/yyyy HH:mm:ss.fff";
            }
        }
}