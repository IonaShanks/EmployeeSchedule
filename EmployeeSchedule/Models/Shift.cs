using System;
using System.ComponentModel.DataAnnotations;

namespace EmployeeSchedule.Models
{
    //Validation to check that the date is not in the past.
    public class FutureDateAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            DateTime date = Convert.ToDateTime(value);
            return date >= DateTime.Now;
        }
    }

    //Validation to check that the start time is before the end time.
    public class DateAfterAttribute : ValidationAttribute
    {
        public DateAfterAttribute(string timeToCompare)
        {
            TimeToCompare = timeToCompare;
        }
        private string TimeToCompare { get; set; }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            DateTime endTime = (DateTime)value;
            DateTime startTime = (DateTime)validationContext.ObjectType.GetProperty(TimeToCompare).GetValue(validationContext.ObjectInstance, null);

            if (startTime >= endTime)
            {
                return new ValidationResult("Shift must be at least one hour");
            }
            else
            {
                return ValidationResult.Success;
            }
        }
    }
    public class Shift
    {
        [Key]
        public int ShiftID { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd-MM-YYYY}", ApplyFormatInEditMode = true), FutureDate(ErrorMessage = "Date cannot be in the past")]
        public DateTime Date { get; set; }
        [Required, DisplayFormat(DataFormatString = "{0:HH:mm}"), Display(Name = "From")]
        public DateTime StartTime { get; set; }
        [Required, DisplayFormat(DataFormatString = "{0:HH:mm}"), DateAfter("StartTime"), Display(Name = "To")]
        public DateTime EndTime { get; set; }

        [Required]
        public int EmployeeID { get; set; }
        public virtual Employee Employee { get; set; }
    }
}