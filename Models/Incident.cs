using System.ComponentModel.DataAnnotations;

namespace UbthIctTicketing.Models
{
    public class Incident
    {
        [Key]
        public int Id { get; set; }

        // Step 1: Reporting Phase
        [Required]
        [Display(Name = "Department")]
        public string Department { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Office / Room")]
        public string Office { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Issue Description")]
        public string Issue { get; set; } = string.Empty;

        [Required]
        public IncidentPriority Priority { get; set; } = IncidentPriority.Medium;

        [Required]
        [Display(Name = "Reported By")]
        public string ReportedBy { get; set; } = string.Empty;

        public DateTime ReportedTime { get; set; } = DateTime.Now;

        public IncidentStatus Status { get; set; } = IncidentStatus.Reported;

        // Photo Attachment Path
        public string? ImagePath { get; set; }

        // Step 2: Assignment Phase
        [Display(Name = "Assigned Team / Unit")]
        public string? AssignedTeam { get; set; }

        [Display(Name = "Assigned Engineer")]
        public string? AssignedEngineer { get; set; }

        [Display(Name = "Estimated Time (ETA)")]
        public string? EstimatedTime { get; set; }

        public string? InitialEngineerReply { get; set; }

        // Step 3: Resolution Phase
        [Display(Name = "Time Completed")]
        public DateTime? TimeCompleted { get; set; }

        [Display(Name = "Resolution Details")]
        public string? Resolution { get; set; }

        [Display(Name = "Confirmed By")]
        public string? ConfirmedBy { get; set; }
    }
}