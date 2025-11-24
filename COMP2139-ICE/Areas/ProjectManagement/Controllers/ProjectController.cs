using COMP2139_ICE.Areas.ProjectManagement.Models;
using COMP2139_ICE.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace COMP2139_ICE.Areas.ProjectManagement.Controllers;

[Area("ProjectManagement")]
[Route("[area]/[controller]/[action]")]
public class ProjectController : Controller
{
    private readonly ApplicationDbContext _context;

    public ProjectController(ApplicationDbContext context)
    {
        _context = context;
    }
    [HttpGet("")]
    public async Task<IActionResult> Index()
    { 
        var projects = await _context.Projects.ToListAsync();
        
        return View(projects);

    }


    [HttpGet("Create")] //page opens
    public IActionResult Create()
    {
       
        return View();
    }
    
    [HttpPost("Create")] // click on the submit button 
    [AutoValidateAntiforgeryToken]

    public async Task<IActionResult> Create(Project project)
    {
        if (ModelState.IsValid)
        {
            project.StartDate = ToUtc(project.StartDate);
            project.EndDate = ToUtc(project.EndDate);
            
            await _context.Projects.AddAsync(project);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        return View(project);
    }

    private DateTime ToUtc(DateTime input)
    {
        if (input.Kind == DateTimeKind.Utc)
            return input;
        if (input.Kind == DateTimeKind.Unspecified)
            return DateTime.SpecifyKind(input, DateTimeKind.Utc);
        return input.ToUniversalTime();
    }
    
    
    [HttpGet("Details/{id:int}")]
    public async Task<IActionResult> Details(int id)

    {
        /*
        var project = new Project
        {
            ProjectId = id, Name = "Project "+ id, Description = "Details of Project" + id
            
        };
        */
        var project = await _context.Projects.FirstOrDefaultAsync(p=>p.ProjectId == id);
        if (project == null)
        {
            return NotFound();
        }
        
        return View(project);
    }

    [HttpGet("Edit/{id:int}")]
    public async Task<IActionResult> Edit(int id)
    {
        var project = await _context.Projects.FindAsync(id);
        if (project == null)
        {
            return NotFound();
        }
        return View(project);
    }
    
    
    [HttpPost("Edit/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("ProjectId","Name","Description")] Project project)
    {
        if (id != project.ProjectId)
        {
            return NotFound();
        }
        
        if (ModelState.IsValid)
        {
            try
            {
                _context.Projects.Update(project);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await ProjectExists(project.ProjectId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction("Index");
        }
        return View(project);
    }
    
    private async Task<bool> ProjectExists(int id)
    {
        return await _context.Projects.AnyAsync(e => e.ProjectId == id);
    }

    [HttpGet("Delete/{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var project = await _context.Projects.FirstOrDefaultAsync(p => p.ProjectId == id);
        if (project == null)
            
        {
            return NotFound();
        }
        
        return View(project);
    }

    [HttpPost("DeleteConfirmed/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var project = await _context.Projects.FindAsync(id);
        if (project != null)
        {
            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        return NotFound();
    }
    
    // Lab 6 - Project Search Functionality
// Custom route for search functionality
// Accessible at /Projects/Search/{searchString?}
[HttpGet("Search/{searchString?}")]
public async Task<IActionResult> Search(string searchString)
{
    // Fetch all projects from the database as an IQueryable collection
    // IQueryable allows us to apply filters before executing the database query
    var projectsQuery = _context.Projects.AsQueryable();

    // Check if a search string was provided (avoids null or empty search issues)
    bool searchPerformed = !string.IsNullOrWhiteSpace(searchString);

    if (searchPerformed)
    {
        // Convert searchString to lowercase to make the search case-insensitive
        searchString = searchString.ToLower();

        // Apply filtering: Match project name or description
        // Description is checked for null before calling ToLower() to prevent NullReferenceException
        projectsQuery = projectsQuery.Where(p =>
            p.Name.ToLower().Contains(searchString) ||
            (p.Description != null && p.Description.ToLower().Contains(searchString)));
    }

    // ❗ WHY ASYNC? ❗
    // Asynchronous execution means this method does not block the thread while waiting for the database.
    // Instead of blocking, ASP.NET Core can process other incoming requests while waiting for the result.
    // This improves scalability and application responsiveness.
    
    // Execute the query asynchronously using `ToListAsync()`
    var projects = await projectsQuery.ToListAsync();
    
    // ❗ HOW ASYNC WORKS HERE? ❗
    // `await` releases the current thread while waiting for the query execution to complete.
    // When the database call finishes, execution resumes on this method at this point.

    // Store search metadata for the view
    ViewData["SearchPerformed"] = searchPerformed;
    ViewData["SearchString"] = searchString;

    // Return the filtered list to the Index view (reusing existing UI)
    return View("Index", projects);
}
    
}