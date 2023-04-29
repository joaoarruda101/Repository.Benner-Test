using Estacionamento.Models;
using Microsoft.EntityFrameworkCore;

namespace FurquimSite.DBContext
{
    public class EstDbContext : DbContext
    {
            public DbSet<VeiculoModel> Veiculo { get; set; }
            
            public EstDbContext(DbContextOptions<EstDbContext> options)
                : base(options)
            {
            }
            public DbSet<AskerSite_.Models.ErrorViewModel> VeiculoModel { get; set; } = default!;
        
    }
}
