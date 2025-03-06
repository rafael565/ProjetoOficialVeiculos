using System.ComponentModel.DataAnnotations;

namespace ControleOficialVeiculos.Enum
{
    public enum StatusAgendamento
    {
        [Display(Name = "Pendente")]
        Pendente = 0,

        [Display(Name = "Concluido")]
        Concluido = 1,

        [Display(Name = "Cancelado")]
        Cancelado = 2,
    }
}
