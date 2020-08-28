namespace EmployeeSchedule.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<EmployeeSchedule.Data.EmployeeScheduleContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }
         
        protected override void Seed(EmployeeSchedule.Data.EmployeeScheduleContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method
            //  to avoid creating duplicate seed data.
            context.Employees.AddOrUpdate(
                  p => p.EmployeeID,
                  new Models.Employee { EmployeeID = 00001, FirstName = "Andrew ", LastName = "Peters", Email = "ap@email.com" },
                  new Models.Employee { EmployeeID = 00002, FirstName = "Simon", LastName = "Adamson", Email = "sa@email.com" },
                  new Models.Employee { EmployeeID = 00003, FirstName = "Julie", LastName = "Robertson", Email = "jr@email.com" }
                );

            context.Shifts.AddOrUpdate(
                  p => p.ShiftID,
                  new Models.Shift { ShiftID = 99999, Date = DateTime.Parse("01/10/2021"), StartTime = DateTime.Parse("09:00"), EndTime = DateTime.Parse("15:00"), EmployeeID = 00001 },
                  new Models.Shift { ShiftID = 99998, Date = DateTime.Parse("01/10/2021"), StartTime = DateTime.Parse("11:00"), EndTime = DateTime.Parse("17:00"), EmployeeID = 00002 },
                  new Models.Shift { ShiftID = 99997, Date = DateTime.Parse("01/10/2021"), StartTime = DateTime.Parse("16:00"), EndTime = DateTime.Parse("22:00"), EmployeeID = 00003 }
                );
        }
    }
}
