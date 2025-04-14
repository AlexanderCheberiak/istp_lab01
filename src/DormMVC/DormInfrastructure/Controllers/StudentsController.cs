using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DormDomain.Model;
using DormInfrastructure;
using DormInfrastructure.Services;
using Microsoft.AspNetCore.Authorization;

namespace DormInfrastructure.Controllers
{
    [Authorize(Roles = "Admin, Manager, Manager Assistant")]
    public class StudentsController : Controller
    {

        private readonly DormContext _context;
        private StudentDataPortServiceFactory _studentDataPortServiceFactory;

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

            return RedirectToAction("Index", "StudentChanges", new { id = student.StudentId, name = student.FullName });
        }

        [HttpGet]
        public IActionResult Import()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Import(IFormFile fileExcel, CancellationToken cancellationToken = default)
        {
            try
            {
                _studentDataPortServiceFactory = new StudentDataPortServiceFactory(_context);
                var importService = _studentDataPortServiceFactory.GetImportService(fileExcel.ContentType);
                using var stream = fileExcel.OpenReadStream();
                await importService.ImportFromStreamAsync(stream, cancellationToken);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View();
            }
        }

        [HttpGet]
        public async Task<IActionResult> Export([FromQuery] string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
    CancellationToken cancellationToken = default)
        {
            _studentDataPortServiceFactory = new StudentDataPortServiceFactory(_context);
            var exportService = _studentDataPortServiceFactory.GetExportService(contentType);

            var memoryStream = new MemoryStream();

            await exportService.WriteToAsync(memoryStream, cancellationToken);

            await memoryStream.FlushAsync(cancellationToken);
            memoryStream.Position = 0;


            return new FileStreamResult(memoryStream, contentType)
            {
                FileDownloadName = $"students_{DateTime.UtcNow.ToShortDateString()}.xlsx"
            };
        }


        // GET: Students/Create
        public IActionResult Create()
        {
            var availableRooms = _context.Rooms
                .Where(r => r.Students.Count() < r.Capacity)
                .Select(r => new { r.RoomId, r.RoomNumber })
                .ToList();

            ViewData["FacultyId"] = new SelectList(_context.Faculties, "FacultyId", "FacultyName");
            ViewData["RoomId"] = new SelectList(availableRooms, "RoomId", "RoomNumber");
            return View();
        }

        public async Task<short> GetMaxStudentId()
        {
            return await _context.Students.MaxAsync(s => s.StudentId);
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

            //int studCount = await _context.Students.CountAsync(s => s.RoomId == student.RoomId);

            if (student.Course <= 0 || student.Course >= 10)
            {
                ModelState.AddModelError("Course", "Номер курсу може бути від 1 до 10.");
            }

            if (student.BirthDate < new DateOnly(1900, 1, 1))
            {
                ModelState.AddModelError("BirthDate", "Дата народження має бути не раніше 1900 року.");
            }

            if (student.CreatedAt < DateTime.Now.AddYears(-10))
            {
                ModelState.AddModelError("CreatedAt", "Дата заселення має бути не раніше ніж 10 років тому.");
            }

            if (student.CreatedAt > DateTime.Now.AddMonths(1))
            {
                ModelState.AddModelError("CreatedAt", "Дата заселення має бути не пізніше ніж через місяць.");
            }

            //if (student.Room.Capacity <= studCount)
            //{
            //    ModelState.AddModelError("RoomId", "Кімната повністю зайнята");
            //}

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

            if (student.Course <= 0 || student.Course >= 10)
            {
                ModelState.AddModelError("Course", "Номер курсу може бути від 1 до 10.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var originalStudent = await _context.Students.AsNoTracking().FirstOrDefaultAsync(s => s.StudentId == id);
                    student.UpdatedAt = DateTime.Now;
                    _context.Update(student);
                    await _context.SaveChangesAsync();

                    // Create StudentChange entries
                    if (originalStudent != null)
                    {
                        if (originalStudent.FullName != student.FullName)
                        {
                            _context.StudentChanges.Add(new StudentChange
                            {
                                StudentId = student.StudentId,
                                ChangeDate = DateOnly.FromDateTime(DateTime.Now),
                                ChangeField = "Ім'я",
                                OldValue = originalStudent.FullName,
                                NewValue = student.FullName
                            });
                        }
                        if (originalStudent.BirthDate != student.BirthDate)
                        {
                            _context.StudentChanges.Add(new StudentChange
                            {
                                StudentId = student.StudentId,
                                ChangeDate = DateOnly.FromDateTime(DateTime.Now),
                                ChangeField = "Дата народження",
                                OldValue = originalStudent.BirthDate.ToString(),
                                NewValue = student.BirthDate.ToString()
                            });
                        }
                        if (originalStudent.FacultyId != student.FacultyId)
                        {
                            if (originalStudent.FacultyId != student.FacultyId)
                            {
                                var oldFaculty = await _context.Faculties.FindAsync(originalStudent.FacultyId);
                                var newFaculty = await _context.Faculties.FindAsync(student.FacultyId);
                                _context.StudentChanges.Add(new StudentChange
                                {
                                    StudentId = student.StudentId,
                                    ChangeDate = DateOnly.FromDateTime(DateTime.Now),
                                    ChangeField = "Факультет",
                                    OldValue = oldFaculty?.FacultyName,
                                    NewValue = newFaculty?.FacultyName
                                });
                            }
                            if (originalStudent.RoomId != student.RoomId)
                            {
                                var oldRoom = await _context.Rooms.FindAsync(originalStudent.RoomId);
                                var newRoom = await _context.Rooms.FindAsync(student.RoomId);
                                _context.StudentChanges.Add(new StudentChange
                                {
                                    StudentId = student.StudentId,
                                    ChangeDate = DateOnly.FromDateTime(DateTime.Now),
                                    ChangeField = "Номер кімнати",
                                    OldValue = oldRoom?.RoomNumber.ToString(),
                                    NewValue = newRoom?.RoomNumber.ToString()
                                });
                            }
                            //_context.StudentChanges.Add(new StudentChange
                            //{
                            //    StudentId = student.StudentId,
                            //    ChangeDate = DateOnly.FromDateTime(DateTime.Now),
                            //    ChangeField = "FacultyId",
                            //    OldValue = originalStudent.FacultyId.ToString(),
                            //    NewValue = student.FacultyId.ToString()
                            //});
                        }
                        if (originalStudent.Course != student.Course)
                        {
                            _context.StudentChanges.Add(new StudentChange
                            {
                                StudentId = student.StudentId,
                                ChangeDate = DateOnly.FromDateTime(DateTime.Now),
                                ChangeField = "Курс",
                                OldValue = originalStudent.Course.ToString(),
                                NewValue = student.Course.ToString()
                            });
                        }
                        if (originalStudent.RoomId != student.RoomId)
                        {
                            var oldRoom = await _context.Rooms.FindAsync(originalStudent.RoomId);
                            var newRoom = await _context.Rooms.FindAsync(student.RoomId);
                            _context.StudentChanges.Add(new StudentChange
                            {
                                StudentId = student.StudentId,
                                ChangeDate = DateOnly.FromDateTime(DateTime.Now),
                                ChangeField = "Номер кімнати",
                                OldValue = oldRoom?.RoomNumber.ToString(),
                                NewValue = newRoom?.RoomNumber.ToString()
                            });
                        }
                        //if (originalStudent.RoomId != student.RoomId)
                        //{
                        //    _context.StudentChanges.Add(new StudentChange
                        //    {
                        //        StudentId = student.StudentId,
                        //        ChangeDate = DateOnly.FromDateTime(DateTime.Now),
                        //        ChangeField = "Кімната",
                        //        OldValue = originalStudent.RoomId.ToString(),
                        //        NewValue = student.RoomId.ToString()
                        //    });
                        //}
                        await _context.SaveChangesAsync();
                    }
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
