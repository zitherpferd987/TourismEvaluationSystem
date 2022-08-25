using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TourismEvaluationSystem.Models
{
    public class Tourists
    {
        [Key]
        public int TouristId { get; set; }
        public string TouristAccountName { get; set; }
        public string TouristUserName { get; set; }
        public string TouristPassword { get; set; }
        public int TouristPhoneNumber { get; set; }
        public string TouristAddress { get; set; }
        public virtual List<ViewPotReviews> ViewPotReviews { get; set; }
    }
}