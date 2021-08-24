namespace ECommerce_Project.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Cart_Product
    {
        public int ID { get; set; }

        public int SessionID { get; set; }

        public int ProductID { get; set; }

        public int ProductCount { get; set; }

        public decimal ProductPrice { get; set; }

        public virtual Product Product { get; set; }

        public virtual Session Session { get; set; }
    }
}
