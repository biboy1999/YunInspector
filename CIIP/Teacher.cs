using System;
using System.Collections.Generic;

namespace YunInspector.CIIP
{
    public partial class Teacher
    {
        public Teacher()
        {
            TeacherCourse = new HashSet<TeacherCourse>();
        }

        public int TeacherId { get; set; }
        public string Name { get; set; }

        public virtual ICollection<TeacherCourse> TeacherCourse { get; set; }
    }
}
