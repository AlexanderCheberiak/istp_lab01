using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DormInfrastructure.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChartsController : ControllerBase
    {
        private record CountByFacultyResponseItem(string Faculty, int Count);
        private record CountByCourseResponseItem(string Course, int Count);

        private readonly DormContext dormContext;

        public ChartsController(DormContext dormContext)
        {
            this.dormContext = dormContext;
        }

        [HttpGet("countByFaculty")]
        public async Task<JsonResult> GetCountByFacultyAsync(CancellationToken cancellationToken)
        {
            var responseItems = await dormContext
                .Students
                .GroupBy(student => student.Faculty.FacultyName)
                .Select(group => new CountByFacultyResponseItem(group.Key.ToString(), group.Count()))
                .ToListAsync(cancellationToken);

            return new JsonResult(responseItems);
        }

        [HttpGet("countByCourse")]
        public async Task<JsonResult> GetCountByCourseAsync(CancellationToken cancellationToken)
        {
            var responseItems = await dormContext
                .Students
                .GroupBy(student => student.Course)
                .Select(group => new CountByCourseResponseItem(group.Key.ToString(), group.Count()))
                .ToListAsync(cancellationToken);

            return new JsonResult(responseItems);
        }
    }
}