﻿using WebMatrix.WebData;

namespace Journals.Web.DataContext
{
    public class SimpleMembershipInitializer
    {
        public SimpleMembershipInitializer()
        {
            // what is this?
            using (var context1 = new JournalsContext())
                context1.Journals.Find(1);

            using (var context = new JournalsContext())
                context.UserProfiles.Find(1);

            if (!WebSecurity.Initialized)
                WebSecurity.InitializeDatabaseConnection("JournalsDB", "UserProfile", "UserId", "UserName", autoCreateTables: true);
        }
    }
}