using System;
using System.Collections.Generic;

namespace YunInspector.CIIP
{
    public partial class TeacherCourse
    {
        public int CourseId { get; set; }
        public int TeacherId { get; set; }

        public virtual Course Course { get; set; }
        public virtual Teacher Teacher { get; set; }
    }
}
