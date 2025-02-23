using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DormDomain.Model;
using DormInfrastructure;

namespace DormInfrastructure.Controllers
{
    public class GuestVisitsController : Controller
    {
        private readonly DormContext _context;

        public GuestVisitsController(DormContext context)
        {
            _context = context;
        }

        // GET: GuestVisits
        public async Task<IActionResult> Index()
        {
            var dormContext = _context.GuestVisits.Include(g => g.Student);
            return View(await dormContext.ToListAsync());
        }

        // GET: GuestVisits/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var guestVisit = await _context.GuestVisits
                .Include(g => g.Student)
                .FirstOrDefaultAsync(m => m.VisitId == id);
            if (guestVisit == null)
            {
                return NotFound();
            }

            return View(guestVisit);
        }

        // GET: GuestVisits/Create
        public IActionResult Create()
        {
            ViewData["StudentId"] = new SelectList(_context.Students, "StudentId", "FullName");
            return View();
        }

        // POST: GuestVisits/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("VisitId,GuestName,StudentId,VisitDate")] GuestVisit guestVisit)
        {
            Student student = _context.Students
                .Include(s => s.Faculty)
                .Include(s => s.Room)
                .FirstOrDefault(s => s.StudentId == guestVisit.StudentId);

            if (student == null)
            {
                ModelState.AddModelError("StudentId", "Invalid student ID.");
            }
            else
            {
                guestVisit.Student = student;
                ModelState.Clear();
                TryValidateModel(guestVisit);
            }

            if (ModelState.IsValid)
            {
                _context.Add(guestVisit);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["StudentId"] = new SelectList(_context.Students, "StudentId", "FullName", guestVisit.StudentId);
            return View(guestVisit);
        }

        // GET: GuestVisits/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var guestVisit = await _context.GuestVisits.FindAsync(id);
            if (guestVisit == null)
            {
                return NotFound();
            }
            ViewData["StudentId"] = new SelectList(_context.Students, "StudentId", "FullName", guestVisit.StudentId);
            return View(guestVisit);
        }

        // POST: GuestVisits/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("VisitId,GuestName,StudentId,VisitDate")] GuestVisit guestVisit)
        {
            if (id != guestVisit.VisitId)
            {
                return NotFound();
            }

            Student student = _context.Students
    .Include(s => s.Faculty)
    .Include(s => s.Room)
    .FirstOrDefault(s => s.StudentId == guestVisit.StudentId);

            if (student == null)
            {
                ModelState.AddModelError("StudentId", "Invalid student ID.");
            }
            else
            {
                guestVisit.Student = student;
                ModelState.Clear();
                TryValidateModel(guestVisit);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(guestVisit);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GuestVisitExists(guestVisit.VisitId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["StudentId"] = new SelectList(_context.Students, "StudentId", "FullName", guestVisit.StudentId);
            return View(guestVisit);
        }

        // GET: GuestVisits/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var guestVisit = await _context.GuestVisits
                .Include(g => g.Student)
                .FirstOrDefaultAsync(m => m.VisitId == id);
            if (guestVisit == null)
            {
                return NotFound();
            }

            return View(guestVisit);
        }

        // POST: GuestVisits/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var guestVisit = await _context.GuestVisits.FindAsync(id);
            if (guestVisit != null)
            {
                _context.GuestVisits.Remove(guestVisit);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GuestVisitExists(int id)
        {
            return _context.GuestVisits.Any(e => e.VisitId == id);
        }
    }
}
