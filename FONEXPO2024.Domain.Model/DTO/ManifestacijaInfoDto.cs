using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FONEXPO2024.Domain.Model.DTO
{

    public class ManifestacijaInfoDto
    {
        public int ManifestacijaID { get; set; }
        public string Naziv { get; set; } = null!;
        public string Grad { get; set; } = null!;
        public string Lokacija { get; set; } = null!;
        public DateTime DatumPocetka { get; set; }
        public DateTime DatumZavrsetka { get; set; }
        public string? DodatneInfo { get; set; }
        public int MaxPosetilacaPoDanu { get; set; }
        public IEnumerable<ExpoDanDto> ExpoDani { get; set; } = Array.Empty<ExpoDanDto>();
    }
    public class IzlozbaDto
    {
        public int IzlozbaID { get; set; }
        public string Umetnik { get; set; } = null!;
        public TimeSpan VremeOtvaranja { get; set; }
        public TimeSpan VremeZatvaranja { get; set; }
    }

    public class ExpoDanDto
    {
        public int ExpoDanID { get; set; }
        public DateTime Datum { get; set; }
        public string Tema { get; set; } = null!;
        public IEnumerable<IzlozbaDto> Izlozbe { get; set; } = Array.Empty<IzlozbaDto>();
        public int SlobodnaMesta { get; set; }
    }

   
}
