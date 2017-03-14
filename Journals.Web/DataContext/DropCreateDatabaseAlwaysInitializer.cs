using System.Data.Entity;

namespace Journals.Web.DataContext
{
    public class DropCreateDatabaseAlwaysInitializer : DropCreateDatabaseAlways<JournalsContext>
    {
        protected override void Seed(JournalsContext context)
        {
            DataInitializer.Initialize(context);
            base.Seed(context);
        }
    }
}