using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Domain.Entities
{
    public class Product
    {
        
        [HiddenInput(DisplayValue = false)]
        [Display(Name = "ID")]
        public int ProductId { get; set; }

        
        [Required(ErrorMessage = "Пожалуйста, введите название товара")]
        [Display(Name = "Название")]
        public string Name { get; set; }

         [Required(ErrorMessage = "Пожалуйста, введите производителя товара")]
         [Display(Name = "Производитель")]
        public string Brand { get; set; }

         [DataType(DataType.MultilineText)]
         [Required(ErrorMessage = "Пожалуйста, введите описание товара")]

         [Display(Name = "Описание")]
        public string Description { get; set; }
         
         [Required(ErrorMessage = "Пожалуйста, введите категорию товара")]
         [Display(Name = "Категория")]
        public string Category { get; set; }

        [Required(ErrorMessage = "Пожалуйста, введите цену  товара")]
        [Range(0.001, double.MaxValue, ErrorMessage="Пожалуйста, введите положительное число")]
         [Display(Name = "Цена (грн)")]
        public decimal Price { get; set; }
    }
}
