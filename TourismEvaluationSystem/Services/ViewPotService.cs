using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TourismEvaluationSystem.Models;

namespace TourismEvaluationSystem.Services
{
    public class ViewPotService
    {
        public List<ViewPots> GetViewPots()
        {
            List<ViewPots> viewPots = new List<ViewPots>();
            using (TourismEvaluationEntity db = new TourismEvaluationEntity())
            {
                viewPots = db.ViewPots.ToList();
            }
            return viewPots;
        }

        public int ViewPotAddByViewModel(CreateViewPotViewModel createViewPotViewModel)
        {
            int flag = 0;
            using (TourismEvaluationEntity db = new TourismEvaluationEntity())
            {
                ViewPots viewPots = new ViewPots();
                viewPots.ViewPotImg = createViewPotViewModel.ViewPotImg;
                viewPots.ViewPotName = createViewPotViewModel.ViewPotName;
                db.ViewPots.Add(viewPots);
                flag = db.SaveChanges();
            }
            return flag;
        }

        public ViewPots GetViewPotByVpId(int vpid)
        {
            ViewPots viewPot;
            using (TourismEvaluationEntity db = new TourismEvaluationEntity())
            {
                viewPot = db.ViewPots.Find(vpid);
            }
            return viewPot;
        }

        public int ViewPotEditSaveByViewModel(EditViewPotViewModel editViewPotViewModel)
        {
            int flag = 0;
            using (TourismEvaluationEntity db = new TourismEvaluationEntity())
            {
                ViewPots editViewPots = new ViewPots();
                editViewPots = db.ViewPots.Find(editViewPotViewModel.ViewPotId);
                editViewPots.ViewPotName = editViewPotViewModel.ViewPotName;
                editViewPots.ViewPotImg = editViewPotViewModel.ViewPotImg;
                flag = db.SaveChanges();

            }
            return flag;
        }

        public int DeleteViewPotByVpId(int vpid)
        {
            int flag = 0;
            using (TourismEvaluationEntity db = new TourismEvaluationEntity())
            {
                ViewPots viewPot = db.ViewPots.Find(vpid);
                db.ViewPots.Remove(viewPot);
                flag = db.SaveChanges();
            }
            return flag;
        }

        public List<ReviewDetailViewModel> GetReviewDetailsByVpId(int vpid)
        {
            List<ReviewDetailViewModel> reviewDetailViewModels;
            using (TourismEvaluationEntity db = new TourismEvaluationEntity())
            {
                var results = from vp in db.ViewPots
                              join vpr in db.ViewPotReviews on vp equals vpr.ViewPots
                              join tr in db.Tourists on vpr.Tourists equals tr
                              where (vp.ViewPotId == vpid)
                              select new ReviewDetailViewModel
                              {
                                  ViewPotReviewId = vpr.ViewPotReviewId,
                                  TouristAccountName = tr.TouristAccountName,
                                  ReviewDateTime = vpr.ReviewDateTime,
                                  ViewScore = vpr.ViewScore,
                                  ServiceScore = vpr.ServiceScore,
                                  WorthScore = vpr.WorthScore,
                              };
                reviewDetailViewModels = results.ToList();
            }
            return reviewDetailViewModels;
        }

        /// <summary>
        /// 景点评论分页函数
        /// </summary>
        /// <param name="PageIndex">第几页</param>
        /// <param name="PageSize">每页记录数</param>
        /// <returns>返回指定页上的景点评论记录</returns>
        public List<ReviewDetailViewModel> GetReviewDetailsPageByVpId(int vpId, int PageIndex, int PageSize)
        {
            List<ReviewDetailViewModel> reviewDetailViewModels = new List<ReviewDetailViewModel>();
            using (TourismEvaluationEntity db = new TourismEvaluationEntity())
            {
                var results = from vp in db.ViewPots
                              join vpr in db.ViewPotReviews on vp equals vpr.ViewPots
                              join tr in db.Tourists on vpr.Tourists equals tr
                              where (vp.ViewPotId == vpId)
                              select new ReviewDetailViewModel
                              {
                                  ViewPotReviewId = vpr.ViewPotReviewId,
                                  TouristAccountName = tr.TouristAccountName,
                                  ReviewDateTime = vpr.ReviewDateTime,
                                  ViewScore = vpr.ViewScore,
                                  ServiceScore = vpr.ServiceScore,
                                  WorthScore = vpr.WorthScore,
                              };
                //降序排列取出指定页记录
                reviewDetailViewModels = results.OrderByDescending(r => r.ViewPotReviewId).Skip((PageIndex - 1) * PageSize).Take(PageSize).ToList();
            }
            return reviewDetailViewModels;
        }

        public int GetReviewDetailsPageCount(int vpId, int PageSize)
        {
            int dbcount;
            using (TourismEvaluationEntity db = new TourismEvaluationEntity())
            {
                dbcount = db.ViewPotReviews.Where(vpr => vpr.ViewPotId.Equals(vpId)).Count();
            }

            if (dbcount % PageSize == 0)
            {
                return dbcount / PageSize;
            }
            else
            {
                return dbcount / PageSize + 1;
            }
        }


        public int DeleteTouristReviewByVprId(int vprId)
        {
            int flag = 0;
            using (TourismEvaluationEntity db = new TourismEvaluationEntity())
            {
                ViewPotReviews viewPotReview = db.ViewPotReviews.Find(vprId);
                db.ViewPotReviews.Remove(viewPotReview);
                flag = db.SaveChanges();
            }

            return flag;
        }

        public List<EvaluationStatisticsViewModel> GetEvaluationStatistics()
        {
            List<EvaluationStatisticsViewModel> evaluationStatisticsViewModels = new List<EvaluationStatisticsViewModel>();
            using (TourismEvaluationEntity db = new TourismEvaluationEntity())
            {
                var vprGroup = from vpr in db.ViewPotReviews
                               group vpr by vpr.ViewPotId into g
                               select new
                               {
                                   key = g.Key,
                                   ReviewCount = g.Count(),
                                   AverageViewScore = g.Average(vpr => vpr.ViewScore),
                                   AverageServiceScore = g.Average(vpr => vpr.ServiceScore),
                                   AverageWorthScore = g.Average(vpr => vpr.WorthScore)
                               };
                var results = from vp in db.ViewPots
                              join vprg in vprGroup on vp.ViewPotId equals vprg.key into vp_vprg
                              from res in vp_vprg.DefaultIfEmpty(new { key = 0, ReviewCount = 0, AverageViewScore = 0.0, AverageServiceScore = 0.0, AverageWorthScore = 0.0 })
                              select new
                              {
                                  key = res.key,
                                  ViewPotName = vp.ViewPotName,
                                  ReviewCount = res.ReviewCount,
                                  AverageViewScore = res.AverageViewScore,
                                  AverageServiceScore = res.AverageServiceScore,
                                  AverageWorthScore = res.AverageWorthScore
                              };


                foreach (var res in results.ToList())
                {
                    EvaluationStatisticsViewModel evaluationStatisticsViewModel = new EvaluationStatisticsViewModel();
                    evaluationStatisticsViewModel.ViewPotId = res.key;
                    evaluationStatisticsViewModel.ViewPotName = res.ViewPotName;
                    evaluationStatisticsViewModel.ReviewCount = res.ReviewCount;
                    evaluationStatisticsViewModel.AverageViewScore = res.AverageViewScore;
                    evaluationStatisticsViewModel.AverageServiceScore = res.AverageServiceScore;
                    evaluationStatisticsViewModel.AverageWorthScore = res.AverageWorthScore;
                    evaluationStatisticsViewModel.AverageTotalScore = (res.AverageViewScore + res.AverageServiceScore + res.AverageWorthScore);
                    evaluationStatisticsViewModels.Add(evaluationStatisticsViewModel);
                }
            }
            return evaluationStatisticsViewModels;
        }
    }

}