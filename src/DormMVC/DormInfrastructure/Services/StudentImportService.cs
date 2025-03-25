using System.ComponentModel.DataAnnotations;
using ClosedXML.Excel;
using DormDomain.Model;
using Microsoft.EntityFrameworkCore;
using static DormInfrastructure.Services.IImportService;

namespace DormInfrastructure.Services
{
    public class StudentImportService : IImportService<Student>
    {
        private readonly DormContext _context;

        public StudentImportService(DormContext context)
        {
            _context = context;
        }

        public async Task ImportFromStreamAsync(Stream stream, CancellationToken cancellationToken)
        {
            if (!stream.CanRead)
            {
                throw new ArgumentException("Дані не можуть бути прочитані", nameof(stream));
            }

            using (XLWorkbook workBook = new XLWorkbook(stream))
            {
                foreach (IXLWorksheet worksheet in workBook.Worksheets)
                {
                    short RoomNumber;
                    if (!short.TryParse(worksheet.Name, out RoomNumber))
                    {
                        throw new ArgumentException("Invalid room number format", nameof(worksheet.Name));
                    }
                    RoomNumber = short.Parse(worksheet.Name);
                    var room = await _context.Rooms.FirstOrDefaultAsync(room => room.RoomNumber == RoomNumber, cancellationToken);

                    foreach (var row in worksheet.RowsUsed().Skip(1))
                    {
                        try
                        {
                            await AddStudentAsync(row, cancellationToken, room);
                        }
                        catch (InvalidOperationException ex)
                        {
                            // Log the error message or handle it as needed
                            // For example, you can store the error message in a list and display it on the web page
                            throw new ValidationException(ex.Message);
                        }
                        catch (System.OverflowException ex)
                        {
                            throw new ValidationException("Номер курсу має бути від 1 до 10.");
                        }
                    }
                }
            }
            await _context.SaveChangesAsync(cancellationToken);
        }

        private async Task AddStudentAsync(IXLRow row, CancellationToken cancellationToken, Room room)
        {
            if (room == null)
            {
                throw new InvalidOperationException("Кімнату не знайдено.");
            }
            if (room.Students.Count >= room.Capacity)
            {
                throw new InvalidOperationException($"Кімната {room.RoomNumber} не має вільних місць.");
            }

            Student student = new Student();
            student.FullName = GetStudentName(row);
            student.BirthDate = GetStudentBirth(row);
            student.Course = GetStudentCourse(row);
            student.CreatedAt = GetStudentDate(row);
            student.Room = room;
            await GetFacultyAsync(row, student, cancellationToken);

            if (student.Faculty == null)
            {
                throw new InvalidOperationException("Факультет не знайдено.");
            }

            if (student.Room == null)
            {
                throw new InvalidOperationException("Кімнату не знайдено.");
            }

            if (student.Course <= 0 || student.Course >= 10)
            {
                throw new InvalidOperationException("Номер курсу може бути від 1 до 10.");
            }

            if (student.BirthDate < new DateOnly(1900, 1, 1))
            {
                throw new InvalidOperationException("Дата народження має бути не раніше 1900 року.");
            }

            if (student.CreatedAt < DateTime.Now.AddYears(-10))
            {
                throw new InvalidOperationException("Дата заселення має бути не раніше ніж 10 років тому.");
            }

            if (student.CreatedAt > DateTime.Now.AddMonths(1))
            {
                throw new InvalidOperationException("Дата заселення має бути не пізніше ніж через місяць.");
            }

            _context.Students.Add(student);
        }

        private static string GetStudentName(IXLRow row)
        {
            return row.Cell(1).Value.ToString();
        }

        private static DateOnly GetStudentBirth(IXLRow row)
        {
            return DateOnly.Parse(row.Cell(2).Value.ToString());
        }

        private static byte GetStudentCourse(IXLRow row)
        {
            return byte.Parse(row.Cell(4).Value.ToString());
        }

        private static DateTime GetStudentDate(IXLRow row)
        {
            return DateTime.Parse(row.Cell(5).Value.ToString());
        }

        private async Task GetFacultyAsync(IXLRow row, Student student, CancellationToken cancellationToken)
        {
            var facName = row.Cell(3).Value.ToString();
            var faculty = await _context.Faculties.FirstOrDefaultAsync(fac => fac.FacultyName == facName, cancellationToken);
            student.Faculty = faculty;
        }
    }

}
