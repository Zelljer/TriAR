using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TriAR.Model
{
    public class Rest : BaseInformation
    {
        public string Quantity { get; set; }
        public string InformF1RegId { get; set; }
        public string InformF2RegId { get; set; }
        public int ProductId { get; set; }
        public virtual Product Product { get; set; }
    }
    public class Product : BaseInformation
    {
        public string FullName { get; set; }
        public string AlcCode { get; set; }
        public string Capacity { get; set; }
        public string AlcVolume { get; set; }
        public string ProductVCode { get; set; }
        public virtual ICollection<Rest> Rests { get; set; }
        public int ProducerId { get; set; }
        public virtual Producer Producer { get; set; }
    }
    public class Producer : BaseInformation
    {
        public string Organization { get; set; }
        public string ClientRegId { get; set; }
        public virtual ICollection<Product> Products { get; set; }
    }
    public class BaseInformation
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
    }
}
