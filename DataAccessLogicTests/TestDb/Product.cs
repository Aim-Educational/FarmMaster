using System.ComponentModel.DataAnnotations;

namespace DataAccessLogicTests.TestDb
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }

        [Required]
        public string Name { get; set; }
    }
}
