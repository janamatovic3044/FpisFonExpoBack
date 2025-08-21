using FONEXPO2024.Domain.Model.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FONEXPO2024.Domain.Model.DTO
{
    public class RegistracijaRequestDTO
    {

        [Required] public string Ime { get; set; } = null!;
        [Required] public string Prezime { get; set; } = null!;
        public string? Profesija { get; set; }
        [Required] public string Adresa1 { get; set; } = null!;
        public string? Adresa2 { get; set; }
        [Required] public string PostanskiBroj { get; set; } = null!;
        [Required] public string Mesto { get; set; } = null!;
        [Required] public string Drzava { get; set; } = null!;
        [Required, EmailAddress] public string Email { get; set; } = null!;
        public bool EmailPotvrdjen { get; set; }
        [Required] public List<int> ExpoDanIDs { get; set; } = new();  // IDs of selected days
        [Required] public int BrojOsoba { get; set; }
        public string? PromoKod { get; set; }  // optional friend code
    }


    public class LoginPrijava
    {
        [Required] public string Token { get; set; } = null!;
        [Required, EmailAddress] public string Email { get; set; } = null!;

    }

    public class CancelPrijavaResponseDto
    {
        public bool IsCancelled { get; set; } = false; // Indicates if the cancellation was successful
        public ErrorDto? Error { get; set; } = null; // Optional error details

    }


    public class PrijavaResponseDto
    {
        public string Token { get; set; } = null!;
        public string StatusPrijave { get; set; } = "Aktivna";
        public DateTime DatumPrijave { get; set; }

        public decimal OriginalPrice { get; set; }

        public decimal FinalPrice { get; set; }

        public bool IsEarlyBird { get; set; }
        public int BrojOsoba { get; set; }

        public int KorisnikID { get; set; }
       

        public string Ime { get; set; } = null!;

        public string Prezime { get; set; } = null!;

        public ErrorDto? Error { get; set; } = null;

        public bool IsCancelled { get; set; } = false; // Indicates if the cancellation was successful

        public List<int> ExpoDanIDs { get; set; } = new();

    }




    public class RegistrationResponseDto
    {
        public string? Token { get; set; }
        public decimal OriginalPrice { get; set; }
        public decimal FinalPrice { get; set; }
        public string? GeneratedPromoKod { get; set; } = "";

        public ErrorDto? Error { get; set; } = null; // Optional error details
    }

    public class ErrorDto
    {

        public string? Details { get; set; }
    }


    public class CenaRacunanjeDTO
    {
        public string Token { get; set; }

        public int BrojOsoba { get; set; }

        // ID-ovi dana koje je korisnik izabrao (npr. [1] = Slikarstvo, [2] = Fotografija, ili [1,2] oba)
        public List<int> ExpoDanIDs { get; set; } = new();

        // Opcioni promo kod (ako želiš da ga uračunaš i u pre-calculations)
        public string? PromoKod { get; set; }
    }


    public class UpdatePrijavaDTO
    {
        [Required] public string Token { get; set; } = null!;
        [Required, EmailAddress] public string Email { get; set; } = null!;

        public int BrojOsoba { get; set; }

        public List<int> ExpoDanIds { get; set; } = new(); // IDs of selected days

    }

}
