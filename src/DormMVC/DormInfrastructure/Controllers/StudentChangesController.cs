using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DormDomain.Model;
using DormInfrastructure;
using Microsoft.AspNetCore.Authorization;

namespace DormInfrastructure.Controllers
{
    [Authorize(Roles = "Admin, Manager, Manager Assistant")]
    public class StudentChangesController : Controller
    {
        private readonly DormContext _context;

        public StudentChangesController(DormContext context)
        {
            _context = context;
        }

        // GET: StudentChanges
        public async Task<IActionResult> Index(int? id, string? name)
        {
            if (id == null) return RedirectToAction("Index", "StudentChanges");
            //Find changes of a student
            ViewBag.StudentId = id;
            ViewBag.FullName = name;
            var changesOfStudent = _context.StudentChanges.Where(c => c.StudentId == id).Include(c => c.Student);
            //var dormContext = _context.StudentChanges.Include(s => s.Student);
            return View(await changesOfStudent.ToListAsync());
        }

        // GET: StudentChanges/Details/5
        public async Task<IActionResult> Details(short? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var studentChange = await _context.StudentChanges
                .Include(s => s.Student)
                .FirstOrDefaultAsync(m => m.ChangeId == id);
            if (studentChange == null)
            {
                return NotFound();
            }

            return View(studentChange);
        }

        // GET: StudentChanges/Create
        public IActionResult Create()
        {
            ViewData["StudentId"] = new SelectList(_context.Students, "StudentId", "FullName");
            return View();
        }

        // POST: StudentChanges/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ChangeId,StudentId,ChangeDate,ChangeField,OldValue,NewValue")] StudentChange studentChange)
        {
            Student student = _context.Students
    .Include(s => s.Faculty)
    .Include(s => s.Room)
    .FirstOrDefault(s => s.StudentId == studentChange.StudentId);

            if (student == null)
            {
                ModelState.AddModelError("StudentId", "Invalid student ID.");
            }
            else
            {
                studentChange.Student = student;
                ModelState.Clear();
                TryValidateModel(studentChange);
            }

            if (ModelState.IsValid)
            {
                _context.Add(studentChange);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["StudentId"] = new SelectList(_context.Students, "StudentId", "FullName", studentChange.StudentId);
            return View(studentChange);
        }

        // GET: StudentChanges/Edit/5
        public async Task<IActionResult> Edit(short? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var studentChange = await _context.StudentChanges.FindAsync(id);
            if (studentChange == null)
            {
                return NotFound();
            }
            ViewData["StudentId"] = new SelectList(_context.Students, "StudentId", "FullName", studentChange.StudentId);
            return View(studentChange);
        }

        // POST: StudentChanges/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(short id, [Bind("ChangeId,StudentId,ChangeDate,ChangeField,OldValue,NewValue")] StudentChange studentChange)
        {
            if (id != studentChange.ChangeId)
            {
                return NotFound();
            }

            Student student = _context.Students
.Include(s => s.Faculty)
.Include(s => s.Room)
.FirstOrDefault(s => s.StudentId == studentChange.StudentId);

            if (student == null)
            {
                ModelState.AddModelError("StudentId", "Invalid student ID.");
            }
            else
            {
                studentChange.Student = student;
                ModelState.Clear();
                TryValidateModel(studentChange);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(studentChange);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentChangeExists(studentChange.ChangeId))
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
            ViewData["StudentId"] = new SelectList(_context.Students, "StudentId", "FullName", studentChange.StudentId);
            return View(studentChange);
        }

        // GET: StudentChanges/Delete/5
        public async Task<IActionResult> Delete(short? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var studentChange = await _context.StudentChanges
                .Include(s => s.Student)
                .FirstOrDefaultAsync(m => m.ChangeId == id);
            if (studentChange == null)
            {
                return NotFound();
            }

            return View(studentChange);
        }

        // POST: StudentChanges/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(short id)
        {
            var studentChange = await _context.StudentChanges.FindAsync(id);
            if (studentChange != null)
            {
                _context.StudentChanges.Remove(studentChange);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StudentChangeExists(short id)
        {
            return _context.StudentChanges.Any(e => e.ChangeId == id);
        }
    }
}
