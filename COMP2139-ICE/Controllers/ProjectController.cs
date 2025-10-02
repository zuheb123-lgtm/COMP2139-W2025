using COMP2139_ICE.Data;
using COMP2139_ICE.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace COMP2139_ICE.Controllers
{
    public class ProjectController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProjectController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Project/
        [HttpGet]
        public IActionResult Index()
        {
            // Get all projects from the database
            var projects = _context.Projects.ToList();
            return View(projects);
        }

        // GET: /Project/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Project/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Project project)
        {
            if (ModelState.IsValid)
            {
                _context.Projects.Add(project);
                _context.SaveChanges();  // save to DB
                return RedirectToAction(nameof(Index));
            }

            return View(project);
        }

        // GET: /Project/Details/5
        [HttpGet]
        public IActionResult Details(int id)
        {
            var project = _context.Projects.FirstOrDefault(p => p.ProjectId == id);

            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }
    }
}