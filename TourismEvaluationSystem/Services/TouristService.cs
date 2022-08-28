using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Web;
using TourismEvaluationSystem.Common;
using TourismEvaluationSystem.Models;

namespace TourismEvaluationSystem.Services
{
    public class TouristService
    {
        public bool ExistTouristByAccountName(string touristAccount)
        {
            bool exist = false;
            using (TourismEvaluationEntity db = new TourismEvaluationEntity())
            {
                if (db.Tourists.Where(tr => tr.TouristAccountName.Equals(touristAccount)).Any())
                {
                    exist = true;
                }
                else
                {
                    exist = false;
                }
            }
            return exist;
        }

        public Tourists GetTouristByAccountName(string touristAccount)
        {
            Tourists tourist;
            using (TourismEvaluationEntity db = new TourismEvaluationEntity())
            {
                tourist = db.Tourists.Where(tr => tr.TouristAccountName.Equals(touristAccount)).FirstOrDefault();
            }
            return tourist;
        }

        public ViewPots GetViewPotByVpId(int vpId)
        {
            ViewPots viewPot;
            using (TourismEvaluationEntity db = new TourismEvaluationEntity())
            {
                viewPot = db.ViewPots.Find(vpId);
            }
            return viewPot;
        }

        public int SaveViewPotReview(ViewPotReviews viewPotReviews)
        {
            int flag = 0;
            using (TourismEvaluationEntity db = new TourismEvaluationEntity())
            {
                db.ViewPotReviews.Add(viewPotReviews);
                flag = db.SaveChanges();
            }
            return flag;
        }

        public List<HistoryTouristReviewViewModel> GetHistoryReviews(int touristId)
        {
            List<HistoryTouristReviewViewModel> historyTouristReviewViewModel = new List<HistoryTouristReviewViewModel>();
            using (TourismEvaluationEntity db = new TourismEvaluationEntity())
            {
                var result = from vp in db.ViewPots
                             join vpr in db.ViewPotReviews on vp equals vpr.ViewPots
                             where vpr.TouristId == touristId
                             select new HistoryTouristReviewViewModel
                             {
                                 ViewPotId = vp.ViewPotId,
                                 ServiceScore = vpr.ServiceScore,
                                 Suggestion = vpr.Suggestion,
                                 ViewPotName = vp.ViewPotName,
                                 ViewScore = vpr.ViewScore,
                                 WorthScore = vpr.WorthScore
                             };
                historyTouristReviewViewModel = result.ToList();
            }
            return historyTouristReviewViewModel;
        }

        public int TouristEditSave(TouristEditViewModel touristEditViewModel, string touristAccountName)
        {
            int flag = 0;
            using (TourismEvaluationEntity db = new TourismEvaluationEntity())
            {
                Tourists tourist = db.Tourists.Where(tr => tr.TouristAccountName.Equals(touristAccountName)).FirstOrDefault();
                tourist.TouristPhoneNumber = touristEditViewModel.TouristPhoneNumber;
                tourist.TouristUserName = touristEditViewModel.TouristUserName;
                tourist.TouristPassword = Security.ApplyHash(touristEditViewModel.TouristPassword);
                tourist.TouristAddress = touristEditViewModel.TouristAddress;
                tourist.TouristAvatar = touristEditViewModel.TouristAvatar;
                flag = db.SaveChanges();
            }
            return flag;
        }

        public int SaveTouristByTouristRegisterViewModel(TouristRegisterViewModel touristRegisterViewModel)
        {
            int flag = 0;
            using (TourismEvaluationEntity db = new TourismEvaluationEntity())
            {
                Tourists tourist = new Tourists();
                tourist.TouristAccountName = touristRegisterViewModel.TouristAccountName;
                tourist.TouristUserName= touristRegisterViewModel.TouristUserName;
                tourist.TouristPassword = Security.ApplyHash(touristRegisterViewModel.TouristPassword);
                tourist.TouristPhoneNumber = touristRegisterViewModel.TouristPhoneNumber;
                tourist.TouristAvatar = touristRegisterViewModel.TouristAvatar;
                tourist.TouristAddress = touristRegisterViewModel.TouristAddress;
                db.Tourists.Add(tourist);
                flag = db.SaveChanges();
            }

            return flag;
        }
    }
}