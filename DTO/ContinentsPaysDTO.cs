using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GIWA_API1.DTO
{
    public class ContinentsPaysDTO
    {
        public int Id_continent { get; set; }
        public string nom_continent { get; set; }
        public int Id_pays { get; set; }
        public string nom_pays { get; set; }

    }
}
