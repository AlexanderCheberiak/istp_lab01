using DormDomain;
using static DormInfrastructure.Services.IImportService;

namespace DormInfrastructure.Services
{
    public interface IDataPortServiceFactory<TEntity>
   where TEntity : Entity
    {
        IImportService<TEntity> GetImportService(string contentType);
        IExportService<TEntity> GetExportService(string contentType);
    }

}
