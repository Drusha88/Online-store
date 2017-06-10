using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using Moq;
using Domain.Abstract;
using WebUI.Controllers;
using System.Web.Mvc;
using WebUI.Models;

namespace UnitTests
{
    [TestClass]
    public class CartTest
    {
        [TestMethod]
        public void Can_Add_New_Lines()
        {
            // arrange
            Product product1 = new Product { ProductId = 1, Name = "Product1" };
            Product product2 = new Product { ProductId = 2, Name = "Product2" };

            Cart cart = new Cart();

            // act
            cart.AddItem(product1, 1);
            cart.AddItem(product2, 1);
            List<CartLine> results = cart.Lines.ToList();

            // assert
            Assert.AreEqual(results.Count(), 2);
            Assert.AreEqual(results[0].Product, product1);
            Assert.AreEqual(results[1].Product, product2);
        }

        [TestMethod]
        public void Can_Add_Quantity_For_Existing_Lines()
        {
            // arrange
            Product product1 = new Product { ProductId = 1, Name = "Product1" };
            Product product2 = new Product { ProductId = 2, Name = "Product2" };

            Cart cart = new Cart();

            // act
            cart.AddItem(product1, 1);
            cart.AddItem(product2, 1);
            cart.AddItem(product1, 5);
            List<CartLine> results = cart.Lines.OrderBy(c => c.Product.ProductId).ToList();

            // assert
            Assert.AreEqual(results.Count(), 2);
            Assert.AreEqual(results[0].Quantity, 6);
            Assert.AreEqual(results[1].Quantity, 1);
        }

        [TestMethod]
        public void Can_Remove_Line()
        {
            // arrange
            Product product1 = new Product { ProductId = 1, Name = "Product1" };
            Product product2 = new Product { ProductId = 2, Name = "Product2" };
            Product product3 = new Product { ProductId = 3, Name = "Product3" };

            Cart cart = new Cart();

            // act
            cart.AddItem(product1, 1);
            cart.AddItem(product2, 1);
            cart.AddItem(product1, 5);
            cart.AddItem(product3, 2);
            cart.RemoveLine(product2);

            // assert
            Assert.AreEqual(cart.Lines.Where(c => c.Product == product2).Count(), 0);
            Assert.AreEqual(cart.Lines.Count(), 2);
        }

        [TestMethod]
        public void Calculate_Cart_Total()
        {
            // arrange
            Product product1 = new Product { ProductId = 1, Name = "Product1", Price = 11};
            Product product2 = new Product { ProductId = 2, Name = "Product2", Price = 22};
           

            Cart cart = new Cart();

            // act
            cart.AddItem(product1, 3);
            cart.AddItem(product2, 1);

            // assert
            Assert.AreEqual(cart.ComputeTotalValue(), 55);
        }

        [TestMethod]
        public void Can_Clear_Contents()
        {
            // arrange
            Product product1 = new Product { ProductId = 1, Name = "Product1", Price = 11 };
            Product product2 = new Product { ProductId = 2, Name = "Product2", Price = 22 };


            Cart cart = new Cart();

            // act
            cart.AddItem(product1, 3);
            cart.AddItem(product2, 1);
            cart.Clear();

            // assert
            Assert.AreEqual(cart.Lines.Count(), 0);
        }

        [TestMethod]
        public void Can_Add_To_Cart()
        {
            // arrange
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new List<Product>{
                new Product {ProductId = 1, Name = "Product1", Category = "Category1"}
            }.AsQueryable());

            Cart cart = new Cart();

            CartController controller = new CartController(mock.Object, null);

            // act
            controller.AddToCart(cart, 1, null);
            // assert
            Assert.AreEqual(cart.Lines.Count(), 1);
            Assert.AreEqual(cart.Lines.ToList()[0].Product.ProductId, 1);
        }


        [TestMethod]
        public void Adding_Product_To_Cart_Goes_To_Cart_Screen()
        {
            // arrange
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new List<Product>{
                new Product {ProductId = 1, Name = "Product1", Category = "Category1"}
            }.AsQueryable());

            Cart cart = new Cart();

            CartController controller = new CartController(mock.Object, null);

            RedirectToRouteResult result = controller.AddToCart(cart, 2, "myUrl");
            // assert
            Assert.AreEqual(result.RouteValues["action"], "Index");
            Assert.AreEqual(result.RouteValues["returnUrl"], "myUrl");
        }


        [TestMethod]
        public void Can_View_Cart_Contents()
        {
            // arrange
            Cart cart = new Cart();
            CartController target = new CartController(null, null);
            // act
            CartIndexViewModel result = (CartIndexViewModel)target.Index(cart, "myUrl").ViewData.Model;
            // assert
            Assert.AreSame(result.Cart, cart);
            Assert.AreEqual(result.ReturnUrl, "myUrl");

        }
        [TestMethod]
        public void Cannot_Checkout_Empty_Cart()
        {
            // arrange
            Mock<IOrderProcessor> mock = new Mock<IOrderProcessor>();
            Cart cart = new Cart();
            ShippingDetails shippingDetails = new ShippingDetails();

            CartController controller = new CartController(null, mock.Object);

            ViewResult result = controller.Checkout(cart, shippingDetails);
            // act
            mock.Verify(m => m.ProcessOrder(It.IsAny<Cart>(), It.IsAny<ShippingDetails>()), Times.Never());
            // assert
            Assert.AreEqual("", result.ViewName);
            Assert.AreEqual(false, result.ViewData.ModelState.IsValid);

        }

        [TestMethod]
        public void Cannot_Checkout_Invalid_ShippingDetails()
        {
            // arrange
            Mock<IOrderProcessor> mock = new Mock<IOrderProcessor>();
            Cart cart = new Cart();
            cart.AddItem(new Product(), 1);

            CartController controller = new CartController(null, mock.Object);
            controller.ModelState.AddModelError("error", "error");

            ViewResult result = controller.Checkout(cart, new ShippingDetails());
            // act
            mock.Verify(m => m.ProcessOrder(It.IsAny<Cart>(), It.IsAny<ShippingDetails>()), Times.Never());
            // assert
            Assert.AreEqual("", result.ViewName);
            Assert.AreEqual(false, result.ViewData.ModelState.IsValid);

        }

        [TestMethod]
        public void Cannot_Checkout_Submit_Order()
        {
            // arrange
            Mock<IOrderProcessor> mock = new Mock<IOrderProcessor>();
            Cart cart = new Cart();
            cart.AddItem(new Product(), 1);

            CartController controller = new CartController(null, mock.Object);
        

            ViewResult result = controller.Checkout(cart, new ShippingDetails());
            // act
            mock.Verify(m => m.ProcessOrder(It.IsAny<Cart>(), It.IsAny<ShippingDetails>()), Times.Once());
            // assert
            Assert.AreEqual("Completed", result.ViewName);
            Assert.AreEqual(true, result.ViewData.ModelState.IsValid);

        }
    }
}
