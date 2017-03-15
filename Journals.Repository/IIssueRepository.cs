using Journals.Model;
using System.Collections.Generic;

namespace Journals.Repository {
    /// <summary>
    /// Interface that represent the skeleton of IssueRepository but it is used also for testing and mocking.
    /// </summary>
    public interface IIssueRepository {
        List<Issue> GetAllIssues(int? userId);

        OperationStatus AddIssue(Issue issue);

        Issue GetIssueById(int Id);

        OperationStatus DeleteIssue(Issue issue);

        OperationStatus UpdateIssue(Issue issue);
    }
}