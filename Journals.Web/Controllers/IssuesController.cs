using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using AutoMapper;
using Journals.Model;
using Journals.Repository;
using Journals.Repository.DataContext;
using Journals.Web.Filters;
using Journals.Web.Helpers;

namespace Journals.Web.Controllers {
    [AuthorizeRedirect(Roles = "Publisher,Subscriber")]
    public class IssuesController : Controller {
        private IIssueRepository _IssueRepository;
        private IStaticMembershipService _membershipService;
        private IJournalRepository _journalRepository;

        /// <summary>
        /// Constructor for IssueController class, with object injections using interfaces
        /// </summary>
        /// <param name="IssueRepo">Repository for Issues</param>
        /// <param name="membershipService">Repository for membership services</param>
        /// <param name="JournalRepo">Repository for Journals</param>
        public IssuesController(IIssueRepository IssueRepo, IStaticMembershipService membershipService, IJournalRepository JournalRepo) {
            _IssueRepository = IssueRepo;
            _membershipService = membershipService;
            _journalRepository = JournalRepo;
        }

        /// <summary>
        /// Index page for showing all or specific issues for Journal
        /// </summary>
        /// <param name="journalId">null: for showing all Issues, or journalId for journal specific issues</param>
        /// <returns></returns>
        public ActionResult Index(int? journalId) {
            var allIssues = _IssueRepository.GetAllIssues(journalId);
            Mapper.Initialize(cfg => cfg.CreateMap<Issue, IssueViewModel>());
            var issues = Mapper.Map<List<Issue>, List<IssueViewModel>>(allIssues);
            return View(issues);
        }

        /// <summary>
        /// Controller for creating Issues
        /// </summary>
        /// <returns></returns>
        public ActionResult Create() {
            var userId = (int)_membershipService.GetUser().ProviderUserKey;
            ViewBag.Journals = _journalRepository.GetAllJournals(userId);
            return View();
        }

        /// <summary>
        /// Function that return File for specific issue.
        /// </summary>
        /// <param name="Id">ID of Issue for searching the file.</param>
        /// <returns></returns>
        public ActionResult GetFile(int Id) {
            Issue j = _IssueRepository.GetIssueById(Id);
            if (j == null)
                throw new System.Web.Http.HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound));

            return File(j.Content, j.ContentType);
        }

        /// <summary>
        /// Post method for creating Issues
        /// </summary>
        /// <param name="Issue">ViewModel representing Issue class</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IssueViewModel Issue) {
            if (ModelState.IsValid) {
                Mapper.Initialize(cfg => cfg.CreateMap<IssueViewModel, Issue>());
                var newIssue = Mapper.Map<IssueViewModel, Issue>(Issue);
                JiHelper.PopulateFileIssue(Issue.File, newIssue);

                newIssue.UserId = (int)_membershipService.GetUser().ProviderUserKey;

                var opStatus = _IssueRepository.AddIssue(newIssue);
                if (!opStatus.Status)
                    throw new System.Web.Http.HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError));

                return RedirectToAction("Index");
            } else
                return View(Issue);
        }

        /// <summary>
        /// View for deliting specific Issue
        /// </summary>
        /// <param name="Id">ID of issue</param>
        /// <returns></returns>
        public ActionResult Delete(int Id) {
            var selectedIssue = _IssueRepository.GetIssueById(Id);
            Mapper.Initialize(cfg => cfg.CreateMap<Issue, IssueViewModel>());
            var Issue = Mapper.Map<Issue, IssueViewModel>(selectedIssue);
            return View(Issue);
        }

        /// <summary>
        /// POST method for deleting Issues
        /// </summary>
        /// <param name="Issue">ViewModel representing the issue</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(IssueViewModel Issue) {
            Mapper.Initialize(cfg => cfg.CreateMap<IssueViewModel, Issue>());
            var selectedIssue = Mapper.Map<IssueViewModel, Issue>(Issue);

            var opStatus = _IssueRepository.DeleteIssue(selectedIssue);
            if (!opStatus.Status)
                throw new System.Web.Http.HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound));

            return RedirectToAction("Index");
        }

        /// <summary>
        /// Returning ViewModel for the Issue.
        /// </summary>
        /// <param name="Id">ID of the Issue</param>
        /// <returns></returns>
        public ActionResult Edit(int Id) {
            var Issue = _IssueRepository.GetIssueById(Id);
            Mapper.Initialize(cfg => cfg.CreateMap<Issue, IssueUpdateViewModel>());
            var selectedIssue = Mapper.Map<Issue, IssueUpdateViewModel>(Issue);

            return View(selectedIssue);
        }

        /// <summary>
        /// POST methd for handling editing of the issues
        /// </summary>
        /// <param name="Issue"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(IssueUpdateViewModel Issue) {
            if (ModelState.IsValid) {
                Mapper.Initialize(cfg => cfg.CreateMap<IssueUpdateViewModel, Issue>());
                var selectedIssue = Mapper.Map<IssueUpdateViewModel, Issue>(Issue);
                JiHelper.PopulateFileIssue(Issue.File, selectedIssue);

                var opStatus = _IssueRepository.UpdateIssue(selectedIssue);
                if (!opStatus.Status)
                    throw new System.Web.Http.HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound));

                return RedirectToAction("Index");
            } else
                return View(Issue);
        }

        protected override void OnException(ExceptionContext filterContext) {
            base.OnException(filterContext);
        }
    }
}
