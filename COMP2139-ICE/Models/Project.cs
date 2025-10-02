namespace COMP2139_ICE.Models;

public class Project
{
    // The unique identifier for the project.
    public int ProjectId { get; set; }

    // The name of the project.
    public string Name { get; set; }

    // An optional description of the project.
    public string Description { get; set; }

    // The start date of the project.
    public DateTime StartDate { get; set; }

    // The end date of the project.
    public DateTime EndDate { get; set; }

    // The current status of the project.
    public string Status { get; set; } = "Not Started";
}