using System;
using System.Collections.Generic;

namespace YunInspector.CIIP
{
    public partial class Department
    {
        public Department()
        {
            CollegeDepartment = new HashSet<CollegeDepartment>();
            Course = new HashSet<Course>();
        }

        public int DepartmentId { get; set; }
        public string Ename { get; set; }
        public string Name { get; set; }

        public virtual ICollection<CollegeDepartment> CollegeDepartment { get; set; }
        public virtual ICollection<Course> Course { get; set; }
    }
}
