using Microsoft.EntityFrameworkCore;

namespace ProjetoOficialVeiculos.Models
{
    public class Contexto: DbContext
    {
        public Contexto(DbContextOptions<Contexto> options) : base(options) { }

        public DbSet<Agendamento> Agendamentos { get; set; }

        public DbSet<Caminhao> Caminhoes { get; set; }

        public DbSet<Empresa> Empresas { get; set; }
        public DbSet<Material> Materiais { get; set; }
        public DbSet<Motorista> Motoristas { get; set; }


    }
}
