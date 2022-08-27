using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TourismEvaluationSystem.Models
{
    public class CreateViewPotViewModel
    {
        [Required(ErrorMessage = "必填")]
        [Display(Name = "景点名称")]
        public string ViewPotName { get; set; }
        [Display(Name = "景点描述")]
        public string ViewPotDescription { get; set; }
        public string ViewPotImg { get; set; }//不作显示，仅供数据传递

        [Required(ErrorMessage = "请选择要上传的图片")]
        [Display(Name = "景点图片")]
        [DataType(DataType.Upload)]
        public HttpPostedFileBase ViewPotImgFile { get; set; }  //注意上传文件类型为HttpPostedFileBase


    }
}