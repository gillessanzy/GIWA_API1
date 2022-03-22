using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GIWA_API1.Modèles
{
    public class Population
    {
        public int Id { get; set; }
        public int annee { get; set; }
        public int population { get; set; }
        public int Id_pays { get; set; }
        [ForeignKey("Id_pays")]
        public Pays pays { get; set; }
    }
}
