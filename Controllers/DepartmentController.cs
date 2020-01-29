using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YunInspector.CIIP;

namespace YunInspector.Controllers
{
    [ApiController]
    [Route("api/department")]
    public class DepartmentController : ControllerBase
    {
        private readonly CIIPContext _context;
        public DepartmentController(CIIPContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<object>> GetAllList()
        {
            var result = _context.CollegeDepartment.Join(_context.Department, d => d.DepartmentId, cd => cd.DepartmentId, (d, cd) => new
            {
                d.CollegeId,
                CollegeName=d.College.Name,
                d.DepartmentId,
                DapartmentName = d.Department.Name
            });
            return await result.ToListAsync();
        }

        [HttpGet]
        [Route("{collegeID}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<object>> GetDepartmentList(int collegeID)
        {
            var result = from department in _context.Set<Department>()
                         join CollegeDepartment in _context.Set<CollegeDepartment>()
                            on department.DepartmentId equals CollegeDepartment.DepartmentId
                         where CollegeDepartment.CollegeId.Value == collegeID
                         select new { department.DepartmentId, department.Name };
            return Ok(await result.ToListAsync());
        }

    }
}