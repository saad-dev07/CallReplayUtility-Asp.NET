using System;
using System.ComponentModel.DataAnnotations;

namespace CallBackUtility.Models
{
    public partial class RecordingVM
    {
        public Int64 RecordId { get; set; }
        public string _startDayName { get; set; }
        public string _startDate { get; set; }

        [StringLength(15)]
        public string inum { get; set; }

        public string dirn{ get; set; }

        public int switchid { get; set; }
        [StringLength(20)]
        public string contactid { get; set; }
      
        public int tarid { get; set; }
        [StringLength(255)]
        public string location { get; set; }
        [StringLength(255)]
        public string nativecallid { get; set; }
        public int archivid { get; set; }
     //   public DateTime startedat { get; set; }

        [StringLength(200)]
        public string callid { get; set; }

        public int? segmentnum { get; set; }
        public Int64 duration { get; set; }

       
        [StringLength(50)]
        public string callingparty { get; set; }

        [StringLength(50)]
        public string services { get; set; }

        [StringLength(150)]
        public string skill { get; set; }


        [StringLength(150)]
        public string agentname { get; set; }
        [StringLength(50)]
        public string agents{ get; set; }

       

     
        
        public int? sessionsCount { get; set; }
       
    }
}
