using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Tarefas.Models
{
    public class NotasModel
    {
        [Key]
        public int Id { get; set; } //chave primaria

        [Required(ErrorMessage = "* Obrigatório!")]
        [Display(Name = "Texto")]
        public string Texto { get; set; } // , obrigatório,
        
        [Required(ErrorMessage = "* Obrigatório!")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        [Display(Name = "Data/Hora")]
        public DateTime dataHora { get; set; }

        [Display(Name = "Usuário")]
        public string Usuario { get; set; }
    }
}