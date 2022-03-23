using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GIWA_API1.DTO
{
    public class PostPaysDTO
    {
        public int Id { get; set; }
        public string nom_pays { get; set; } = "";
        public int Id_continent { get; set; }
    }
}
