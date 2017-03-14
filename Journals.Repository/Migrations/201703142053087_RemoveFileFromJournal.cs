namespace Journals.Repository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveFileFromJournal : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Journals", "FileName");
            DropColumn("dbo.Journals", "ContentType");
            DropColumn("dbo.Journals", "Content");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Journals", "Content", c => c.Binary());
            AddColumn("dbo.Journals", "ContentType", c => c.String());
            AddColumn("dbo.Journals", "FileName", c => c.String());
        }
    }
}
