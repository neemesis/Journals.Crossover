using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using Journals.Model;
using Journals.Repository;
using Journals.Repository.DataContext;

namespace Journals.Repository {
    public class IssueRepository : RepositoryBase<JournalsContext>, IIssueRepository {

        /// <summary>
        /// Returning all Issues under specific JournalId
        /// </summary>
        /// <param name="journalId">ID of the Journal</param>
        /// <returns>List<Issue></returns>
        public List<Issue> GetAllIssuesWithId(int journalId) {
            using (DataContext) {
                foreach (var ta in DataContext.Issues) {
                    Debug.WriteLine(ta.Title);
                }
                var t = DataContext.Issues.Where(j => j.Id > 0 && j.JournalId == journalId).ToList();
                return t;
            }
        }

        public List<Issue> GetAllIssues(int? id) {
            if (id.HasValue) {
                return GetAllIssuesWithId(id.Value);
            } else {
                return GetAllIssuesList();
            }
        }

        /// <summary>
        /// Get all issues regardless of Journal
        /// </summary>
        /// <returns>List<Issue></returns>
        public List<Issue> GetAllIssuesList() {
            using (DataContext) {
                foreach (var ta in DataContext.Issues) {
                    Debug.WriteLine(ta.Title);
                }
                var t = DataContext.Issues.Where(j => j.Id > 0).ToList();
                return t;
            }
        }

        /// <summary>
        /// Return specific Issue by ID
        /// </summary>
        /// <param name="Id">ID of Issue</param>
        /// <returns></returns>
        public Issue GetIssueById(int Id) {
            var t = DataContext;
            using (t)
                return t.Issues.SingleOrDefault(j => j.Id == Id);
        }

        /// <summary>
        /// Adding new issue.
        /// </summary>
        /// <param name="newissue">Object of Issue class</param>
        /// <returns>OperationStatus</returns>
        public OperationStatus AddIssue(Issue newissue) {
            var opStatus = new OperationStatus { Status = true };
            try {
                using (DataContext) {
                    newissue.ModifiedDate = DateTime.Now;
                    var j = DataContext.Issues.Add(newissue);
                    DataContext.SaveChanges();
                }
            } catch (Exception e) {
                opStatus = OperationStatus.CreateFromException("Error adding issue: ", e);
            }

            return opStatus;
        }

        /// <summary>
        /// Deleting specific issue.
        /// </summary>
        /// <param name="issue">Object of Issue class</param>
        /// <returns>OperationStatus</returns>
        public OperationStatus DeleteIssue(Issue issue) {
            var opStatus = new OperationStatus { Status = true };
            try {
                using (DataContext) {
                    var issueToBeDeleted = DataContext.Issues.Find(issue.Id);
                    DataContext.Issues.Remove(issueToBeDeleted);
                    DataContext.SaveChanges();
                }
            } catch (Exception e) {
                opStatus = OperationStatus.CreateFromException("Error deleting issue: ", e);
            }

            return opStatus;
        }

        /// <summary>
        /// Updating issues.
        /// </summary>
        /// <param name="issue">Issue that needs to be updated.</param>
        /// <returns>OperationStatus</returns>
        public OperationStatus UpdateIssue(Issue issue) {
            var opStatus = new OperationStatus { Status = true };
            try {
                var j = DataContext.Issues.Find(issue.Id);
                if (issue.Title != null)
                    j.Title = issue.Title;

                if (issue.Description != null)
                    j.Description = issue.Description;

                if (issue.Content != null)
                    j.Content = issue.Content;

                if (issue.ContentType != null)
                    j.ContentType = issue.ContentType;

                if (issue.FileName != null)
                    j.FileName = issue.FileName;

                j.ModifiedDate = DateTime.Now;

                DataContext.Entry(j).State = EntityState.Modified;
                DataContext.SaveChanges();
            } catch (DbEntityValidationException e) {
                foreach (var eve in e.EntityValidationErrors) {
                    OperationStatus.CreateFromException(string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:", eve.Entry.Entity.GetType().Name, eve.Entry.State), e);
                }
            } catch (Exception e) {
                opStatus = OperationStatus.CreateFromException("Error updating issue: ", e);
            }

            return opStatus;
        }
    }
}