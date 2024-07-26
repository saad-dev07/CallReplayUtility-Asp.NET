using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CallBackUtility.Models
{
    public class session
    {
        public int id { get; set; }

        [Column(TypeName = "ntext")]
        public string inums { get; set; }

        [StringLength(255)]
        public string agentLoginId { get; set; }

        [StringLength(50)]
        public string extension { get; set; }

        [StringLength(250)]
        public string duration { get; set; }

        [StringLength(255)]
        public string parentinum { get; set; }

        public int? n { get; set; }

        [StringLength(50)]
        public string started { get; set; }
    }
}