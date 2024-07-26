using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CallBackUtility.Models
{
    public class Recording
    {[Key]
        public Int64 RecordId { get; set; }
        [StringLength(10)]
        public string startdayname { get; set; }
        public decimal t_duration { get; set; }
        public string _duration { get; set; }
        [NotMapped]
        public int _year { get; set; }
        [StringLength(10)]
        public string _startdate { get; set; }
        [StringLength(150)]
        public string agentname { get; set; }
        [Required]
        [StringLength(15)]
        public string inum { get; set; }
        public int tarid { get; set; }

        [StringLength(50)]
        public string callingparty { get; set; }
        [StringLength(50)]
        public string calledparty { get; set; }
       
     
     

        [StringLength(100)]
        public string callid { get; set; }





        public int dirn { get; set; }





       
        [NotMapped]
        public int sessionsCount { get; set; }

        [StringLength(150)]
        public string skill { get; set; }
        [StringLength(50)]
        public string services { get; set; }


    }
}