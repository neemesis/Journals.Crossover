using Journals.Model;
using System.Collections.Generic;

namespace Journals.Repository
{
    public interface IIssueRepository
    {
        List<Issue> GetAllIssues(int? userId);

        OperationStatus AddIssue(Issue issue);

        Issue GetIssueById(int Id);

        OperationStatus DeleteIssue(Issue issue);

        OperationStatus UpdateIssue(Issue issue);
    }
}