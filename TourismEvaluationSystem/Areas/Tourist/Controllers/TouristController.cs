using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Web;
using System.Web.Mvc;
using TourismEvaluationSystem.Models;
using TourismEvaluationSystem.Services;
using TourismEvaluationSystem.Common;
using System.Xml.Linq;
using TourismEvaluationSystem.Filters;
using System.Drawing;
using System.Security.Policy;
using System.Text.RegularExpressions;
using static System.Net.WebRequestMethods;


namespace TourismEvaluationSystem.Areas.Tourist.Controllers
{
                           // GET: Admin/Admin
    public class TouristController : Controller
    {
        const int IMGSIZE = 50;//图片大小限制，单位为KB
        // GET: Tourist/Tourist
        [TouristControllerFilterAttribute(Name = "TouristControllerFilterAttribute")]
        public ActionResult Index()
        {
            ViewPotService viewPotService = new ViewPotService();
            ViewData.Model = viewPotService.GetViewPots();
            ViewData["Avatar"] = ((Tourists)Session["Tourist"]).TouristAvatar;
            ViewData["Tourist"] = ((Tourists)Session["Tourist"]).TouristUserName;
            ViewData["Address"] = ((Tourists)Session["Tourist"]).TouristAddress;
            return View();
        }

        public ActionResult TouristLogin(TouristLoginViewModel touristLoginViewModel)
        {
            if (HttpContext.Request.RequestType.Equals("POST"))
            {
                //下面if语句对验证码做后台验证
                if (TempData["VerificationCode"] == null || TempData["VerificationCode"].ToString() != touristLoginViewModel.VerificationCode.ToUpper())
                {
                    ModelState.AddModelError("Message", "验证码不正确");
                    return View(touristLoginViewModel);
                }

                if (ModelState.IsValid)
                {
                    TouristService touristService = new TouristService();
                    if (!touristService.ExistTouristByAccountName(touristLoginViewModel.TouristAccount))
                    {
                        ModelState.AddModelError("Message", "该用户名不存在！");
                    }
                    else
                    {
                        Tourists tourist = touristService.GetTouristByAccountName(touristLoginViewModel.TouristAccount);
                        if (!tourist.TouristPassword.Equals(Security.ApplyHash(touristLoginViewModel.TouristPassword)))
                        {
                            ModelState.AddModelError("Message", "该口令错误！");
                        }
                        else
                        {
                            Session["Tourist"] = tourist;
                            return RedirectToAction("Index");
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError("Message", "请检查登录信息是否符合规范！");
                }
                return View(touristLoginViewModel);
            }
            return View();
        }

        public ActionResult TouristRegister(TouristRegisterViewModel touristRegisterViewModel)
        {

            if (HttpContext.Request.RequestType.Equals("POST"))
            {
                //下面if语句对验证码做后台验证
                if (TempData["VerificationCode"] == null || TempData["VerificationCode"].ToString() != touristRegisterViewModel.VerificationCode.ToUpper())
                {
                    ModelState.AddModelError("Message", "验证码不正确");
                    return View(touristRegisterViewModel);
                }

                if (ModelState.IsValid)
                {
                    TouristService touristService = new TouristService();
                    #region 文件上传处理
                    //接收文件数据
                    HttpPostedFileBase postFile = touristRegisterViewModel.TouristAvatarFile;
                    if (postFile == null)
                        ModelState.AddModelError("Message", "请选择图片后上传！");
                    else
                    {
                        if (postFile.ContentLength > IMGSIZE * 1024)
                            ModelState.AddModelError("Message", string.Format("图片大小不得超过{0}KB！", IMGSIZE));
                        else
                        {
                            string fileName = Path.GetFileName(postFile.FileName); //获取文件名称
                            string fileExt = Path.GetExtension(fileName).ToLower();   //获取文件扩展名称
                            if (!fileExt.Equals(".jpg"))
                                ModelState.AddModelError("Message", "只接受jpg格式的图片！");
                            else
                            {
                                string imgDir = "/Content/Images/";//保存位置的服务器路径（虚拟路径）
                                                                   //新文件主名
                                string newImgName = DateTime.Now.Year + "_" + DateTime.Now.Month + "_"
                                    + DateTime.Now.Day + "_" + DateTime.Now.Hour + "_" + DateTime.Now.Minute
                                    + "_" + DateTime.Now.Second + "_" + DateTime.Now.Millisecond;
                                string fullDir = imgDir + newImgName + fileExt;//完整的虚拟路径与文件名
                                //文件保存                 
                                postFile.SaveAs(Request.MapPath(fullDir));//注意：Request.MapPath()可将虚拟路径转为本机实际物理路径
                                touristRegisterViewModel.TouristAvatar = newImgName + fileExt;
                            }
                        }
                    }
                    #endregion
                    if (touristService.ExistTouristByAccountName(touristRegisterViewModel.TouristAccountName))
                    {
                        ModelState.AddModelError("Message", "用户昵称已存在！");
                    }
                    else
                    {
                        if (!touristRegisterViewModel.TouristPassword.Equals(touristRegisterViewModel.TouristPasswordConfirm))
                        {
                            ModelState.AddModelError("Message", "用户俩次密码不一致！");
                        }
                        else
                        {
                            Regex reg = new Regex(@"^\d{11}$");
                            if (!reg.IsMatch(touristRegisterViewModel.TouristPhoneNumber))
                            {
                                ModelState.AddModelError("Message", "用户手机号码不能出现数字以外的字母！");
                            }
                            else
                            {
                                touristService.SaveTouristByTouristRegisterViewModel(touristRegisterViewModel);
                                return RedirectToAction("Index");
                            }
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError("Message", "请检查注册信息是否符合规范！");

                }
                return View(touristRegisterViewModel);
            }

            return View();
        }


        [TouristControllerFilterAttribute(Name = "TouristControllerFilterAttribute")]
        public ActionResult ViewPotReviewByTourist(int vpId)
        {
            TouristService touristService = new TouristService();
            ViewData.Model = touristService.GetViewPotByVpId(vpId);
            //return Content(touristService.GetViewPotByVpId(vpId).ViewPotName);
            return View();
        }

        [TouristControllerFilterAttribute(Name = "TouristControllerFilterAttribute")]
        public ActionResult ViewPotReviewByTouristSave()
        {
            int vpId = int.Parse(Request.Form["vpId"]);
            int vs = int.Parse(Request.Form["vs"]);
            int ss = int.Parse(Request.Form["ss"]);
            int ws = int.Parse(Request.Form["ws"]);
            string yourAdvice = Request.Form["yourAdvice"];
            TouristService touristService = new TouristService();
            ViewPotReviews viewPotReviews = new ViewPotReviews();
            viewPotReviews.ViewPotId = vpId;
            viewPotReviews.TouristId = ((Tourists)Session["Tourist"]).TouristId;
            viewPotReviews.ReviewDateTime = DateTime.Now;
            viewPotReviews.ViewScore = vs;
            viewPotReviews.ServiceScore = ss;
            viewPotReviews.WorthScore = ws;
            viewPotReviews.Suggestion = yourAdvice;
            int result = touristService.SaveViewPotReview(viewPotReviews);
            return RedirectToAction("Index");
        }

        [TouristControllerFilterAttribute(Name = "TouristControllerFilterAttribute")]
        public ActionResult HistoryEvaluationStatistics()
        {
            int touristId = ((Tourists)Session["Tourist"]).TouristId;
            TouristService touristService = new TouristService();
            ViewData.Model = touristService.GetHistoryReviews(touristId);
            return View();
        }

        [TouristControllerFilterAttribute(Name = "TouristControllerFilterAttribute")]
        public ActionResult TouristLogout()
        {
            Session.Clear();
            return RedirectToAction("Index");
        }

        [TouristControllerFilterAttribute(Name = "TouristControllerFilterAttribute")]
        public ActionResult TouristAccountEdit(TouristEditViewModel touristEditViewModel)
        {
            TouristService touristService = new TouristService();
            if (HttpContext.Request.RequestType.Equals("POST"))
            {
                //下面if语句对验证码做后台验证
                if (TempData["VerificationCode"] == null || TempData["VerificationCode"].ToString() != touristEditViewModel.VerificationCode.ToUpper())
                {
                    ModelState.AddModelError("Message", "验证码不正确");
                    return View(touristEditViewModel);
                }
                if (ModelState.IsValid)
                {
                    #region 文件上传处理
                    //接收文件数据
                    HttpPostedFileBase postFile = touristEditViewModel.TouristAvatarFile;
                    if (postFile == null)
                        ModelState.AddModelError("Message", "请选择图片后上传！");
                    else
                    {
                        if (postFile.ContentLength > IMGSIZE * 1024)
                            ModelState.AddModelError("Message", string.Format("图片大小不得超过{0}KB！", IMGSIZE));
                        else
                        {
                            string fileName = Path.GetFileName(postFile.FileName); //获取文件名称
                            string fileExt = Path.GetExtension(fileName).ToLower();   //获取文件扩展名称
                            if (!fileExt.Equals(".jpg"))
                                ModelState.AddModelError("Message", "只接受jpg格式的图片！");
                            else
                            {
                                string imgDir = "/Content/Images/";//保存位置的服务器路径（虚拟路径）
                                                                   //新文件主名
                                string newImgName = DateTime.Now.Year + "_" + DateTime.Now.Month + "_"
                                    + DateTime.Now.Day + "_" + DateTime.Now.Hour + "_" + DateTime.Now.Minute
                                    + "_" + DateTime.Now.Second + "_" + DateTime.Now.Millisecond;
                                string fullDir = imgDir + newImgName + fileExt;//完整的虚拟路径与文件名
                                #region 删除旧图像
                                string oldImgName = Request.MapPath(imgDir + touristEditViewModel.TouristAvatar);
                                if (System.IO.File.Exists(oldImgName))
                                {
                                    System.IO.File.Delete(oldImgName);
                                }
                                #endregion
                                //文件保存                 
                                postFile.SaveAs(Request.MapPath(fullDir));//注意：Request.MapPath()可将虚拟路径转为本机实际物理路径
                                touristEditViewModel.TouristAvatar = newImgName + fileExt;
                            }
                        }
                    }
                    #endregion
                    if (!touristEditViewModel.TouristPassword.Equals(touristEditViewModel.TouristPasswordConfirm))
                    {
                        ModelState.AddModelError("Message", "俩次密码输入不一致！");
                    }
                    else
                    {
                        Regex reg = new Regex(@"^\d{11}$");
                        if (!reg.IsMatch(touristEditViewModel.TouristPhoneNumber.ToString()))
                        {
                            ModelState.AddModelError("Message", "用户手机号码不能出现数字以外的字母！");
                        }
                        else
                        {
                            touristService.TouristEditSave(touristEditViewModel, ((Tourists)Session["Tourist"]).TouristAccountName);
                            Session.Clear();
                            return RedirectToAction("Index");
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError("Message", "请检查输入内容是否符合规范！");
                }

                return View(touristEditViewModel);
            }
            Tourists tourist = Session["Tourist"] as Tourists;
            touristEditViewModel.TouristAccountName = tourist.TouristAccountName;
            touristEditViewModel.TouristUserName = tourist.TouristUserName;
            touristEditViewModel.TouristPhoneNumber = tourist.TouristPhoneNumber;
            touristEditViewModel.TouristAddress = tourist.TouristAddress;
            touristEditViewModel.TouristAvatar = tourist.TouristAvatar;
            return View(touristEditViewModel);
        }

        /// <summary>
        /// 验证码
        /// </summary>
        /// <returns></returns>
        public ActionResult VerificationCode()
        {
            string verificationCode = Security.CreateVerificationText(4);
            Bitmap _img = Security.CreateVerificationImage(verificationCode, 160, 30);
            _img.Save(Response.OutputStream, System.Drawing.Imaging.ImageFormat.Jpeg);
            TempData["VerificationCode"] = verificationCode.ToUpper();
            return null;
        }
    }
}