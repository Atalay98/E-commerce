namespace ECommerce_Project.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Order")]
    public partial class Order
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Order()
        {
            Order_Product = new HashSet<Order_Product>();
        }

        public int ID { get; set; }

        public decimal TotalProductsPrice { get; set; }

        public DateTime OrderDate { get; set; }

        public int CargoID { get; set; }

        public int UserID { get; set; }

        public int PaymentID { get; set; }

        public decimal TotalPrice { get; set; }

        [Required]
        [StringLength(20)]
        public string Status { get; set; }

        public virtual Cargo Cargo { get; set; }

        public virtual Payment Payment { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Order_Product> Order_Product { get; set; }

        public virtual User User { get; set; }
    }
}
