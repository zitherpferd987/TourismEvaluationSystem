using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace TourismEvaluationSystem.Models
{
    public class TouristRegisterViewModel
    {
        [Required(ErrorMessage = "必填")]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "{2}到{1}个字符")]
        [Display(Name = "*用户账户")]
        public string TouristAccountName { get; set; }
        [Required(ErrorMessage = "必填")]
        [Display(Name = "*用户昵称")]
        [StringLength(20, MinimumLength = 2, ErrorMessage = "{2}到{1}个字符")]
        public string TouristUserName { get; set; }
        [Required(ErrorMessage = "必填")]
        [Display(Name = "*用户密码")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "{2}到{1}个字符")]
        [DataType(DataType.Password)]
        public string TouristPassword { get; set; }
        [Required(ErrorMessage = "必填")]
        [Display(Name = "*确认用户密码")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "{2}到{1}个字符")]
        [DataType(DataType.Password)]
        public string TouristPasswordConfirm { get; set; }
        [Required(ErrorMessage = "必填")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "{2}到{1}个字符")]
        [Display(Name = "*用户手机号")]
        [DataType(DataType.Password)]
        public string TouristPhoneNumber { get; set; }
        [Display(Name = "用户住址")]
        public string TouristAddress { get; set; }
        public string TouristAvatar { get; set; }

        [Required(ErrorMessage = "请选择要上传的图片")]
        [Display(Name = "*游客头像")]
        [DataType(DataType.Upload)]
        public HttpPostedFileBase TouristAvatarFile { get; set; }  //注意上传文件类型为HttpPostedFileBase
        /// <summary>
        /// 验证码
        /// </summary>
        [Required(ErrorMessage = "必填")]
        [StringLength(4, MinimumLength = 4, ErrorMessage = "验证码不正确")]
        [Display(Name = "*验证码")]
        public string VerificationCode { get; set; }
    }
}