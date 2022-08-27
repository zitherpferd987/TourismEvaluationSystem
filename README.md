# TourismEvaluationSystem
MVC project about to charge and use about viewpot comments, there are two tunnel to manage the web, like tourist and admin.
## Function (Tourist)
As a tourist, you can loging the tourism evaluation system to by your own account.
First, you can login or register one to get permissions (that means when you get in Tourist Area, the filter will stop you if you didn't get session about your own id or the session is Null). 
About the main side, you can see lots of viewpots from viewpot model that you can take reviews or watch their details, click in you can find out the side about the viewpot which you get in with an Id (get in by a click buttom will be Get method but if you send those data that it be post to database).
Above the web page, you can have three or four action links to get, you can also view the history your reviews by these viewpots, you can get the more details from the ViewModel. 
Then you can edit your account like your phone number, address or even avatar, it appears on index side. So by that if you finish your edit. you will login again because the session get cleared.
## Function (Admin)
It also be managed by database called admin, if you login with admin account you will access the admin area (deriect to action to index).
you can charge viewpots or even tourist accounts that you can play CRUD with them, its also a basic controll to make a admin that should do.
## TIPS
If you are the first to use this project, and also want to try on you PC, then you should aware on these prepares to ready play them.
First, my project database is basic on MSSQL, and also I didn't use sa (considered about the security problem), which means you should go to `Web.config`, then find:

`connectionStrings`  
		`add name="connstr" connectionString="server=.;uid=zyx1;pwd=zyx123;database=TourismEvaluationDatabase"providerName="System.Data.SqlClient"`  
`connectionStrings`  
From this code, we can get `uid` and `pwd` and the database name, to generate the base, first you should go to visual studio SQL server to create the account or use Microsoft SQL management tool to create it (use SQL verified).  
Make sure you can create successful, then get in `TourismEvaluationEntity.cs` in Model folder, then you will see the four code that you can create.
```cs
            //数据库不存在时创建数据库
            //Database.SetInitializer<TourismEvaluationEntity>(new CreateDatabaseIfNotExists<TourismEvaluationEntity>());
            //模型改变时重新创建数据库
            Database.SetInitializer<TourismEvaluationEntity>(new DropCreateDatabaseIfModelChanges<TourismEvaluationEntity>());
            //每次启动应用程序时创建数据库
            //Database.SetInitializer<TourismEvaluationEntity>(new DropCreateDatabaseAlways<TourismEvaluationEntity>());
            //从不创建数据库
            //Database.SetInitializer<TourismEvaluationEntity>(null);
 ```
If you create the database in first time, you should choose the first code to generate the database, or if you have it but modified, then you can choose the second or the third code.  
Then build it and start, go to `./admin/Admin/Initialize` and wait to pop the content "OK", then all the initialization is finished. and then you can enjoy my MVC peoject.

##### CSDN link
[CSDN](https://blog.csdn.net/weixin_42103865?spm=1010.2135.3001.5343)
