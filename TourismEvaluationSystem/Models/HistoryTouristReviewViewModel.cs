using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace TourismEvaluationSystem.Models
{
    public class HistoryTouristReviewViewModel
    {
        [Display(Name = "景点Id")]
        public int ViewPotId { get; set; }

        [Required(ErrorMessage = "必填")]
        [Display(Name = "景点名称")]
        public string ViewPotName { get; set; }
        [Display(Name = "游客描述")]
        public string Suggestion { get; set; }

        [Display(Name = "景色得分")]
        public int ViewScore { get; set; }
        [Display(Name = "服务得分")]
        public int ServiceScore { get; set; }
        [Display(Name = "性价比得分")]
        public int WorthScore { get; set; }
    }
}