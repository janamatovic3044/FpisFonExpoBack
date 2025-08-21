using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FONEXPO2024.Domain.Model.Entities
{
    public class Prijava
    {
        [Key]
        public int PrijavaID { get; set; }

        [Required]
        public string Token { get; set; } = null!;
        public string StatusPrijave { get; set; } = "Aktivna";
        public DateTime DatumPrijave { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal OriginalPrice { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal FinalPrice { get; set; }

        public bool IsEarlyBird { get; set; }
        public int BrojOsoba { get; set; }

        public int KorisnikID { get; set; }
        public Korisnik Korisnik { get; set; } = null!;

        public int? PromoKodID { get; set; }
        public PromoKod? PromoKod { get; set; }

        public ICollection<PrijavaDan> Dani { get; set; } = new List<PrijavaDan>();
    }


}
