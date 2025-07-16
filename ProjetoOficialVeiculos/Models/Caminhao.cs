using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ProjetoOficialVeiculos.Models
{
    [Table("Caminhoes")]
    public class Caminhao
    {
        [Display(Name = "ID: ")]
        public int id { get; set; }

        
        [StringLength(35)]
        [Display(Name = "Placa do  Veiculo: ")]
        public string placa { get; set; }
    }
}

