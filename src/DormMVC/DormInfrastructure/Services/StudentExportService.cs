using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;

namespace DormInfrastructure.Services
{
    public class StudentExportService : IExportService<DormDomain.Model.Student>
    {
        private const string RootWorksheetName = "";

        private static readonly IReadOnlyList<string> HeaderNames =
            new string[]
            {
                "Студент",
                "Дата народження",
                "Факультет",
                "Курс",
                "Дата заселення",
            };
        private readonly DormContext _context;

        private static void WriteHeader(IXLWorksheet worksheet)
        {
            for (int columnIndex = 0; columnIndex < HeaderNames.Count; columnIndex++)
            {
                worksheet.Cell(1, columnIndex + 1).Value = HeaderNames[columnIndex];
            }
            worksheet.Row(1).Style.Font.Bold = true;
        }

        private void WriteStudent(IXLWorksheet worksheet, DormDomain.Model.Student student, int rowIndex)
        {
            var columnIndex = 1;
            worksheet.Cell(rowIndex, columnIndex++).Value = student.FullName;

            var faculty = _context.Faculties.FirstOrDefault(fac => fac.FacultyId == student.FacultyId);
            worksheet.Cell(rowIndex, 2).Value = student.BirthDate.ToString();
            worksheet.Cell(rowIndex, 3).Value = faculty.FacultyName.ToString();
            worksheet.Cell(rowIndex, 4).Value = student.Course;
            worksheet.Cell(rowIndex, 5).Value = student.CreatedAt.ToString("dd/MM/yyyy");
        }

        private void WriteStudents(IXLWorksheet worksheet, ICollection<DormDomain.Model.Student> students)
        {
            WriteHeader(worksheet);
            int rowIndex = 2;
            foreach (var student in students)
            {
                WriteStudent(worksheet, student, rowIndex);
                rowIndex++;
            }
        }

        private void WriteRooms(XLWorkbook workbook, ICollection<DormDomain.Model.Room> rooms)
        {
            //для усіх категорій формуємо сторінки
            foreach (var room in rooms)
            {

                if (room is not null)
                {
                    var worksheet = workbook.Worksheets.Add(room.RoomNumber.ToString());
                    WriteStudents(worksheet, room.Students.ToList());
                }
            }
        }

        public StudentExportService(DormContext context)
        {
            _context = context;
        }

        public async Task WriteToAsync(Stream stream, CancellationToken cancellationToken)
        {
            if (!stream.CanWrite)
            {
                throw new ArgumentException("Input stream is not writable");
            }
            //тут для прикладу пишемо усі книги в усіх категоріях, в своїх проєктах потрібно писати лише вибрані категорії та книги
            var rooms = await _context.Rooms
                .Include(room => room.Students)
                .ToListAsync(cancellationToken);

            var workbook = new XLWorkbook();

            WriteRooms(workbook, rooms);
            workbook.SaveAs(stream);
        }

    }

}
