using COMP2139_ICE.Areas.ProjectManagement.Models;
using COMP2139_ICE.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace COMP2139_ICE.Areas.ProjectManagement.Controllers;

[Area("ProjectManagement")]
[Route("[area]/[controller]/[action]")]

public class ProjectTaskController : Controller
{
    private readonly ApplicationDbContext _context; 
    
    public ProjectTaskController(ApplicationDbContext context) : base() 
    { 
        _context = context; 
    } 

    [HttpGet("{projectId:int}")] 
    public async Task<IActionResult> Index(int projectId) 

    { 
        var tasks = await _context.ProjectTasks 
                            .Where(t => t.ProjectId == projectId) 
                            .ToListAsync(); 
        ViewBag.ProjectId = projectId;   
        return View(tasks); 

    } 
    
    [HttpGet("Details/{id:int}")] 

    public async Task<IActionResult> Details(int id) 

    { 

        var task = await _context.ProjectTasks 
                        .Include(t => t.Project) // Include related project data 
                        .FirstOrDefaultAsync(t => t.ProjectTaskId == id); 
        if (task == null) 

        { 
            return NotFound(); 
        } 
        return View(task); 

    } 
    
    [HttpGet("Create/{projectId:int}")] 

    public async Task<IActionResult> Create(int projectId) 

    { 
        var project = _context.Projects.FindAsync(projectId); 
        if (project == null) 
        { 
            return NotFound(); 
        } 
        
        var task = new ProjectTask 

        { 
            ProjectId = projectId, 
            Title = "", 
            Description = "" 
        }; 
        
        return View(task); 

    } 
    
    [HttpPost("Create/{projectId:int}")] 
    [ValidateAntiForgeryToken] 

    public async Task<IActionResult> Create([Bind("Title", "Description", "ProjectId")] ProjectTask task) 
    { 
        if (ModelState.IsValid) 
        { 
            await _context.ProjectTasks.AddAsync(task); 
            await _context.SaveChangesAsync(); 
            
            return RedirectToAction(nameof(Index), new { projectId = task.ProjectId }); 
        } 
        
        var projects = await _context.Projects.ToListAsync();

        ViewBag.Projects = new SelectList(projects, "ProjectId", "Name", task.ProjectId); 

        return View(task); 

    } 

    [HttpGet("Edit/{id:int}")] 
    public async Task<IActionResult> Edit(int id) 

    { 
        var task = await _context.ProjectTasks 

                            .Include(t => t.Project) // Include related project data 

                            .FirstOrDefaultAsync(t => t.ProjectTaskId == id); 

        if (task == null) 

        { 
            return NotFound(); 
        } 
        
        var projects = await _context.Projects.ToListAsync();
        
        ViewBag.Projects = new SelectList(projects, "ProjectId", "Name", task.ProjectId); 
        return View(task); 

    } 
    
    [HttpPost("Edit/{id:int}")] 
    [ValidateAntiForgeryToken] 

    public async Task<IActionResult> Edit(int id, [Bind("ProjectTaskId", "Title", "Description", "ProjectId")] ProjectTask task) 

    { 
        if (id != task.ProjectTaskId) 

        { 
            return NotFound(); 
        } 
        
        if (ModelState.IsValid) 
        { 
            _context.ProjectTasks.Update(task); 

            await _context.SaveChangesAsync(); 

            return RedirectToAction(nameof(Index), new { projectId = task.ProjectId }); 
        } 
        
        ViewBag.Projects = new SelectList(_context.Projects, "ProjectId", "Name", task.ProjectId); 
        return View(task); 

    } 
    
    [HttpGet("Delete/{id:int}")] 
    public async Task<IActionResult> Delete(int id) 
    { 
        var task = await _context.ProjectTasks 
                            .Include(t => t.Project) // Include related project data 

                            .FirstOrDefaultAsync(t => t.ProjectTaskId == id); 
        if (task == null) 
        { 
            return NotFound(); 
        } 
        return View(task); 

    } 
    
    [HttpPost("DeleteConfirmed/{id:int}")] 
    [ValidateAntiForgeryToken] 

    public async Task<IActionResult> DeleteConfirmed(int projectTaskId) 
    { 
        var task = await _context.ProjectTasks.FindAsync(projectTaskId); 
        if (task != null) 
        { 

            _context.ProjectTasks.Remove(task); 
            await _context.SaveChangesAsync(); 
            return RedirectToAction(nameof(Index), new { projectId = task.ProjectId }); 

        } 
        return NotFound(); 
    } 
    
    // Lab 6 - Search ProjectTasks
// GET: ProjectTasks/Search/{projectId?}/{searchString?}
    [HttpGet("Search")]
    public async Task<IActionResult> Search(int? projectId, string searchString)
    {
        // Start with all tasks as an IQueryable query (deferred execution)
        var taskQuery = _context.ProjectTasks.AsQueryable();

        // Track whether a search was performed
        bool searchPerformed = !string.IsNullOrWhiteSpace(searchString);

        // If a projectId is provided, filter by project
        if (projectId.HasValue)
        {
            taskQuery = taskQuery.Where(t => t.ProjectId == projectId.Value);
        }

        // ❗ FIXED: Apply search filter when searchString is provided
        if (searchPerformed)
        {
            searchString = searchString.ToLower(); // Case-insensitive search

            // Ensure null-safe search on nullable Description
            taskQuery = taskQuery.Where(t =>
                t.Title.ToLower().Contains(searchString) ||
                (t.Description != null && t.Description.ToLower().Contains(searchString))
            );
        }

        // ❗ WHY ASYNC? ❗
        // The database query is executed asynchronously using `ToListAsync()`
        // This prevents blocking the main thread while waiting for the result.
        var tasks = await taskQuery.ToListAsync();

        // Pass search metadata to the view for UI updates
        ViewBag.ProjectId = projectId;
        ViewData["SearchPerformed"] = searchPerformed;
        ViewData["SearchString"] = searchString;

        // Reuse Index view to display filtered results
        return View("Index", tasks);
    }
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
}