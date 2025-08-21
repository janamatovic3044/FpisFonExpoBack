using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FONEXPO2024.Domain.Model.Entities
{
    public class ExpoDan
    {
        [Key]
        public int ExpoDanID { get; set; }

        public int ManifestacijaID { get; set; }
        public Manifestacija Manifestacija { get; set; } = null!;

        public DateTime Datum { get; set; }

        [Required]
        public string Tema { get; set; } = null!;

        public ICollection<Izlozba> Izlozbe { get; set; } = new List<Izlozba>();
        public ICollection<CenaDana> CeneDana { get; set; } = new List<CenaDana>();
        public ICollection<PrijavaDan> Prijave { get; set; } = new List<PrijavaDan>();
    }
}
