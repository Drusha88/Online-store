using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Domain.Abstract;
using System.Collections.Generic;
using Domain.Entities;
using WebUI.Controllers;
using System.Linq;
using System.Web.Mvc;
using WebUI.Models;
using WebUI.HtmlHelpers;


namespace UnitTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void Can_Paginate()
        {
            // arrange
            Mock<IProductRepository> mock = new Mock<IProductRepository>();

            mock.Setup(m => m.Products).Returns(new List<Product>
            {
                new Product{ProductId = 1, Name = "Product1"},
                new Product{ProductId = 2, Name = "Product2"},
                new Product{ProductId = 3, Name = "Product3"},
                new Product{ProductId = 4, Name = "Product4"},
                new Product{ProductId = 5, Name = "Product5"},
                new Product{ProductId = 6, Name = "Product6"}
            });

            ProductsController controller = new ProductsController(mock.Object);
            controller.pageSize = 4;

            // act
            ProductsListViewModel result = (ProductsListViewModel)controller.List(null, 2).Model;

            // assert
            List<Product> Products = result.Products.ToList();
            Assert.IsTrue(Products.Count == 2);
            Assert.AreEqual(Products[0].Name, "Product5");
            Assert.AreEqual(Products[1].Name, "Product6");
        }

        [TestMethod]
        public void Can_Generate_Page_Links()
        {
            // arrange
            HtmlHelper myHelper = null;
            PagingInfo pagingInfo = new PagingInfo
            {
                CurrentPage = 2,
                TotalItems = 28,
                ItemsPerPage = 10
            };
            Func<int, string> pageUrlDelegate = i => "Page" + i;

            // act
            MvcHtmlString result = myHelper.PageLinks(pagingInfo, pageUrlDelegate);

            // assert
            Assert.AreEqual(@"<a class=""btn btn-default"" href=""Page1"">1</a>"
                + @"<a class=""btn btn-default btn-primary selected"" href=""Page2"">2</a>"
                + @"<a class=""btn btn-default"" href=""Page3"">3</a>",
                result.ToString());
        }

        [TestMethod]
        public void Can_Send_Pagination_View_Model()
        {
            // arrange
            Mock<IProductRepository> mock = new Mock<IProductRepository>();

            mock.Setup(m => m.Products).Returns(new List<Product>
            {
                new Product{ProductId = 1, Name = "Product1"},
                new Product{ProductId = 2, Name = "Product2"},
                new Product{ProductId = 3, Name = "Product3"},
                new Product{ProductId = 4, Name = "Product4"},
                new Product{ProductId = 5, Name = "Product5"},
                new Product{ProductId = 6, Name = "Product6"}
            });

            ProductsController controller = new ProductsController(mock.Object);
            controller.pageSize = 4;

            // act
            ProductsListViewModel result = (ProductsListViewModel)controller.List(null, 2).Model;
            
            // assert
            PagingInfo pagingInfo = result.PagingInfo;
            Assert.AreEqual(pagingInfo.CurrentPage, 2);
            Assert.AreEqual(pagingInfo.ItemsPerPage, 4);
            Assert.AreEqual(pagingInfo.TotalItems, 6);
            Assert.AreEqual(pagingInfo.TotalPages, 2);
        }

        [TestMethod]
        public void Can_Filter_Products()
        {
            // arrange
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(p => p.Products).Returns(new List<Product>
            {
                new Product{ProductId = 1, Name = "Product1", Category="Category1"},
                new Product{ProductId = 2, Name = "Product2", Category="Category2"},
                new Product{ProductId = 3, Name = "Product3", Category="Category3"},
                new Product{ProductId = 4, Name = "Product4", Category="Category1"},
                new Product{ProductId = 5, Name = "Product5", Category="Category2"},
                new Product{ProductId = 6, Name = "Product6", Category="Category3"}
            });

            ProductsController controller = new ProductsController(mock.Object);
            controller.pageSize = 4;

            // act
            List<Product> result = ((ProductsListViewModel)controller.List("Category2", 1).Model).Products.ToList();

            // assert
            Assert.AreEqual(result.Count(), 2);
            Assert.IsTrue(result[0].Name == "Product2" && result[0].Category == "Category2");
            Assert.IsTrue(result[1].Name == "Product5" && result[1].Category == "Category2");
        }

        [TestMethod]
        public void Can_Create_Categories()
        {
            // arrange
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(p => p.Products).Returns(new List<Product>
            {
                new Product{ProductId = 1, Name = "Product1", Category="Category1"},
                new Product{ProductId = 2, Name = "Product2", Category="Category2"},
                new Product{ProductId = 3, Name = "Product3", Category="Category3"},
                new Product{ProductId = 4, Name = "Product4", Category="Category1"},
                new Product{ProductId = 5, Name = "Product5", Category="Category2"},
                new Product{ProductId = 6, Name = "Product6", Category="Category3"}
            });

            NavController target = new NavController(mock.Object);

            // act
            List<string> result = ((IEnumerable<string>)target.Menu().Model).ToList();

            // assert
            Assert.AreEqual(result.Count(), 3);
            Assert.AreEqual(result[0], "Category1");
            Assert.AreEqual(result[1], "Category2");
            Assert.AreEqual(result[2], "Category3");
        }

        [TestMethod]
        public void Indicates_Selected_Category()
        {
            // arrange
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(p => p.Products).Returns(new List<Product>
            {
                new Product{ProductId = 1, Name = "Product1", Category="Category1"},
                new Product{ProductId = 2, Name = "Product2", Category="Category2"},
                new Product{ProductId = 3, Name = "Product3", Category="Category3"},
                new Product{ProductId = 4, Name = "Product4", Category="Category1"},
                new Product{ProductId = 5, Name = "Product5", Category="Category2"},
                new Product{ProductId = 6, Name = "Product6", Category="Category3"}
            });

            NavController target = new NavController(mock.Object);

            string categoryToSelect = "Category2";

            // act
            string result = target.Menu(categoryToSelect).ViewBag.SelectedCategory;

            // assert
            Assert.AreEqual(categoryToSelect, result);
        }

        [TestMethod]
        public void Generete_Genre_Specific_Products_Count()
        {
            // arrange
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(p => p.Products).Returns(new List<Product>
            {
                new Product{ProductId = 1, Name = "Product1", Category="Category1"},
                new Product{ProductId = 2, Name = "Product2", Category="Category2"},
                new Product{ProductId = 3, Name = "Product3", Category="Category3"},
                new Product{ProductId = 4, Name = "Product4", Category="Category1"},
                new Product{ProductId = 5, Name = "Product5", Category="Category2"},
                new Product{ProductId = 6, Name = "Product6", Category="Category1"}
            });

            ProductsController controller = new ProductsController(mock.Object);
            controller.pageSize = 4;

            // act
            int res1 = ((ProductsListViewModel)controller.List("Category1").Model).PagingInfo.TotalItems;
            int res2 = ((ProductsListViewModel)controller.List("Category2").Model).PagingInfo.TotalItems;
            int res3 = ((ProductsListViewModel)controller.List("Category3").Model).PagingInfo.TotalItems;
            int resAll = ((ProductsListViewModel)controller.List(null).Model).PagingInfo.TotalItems;

            // assert
            Assert.AreEqual(res1, 3);
            Assert.AreEqual(res2, 2);
            Assert.AreEqual(res3, 1);
            Assert.AreEqual(resAll, 6);
        }


    }
}
