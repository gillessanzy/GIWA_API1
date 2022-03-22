using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace GIWA_API1.Modèles
{
    public class Pays
    { 
        public int Id { get; set; }
        public string nom_pays { get; set; } = "";
        public int Id_continent { get; set; }
        [ForeignKey("Id_continent")]
        public Continent continent { get; set; }
    }
}
