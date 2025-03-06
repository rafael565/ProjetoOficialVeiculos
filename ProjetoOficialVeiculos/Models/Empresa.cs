using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ProjetoOficialVeiculos.Models
{

    [Table("Empresas")]
    public class Empresa
    {
        [Display(Name = "ID: ")]
        public int id { get; set; }


        [Required(ErrorMessage = "Campo nome é obrigatório")]
        [StringLength(35)]
        [Display(Name = "Nome Da Empresa: ")]
        public string nome { get; set; }

        [Required(ErrorMessage = "Campo CNPJ é obrigatório")]
        [StringLength(35)]
        [Display(Name = "CNPJ: ")]
        public string cnpj { get; set; }


        [Required(ErrorMessage = "Campo contato é obrigatório")]
        [StringLength(35)]
        [Display(Name = "Contato : ")]
        public string contato { get; set; }

        [Required(ErrorMessage = "Campo endereço é obrigatório")]
        [StringLength(35)]
        [Display(Name = "endereço : ")]
        public string endereco { get; set; }

    }
}
