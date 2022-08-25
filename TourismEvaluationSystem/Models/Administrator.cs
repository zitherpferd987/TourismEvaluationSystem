using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TourismEvaluationSystem.Models
{
    //口令不得低于六位，需要英文和数字
    public class Administrator
    {
        [Key]
        public int AdminId { get; set; }
        [Display(Name = "管理员账户名")]
        [Required]
        public string AdminAccount { get; set; }
        [Display(Name = "管理员密码")]
        [Required]
        public string AdminPassword { get; set; }
    }
}