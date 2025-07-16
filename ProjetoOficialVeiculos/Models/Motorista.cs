using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ProjetoOficialVeiculos.Models
{
    [Table("Motoristas")]
    public class Motorista
    {
        [Display(Name = "ID: ")]
        public int id { get; set; }

        
        [StringLength(35)]
        [Display(Name = "Nome Do Motorista: ")]
        public string nome { get; set; }
    }
}
