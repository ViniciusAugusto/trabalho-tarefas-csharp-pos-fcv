using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Tarefas.Models
{
    public class TarefaModel
    {
        [Key]
        public int Id { get; set; } // chave primária
        
        [Required (ErrorMessage = "* Obrigatório!")]
        [Display(Name = "Título")]
        public string Titulo { get; set; } // , obrigatório, máximo de 30 caracteres

        [Display(Name = "Descrição")]
        public string Descricao { get; set; } // descricao, opcional, máximo de 150 caracteres

        [Required(ErrorMessage = "* Obrigatório!")]
        public string Criador { get; set; } // quem criou a tarefa, obrigatória, vinda do usuário que estiver logado

        [Display(Name = "Responsável")]
        public string Responsavel { get; set; } // responsável, opcional, a pessoa que deverá fazer a tarefa

        [Required(ErrorMessage = "* Obrigatório!")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        [Display(Name = "Criado em")]
        public DateTime DataCriacao { get; set; } // data da criação, obrigatório

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        [Display(Name = "Concluído em")]
        public DateTime? DataConclusao { get; set; } // data da conclusão, opcional

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        [Display(Name = "Agendado para")]
        public DateTime? DataAgendamento { get; set; } // data do agendamento, opcional

        [Display(Name = "Time")]
        public int TimeModelId { get; set; }

        public virtual TimeModel Time { get; set; }
    }
}