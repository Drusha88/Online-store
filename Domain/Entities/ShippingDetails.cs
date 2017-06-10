using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class ShippingDetails
    {
        [Required (ErrorMessage = "Укажите Ваше имя")]
        [Display(Name="Имя")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Укажите Вашу фамилию")]
        [Display(Name = "Фамилия")]
        public string SecondName { get; set; }
        [Required(ErrorMessage = "Укажите адрес доставки")]
        [Display(Name = "Адрес доставки")]
        public string Address { get; set; }
        [Required(ErrorMessage = "Укажите Ваш город")]
        [Display(Name = "Город доставки")]
        public string City { get; set; }
        [Required(ErrorMessage = "Укажите Вашу страну")]
        [Display(Name = "Страна доставки")]
        public string Country { get; set; }

        [Required(ErrorMessage = "Email")]
        [Display(Name = "E-mail")]
        public string Email { get; set; }
      
      

    }
}
