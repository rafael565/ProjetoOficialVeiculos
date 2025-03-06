using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using ControleOficialVeiculos.Enum;

namespace ProjetoOficialVeiculos.Models
{
        [Table("Agendamentos")]
    public class Agendamento
    {
        [Key]
        [Display(Name = "ID: ")]
        public int id { get; set; }

        public Caminhao caminhao { get; set; }
        [Display(Name = "Caminhões: ")]
        public int caminhaoID { get; set; }

        public Empresa empresa { get; set; }
        [Display(Name = "Empresas: ")]
        public int empresaID { get; set; }

        public Material material { get; set; }
        [Display(Name = "Materiais: ")]
        public int MaterialID { get; set; }

        [Required]
        [Display(Name = "Data do Agendamento: ")]
        public DateTime DataAgendamento { get; set; }

        [Required]
        [Display(Name = "Peso Carregado (kg): ")]
        public string PesoCarregado { get; set; }


        [Display(Name = "Status do Agendamento: ")]
        public StatusAgendamento Status { get; set; } = StatusAgendamento.Pendente;

        [Display(Name = "Data de Conclusão: ")]
        public DateTime DataConclusao { get; set; }

        [Display(Name = "Ordem na Fila")]
        public int OrdemFila { get; set; }

        public Motorista motorista { get; set; }
        [Display(Name = "Motoristas: ")]
        public int MotoristaID { get; set; }



    }
}
