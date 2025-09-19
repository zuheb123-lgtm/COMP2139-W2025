using System.ComponentModel.DataAnnotations;
namespace COMP2139_ICE.Models;


public class Project 
{    
    
    public int ProjectId { get; set; }
    
    public string Name { get; set; } = null!;
    
    public string? Description { get; set; } 
    
    [DataType(DataType.Date)]
    public DateTime StartDate { get; set; }
    
    [DataType(DataType.Date)]
    public DateTime EndDate { get; set; }
    
    public string? Status {   get; set; }
}