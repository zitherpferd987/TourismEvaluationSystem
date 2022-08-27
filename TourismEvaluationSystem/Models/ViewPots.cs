using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TourismEvaluationSystem.Models
{
    public class ViewPots
    {
        [Key]
        public int ViewPotId { get; set; }
        public string ViewPotName { get; set; }
        public string ViewPotImg { get; set; }
        public string ViewPotDescription { get; set; }

        public virtual List<ViewPotReviews> ViewPotReviews { get; set; }
    }
}