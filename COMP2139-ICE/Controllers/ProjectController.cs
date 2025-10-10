using Microsoft.AspNetCore.Mvc;
using COMP2139_ICE.Data;
using COMP2139_ICE.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace COMP2139_ICE.Controllers;

public class ProjectController : Controller
{
    private readonly ApplicationDbContext _context;

    public ProjectController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var projects = _context.Projects.ToList();
        return View(projects);
    }
    [HttpGet]
    public IActionResult Create()
    {

        return View();
    }

    [HttpPost]
    [AutoValidateAntiforgeryToken]
    public IActionResult Create(Project project)
    {
        if (ModelState.IsValid)
        {
            _context.Add(project);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        return View(project);
    }
    
    [HttpGet]
    public IActionResult Details(int id)
    {
        var project = _context.Projects.FirstOrDefault(p => p.ProjectId == id);
        if (project == null) return NotFound();
        return View(project);
    }
    [HttpGet]
    public IActionResult Edit(int id)
    {
        var project = _context.Projects.Find(id);
        if(project == null)
        {
            return NotFound();
        }
        return View(project);
    } 
    
//Lab4 - Part3 - #2 
    [HttpPost] 
    [ValidateAntiForgeryToken] 
    
    public IActionResult Edit(int id, [Bind("ProjectId","Name","Description")] Project project) 
    { 
        if (id != project.ProjectId) 
        { 
            return NotFound(); 
        } 
        if (ModelState .IsValid) 
        { 
            try 
            { 
                _context.Projects.Update(project); 
                _context.SaveChanges(); 
            } 
            catch (DbUpdateConcurrencyException) 
            { 
                if (!ProjectExists(project.ProjectId)) 
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
    private bool ProjectExists(int id) 
    { 
        return _context.Projects.Any(e => e.ProjectId == id); 
    }

    [HttpGet]
    public IActionResult Delete(int id)
    {
        var project = _context.Projects.FirstOrDefault(p => p.ProjectId == id);
        if (project == null)
        {
            return NotFound();
        }
        return View(project);
    }
    
    [HttpPost, ActionName("DeleteConfirmed")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id)
    {
        var project = _context.Projects.Find(id);
        if (project != null)
        {
            _context.Projects.Remove(project);
            _context.SaveChanges();
            return RedirectToActionPermanent("Index");
        }
        return NotFound();
    }
}

