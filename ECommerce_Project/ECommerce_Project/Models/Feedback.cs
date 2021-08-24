namespace ECommerce_Project.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Feedback")]
    public partial class Feedback
    {
        public int ID { get; set; }

        [Required]
        [StringLength(500)]
        public string Message { get; set; }

        public DateTime Date { get; set; }

        public int UserID { get; set; }

        [Required]
        [StringLength(25)]
        public string Subject { get; set; }

        public virtual User User { get; set; }
    }
}
