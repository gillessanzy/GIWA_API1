using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GIWA_API1.DTO
{
    public class PopulationsDTO
    {
        public int Id { get; set; }
        public int annee { get; set; }
        public int population { get; set; }
        public int Id_pays { get; set; }
        public string Nom_pays { get; set; }
    }
}
