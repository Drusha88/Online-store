using Domain.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebUI.Models;

namespace WebUI.Controllers
{
    public class ProductsController : Controller
    {
        //
        // GET: /Product/
        private IProductRepository repository;
        public int pageSize = 5;
        public ProductsController(IProductRepository repository)
        {
            this.repository = repository;
        }
        public ViewResult List(string category, int page = 1)
        {
            ProductsListViewModel model = new ProductsListViewModel
            {
                Products = repository.Products
                .Where(p => category == null || p.Category == category)
                .OrderBy(product => product.ProductId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize),
                PagingInfo = new PagingInfo
                {
                    CurrentPage = page,
                    ItemsPerPage = pageSize,
                    TotalItems = category == null ?
                    repository.Products.Count() :
                    repository.Products.Where(p => p.Category == category).Count()
                },
                CurrentCategory = category
            };
            return View(model);
        }
    }
}