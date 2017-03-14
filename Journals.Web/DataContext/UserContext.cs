using Journals.Model;
using System.Data.Entity;

namespace Journals.Web.DataContext
{
    public class UsersContext : DbContext
    {
        public UsersContext()
            : base("JournalsDB")
        {
        }

        
    }
}