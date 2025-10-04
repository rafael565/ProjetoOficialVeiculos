
using System.Collections.Generic;

namespace ProjetoOficialVeiculos.Models.Account
{
    public class UserWithRolesViewModel
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public List<string> Roles { get; set; } = new List<string>();
    }
}