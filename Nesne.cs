using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSV_Console
{
    internal class Nesne
    {
    }
    public class Image
    {
        public int Id { get; set; }
        public string Photo { get; set; }
        public int Product_Id { get; set; }

        public Product Product { get; set; }
    }
    public class Product
    {
        [Key]
        public int Product_Id { get; set; }
        public string Name { get; set; }
        public int Code { get; set; }
        public string Description { get; set; }
        public int Stock { get; set; }
        public string Barcode { get; set; }
        public decimal Price { get; set; }
        public string User_Id { get; set; }

        public User User { get; set; }
        public List<Image> ProductImage { get; set; }
    }
    public class User
    {
        [Key]
        public int User_Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public int Active { get; set; }
        public string Password { get; set; }
        public List<Product> Products { get; set; }
    }
}
