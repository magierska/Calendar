namespace DataAccesLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class databaseMigration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ColorEs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        R = c.Int(nullable: false),
                        G = c.Int(nullable: false),
                        B = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.EMailAddresses",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Address = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.EventApprovals",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        EventId = c.Int(nullable: false),
                        Accepted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Events", t => t.EventId)
                .ForeignKey("dbo.Users", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.EventId);
            
            CreateTable(
                "dbo.Events",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Start = c.DateTime(nullable: false),
                        End = c.DateTime(nullable: false),
                        Deleted = c.Boolean(nullable: false),
                        OwnerId = c.Int(nullable: false),
                        Canceled = c.Boolean(nullable: false),
                        Type_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.OwnerId)
                .ForeignKey("dbo.Types", t => t.Type_Id)
                .Index(t => t.OwnerId)
                .Index(t => t.Type_Id);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserName = c.String(),
                        Password = c.String(),
                        MailId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.EMailAddresses", t => t.MailId)
                .Index(t => t.MailId);
            
            CreateTable(
                "dbo.Plans",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Deleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Types",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Color_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ColorEs", t => t.Color_Id)
                .Index(t => t.Color_Id);
            
            CreateTable(
                "dbo.PlanEvent",
                c => new
                    {
                        PlanId = c.Int(nullable: false),
                        EventId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.PlanId, t.EventId })
                .ForeignKey("dbo.Plans", t => t.PlanId, cascadeDelete: true)
                .ForeignKey("dbo.Events", t => t.EventId, cascadeDelete: true)
                .Index(t => t.PlanId)
                .Index(t => t.EventId);
            
            CreateTable(
                "dbo.PlanUsers",
                c => new
                    {
                        UserId = c.Int(nullable: false),
                        PlanId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.UserId, t.PlanId })
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.Plans", t => t.PlanId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.PlanId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Events", "Type_Id", "dbo.Types");
            DropForeignKey("dbo.Types", "Color_Id", "dbo.ColorEs");
            DropForeignKey("dbo.Events", "OwnerId", "dbo.Users");
            DropForeignKey("dbo.PlanUsers", "PlanId", "dbo.Plans");
            DropForeignKey("dbo.PlanUsers", "UserId", "dbo.Users");
            DropForeignKey("dbo.PlanEvent", "EventId", "dbo.Events");
            DropForeignKey("dbo.PlanEvent", "PlanId", "dbo.Plans");
            DropForeignKey("dbo.Users", "MailId", "dbo.EMailAddresses");
            DropForeignKey("dbo.EventApprovals", "UserId", "dbo.Users");
            DropForeignKey("dbo.EventApprovals", "EventId", "dbo.Events");
            DropIndex("dbo.PlanUsers", new[] { "PlanId" });
            DropIndex("dbo.PlanUsers", new[] { "UserId" });
            DropIndex("dbo.PlanEvent", new[] { "EventId" });
            DropIndex("dbo.PlanEvent", new[] { "PlanId" });
            DropIndex("dbo.Types", new[] { "Color_Id" });
            DropIndex("dbo.Users", new[] { "MailId" });
            DropIndex("dbo.Events", new[] { "Type_Id" });
            DropIndex("dbo.Events", new[] { "OwnerId" });
            DropIndex("dbo.EventApprovals", new[] { "EventId" });
            DropIndex("dbo.EventApprovals", new[] { "UserId" });
            DropTable("dbo.PlanUsers");
            DropTable("dbo.PlanEvent");
            DropTable("dbo.Types");
            DropTable("dbo.Plans");
            DropTable("dbo.Users");
            DropTable("dbo.Events");
            DropTable("dbo.EventApprovals");
            DropTable("dbo.EMailAddresses");
            DropTable("dbo.ColorEs");
        }
    }
}
