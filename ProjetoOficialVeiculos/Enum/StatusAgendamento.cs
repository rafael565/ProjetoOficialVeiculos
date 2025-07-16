using System.ComponentModel.DataAnnotations;

namespace ControleOficialVeiculos.Enum
{
    public enum StatusAgendamento
    {
        [Display(Name = "Aguardando Liberação")]
        Pendente = 0,

        [Display(Name = "Concluido Carregamento")]
        Concluido = 1,

        [Display(Name = "Cancelado")]
        Cancelado = 2,

        [Display(Name = "Em Carregamento")]
        Carregamento = 3,

        [Display(Name = "Finalizado Faturamento")]
        Faturamento = 4,






    }
}
