using System;
using System.Collections.Generic;

namespace YunInspector.CIIP
{
    public partial class Comment
    {
        public int CommentId { get; set; }
        public int? CourseId { get; set; }
        public int? UserId { get; set; }
        public DateTime? PostDate { get; set; }
        public int? Rating { get; set; }
        public string CommentContent { get; set; }
        public int? ReportCount { get; set; }
        public bool IsDeleted { get; set; }

        public virtual Course Course { get; set; }
        public virtual User User { get; set; }
    }
}
