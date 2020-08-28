using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using EmployeeSchedule.Data;
using EmployeeSchedule.Models;

namespace EmployeeSchedule.Controllers
{
    public class ShiftsController : ApiController
    {
        private EmployeeScheduleContext db = new EmployeeScheduleContext();

        //Returns a list of all the shifts for a specific employee
        public List<Shift> getEmpShifts(int empID)
        {
            var shiftList = new List<Shift>();
            foreach (Shift s in db.Shifts.Where(r => r.EmployeeID == empID).AsNoTracking())
            {
                shiftList.Add(s);
            }
            return shiftList;
        }

        //Returns a list of times of all the shifts for a specific employee on a specific date
        public List<DateTime> getEmpShiftTimes(int empID, Shift newShift, String type)
        {
            var empShifts = getEmpShifts(empID);
            var timeList = new List<DateTime>();
            //This is required for updating time of a shift but only on an edit (so that you can edit the current shift without getting an error of already having a shift at that time)
            if (type == "Edit")
            { 
                var empShift2 = empShifts.Where(e => e.ShiftID != newShift.ShiftID);
                foreach (Shift s in empShift2.Where(r => r.Date == newShift.Date))
                {
                    for (int i = (s.EndTime - s.StartTime).Hours - 1; i >= 0; i--)
                    {
                        timeList.Add(s.StartTime.AddHours(i));
                    }
                }
            }
            else
            {
                foreach (Shift s in empShifts.Where(r => r.Date == newShift.Date))
                {
                    for (int i = (s.EndTime - s.StartTime).Hours - 1; i >= 0; i--)
                    {
                        timeList.Add(s.StartTime.AddHours(i));
                    }
                }
            }
            return timeList;
        }

        //Checks shifts are not overlaping or at the same time (true shows a conflict false shows no conflict)
        bool HasShift(Shift newShift, String type)
        {
            bool shiftOk = true;
            bool dateMatch = false;
            
            //Gets the list of all the employees shifts
            var empShifts = getEmpShifts(newShift.EmployeeID); 
            
            //Compares the new shift with the employees current shifts to check if the date of the new shift matches any current sheduled shifts
            foreach (Shift s in empShifts)
            {
                if (s.Date == newShift.Date)
                {
                    dateMatch = true;
                }
            }

            //If the dates of the shift match then check the times
            if (dateMatch)
            {
                //Get the employees shift times for specified date
                var timeList = getEmpShiftTimes(newShift.EmployeeID, newShift, type);

                //Goes through each time in the employees time list and checks it doesn't match
                foreach (DateTime t in timeList)
                {
                    var timeList2 = new List<DateTime>();
                    for (int i = (newShift.EndTime - newShift.StartTime).Hours; i >= 0; i--)
                    {
                        timeList2.Add(newShift.StartTime.AddHours(i));
                    }
                    foreach (DateTime j in timeList2)
                    {
                        if (t == j)
                        {
                            shiftOk = false;
                        }
                    }
                }
            }

            if (!shiftOk)
            {
                return true;
            }
            return false;
        }

        //Comparing two shifts for shift swapping so they don't overlap times of shifts.
        bool CompareShift(Shift s1, Shift s2, string type)
        {
            bool shiftOk = true;
            bool dateMatch = false;

            //Gets the list of all the employees shifts
            var empShifts1 = getEmpShifts(s1.EmployeeID);
            var empShifts2 = getEmpShifts(s2.EmployeeID);

            //Compares the new shift with the employees current shifts to check if the date of the new shift matches any current sheduled shifts
            foreach (Shift s in empShifts1)
            {
                if (s.Date == s2.Date)
                {
                    dateMatch = true;
                }
            }
            foreach (Shift s in empShifts2)
            {
                if (s.Date == s1.Date)
                {
                    dateMatch = true;
                }
            }

            //If the dates of the shift match then check the times
            if (dateMatch)
            {
                //Get the employees shift times for specified date
                var s1Time = getEmpShiftTimes(s1.EmployeeID, s1, type);
                var s2Time = getEmpShiftTimes(s2.EmployeeID, s2, type);

                //Goes through each time in the employees time list and checks it doesn't match
                foreach (DateTime t in s1Time)
                {
                    var timeList = new List<DateTime>();
                    for (int i = (s1.EndTime - s2.StartTime).Hours; i >= 0; i--)
                    {
                        timeList.Add(s2.StartTime.AddHours(i));
                    }
                    foreach (DateTime j in s2Time)
                    {
                        if (t == j)
                        {
                            shiftOk = false;
                        }
                    }
                }
            }

            if (!shiftOk)
            {
                return true;
            }
            return false;            
        }


        // GET: api/Shifts
        public IQueryable<Shift> GetShifts()
        {
            return db.Shifts;
        }

        // GET: api/Shifts/5
        //Use Employee ID to find all the shifts of that specific Employee
        [ResponseType(typeof(Shift))]
        public IHttpActionResult GetShift(int id)
        {
            Employee emp = db.Employees.Find(id);
            if (emp == null)
            {
                return NotFound();
            }
            //Get the shifts for the specific employee
            var shift = getEmpShifts(id);
            if (shift == null)
            {
                return NotFound();
            }

            return Ok(shift);
        }

        // PUT: api/Shifts/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutShift(int id, Shift shift)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            Shift shiftLkup = shift;
            //Checks the shift doesn't conflict another
            if (HasShift(shiftLkup, "Edit"))
            {
                return BadRequest("Already has a shift during this time");
            }

            if (id != shift.ShiftID)
            {
                return BadRequest();
            }

            db.Entry(shift).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ShiftExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // PUT: api/Shifts/5
        [System.Web.Http.HttpPut]
        [ResponseType(typeof(void))]
        public IHttpActionResult SwapShifts(int id, int id2)
        {
            Shift shift1 = db.Shifts.Find(id);
            Shift shift2 = db.Shifts.Find(id2);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != shift1.ShiftID && id2 != shift2.ShiftID)
            {
                return BadRequest();
            }

            //Checks the shift doesn't conflict another
            if (CompareShift(shift1, shift2, ""))
            {
                return BadRequest("Already has a shift during this time");
            }

            int newId1 = shift1.EmployeeID;
            int newId2 = shift2.EmployeeID;
            //swaps the employee id on the shifts as long as there is no conflict in shift times
            shift1.EmployeeID = newId2;
            shift2.EmployeeID = newId1;

            db.Entry(shift1).State = EntityState.Modified;
            db.Entry(shift2).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ShiftExists(id) || !ShiftExists(id2))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Shifts
        [ResponseType(typeof(Shift))]
        public IHttpActionResult PostShift(Shift shift)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //Checks the shift doesn't conflict another
            if (HasShift(shift, ""))
            {
                return BadRequest("Already has a shift during this time");
            }

            db.Shifts.Add(shift);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = shift.ShiftID }, shift);
        }

        // DELETE: api/Shifts/5
        [ResponseType(typeof(Shift))]
        public IHttpActionResult DeleteShift(int id)
        {
            Shift shift = db.Shifts.Find(id);
            if (shift == null)
            {
                return NotFound();
            }

            db.Shifts.Remove(shift);
            db.SaveChanges();

            return Ok(shift);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ShiftExists(int id)
        {
            return db.Shifts.Count(e => e.ShiftID == id) > 0;
        }
    }
}