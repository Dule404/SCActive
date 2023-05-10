using System;

namespace backend.Services.Interfaces
{
    public interface IDateTimeService
    {
        DateTime GetDateTimeNow(string zone = "");
        public string GetDateTimeFormat();
    }
}