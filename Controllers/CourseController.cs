using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using YunInspector.CIIP;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Ganss.XSS;
using YunInspector.models;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json.Linq;

namespace YunInspector.Controllers
{
    [ApiController]
    [Route("api/course")]
    public class CourseController : ControllerBase
    {
        private readonly CIIPContext _context;
        public CourseController(CIIPContext context)
        {
            _context = context;
        }

        public class SearchQuery
        {
            [FromQuery(Name = "college")]
            public int? college { get; set; }
            [FromQuery(Name = "department")]
            public int? department { get; set; }
            [FromQuery(Name = "teacher")]
            public string teacher { get; set; }
            [FromQuery(Name = "name")]
            public string name { get; set; }
            [FromQuery(Name = "req")]
            public int? req { get; set; }
            [FromQuery(Name = "page")]
            public int page { get; set; }
        }

        [HttpGet]
        [Route("")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IEnumerable<object>> GetList([FromQuery]SearchQuery queryParm)
        {
            var data = _context.Course.AsQueryable();
            if (queryParm.college != null)
            {
                data = data.Where(x => x.CollegeId == queryParm.college);
            }
            if (queryParm.department != null)
            {
                data = data.Where(x => x.DepartmentId == queryParm.department);
            }
            if (queryParm.teacher != null && queryParm.teacher != String.Empty)
            {
                data = data.Where(x => EF.Functions.Like(x.Teacher, $"%{queryParm.teacher}%"));
            }
            if (queryParm.name != null && queryParm.name != String.Empty)
            {
                data = data.Where(x => EF.Functions.Like(x.Name, $"%{queryParm.name}%"));
            }
            if (queryParm.req != null)
            {
                data = data.Where(x => x.Req == queryParm.req);
            }

            int total = await data.CountAsync();

            var result = data.Skip(queryParm.page * 50)
                             .Take(50)
                             .Include(x => x.Department)
                             .Include(x => x.College)
                             .Include(x => x.Comment).ThenInclude(x => x.User)
                             .Select(x => new
                             {
                                 x.CourseId,
                                 x.CollegeId,
                                 x.DepartmentId,
                                 CollegeName = x.College.Name,
                                 DepartmentName = x.Department.Name,
                                 x.CourseNo,
                                 x.CollegeNo,
                                 x.ClassNo,
                                 x.Class,
                                 CourseName = x.Name,
                                 x.Teacher,
                                 x.Req,
                                 x.TimetableClassroom,
                                 x.Credit,
                                 AvgRating = Math.Round(x.Comment.Where(c => c.IsDeleted == false && c.User.IsBlocked == false)
                                                                 .Average(c => c.Rating).Value, 2)
                             });

            JArray res = JArray.FromObject(await result.ToListAsync());
            JObject json = new JObject(new JProperty("total", total), new JProperty("data", res));
            return json;
        }

        [HttpGet]
        [Route("{CourseID}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IEnumerable<object>> GetDetail(int CourseID)
        {
            var result = _context.Course.Where(x => x.CourseId == CourseID)
                                        .Include(x => x.Department)
                                        .Include(x => x.College)
                                        .Include(x => x.Comment)
                                        .ThenInclude(x => x.User)
                                        .Select(x => new
                                        {
                                            x.CollegeId,
                                            departmnetName = x.Department.Name,
                                            collegeName = x.College.Name,
                                            x.Name,
                                            x.Class,
                                            x.ClassNo,
                                            x.Req,
                                            x.Credit,
                                            x.TimetableClassroom,
                                            x.Teacher,
                                            x.Detail,
                                            AvgRating = Math.Round(x.Comment.Where(x => x.IsDeleted == false && x.User.IsBlocked == false)
                                                                 .Average(x => x.Rating).Value, 2)
                                        });
            return await result.ToListAsync();
        }

        [HttpGet]
        [Route("{CourseID}/comment")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IEnumerable<object>> GetComment(int CourseID)
        {
            var result = _context.Comment.Where(x => x.Course.CourseId == CourseID && x.IsDeleted == false && x.User.IsBlocked == false)
                                         .Include(x => x.User)
                                         .Include(x => x.Course)
                                         .Select(x => new
                                         {
                                             x.Rating,
                                             Content = x.CommentContent,
                                             PostDate = x.PostDate
                                         })
                                         .OrderByDescending(x => x.PostDate);
            return await result.ToListAsync();
        }

        [Authorize]
        [HttpPost]
        [Route("{CourseID}/comment")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> GetComment(int CourseID, [FromBody]CommentInput content)
        {
            //input validation
            if (content.Rating > 10 || content.Rating < 0)
                return BadRequest();

            //user auth
            var tokenUserId = int.Parse(User.Identity.Name);

            var newComment = new Comment()
            {
                UserId = tokenUserId,
                CourseId = CourseID,
                PostDate = DateTime.Now,
                Rating = content.Rating,
                ReportCount = 0,
                IsDeleted = false
            };

            var santitizer = new HtmlSanitizer();
            newComment.CommentContent = santitizer.Sanitize(content.CommentContent);

            if (newComment.CommentContent == String.Empty)
            {
                return BadRequest();
            }
            await _context.Comment.AddAsync(newComment);
            await _context.SaveChangesAsync();
            return Ok("Comment added.");
        }
    }
}