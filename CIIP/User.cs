using System;
using System.Collections.Generic;

namespace YunInspector.CIIP
{
    public partial class User
    {
        public User()
        {
            Comment = new HashSet<Comment>();
        }

        public int UserId { get; set; }
        public string GoogleId { get; set; }
        public bool IsBlocked { get; set; }

        public virtual ICollection<Comment> Comment { get; set; }
    }
}
