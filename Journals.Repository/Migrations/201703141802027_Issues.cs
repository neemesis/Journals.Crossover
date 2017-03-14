namespace Journals.Repository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Issues : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Issues",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false),
                        Description = c.String(nullable: false),
                        FileName = c.String(),
                        ContentType = c.String(),
                        Content = c.Binary(),
                        ModifiedDate = c.DateTime(nullable: false),
                        UserId = c.Int(nullable: false),
                        JournalId = c.Int(nullable: false),
                        test = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Journals", t => t.JournalId)
                .ForeignKey("dbo.UserProfile", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.JournalId);
            
            CreateTable(
                "dbo.Journals",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false),
                        Description = c.String(nullable: false),
                        FileName = c.String(),
                        ContentType = c.String(),
                        Content = c.Binary(),
                        ModifiedDate = c.DateTime(nullable: false),
                        UserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.UserProfile", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.UserProfile",
                c => new
                    {
                        UserId = c.Int(nullable: false, identity: true),
                        UserName = c.String(),
                    })
                .PrimaryKey(t => t.UserId);
            
            CreateTable(
                "dbo.Subscriptions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        JournalId = c.Int(nullable: false),
                        UserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Journals", t => t.JournalId)
                .ForeignKey("dbo.UserProfile", t => t.UserId)
                .Index(t => t.JournalId)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Subscriptions", "UserId", "dbo.UserProfile");
            DropForeignKey("dbo.Subscriptions", "JournalId", "dbo.Journals");
            DropForeignKey("dbo.Issues", "UserId", "dbo.UserProfile");
            DropForeignKey("dbo.Journals", "UserId", "dbo.UserProfile");
            DropForeignKey("dbo.Issues", "JournalId", "dbo.Journals");
            DropIndex("dbo.Subscriptions", new[] { "UserId" });
            DropIndex("dbo.Subscriptions", new[] { "JournalId" });
            DropIndex("dbo.Journals", new[] { "UserId" });
            DropIndex("dbo.Issues", new[] { "JournalId" });
            DropIndex("dbo.Issues", new[] { "UserId" });
            DropTable("dbo.Subscriptions");
            DropTable("dbo.UserProfile");
            DropTable("dbo.Journals");
            DropTable("dbo.Issues");
        }
    }
}
