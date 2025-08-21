using FONEXPO2024.DataAccess;
using FONEXPO2024.Domain.Model.DTO;
using FONEXPO2024.Interfaces.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FONEXPO2024.Services.Sevices
{
    public class ManifestacijaService(FonExpoDbContext context) : IManifestacijaService
    {
        public async Task<ManifestacijaInfoDto?> GetManifestacijaInfo()
        {
            var manifestacija = await context.Manifestacije
                .Include(m => m.ExpoDani)
                    .ThenInclude(ed => ed.Izlozbe)
                .Include(m => m.ExpoDani)
                    .ThenInclude(ed => ed.CeneDana)
                .FirstOrDefaultAsync();

            var rezervacijePoDanu = await context.PrijaveDana
            .GroupBy(pd => pd.ExpoDanID)
            .Select(g => new { g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Key, x => x.Count);

            if (manifestacija == null) return null;

            var dto = new ManifestacijaInfoDto
            {
                ManifestacijaID = manifestacija.ManifestacijaID,
                Naziv = manifestacija.Naziv,
                Grad = manifestacija.Grad,
                Lokacija = manifestacija.Lokacija,
                DatumPocetka = manifestacija.DatumPocetka,
                DatumZavrsetka = manifestacija.DatumZavrsetka,
                DodatneInfo = manifestacija.DodatneInfo,
                MaxPosetilacaPoDanu = manifestacija.MaxPosetilacaPoDanu,
                ExpoDani = manifestacija.ExpoDani.Select(ed => new ExpoDanDto
                {
                    ExpoDanID = ed.ExpoDanID,
                    Datum = ed.Datum,
                    Tema = ed.Tema,
                    Izlozbe = ed.Izlozbe.Select(i => new IzlozbaDto
                    {
                        IzlozbaID = i.IzlozbaID,
                        Umetnik = i.Umetnik,
                        VremeOtvaranja = i.VremeOtvaranja,
                        VremeZatvaranja = i.VremeZatvaranja
                    }),
                    SlobodnaMesta = manifestacija.MaxPosetilacaPoDanu
                - rezervacijePoDanu.GetValueOrDefault(ed.ExpoDanID)

                })
            };

            // prebroji jednom, asinkrono
        

            // kasnije, u čistoj projekciji, samo pogledaj broj
           

            return dto;
        }
    }
}
