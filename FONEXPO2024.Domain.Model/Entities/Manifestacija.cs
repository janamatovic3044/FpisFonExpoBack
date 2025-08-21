using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FONEXPO2024.Domain.Model.Entities
{
    public class Manifestacija
    {
        [Key]
        public int ManifestacijaID { get; set; }

        [Required]
        public string Naziv { get; set; } = null!;

        [Required]
        public string Grad { get; set; } = null!;

        [Required]
        public string Lokacija { get; set; } = null!;

        public DateTime DatumPocetka { get; set; }
        public DateTime DatumZavrsetka { get; set; }

        public string? DodatneInfo { get; set; }

        public int MaxPosetilacaPoDanu { get; set; }

        public ICollection<ExpoDan> ExpoDani { get; set; } = new List<ExpoDan>();
    }
}
