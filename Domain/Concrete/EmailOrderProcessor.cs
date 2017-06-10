using Domain.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Mail;
using System.Configuration;
using Domain.Entities;

namespace Domain.Concrete
{
    public class EmailSettings
    {
        public string ServerName;
        public int ServerPort;
        public bool UseSsl;
        public string Login;
        public string Password;
        public bool WriteAsFile;
        public string FileLocation = @"c:\product_store_emails";
        public EmailSettings()
        {
            ServerName = ConfigurationManager.AppSettings["ServerName"];
            ServerPort = Convert.ToInt32(ConfigurationManager.AppSettings["ServerPort"]);
            Login = ConfigurationManager.AppSettings["Login"];
            Password = ConfigurationManager.AppSettings["Password"];
            UseSsl = Convert.ToBoolean(ConfigurationManager.AppSettings["UseSsl"]);
            WriteAsFile = Convert.ToBoolean(ConfigurationManager.AppSettings["WriteAsFile"]);
        }

    }

    public class EmailOrderProcessor : IOrderProcessor
    {
        private EmailSettings emailSettings;
        public EmailOrderProcessor(EmailSettings settings)
        {
            emailSettings = settings;
        }

        public void ProcessOrder(Entities.Cart cart, Entities.ShippingDetails shippingDetails)
        {
            using (var smtpClient = new SmtpClient())
            {
                smtpClient.EnableSsl = emailSettings.UseSsl;
                smtpClient.Host = emailSettings.ServerName;
                smtpClient.Port = emailSettings.ServerPort;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential(emailSettings.Login, emailSettings.Password);

                if (emailSettings.WriteAsFile)
                {
                    smtpClient.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;
                    smtpClient.PickupDirectoryLocation = emailSettings.FileLocation;
                    smtpClient.EnableSsl = false;
                }

                StringBuilder body = new StringBuilder()
                .AppendLine("Новый заказ обработан")
                .AppendLine("---")
                .AppendLine("Товары:");

                foreach (var line in cart.Lines)
                {
                    var subtotal = line.Product.Price * line.Quantity;
                    body.AppendFormat("{0} x {1} (итого: {2:0.00})", line.Quantity, line.Product.Name, subtotal);
                }

                body.AppendLine()
                    .AppendFormat("Общая стоимость: {0:0.00}", cart.ComputeTotalValue())
                    .AppendLine()
                    .AppendLine("---")
                    .AppendLine("Доставка:")
                    .AppendLine(shippingDetails.FirstName + " " + shippingDetails.SecondName)
                    .AppendLine(shippingDetails.Country)
                    .AppendLine(shippingDetails.City)
                    .AppendLine(shippingDetails.Address)
                    .AppendLine("---")
                    .AppendLine("Спасибо за Ваш заказ, в ближайшее время с Вами свяжется наш менеджер.");

                MailMessage mailMessage = new MailMessage(
                    emailSettings.Login,
                    shippingDetails.Email,
                    "Новый заказ отправлен!",
                    body.ToString()
                    );

                if (emailSettings.WriteAsFile)
                {
                    mailMessage.BodyEncoding = Encoding.UTF8;
                }

                try
                {
                    smtpClient.Send(mailMessage);
                }
                catch (Exception)
                {
                }
            }
        }
    }
}
