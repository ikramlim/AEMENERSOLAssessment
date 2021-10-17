using System;
using System.Collections.Generic;

#nullable disable

namespace WebAppAssessmentPart1.Models
{
    public partial class Platform
    {
        //public Platform()
        //{
        //    Wells = new HashSet<Well>();
        //}

        public int Id { get; set; }
        public string UniqueName { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public virtual ICollection<Well> Wells { get; set; }
    }
}
