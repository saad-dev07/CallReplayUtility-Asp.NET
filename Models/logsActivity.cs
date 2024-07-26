using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CallBackUtility.Models
{
    public class logsActivity
    {
        public int id { get; set; }

        public string message { get; set; }

        public string logType { get; set; }

        public string user { get; set; }

        public DateTimeOffset createdAt { get; set; }

        public DateTimeOffset updatedAt { get; set; }
    }
}