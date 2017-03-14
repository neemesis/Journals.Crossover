namespace Journals.Repository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EmailForUser : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserProfile", "Email", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserProfile", "Email");
        }
    }
}
