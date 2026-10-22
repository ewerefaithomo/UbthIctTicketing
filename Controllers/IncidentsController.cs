using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UbthIctTicketing.Data;
using UbthIctTicketing.Models;

namespace UbthIctTicketing.Controllers
{
    public class IncidentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public IncidentsController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // Helper method to check active user session
        private bool IsUserAuthenticated()
        {
            return !string.IsNullOrEmpty(HttpContext.Session.GetString("UserRole"));
        }

        // GET: Incidents/Details/1
        public async Task<IActionResult> Details(int? id)
        {
            if (!IsUserAuthenticated()) return RedirectToAction("Login", "Account");

            if (id == null) return NotFound();

            var incident = await _context.Incidents
                .FirstOrDefaultAsync(m => m.Id == id);

            if (incident == null) return NotFound();

            return View(incident);
        }

        // GET: Incidents/Dashboard
        public async Task<IActionResult> Dashboard(DateTime? startDate, DateTime? endDate)
        {
            if (!IsUserAuthenticated()) return RedirectToAction("Login", "Account");

            var query = _context.Incidents.AsQueryable();

            if (startDate.HasValue)
            {
                query = query.Where(i => i.ReportedTime >= startDate.Value.Date);
            }

            if (endDate.HasValue)
            {
                query = query.Where(i => i.ReportedTime <= endDate.Value.Date.AddDays(1).AddTicks(-1));
            }

            var dataset = await query.ToListAsync();

            ViewBag.TotalIncidents = dataset.Count;
            ViewBag.ClosedIncidents = dataset.Count(i => i.Status == IncidentStatus.Closed);
            ViewBag.OpenIncidents = dataset.Count(i => i.Status != IncidentStatus.Closed);

            ViewBag.DepartmentReport = dataset
                .GroupBy(i => i.Department)
                .Select(g => new
                {
                    Department = string.IsNullOrWhiteSpace(g.Key) ? "General / Unspecified" : g.Key,
                    Count = g.Count()
                })
                .OrderByDescending(x => x.Count)
                .ToList();

            ViewBag.StartDate = startDate?.ToString("yyyy-MM-dd");
            ViewBag.EndDate = endDate?.ToString("yyyy-MM-dd");

            return View();
        }

        // GET: Incidents
        public async Task<IActionResult> Index()
        {
            if (!IsUserAuthenticated()) return RedirectToAction("Login", "Account");

            ViewBag.UserRole = HttpContext.Session.GetString("UserRole");

            var incidents = await _context.Incidents.OrderByDescending(i => i.ReportedTime).ToListAsync();
            return View(incidents);
        }

        // GET: Incidents/Create
        public IActionResult Create()
        {
            if (!IsUserAuthenticated()) return RedirectToAction("Login", "Account");

            // Prevent 'support' role from accessing report incident page directly
            if (HttpContext.Session.GetString("UserRole") == "support")
            {
                return RedirectToAction(nameof(Index));
            }

            return View();
        }

        // POST: Incidents/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Department,Office,Issue,Priority,ReportedBy")] Incident incident, IFormFile? imageFile)
        {
            if (!IsUserAuthenticated()) return RedirectToAction("Login", "Account");

            if (HttpContext.Session.GetString("UserRole") == "support")
            {
                return RedirectToAction(nameof(Index));
            }

            if (ModelState.IsValid)
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    string uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");

                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(imageFile.FileName);
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(fileStream);
                    }

                    incident.ImagePath = "/uploads/" + uniqueFileName;
                }

                incident.ReportedTime = DateTime.Now;
                incident.Status = IncidentStatus.Reported;
                _context.Add(incident);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(incident);
        }

        // POST: Incidents/Assign
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Assign(int id, string assignedTeam, string assignedEngineer, string estimatedTime)
        {
            if (!IsUserAuthenticated()) return RedirectToAction("Login", "Account");

            if (HttpContext.Session.GetString("UserRole") == "support")
            {
                return RedirectToAction(nameof(Index));
            }

            var incident = await _context.Incidents.FindAsync(id);
            if (incident == null) return NotFound();

            incident.AssignedTeam = assignedTeam;
            incident.AssignedEngineer = assignedEngineer;
            incident.EstimatedTime = estimatedTime;
            incident.Status = IncidentStatus.InProgress;
            incident.InitialEngineerReply = $"Received. Assigned to {assignedEngineer}. ETA: {estimatedTime}";

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // POST: Incidents/Close
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Close(int id, string resolution, string confirmedBy)
        {
            if (!IsUserAuthenticated()) return RedirectToAction("Login", "Account");

            var incident = await _context.Incidents.FindAsync(id);
            if (incident == null) return NotFound();

            incident.Resolution = resolution;
            incident.ConfirmedBy = confirmedBy;
            incident.TimeCompleted = DateTime.Now;
            incident.Status = IncidentStatus.Closed;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}