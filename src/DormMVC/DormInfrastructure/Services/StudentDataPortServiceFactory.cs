using DormDomain.Model;

using static DormInfrastructure.Services.IImportService;

namespace DormInfrastructure.Services
{
    public class StudentDataPortServiceFactory
            : IDataPortServiceFactory<Student>
    {
        private readonly DormContext _context;
        public StudentDataPortServiceFactory(DormContext context)
        {
            _context = context;
        }
        public IImportService<Student> GetImportService(string contentType)
        {
            if (contentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                return (IImportService<Student>)new StudentImportService(_context);
            }
            throw new NotImplementedException($"No import service implemented for students with content type {contentType}");
        }
        public IExportService<Student> GetExportService(string contentType)
        {
            if (contentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                return (IExportService<Student>)new StudentExportService(_context);
            }
            throw new NotImplementedException($"No export service implemented for students with content type {contentType}");
        }
    }

}
