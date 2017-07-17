using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Tarefas.Models
{
    public class TimeModel
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "* Obrigatório")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "* Obrigatório")]
        public string Dono { get; set; }

        public virtual ICollection<UsuariosTarefasModel> Usuarios { get; set; }
    }
}