using System;
using System.Collections.Generic;

namespace YunInspector.CIIP
{
    public partial class College
    {
        public College()
        {
            CollegeDepartment = new HashSet<CollegeDepartment>();
            Course = new HashSet<Course>();
        }

        public int CollegeId { get; set; }
        public string Name { get; set; }

        public virtual ICollection<CollegeDepartment> CollegeDepartment { get; set; }
        public virtual ICollection<Course> Course { get; set; }
    }
}
