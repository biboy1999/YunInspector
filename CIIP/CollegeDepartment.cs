using System;
using System.Collections.Generic;

namespace YunInspector.CIIP
{
    public partial class CollegeDepartment
    {
        public int? CollegeId { get; set; }
        public int? DepartmentId { get; set; }

        public virtual College College { get; set; }
        public virtual Department Department { get; set; }
    }
}
