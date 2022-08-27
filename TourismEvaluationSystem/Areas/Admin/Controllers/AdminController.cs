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

namespace TourismEvaluationSystem.Areas.Admin.Controllers
{
    public class AdminController : Controller
    {
        const int IMGSIZE = 50;//图片大小限制，单位为KB
        // GET: Admin/Admin
        [AdministratorControllerFilterAttribute(Name = "AdministratorControllerFilterAttribute")]
        public ActionResult Index()
        {
            ViewPotService viewPotService = new ViewPotService();
            ViewData.Model = viewPotService.GetViewPots();
            return View();
        }

        public ActionResult AdminLogin(AdminLoginViewModel adminLoginViewModel)
        {
            if (HttpContext.Request.RequestType.Equals("POST"))
            {
                //下面if语句对验证码做后台验证
                if (TempData["VerificationCode"] == null || TempData["VerificationCode"].ToString() != adminLoginViewModel.VerificationCode.ToUpper())
                {
                    ModelState.AddModelError("Message", "验证码不正确");
                    return View(adminLoginViewModel);
                }

                if (ModelState.IsValid)
                {
                    AdminService adminService = new AdminService();
                    if (!adminService.ExistAdminByAccountName(adminLoginViewModel.AdminAccount))
                    {
                        ModelState.AddModelError("Message", "该用户名不存在！");
                    }
                    else
                    {
                        Administrator administrator = adminService.GetAdminByAccountName(adminLoginViewModel.AdminAccount);
                        if (!administrator.AdminPassword.Equals(Security.ApplyHash(adminLoginViewModel.AdminPassword)))
                        {
                            ModelState.AddModelError("Message", "该口令错误！");
                        }
                        else
                        {
                            Session["Admin"] = administrator;
                            return RedirectToAction("Index");
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError("Message", "请检查登录信息是否符合规范！");
                }
                return View(adminLoginViewModel);
            }
            return View();
        }

        public ActionResult AdminPasswordEdit(AdminPasswordEditViewModel adminPasswordEditViewModel)
        {
            if (HttpContext.Request.RequestType.Equals("POST"))
            {
                if (ModelState.IsValid)
                {
                    if (!adminPasswordEditViewModel.AdminPassword.Equals(adminPasswordEditViewModel.AdminPasswordConfirmed))
                    {
                        ModelState.AddModelError("Message", "俩次密码输入不一致！");
                    }
                    else
                    {
                        AdminService adminService = new AdminService();
                        adminService.AdminPasswordEditSaveByAdminViewModel(adminPasswordEditViewModel, ((Administrator)Session["Admin"]).AdminAccount);
                        Session.Clear();
                        return RedirectToAction("Index");
                    }
                }
                else
                {
                    ModelState.AddModelError("Message", "请检查输入内容是否符合规范！");
                }

                return View(adminPasswordEditViewModel);
            }
            Administrator administrator = Session["Admin"] as Administrator;
            return View(adminPasswordEditViewModel);
        }

        [AdministratorControllerFilterAttribute(Name = "AdministratorControllerFilterAttribute")]
        public ActionResult CreateViewPot(CreateViewPotViewModel createViewPotViewModel)
        {
            if (HttpContext.Request.RequestType.Equals("POST"))
            {
                if (ModelState.IsValid)
                {
                    //return Content("ok");
                    #region 文件上传处理
                    //接收文件数据
                    HttpPostedFileBase postFile = createViewPotViewModel.ViewPotImgFile;
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
                                createViewPotViewModel.ViewPotImg = newImgName + fileExt;//给菜品名字段赋值
                                ViewPotService viewPotService = new ViewPotService();
                                int result = viewPotService.ViewPotAddByViewModel(createViewPotViewModel);
                                return RedirectToAction("Index");
                            }
                        }
                    }
                    #endregion
                }
                else
                {
                    ModelState.AddModelError("Message", "请检查信息是否符合填写规则！");
                }
                return View(createViewPotViewModel);
            }
            return View();
        }

        [AdministratorControllerFilterAttribute(Name = "AdministratorControllerFilterAttribute")]
        public ActionResult EditViewPot(EditViewPotViewModel editViewPotViewModel)
        {
            ViewPotService viewPotService = new ViewPotService();
            if (HttpContext.Request.RequestType.Equals("POST"))
            {
                if (ModelState.IsValid)
                {
                    #region 文件上传处理
                    //接收文件数据
                    HttpPostedFileBase postFile = editViewPotViewModel.ViewPotImgFile;
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
                                string oldImgName = Request.MapPath(imgDir + editViewPotViewModel.ViewPotImg);
                                //return Content(editViewPotViewModel.ViewPotImg);
                                if (System.IO.File.Exists(oldImgName))
                                {
                                    System.IO.File.Delete(oldImgName);
                                }
                                #endregion
                                //文件保存                 
                                postFile.SaveAs(Request.MapPath(fullDir));//注意：Request.MapPath()可将虚拟路径转为本机实际物理路径
                                editViewPotViewModel.ViewPotImg = newImgName + fileExt;//给菜品名字段赋值
                                int result = viewPotService.ViewPotEditSaveByViewModel(editViewPotViewModel);
                                return RedirectToAction("Index");
                            }
                        }
                    }
                    #endregion
                }
                else
                {
                    ModelState.AddModelError("Message", "请检查信息是否符合填写规则！");
                }
                return View(editViewPotViewModel);
            }

            int vpid = int.Parse(Request.QueryString["vpid"].ToString());
            ViewPots viewPot = new ViewPots();
            viewPot = viewPotService.GetViewPotByVpId(vpid);
            editViewPotViewModel.ViewPotId = vpid;
            editViewPotViewModel.ViewPotName = viewPot.ViewPotName;
            editViewPotViewModel.ViewPotImg = viewPot.ViewPotImg;
            editViewPotViewModel.ViewPotDescription = viewPot.ViewPotDescription;


            return View(editViewPotViewModel);
        }

        [AdministratorControllerFilterAttribute(Name = "AdministratorControllerFilterAttribute")]
        public ActionResult DeleteViewPot(int vpid)
        {
            ViewPotService viewPotService = new ViewPotService();
            #region 删除旧图像
            string imgDir = "/Content/Images/";
            ViewPots viewPots = viewPotService.GetViewPotByVpId(vpid);
            string oldImgName = Request.MapPath(imgDir + viewPots.ViewPotImg);
            //return Content(oldImgName);
            if (System.IO.File.Exists(oldImgName))
            {
                System.IO.File.Delete(oldImgName);
            }
            #endregion
            int result = viewPotService.DeleteViewPotByVpId(vpid);
            return RedirectToAction("Index");
        }

        [AdministratorControllerFilterAttribute(Name = "AdministratorControllerFilterAttribute")]
        public ActionResult ShowReviewDetails()
        {
            int vpId = int.Parse(Request.QueryString["vpId"].ToString());
            ViewPotService viewPotService = new ViewPotService();
            #region 不分页显示
            //ViewData.Model = viewPotService.GetReviewDetailsByVpId(vpId);
            #endregion

            #region 分页显示
            int PageIndex = 1;
            int PageSize = 5;
            string pis = Request.QueryString["pis"] as string;
            if (pis != null)
                PageIndex = int.Parse(pis);
            ViewData.Model = viewPotService.GetReviewDetailsPageByVpId(vpId, PageIndex, PageSize);
            ViewData["pageCount"] = viewPotService.GetReviewDetailsPageCount(vpId, PageSize);
            #endregion

            ViewData["ViewPotName"] = viewPotService.GetViewPotByVpId(vpId).ViewPotName;
            ViewData["vpId"] = vpId;
            return View();
        }

        [AdministratorControllerFilterAttribute(Name = "AdministratorControllerFilterAttribute")]
        public ActionResult DeleteTouristReview()
        {
            int vprId = int.Parse(Request.QueryString["vprId"].ToString());
            int vpId = int.Parse(Request.QueryString["vpId"].ToString());
            ViewPotService viewPotService = new ViewPotService();
            int result = viewPotService.DeleteTouristReviewByVprId(vprId);
            return RedirectToAction("ShowReviewDetails", "Admin", new { vpId = vpId });
        }

        [AdministratorControllerFilterAttribute(Name = "AdministratorControllerFilterAttribute")]
        public ActionResult EvaluationStatistics()
        {
            ViewPotService viewPotService = new ViewPotService();
            ViewData.Model = viewPotService.GetEvaluationStatistics();
            return View();
        }

        [AdministratorControllerFilterAttribute(Name = "AdministratorControllerFilterAttribute")]
        public ActionResult AdminLogout()
        {
            Session.Clear();
            return RedirectToAction("Index");
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

        //[AdministratorControllerFilterAttribute(Name = "AdministratorControllerFilterAttribute")]
        public ActionResult Initialize()
        {
            using (TourismEvaluationEntity db = new TourismEvaluationEntity())
            {
                db.Database.ExecuteSqlCommand("Delete From ViewPots");
                db.Database.ExecuteSqlCommand("Delete From Tourists");
                db.Database.ExecuteSqlCommand("Delete From Administrators");

                Tourists tourist1 = new Tourists { TouristAccountName = "zhangsan", TouristUserName = "张三", TouristPassword = Security.ApplyHash("zhangsan123"), TouristAddress = "上海", TouristPhoneNumber = "11451419198" };
                Tourists tourist2 = new Tourists { TouristAccountName = "lisi123", TouristUserName = "李四", TouristPassword = Security.ApplyHash("lisi123") };
                Tourists tourist3 = new Tourists { TouristAccountName = "wangwu", TouristUserName = "王五", TouristPassword = Security.ApplyHash("wangwu123") };
                db.Tourists.Add(tourist1);
                db.Tourists.Add(tourist2);
                db.Tourists.Add(tourist3);
                db.SaveChanges();

                ViewPots viewPot1 = new ViewPots { ViewPotName = "黄山", ViewPotImg = "HuangShan.jpg", ViewPotDescription = "黃山位於中國安徽省南部黃山市境內，南北長約40公里，東西寬約30公里，山脈面積1200平方公里，核心景區面積約160.6平方公里，主體以花崗岩構成，最高處蓮花峰，海拔1864米。" };
                ViewPots viewPot2 = new ViewPots { ViewPotName = "秦皇陵", ViewPotImg = "QinHuangLin.jpg", ViewPotDescription = "秦始皇帝陵是秦朝始皇帝的陵墓，位於中國陝西省西安市市中心以東31公里臨潼區驪山，原名驪山園。現存陵冢高76米，陵冢位於內城西南，坐西面東，放置棺槨和陪葬器物的地方為秦始皇陵建築群的核心，目前尚未發掘。" };
                ViewPots viewPot3 = new ViewPots { ViewPotName = "靜安寺", ViewPotImg = "NingHai.jpg", ViewPotDescription = "靜安寺是一座佛教寺廟，位於中國上海市靜安區南京西路1686號，其歷史可以追溯到公元3世紀的三國時期，是中國江南地區歷史悠久、頗具影響的名剎之一。上海四大佛寺之一。" };
                db.ViewPots.Add(viewPot1);
                db.ViewPots.Add(viewPot2);
                db.ViewPots.Add(viewPot3);
                db.SaveChanges();

                ViewPotReviews viewPotReview1 = new ViewPotReviews { ReviewDateTime = DateTime.Now, ViewScore = 5, ServiceScore = 4, WorthScore = 3, Tourists = tourist1, ViewPots = viewPot1 };
                ViewPotReviews viewPotReview2 = new ViewPotReviews { ReviewDateTime = DateTime.Now, ViewScore = 1, ServiceScore = 5, WorthScore = 4, Tourists = tourist2, ViewPots = viewPot2 };
                ViewPotReviews viewPotReview3 = new ViewPotReviews { ReviewDateTime = DateTime.Now, ViewScore = 2, ServiceScore = 3, WorthScore = 3, Tourists = tourist3, ViewPots = viewPot3 };
                ViewPotReviews viewPotReview4 = new ViewPotReviews { ReviewDateTime = DateTime.Now, ViewScore = 1, ServiceScore = 4, WorthScore = 1, Tourists = tourist3, ViewPots = viewPot1 };
                ViewPotReviews viewPotReview5 = new ViewPotReviews { ReviewDateTime = DateTime.Now, ViewScore = 1, ServiceScore = 3, WorthScore = 3, Tourists = tourist1, ViewPots = viewPot1 };
                ViewPotReviews viewPotReview6 = new ViewPotReviews { ReviewDateTime = DateTime.Now, ViewScore = 4, ServiceScore = 4, WorthScore = 3, Tourists = tourist2, ViewPots = viewPot1 };
                ViewPotReviews viewPotReview7 = new ViewPotReviews { ReviewDateTime = DateTime.Now, ViewScore = 2, ServiceScore = 3, WorthScore = 1, Tourists = tourist1, ViewPots = viewPot1 };
                ViewPotReviews viewPotReview8 = new ViewPotReviews { ReviewDateTime = DateTime.Now, ViewScore = 2, ServiceScore = 3, WorthScore = 1, Tourists = tourist1, ViewPots = viewPot1 };
                ViewPotReviews viewPotReview9 = new ViewPotReviews { ReviewDateTime = DateTime.Now, ViewScore = 2, ServiceScore = 3, WorthScore = 1, Tourists = tourist3, ViewPots = viewPot1 };
                ViewPotReviews viewPotReview10 = new ViewPotReviews { ReviewDateTime = DateTime.Now, ViewScore = 2, ServiceScore = 3, WorthScore = 1, Tourists = tourist2, ViewPots = viewPot1 };
                db.ViewPotReviews.Add(viewPotReview1);
                db.ViewPotReviews.Add(viewPotReview2);
                db.ViewPotReviews.Add(viewPotReview3);
                db.ViewPotReviews.Add(viewPotReview4);
                db.ViewPotReviews.Add(viewPotReview5);
                db.ViewPotReviews.Add(viewPotReview6);
                db.ViewPotReviews.Add(viewPotReview7);
                db.ViewPotReviews.Add(viewPotReview8);
                db.ViewPotReviews.Add(viewPotReview9);
                db.ViewPotReviews.Add(viewPotReview10);
                db.SaveChanges();

                Administrator administrator1 = new Administrator { AdminAccount = "zyx", AdminPassword = Security.ApplyHash("zyx123") };
                db.Administrators.Add(administrator1);
                db.SaveChanges();
            }

            return Content("Ok!");
        }
    }
}