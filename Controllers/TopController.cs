using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YunInspector.CIIP;

namespace YunInspector.Controllers
{
    [ApiController]
    [Route("api/top")]
    public class TopController : ControllerBase
    {
        private readonly CIIPContext _context;
        public TopController(CIIPContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("course")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<object>> GetTopCourse()
        {
            var query = _context.Comment.Where(x => x.IsDeleted == false && x.User.IsBlocked == false)
                                        .Include(x => x.User)
                                        .GroupBy(x => x.CourseId)
                                        .Select(x => new
                                        {
                                            CourseId = x.Key,
                                            AvgRating = Math.Round(x.Average(p => p.Rating).Value, 2)
                                        })
                                        .OrderByDescending(x => x.AvgRating)
                                        .Join(_context.Course, c => c.CourseId, m => m.CourseId, (c, m) => new
                                        {
                                            CourseID = c.CourseId,
                                            College = m.College.Name,
                                            Department = m.Department.Name,
                                            CourseName = m.Name,
                                            m.Req,
                                            c.AvgRating
                                        });

            return Ok(await query.ToListAsync());
        }
        [HttpGet]
        [Route("comment")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> GetNewestComment()
        {
            var query = _context.Comment.Where(x => x.IsDeleted == false && x.User.IsBlocked == false)
                                        .OrderByDescending(x => x.PostDate)
                                        .Include(x => x.User)
                                        .Include(x => x.Course).ThenInclude(x => x.College)
                                        .Include(x => x.Course).ThenInclude(x => x.Department)
                                        .Select(x => new
                                        {
                                            CourseID = x.CourseId,
                                            College = x.Course.College.Name,
                                            Department = x.Course.Department.Name,
                                            CourseName = x.Course.Name,
                                            Req = x.Course.Req,
                                            Content = x.CommentContent,
                                            Rating = x.Rating,
                                            PostDate = x.PostDate
                                        });
            return Ok(await query.ToListAsync());
        }
    }
}