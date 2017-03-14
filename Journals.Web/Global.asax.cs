using AutoMapper;
using Journals.Model;
using Journals.Repository.DataContext;
using Journals.Web.IoC;
using System;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Journals.Web.BackgroundJobs;
using Quartz;
using Quartz.Impl;
using GlobalConfiguration = System.Web.Http.GlobalConfiguration;

namespace Journals.Web
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode,
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        private static SimpleMembershipInitializer _initializer;
        private static object _initializerLock = new object();
        private static bool _isInitialized;

        protected void Application_Start()
        {
            Database.SetInitializer<JournalsContext>(new ModelChangedInitializer());
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuthConfig.RegisterAuth();

            var mappingContainer = IoCMappingContainer.GetInstance();
            DependencyResolver.SetResolver(new IoCScopeContainer(mappingContainer));
            Mapper.Initialize(cfg => cfg.CreateMap<Journal, JournalViewModel>());
            Mapper.Initialize(cfg => cfg.CreateMap<Journal, JournalViewModel>());
            Mapper.Initialize(cfg => cfg.CreateMap<JournalViewModel, Journal>());

            Mapper.Initialize(cfg => cfg.CreateMap<Journal, JournalUpdateViewModel>());
            Mapper.Initialize(cfg => cfg.CreateMap<JournalUpdateViewModel, Journal>());

            Mapper.Initialize(cfg => cfg.CreateMap<Journal, SubscriptionViewModel>());
            Mapper.Initialize(cfg => cfg.CreateMap<SubscriptionViewModel, Journal>());

            LazyInitializer.EnsureInitialized(ref _initializer, ref _isInitialized, ref _initializerLock);

            // SEED users
            new DataContext.ModelChangedInitializer().PSeed(new DataContext.JournalsContext());


            // Daily Emails
            IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();
            scheduler.Start();

            IJobDetail job = JobBuilder.Create<EmailJob>().Build();
            ITrigger trigger = TriggerBuilder.Create()
                .WithDailyTimeIntervalSchedule
                  (s =>
                     s.WithIntervalInHours(24)
                    .OnEveryDay()
                    .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(0, 0))
                  )
                .Build();

            scheduler.ScheduleJob(job, trigger);

            //SendDailyEmails();
        }

        

        protected void Application_Error(object sender, EventArgs e)
        {
            // Get the exception object.
            Exception exc = Server.GetLastError();

            if (exc.GetType() == typeof(HttpException))
            {
                if (exc.Message.Contains("Maximum request length exceeded."))
                    Response.Redirect(String.Format("~/Error/RequestLengthExceeded"));
            }

            Server.ClearError();
        }
    }
}