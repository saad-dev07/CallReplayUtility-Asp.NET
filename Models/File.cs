using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CallBackUtility.Models
{
    public class File
    {
        [Key]
        public int Id { get; set; }
        public DateTime LoggedOn { get; set; }
        [StringLength(50)]
        public string TarFIleName { get; set; }
        [StringLength(500)]
        public string FilePath { get; set; }
        
        [StringLength(100)]
        public string FileName { get; set; }
        [StringLength(500)]
        public string TarFilePath { get; set; }
    }
}