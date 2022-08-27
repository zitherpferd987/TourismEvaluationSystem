using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TourismEvaluationSystem.Models
{
    public class TouristEditViewModel
    {
        [Display(Name = "用户账号")]
        public string TouristAccountName { get; set; }
        [Display(Name = "用户名")]
        [Required(ErrorMessage = "必填")]
        [StringLength(20, MinimumLength = 2, ErrorMessage = "{2}到{1}个字符")]
        public string TouristUserName { get; set; }
        [Display(Name = "用户密码")]
        [Required(ErrorMessage = "必填")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "{2}到{1}个字符")]
        [DataType(DataType.Password)]
        public string TouristPassword { get; set; }
        [Display(Name = "确认用户密码")]
        [Required(ErrorMessage = "必填")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "{2}到{1}个字符")]
        [DataType(DataType.Password)]
        public string TouristPasswordConfirm { get; set; }
        [Display(Name = "用户手机号")]
        public string TouristPhoneNumber { get; set; }
        [Display(Name = "用户住址")]
        public string TouristAddress { get; set; }
        [Display(Name = "用户头像")]
        public string TouristAvatar { get; set; }

        [Required(ErrorMessage = "请选择要上传的图片")]
        [Display(Name = "游客头像")]
        [DataType(DataType.Upload)]
        public HttpPostedFileBase TouristAvatarFile { get; set; }  //注意上传文件类型为HttpPostedFileBase
        /// <summary>
        /// 验证码
        /// </summary>
        [Required(ErrorMessage = "必填")]
        [StringLength(4, MinimumLength = 4, ErrorMessage = "验证码不正确")]
        [Display(Name = "验证码")]
        public string VerificationCode { get; set; }
    }
}