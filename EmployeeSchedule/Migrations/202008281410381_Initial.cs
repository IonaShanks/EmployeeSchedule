namespace EmployeeSchedule.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Employees",
                c => new
                    {
                        EmployeeID = c.Int(nullable: false, identity: true),
                        FirstName = c.String(nullable: false),
                        LastName = c.String(nullable: false),
                        Email = c.String(nullable: false, maxLength: 200, unicode: false),
                    })
                .PrimaryKey(t => t.EmployeeID)
                .Index(t => t.Email, unique: true);
            
            CreateTable(
                "dbo.Shifts",
                c => new
                    {
                        ShiftID = c.Int(nullable: false, identity: true),
                        Date = c.DateTime(nullable: false),
                        StartTime = c.DateTime(nullable: false),
                        EndTime = c.DateTime(nullable: false),
                        EmployeeID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ShiftID)
                .ForeignKey("dbo.Employees", t => t.EmployeeID, cascadeDelete: true)
                .Index(t => t.EmployeeID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Shifts", "EmployeeID", "dbo.Employees");
            DropIndex("dbo.Shifts", new[] { "EmployeeID" });
            DropIndex("dbo.Employees", new[] { "Email" });
            DropTable("dbo.Shifts");
            DropTable("dbo.Employees");
        }
    }
}
