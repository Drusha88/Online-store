using Domain.Abstract;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Concrete
{
    public class EFProductRepository : IProductRepository
    {
        EFDbContext context = new EFDbContext();
        public IEnumerable<Product> Products
        {
            get { return context.Products; }
        }


        public void SaveProduct(Product product)
        {
            if(product.ProductId == 0)
            {
                context.Products.Add(product);
            }
            else
            {
                Product dbEntry = context.Products.Find(product.ProductId);
                if(dbEntry != null)
                {
                    dbEntry.Brand = product.Brand;
                    dbEntry.Category = product.Category;
                    dbEntry.Description = product.Description;
                    dbEntry.Name = product.Name;
                    dbEntry.Price = product.Price;

                }
            }
            context.SaveChanges();
        }
    }
}
