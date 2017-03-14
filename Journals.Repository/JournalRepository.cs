using Journals.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using Journals.Repository.DataContext;

namespace Journals.Repository {
    public class JournalRepository : RepositoryBase<JournalsContext>, IJournalRepository {
        public List<Journal> GetAllJournals(int userId) {
            using (DataContext) {
                foreach (var ta in DataContext.Journals) {
                    Debug.WriteLine(ta.Title);
                }
                var t = DataContext.Journals.Where(j => j.Id > 0 && j.UserId == userId).ToList();
                return t;
            }
        }

        public List<Issue> GetAllIssues(int journalId) {
            using (DataContext) {
                foreach (var ta in DataContext.Journals) {
                    Debug.WriteLine(ta.Title);
                }
                var t = DataContext.Issues.Where(j => j.Id > 0 && j.JournalId == journalId).ToList();
                return t;
            }
        }

        public Journal GetJournalById(int Id) {
            var t = DataContext;
            using (t)
                return t.Journals.SingleOrDefault(j => j.Id == Id);
        }

        public OperationStatus AddJournal(Journal newJournal) {
            var opStatus = new OperationStatus { Status = true };
            try {
                using (DataContext) {
                    newJournal.ModifiedDate = DateTime.Now;
                    var j = DataContext.Journals.Add(newJournal);
                    DataContext.SaveChanges();
                }
            } catch (Exception e) {
                opStatus = OperationStatus.CreateFromException("Error adding journal: ", e);
            }

            return opStatus;
        }

        public OperationStatus DeleteJournal(Journal journal) {
            var opStatus = new OperationStatus { Status = true };
            try {
                using (DataContext) {
                    var subscriptions = DataContext.Subscriptions.Where(j => j.JournalId == journal.Id);
                    foreach (var subscription in subscriptions) {
                        DataContext.Subscriptions.Remove(subscription);
                    }

                    var journalToBeDeleted = DataContext.Journals.Find(journal.Id);
                    DataContext.Journals.Remove(journalToBeDeleted);
                    DataContext.SaveChanges();
                }
            } catch (Exception e) {
                opStatus = OperationStatus.CreateFromException("Error deleting journal: ", e);
            }

            return opStatus;
        }

        public OperationStatus UpdateJournal(Journal journal) {
            var opStatus = new OperationStatus { Status = true };
            try {
                var j = DataContext.Journals.Find(journal.Id);
                if (journal.Title != null)
                    j.Title = journal.Title;

                if (journal.Description != null)
                    j.Description = journal.Description;

                j.ModifiedDate = DateTime.Now;

                DataContext.Entry(j).State = EntityState.Modified;
                DataContext.SaveChanges();
            } catch (DbEntityValidationException e) {
                foreach (var eve in e.EntityValidationErrors) {
                    OperationStatus.CreateFromException(string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:", eve.Entry.Entity.GetType().Name, eve.Entry.State), e);
                }
            } catch (Exception e) {
                opStatus = OperationStatus.CreateFromException("Error updating journal: ", e);
            }

            return opStatus;
        }
    }
}