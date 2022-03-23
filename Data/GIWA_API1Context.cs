using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using GIWA_API1.Modèles;

namespace GIWA_API1.Data
{
    public class GIWA_API1Context : DbContext
    {
        public GIWA_API1Context (DbContextOptions<GIWA_API1Context> options)
            : base(options)
        {
        }

        public DbSet<GIWA_API1.Modèles.Continent> Continent { get; set; }

        public DbSet<GIWA_API1.Modèles.Pays> Pays { get; set; }

        public DbSet<GIWA_API1.Modèles.Population> Population { get; set; }
    }
}
