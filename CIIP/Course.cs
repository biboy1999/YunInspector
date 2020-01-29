using System;
using System.Collections.Generic;

namespace YunInspector.CIIP
{
    public partial class Course
    {
        public Course()
        {
            Comment = new HashSet<Comment>();
            TeacherCourse = new HashSet<TeacherCourse>();
        }

        public int CourseId { get; set; }
        public int? CollegeId { get; set; }
        public int? DepartmentId { get; set; }
        public int? CourseNo { get; set; }
        public string CollegeNo { get; set; }
        public string Name { get; set; }
        public string Class { get; set; }
        public string ClassNo { get; set; }
        public int? Req { get; set; }
        public string Credit { get; set; }
        public string TimetableClassroom { get; set; }
        public string Teacher { get; set; }
        public int? Selected { get; set; }
        public string Max { get; set; }
        public string Detail { get; set; }

        public virtual College College { get; set; }
        public virtual Department Department { get; set; }
        public virtual ICollection<Comment> Comment { get; set; }
        public virtual ICollection<TeacherCourse> TeacherCourse { get; set; }
    }
}
