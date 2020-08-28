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

        //Get a list of all the shifts for a specific employee
        public List<Shift> getEmpShifts(int empID)
        {
            var shiftList = new List<Shift>();
            foreach (Shift s in db.Shifts.Where(r => r.EmployeeID == empID))
            {
                shiftList.Add(s);
            }
            return shiftList;
        }

        //Checks shifts are not overlaping or at the same time (true shows a conflict false shows no conflict)
        bool HasShift(Shift newShift)
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
                var timeList = new List<DateTime>();
                //Goes through each of the employees shifts and adds the times to a list for the corresponding date
                foreach (Shift s in empShifts.Where(r => r.Date == newShift.Date))
                {
                    for (int i = (s.EndTime - s.StartTime).Hours - 1; i >= 0; i--)
                    {
                        timeList.Add(s.StartTime.AddHours(i));
                    }
                }

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


        // GET: api/Shifts
        public IQueryable<Shift> GetShifts()
        {
            return db.Shifts;
        }

        // GET: api/Shifts/5
        //Use Employee ID to find all the shifts of that specific Employee
        [ResponseType(typeof(Shift))]
        public IHttpActionResult GetShift(int empId)
        {
            Employee emp = db.Employees.Find(empId);
            if (emp == null)
            {
                return NotFound();
            }
            //Get the shifts for the specific employee
            var shift = getEmpShifts(emp.EmployeeID);
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

            if (id != shift.ShiftID)
            {
                return BadRequest();
            }

            //Checks the shift doesn't conflict another
            if (HasShift(shift))
            {
                return BadRequest("Already has a shift during this time");
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

        // POST: api/Shifts
        [ResponseType(typeof(Shift))]
        public IHttpActionResult PostShift(Shift shift)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //Checks the shift doesn't conflict another
            if (HasShift(shift))
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