using System.Collections.Generic;

namespace backend.Models.DbResponse
{
    public class DbResponse
    {
        public bool Status { get; set; }
        public string[] Message { get; set; }
        public IEnumerable<object> Data { get; set; }

        public DbResponse()
        {
            Status = false;
            Message = null;
            Data = null;
        }
    }
}