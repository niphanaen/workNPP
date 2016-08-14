using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PPcore.Models
{
    public partial class SecurityMenus
    {
        public Guid MenuId { get; set; }
        public string MenuName { get; set; }
        public string MenuController { get; set; }
        public string MenuAction { get; set; }

        [Display(Name = "สร้างโดย")]
        public Guid CreatedBy { get; set; }

        [Display(Name = "วันที่สร้าง")]
        //[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd MMMM yyyy}")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd-MM-yyyy}")]
        public DateTime CreatedDate { get; set; }

        [Display(Name = "แก้ไขโดย")]
        public Guid EditedBy { get; set; }

        [Display(Name = "วันที่แก้ไข")]
        //[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd MMMM yyyy}")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd-MM-yyyy}")]
        public DateTime EditedDate { get; set; }

        public string x_status { get; set; }
        public string x_note { get; set; }
        public string x_log { get; set; }
    }
}
