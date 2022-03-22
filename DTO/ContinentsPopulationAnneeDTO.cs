using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GIWA_API1.DTO
{
    public class ContinentsPopulationAnneeDTO
    {
        public int Id_continent { get; set; }
        public string nom_continent { get; set; }
        public int annee { get; set; }
        public int population { get; set; }
    }
}
