using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ProjetoOficialVeiculos.Models
{
    [Table("Materiais")]
    public class Material
    {
        [Display(Name = "ID: ")]
        public int id { get; set; }

        [Required(ErrorMessage = "Campo nome é obrigatório")]
        [StringLength(35)]
        [Display(Name = "Nome: ")]
        public string nome { get; set; }

        [Required(ErrorMessage = "Campo Quantidade é obrigatório")]
        [Display(Name = "Quantidade: ")]
        public string quantidade { get; set; }

    }
}