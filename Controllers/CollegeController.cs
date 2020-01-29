using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YunInspector.CIIP;

namespace YunInspector.Controllers
{
    [ApiController]
    [Route("api/college")]
    public class CollegeController : ControllerBase
    {
        private readonly CIIPContext _context;
        public CollegeController(CIIPContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<College>> GetCollegeList()
        {
            return Ok(await _context.College.ToListAsync());
        }
    }
}