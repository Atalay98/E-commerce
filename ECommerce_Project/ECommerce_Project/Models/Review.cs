namespace ECommerce_Project.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Review")]
    public partial class Review
    {
        public int ID { get; set; }

        public int UserID { get; set; }

        public int ProductID { get; set; }

        [Column(TypeName = "text")]
        public string Comment { get; set; }

        public int? Rating { get; set; }

        public DateTime Date { get; set; }

        public virtual Product Product { get; set; }

        public virtual User User { get; set; }
    }
}
