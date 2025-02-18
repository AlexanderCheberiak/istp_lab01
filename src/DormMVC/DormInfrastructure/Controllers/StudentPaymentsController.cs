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
    public class StudentPaymentsController : Controller
    {
        private readonly DormContext _context;

        public StudentPaymentsController(DormContext context)
        {
            _context = context;
        }

        // GET: StudentPayments
        public async Task<IActionResult> Index()
        {
            var dormContext = _context.StudentPayments.Include(s => s.PaymentType).Include(s => s.Student);
            return View(await dormContext.ToListAsync());
        }

        // GET: StudentPayments/Details/5
        public async Task<IActionResult> Details(short? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var studentPayment = await _context.StudentPayments
                .Include(s => s.PaymentType)
                .Include(s => s.Student)
                .FirstOrDefaultAsync(m => m.PaymentId == id);
            if (studentPayment == null)
            {
                return NotFound();
            }

            return View(studentPayment);
        }

        // GET: StudentPayments/Create
        public IActionResult Create()
        {
            ViewData["PaymentTypeId"] = new SelectList(_context.PaymentTypes, "PaymentTypeId", "PaymentName");
            ViewData["StudentId"] = new SelectList(_context.Students, "StudentId", "FullName");
            return View();
        }

        // POST: StudentPayments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PaymentId,StudentId,Amount,PaymentTypeId,PaymentDate")] StudentPayment studentPayment)
        {
            //PaymentType? paymentType = _context.PaymentTypes.FirstOrDefault(pt => pt.PaymentTypeId == studentPayment.PaymentTypeId);
            //if (paymentType == null)
            //{
            //    ModelState.AddModelError("PaymentTypeId", "Invalid Payment Type");
            //    ViewData["PaymentTypeId"] = new SelectList(_context.PaymentTypes, "PaymentTypeId", "PaymentName", studentPayment.PaymentTypeId);
            //    ViewData["StudentId"] = new SelectList(_context.Students, "StudentId", "FullName", studentPayment.StudentId);
            //    return View(studentPayment);
            //}
            Student student = _context.Students.FirstOrDefault(s => s.StudentId == studentPayment.StudentId);
            //studentPayment.PaymentType = paymentType;
            studentPayment.Student = student;
            ModelState.Clear();
            TryValidateModel(studentPayment);

            if (ModelState.IsValid)
            {
                _context.Add(studentPayment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PaymentTypeId"] = new SelectList(_context.PaymentTypes, "PaymentTypeId", "PaymentName", studentPayment.PaymentTypeId);
            ViewData["StudentId"] = new SelectList(_context.Students, "StudentId", "FullName", studentPayment.StudentId);
            return View(studentPayment);
        }

        // GET: StudentPayments/Edit/5
        public async Task<IActionResult> Edit(short? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var studentPayment = await _context.StudentPayments.FindAsync(id);
            if (studentPayment == null)
            {
                return NotFound();
            }
            ViewData["PaymentTypeId"] = new SelectList(_context.PaymentTypes, "PaymentTypeId", "PaymentName", studentPayment.PaymentTypeId);
            ViewData["StudentId"] = new SelectList(_context.Students, "StudentId", "FullName", studentPayment.StudentId);
            return View(studentPayment);
        }

        // POST: StudentPayments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(short id, [Bind("PaymentId,StudentId,Amount,PaymentTypeId,PaymentDate")] StudentPayment studentPayment)
        {
            if (id != studentPayment.PaymentId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(studentPayment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentPaymentExists(studentPayment.PaymentId))
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
            ViewData["PaymentTypeId"] = new SelectList(_context.PaymentTypes, "PaymentTypeId", "PaymentName", studentPayment.PaymentTypeId);
            ViewData["StudentId"] = new SelectList(_context.Students, "StudentId", "FullName", studentPayment.StudentId);
            return View(studentPayment);
        }

        // GET: StudentPayments/Delete/5
        public async Task<IActionResult> Delete(short? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var studentPayment = await _context.StudentPayments
                .Include(s => s.PaymentType)
                .Include(s => s.Student)
                .FirstOrDefaultAsync(m => m.PaymentId == id);
            if (studentPayment == null)
            {
                return NotFound();
            }

            return View(studentPayment);
        }

        // POST: StudentPayments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(short id)
        {
            var studentPayment = await _context.StudentPayments.FindAsync(id);
            if (studentPayment != null)
            {
                _context.StudentPayments.Remove(studentPayment);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StudentPaymentExists(short id)
        {
            return _context.StudentPayments.Any(e => e.PaymentId == id);
        }
    }
}
