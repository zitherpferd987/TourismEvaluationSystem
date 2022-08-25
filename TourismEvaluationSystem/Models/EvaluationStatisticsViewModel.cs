using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TourismEvaluationSystem.Models
{
    public class EvaluationStatisticsViewModel
    {
        [Display(Name = "景点Id")]
        public int ViewPotId { get; set; }
        [Display(Name = "景点名称")]
        public string ViewPotName { get; set; }
        [Display(Name = "评论人次")]
        public int ReviewCount { get; set; }
        [Display(Name = "景色平均分")]
        public double AverageViewScore { get; set; }
        [Display(Name = "服务平均分")]
        public double AverageServiceScore { get; set; }
        [Display(Name = "性价比平均分")]
        public double AverageWorthScore { get; set; }
        [Display(Name = "总分")]
        public double AverageTotalScore { get; set; }
    }
}