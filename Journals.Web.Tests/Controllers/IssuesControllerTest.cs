using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Security;
using AutoMapper;
using Journals.Model;
using Journals.Repository;
using Journals.Web.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.JustMock;

namespace Medico.Web.Tests.Controllers {
    [TestClass]
    public class IssuesControllerTest {

        [TestMethod]
        public void Index_Returns_All_Issues() {
            Mapper.Initialize(cfg => cfg.CreateMap<Issue, IssueViewModel>());

            //Arrange
            var membershipRepository = Mock.Create<IStaticMembershipService>();
            var userMock = Mock.Create<MembershipUser>();
            Mock.Arrange(() => userMock.ProviderUserKey).Returns(1);
            Mock.Arrange(() => membershipRepository.GetUser()).Returns(userMock);
            int? userId = null;
            var journalRepository = Mock.Create<IJournalRepository>();
            var issueRepository = Mock.Create<IIssueRepository>();
            Mock.Arrange(() => issueRepository.GetAllIssues(userId)).Returns(new List<Issue>(){
                    new Issue{ Id=1, Description="TestDesc", Title="Tester", UserId=1, ModifiedDate= DateTime.Now},
                    new Issue{ Id=2, Description="TestDesc2", Title="Tester2", UserId=1, ModifiedDate = DateTime.Now}
            }).MustBeCalled();

            //Act
            IssuesController controller = new IssuesController(issueRepository, membershipRepository, journalRepository);
            ViewResult actionResult = (ViewResult)controller.Index(null);
            var model = actionResult.Model as IEnumerable<IssueViewModel>;

            //Assert
            Assert.AreEqual(2, model.Count());
        }

        [TestMethod]
        public void Test_Create_ViewBag() {
            Mapper.Initialize(cfg => cfg.CreateMap<Issue, IssueViewModel>());

            //Arrange
            var membershipRepository = Mock.Create<IStaticMembershipService>();
            var journalRepository = Mock.Create<IJournalRepository>();
            var issueRepository = Mock.Create<IIssueRepository>();
            var userMock = Mock.Create<MembershipUser>();
            Mock.Arrange(() => userMock.ProviderUserKey).Returns(1);
            Mock.Arrange(() => membershipRepository.GetUser()).Returns(userMock);
            int? userId = null;

            Mock.Arrange(() => journalRepository.GetAllJournals((int)userMock.ProviderUserKey)).Returns(new List<Journal>(){
                    new Journal{ Id=1, Description="TestDesc", Title="Tester", UserId=1, ModifiedDate= DateTime.Now},
                    new Journal{ Id=2, Description="TestDesc2", Title="Tester2", UserId=1, ModifiedDate = DateTime.Now}
            }).MustBeCalled();

            var controller = new IssuesController(issueRepository, membershipRepository, journalRepository);

            var actionResult = (ViewResult) controller.Create();
            var model = actionResult.ViewBag.Journals;

            //Assert
            Assert.AreEqual(2, model.Count);
        }

        [TestMethod]
        public void Index_Return_IssuesByID() {
            Mapper.Initialize(cfg => cfg.CreateMap<Issue, IssueViewModel>());

            //Arrange
            var membershipRepository = Mock.Create<IStaticMembershipService>();
            var journalRepository = Mock.Create<IJournalRepository>();
            var issueRepository = Mock.Create<IIssueRepository>();
            var userMock = Mock.Create<MembershipUser>();
            Mock.Arrange(() => userMock.ProviderUserKey).Returns(1);
            Mock.Arrange(() => membershipRepository.GetUser()).Returns(userMock);
            int? userId = null;

            Mock.Arrange(() => issueRepository.GetAllIssues(userId)).Returns(new List<Issue>(){
                    new Issue{ Id=1, Description="TestDesc", Title="Tester", UserId=1, ModifiedDate= DateTime.Now},
                    new Issue{ Id=5, Description="TestDesc2", Title="Tester2", UserId=1, ModifiedDate = DateTime.Now}
            });

            Mock.Arrange(() => issueRepository.GetAllIssues(5)).Returns(new List<Issue>(){
                    new Issue{ Id=5, Description="TestDesc2", Title="Tester2", UserId=1, ModifiedDate = DateTime.Now}
            }).MustBeCalled();

            var controller = new IssuesController(issueRepository, membershipRepository, journalRepository);
            var actionResult = (ViewResult)controller.Index(5);
            var model = actionResult.Model as IEnumerable<IssueViewModel>;

            //Assert
            Assert.AreEqual(1, model.Count());
        }
    }
}
