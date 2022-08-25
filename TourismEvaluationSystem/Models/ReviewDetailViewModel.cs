using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace TourismEvaluationSystem.Models
{
    public class ReviewDetailViewModel
    {
        [Display(Name = "投票Id")]
        public int ViewPotReviewId { get; set; }
        [Display(Name = "投票顾客昵称")]
        public string TouristAccountName { get; set; }
        [Display(Name = "投票时间")]
        public DateTime ReviewDateTime { get; set; }
        [Display(Name = "景色得分")]
        public int ViewScore { get; set; }
        [Display(Name = "服务得分")]
        public int ServiceScore { get; set; }
        [Display(Name = "性价比得分")]
        public int WorthScore { get; set; }
    }
}