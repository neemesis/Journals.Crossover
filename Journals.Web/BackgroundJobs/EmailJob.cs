using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using Journals.Model;
using Journals.Repository.DataContext;
using Quartz;

namespace Journals.Web.BackgroundJobs {
    public class EmailJob : IJob {

        public void Execute(IJobExecutionContext context) {
            SendDailyEmails();
        }

        private void SendDailyEmails() {
            var db = new JournalsContext();

            var todayIssues = (from ti in db.Issues where ti.ModifiedDate > DateTime.Today select ti).ToList();

            foreach (var todayIssue in todayIssues) {
                var subscribedUsers = (from s in db.Subscriptions
                                       join u in db.UserProfiles
                                       on s.UserId equals u.UserId
                                       where s.JournalId == todayIssue.JournalId
                                       select u).ToList();

                foreach (var subscribedUser in subscribedUsers) {
                    SendEmail(new EmailSkeleton {
                        User = subscribedUser.UserName,
                        Email = subscribedUser.Email,
                        Subject = "New issue published!",
                        Body = todayIssue.Title
                    });
                }
            }
        }

        private void SendEmail(EmailSkeleton email) {
            using (var message = new MailMessage("journals@gmail.com", email.Email)) {
                message.Subject = email.Subject;
                message.Body = email.Body;
                using (var client = new SmtpClient {
                    EnableSsl = true,
                    Host = "smtp.gmail.com",
                    Port = 587,
                    Credentials = new NetworkCredential("journals@gmail.com", "Passw0rd")
                }) {
                    client.Send(message);
                }
            }
        }
    }

    public class EmailSkeleton {
        public string User { get; set; }
        public string Email { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
    }
}