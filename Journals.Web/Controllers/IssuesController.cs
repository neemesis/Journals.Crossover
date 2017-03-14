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
    [AuthorizeRedirect(Roles = "Publisher")]
    public class IssuesController : Controller {
        private IIssueRepository _IssueRepository;
        private IStaticMembershipService _membershipService;
        private IJournalRepository _journalRepository;

        public IssuesController(IIssueRepository IssueRepo, IStaticMembershipService membershipService, IJournalRepository JournalRepo) {
            _IssueRepository = IssueRepo;
            _membershipService = membershipService;
            _journalRepository = JournalRepo;
        }

        public ActionResult Index(int? journalId) {
            var id = journalId ?? 1; 
            List<Issue> allIssues = _IssueRepository.GetAllIssues(id);
            Mapper.Initialize(cfg => cfg.CreateMap<Issue, IssueViewModel>());
            var issues = Mapper.Map<List<Issue>, List<IssueViewModel>>(allIssues);
            return View(issues);
        }

        public ActionResult Create() {
            var userId = (int)_membershipService.GetUser().ProviderUserKey;
            ViewBag.Journals = _journalRepository.GetAllJournals(userId);
            return View();
        }

        public ActionResult GetFile(int Id) {
            Issue j = _IssueRepository.GetIssueById(Id);
            if (j == null)
                throw new System.Web.Http.HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound));

            return File(j.Content, j.ContentType);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IssueViewModel Issue, int journalId) {
            if (ModelState.IsValid) {
                Mapper.Initialize(cfg => cfg.CreateMap<IssueViewModel, Issue>());
                var newIssue = Mapper.Map<IssueViewModel, Issue>(Issue);
                JiHelper.PopulateFileIssue(Issue.File, newIssue);

                newIssue.UserId = (int)_membershipService.GetUser().ProviderUserKey;
                // TODO fix
                newIssue.JournalId = 1;

                var opStatus = _IssueRepository.AddIssue(newIssue);
                if (!opStatus.Status)
                    throw new System.Web.Http.HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError));

                return RedirectToAction("Index");
            } else
                return View(Issue);
        }

        public ActionResult Delete(int Id) {
            var selectedIssue = _IssueRepository.GetIssueById(Id);
            Mapper.Initialize(cfg => cfg.CreateMap<Issue, IssueViewModel>());
            var Issue = Mapper.Map<Issue, IssueViewModel>(selectedIssue);
            return View(Issue);
        }

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

        public ActionResult Edit(int Id) {
            var Issue = _IssueRepository.GetIssueById(Id);

            var selectedIssue = Mapper.Map<Issue, IssueUpdateViewModel>(Issue);

            return View(selectedIssue);
        }

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
