using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TourismEvaluationSystem.Models
{
    public class ViewPotReviews
    {
        [Key]
        public int ViewPotReviewId { get; set; }
        public DateTime ReviewDateTime { get; set; }
        public int ViewScore { get; set; }
        public int ServiceScore { get; set; }
        public int WorthScore { get; set; }
        public string Suggestion { get; set; }
        public int ViewPotId { get; set; }
        public int TouristId { get; set; }
        public virtual Tourists Tourists { get; set; }
        public virtual ViewPots ViewPots { get; set; }
    }
}