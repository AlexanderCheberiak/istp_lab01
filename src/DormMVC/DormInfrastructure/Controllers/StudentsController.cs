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
    public class StudentsController : Controller
    {

        private readonly DormContext _context;

        public StudentsController(DormContext context)
        {
            _context = context;
        }

        // GET: Students
        public async Task<IActionResult> Index()
        {
            var dormContext = _context.Students.Include(s => s.Faculty).Include(s => s.Room);
            return View(await dormContext.ToListAsync());
        }

        // GET: Students/Details/5
        public async Task<IActionResult> Details(short? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .Include(s => s.Faculty)
                .Include(s => s.Room)
                .FirstOrDefaultAsync(m => m.StudentId == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // GET: Students/Create
        public IActionResult Create()
        {
            ViewData["FacultyId"] = new SelectList(_context.Faculties, "FacultyId", "FacultyName");
            ViewData["RoomId"] = new SelectList(_context.Rooms, "RoomId", "RoomNumber");
            return View();
        }

        // POST: Students/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("StudentId,FullName,BirthDate,FacultyId,Course,RoomId,CreatedAt,UpdatedAt")] Student student)
        {
            Faculty faculty = _context.Faculties.FirstOrDefault(f => f.FacultyId == student.FacultyId);
            Room room = _context.Rooms.FirstOrDefault(r => r.RoomId == student.RoomId);
            student.Faculty = faculty;
            student.Room = room;
            ModelState.Clear();
            TryValidateModel(student);

            if (ModelState.IsValid)
            {
                _context.Add(student);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["FacultyId"] = new SelectList(_context.Faculties, "FacultyId", "FacultyId", student.FacultyId);
            ViewData["RoomId"] = new SelectList(_context.Rooms, "RoomId", "RoomId", student.RoomId);
            return View(student);
        }

        // GET: Students/Edit/5
        public async Task<IActionResult> Edit(short? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }
            ViewData["FacultyId"] = new SelectList(_context.Faculties, "FacultyId", "FacultyId", student.FacultyId);
            ViewData["RoomId"] = new SelectList(_context.Rooms, "RoomId", "RoomId", student.RoomId);
            return View(student);
        }

        // POST: Students/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(short id, [Bind("StudentId,FullName,BirthDate,FacultyId,Course,RoomId,CreatedAt,UpdatedAt")] Student student)
        {
            if (id != student.StudentId)
            {
                return NotFound();
            }

            Faculty faculty = _context.Faculties.FirstOrDefault(f => f.FacultyId == student.FacultyId);
            Room room = _context.Rooms.FirstOrDefault(r => r.RoomId == student.RoomId);
            student.Faculty = faculty;
            student.Room = room;
            ModelState.Clear();
            TryValidateModel(student);

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(student);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentExists(student.StudentId))
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
            ViewData["FacultyId"] = new SelectList(_context.Faculties, "FacultyId", "FacultyId", student.FacultyId);
            ViewData["RoomId"] = new SelectList(_context.Rooms, "RoomId", "RoomId", student.RoomId);
            return View(student);
        }

        // GET: Students/Delete/5
        public async Task<IActionResult> Delete(short? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .Include(s => s.Faculty)
                .Include(s => s.Room)
                .FirstOrDefaultAsync(m => m.StudentId == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // POST: Students/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(short id)
        {
            var student = await _context.Students
                .Include(s => s.GuestVisits)
                .FirstOrDefaultAsync(s => s.StudentId == id);

            if (student != null)
            {
                // Remove related GuestVisits first
                _context.GuestVisits.RemoveRange(student.GuestVisits);
                _context.Students.Remove(student);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool StudentExists(short id)
        {
            return _context.Students.Any(e => e.StudentId == id);
        }
    }
}
