using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Domain.Abstract;
using Domain.Entities;
using WebUI.Controllers;
using System.Collections.Generic;
using WebUI.Controllers;
using System.Linq;
using System.Web.Mvc;

namespace UnitTests
{
    [TestClass]
    public class AdminTests
    {
        [TestMethod]
        public void Index_Contains_All_Products()
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

            AdminController controller = new AdminController(mock.Object);
        

            // act
            List<Product> result = ((IEnumerable<Product>)controller.Index().ViewData.Model).ToList();

            // assert
            Assert.IsTrue(result.Count == 6);
            Assert.AreEqual(result[0].Name, "Product1");
            Assert.AreEqual(result[1].Name, "Product2");
        }
        [TestMethod]
        public void Can_Edit_Product()
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

            AdminController controller = new AdminController(mock.Object);

            // act
            Product product1 = controller.Edit(1).ViewData.Model as Product;
            Product product2 = controller.Edit(2).ViewData.Model as Product;
            Product product5 = controller.Edit(5).ViewData.Model as Product;

            // assert
            Assert.AreEqual(1, product1.ProductId);
            Assert.AreEqual(2, product2.ProductId);
            Assert.AreEqual(5, product5.ProductId);
        }

        [TestMethod]
        public void Cannot_Edit_Nonexistent_Product()
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

            AdminController controller = new AdminController(mock.Object);

            // act
            Product result = controller.Edit(7).ViewData.Model as Product;

            // assert
            Assert.IsNull(result);
        }
        [TestMethod]
        public void Can_Save_Valid_Changes()
        {
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            AdminController controller = new AdminController(mock.Object);

            Product product = new Product { Name = "Test" };

            ActionResult result = controller.Edit(product);

            mock.Verify(m => m.SaveProduct(product));

            Assert.IsNotInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public void Can_Save_Invalid_Changes()
        {
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            AdminController controller = new AdminController(mock.Object);

            Product product = new Product { Name = "Test" };

            controller.ModelState.AddModelError("error", "error");

            ActionResult result = controller.Edit(product);

            mock.Verify(m => m.SaveProduct(It.IsAny<Product>()), Times.Never());

            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }
    }
    
}

  
