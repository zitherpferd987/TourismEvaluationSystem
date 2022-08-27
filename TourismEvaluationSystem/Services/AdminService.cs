using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Web;
using TourismEvaluationSystem.Models;
using TourismEvaluationSystem.Common;

namespace TourismEvaluationSystem.Services
{
    public class AdminService
    {
        public bool ExistAdminByAccountName(string adminAccount)
        {
            bool exist = false;
            using (TourismEvaluationEntity db = new TourismEvaluationEntity())
            {
                if (db.Administrators.Where(ad => ad.AdminAccount.Equals(adminAccount)).Any())
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

        public int AdminPasswordEditSaveByAdminViewModel(AdminPasswordEditViewModel adminPasswordEditViewModel, string adminAccount)
        {
            int flag = 0;
            using (TourismEvaluationEntity db = new TourismEvaluationEntity())
            {
                Administrator administrator = db.Administrators.Where(ad => ad.AdminAccount.Equals(adminAccount)).FirstOrDefault();
                administrator.AdminPassword =  Security.ApplyHash(adminPasswordEditViewModel.AdminPassword);
                flag = db.SaveChanges();
            }
            return flag;
        }

        public Administrator GetAdminByAccountName(string adminAccount)
        {
            Administrator administrator;
            using (TourismEvaluationEntity db = new TourismEvaluationEntity())
            {
                administrator = db.Administrators.Where(ad => ad.AdminAccount.Equals(adminAccount)).FirstOrDefault();
            }
            return administrator;
        }
    }
}