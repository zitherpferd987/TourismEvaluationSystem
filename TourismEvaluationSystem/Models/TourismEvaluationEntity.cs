using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace TourismEvaluationSystem.Models
{
    public class TourismEvaluationEntity : DbContext
    {
        public TourismEvaluationEntity() : base("name=connstr")
        {
            //数据库不存在时创建数据库
            //Database.SetInitializer<TourismEvaluationEntity>(new CreateDatabaseIfNotExists<TourismEvaluationEntity>());
            //模型改变时重新创建数据库
            Database.SetInitializer<TourismEvaluationEntity>(new DropCreateDatabaseIfModelChanges<TourismEvaluationEntity>());
            //每次启动应用程序时创建数据库
            //Database.SetInitializer<TourismEvaluationEntity>(new DropCreateDatabaseAlways<TourismEvaluationEntity>());
            //从不创建数据库
            //Database.SetInitializer<TourismEvaluationEntity>(null);
        }

        public DbSet<Tourists> Tourists { get; set; }
        public DbSet<ViewPots> ViewPots { get; set; }
        public DbSet<ViewPotReviews> ViewPotReviews { get; set; }
        public DbSet<Administrator> Administrators { get; set; }
    }
}