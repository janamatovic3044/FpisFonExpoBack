using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FONEXPO2024.Domain.Model.Entities
{
    public class Korisnik
    {
        [Key]
        public int KorisnikID { get; set; }

        [Required]
        public string Ime { get; set; } = null!;
        [Required]
        public string Prezime { get; set; } = null!;
        public string? Profesija { get; set; }

        [Required]
        public string Adresa1 { get; set; } = null!;
        public string? Adresa2 { get; set; }

        [Required]
        public string PostanskiBroj { get; set; } = null!;
        [Required]
        public string Mesto { get; set; } = null!;
        [Required]
        public string Drzava { get; set; } = null!;

        [Required, EmailAddress]
        public string Email { get; set; } = null!;
        public bool EmailPotvrdjen { get; set; }

        public ICollection<Prijava> Prijave { get; set; } = new List<Prijava>();
    }
}
