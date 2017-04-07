using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebUI.Models
{
    public class ProductsListViewModel
    {
        public PagingInfo PagingInfo { get; set; }
        public IEnumerable<Product> Products { get; set; }
        public string CurrentCategory { get; set; }
    }
}