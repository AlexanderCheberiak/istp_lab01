using DocumentFormat.OpenXml.Vml.Office;
using Microsoft.CodeAnalysis.Editing;
using DormDomain.Model;
using DormDomain;

namespace DormInfrastructure.Services
{
    public interface IImportService
    {
        public interface IImportService<TEntity>
    where TEntity : Entity
        {
            Task ImportFromStreamAsync(Stream stream, CancellationToken cancellationToken);
        }

    }
}
