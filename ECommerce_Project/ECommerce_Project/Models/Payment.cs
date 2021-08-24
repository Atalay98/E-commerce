namespace ECommerce_Project.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Payment")]
    public partial class Payment
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Payment()
        {
            Orders = new HashSet<Order>();
        }

        public int ID { get; set; }

        [Required]
        [StringLength(50)]
        public string Type { get; set; }

        [Required]
        [StringLength(25)]
        public string Sehir { get; set; }

        [Required]
        [StringLength(25)]
        public string Ilce { get; set; }

        [Required]
        [StringLength(100)]
        public string Address { get; set; }

        [StringLength(16)]
        public string CreditCartNo { get; set; }

        [StringLength(5)]
        public string ExpiryDate { get; set; }

        [StringLength(3)]
        public string Cvv { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Order> Orders { get; set; }
    }
}
