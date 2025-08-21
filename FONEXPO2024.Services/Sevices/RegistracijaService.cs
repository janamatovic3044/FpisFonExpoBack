using FONEXPO2024.DataAccess;
using FONEXPO2024.Domain.Model.DTO;
using FONEXPO2024.Domain.Model.Entities;
using FONEXPO2024.Domain.Model.Helper;
using FONEXPO2024.Interfaces.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FONEXPO2024.Services.Sevices
{
    public class RegistracijaService(FonExpoDbContext context) : IRegistracijaService
    {
        private readonly DateTime _earlyBirdDeadline = new DateTime(2025, 9, 1);



        public async Task<decimal> IzracunajCenu(CenaRacunanjeDTO dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));
            if (dto.BrojOsoba <= 0) throw new ArgumentException("Broj osoba mora biti veći od nule.");
            if (dto.ExpoDanIDs == null || dto.ExpoDanIDs.Count == 0)
                throw new ArgumentException("Mora biti izabran bar jedan dan.");


            var dayPrices = await context.CeneDana
                .AsNoTracking()
                .Where(c => dto.ExpoDanIDs.Contains(c.ExpoDanID))
                .Select(c => c.Cena)
                .ToListAsync();

            if (dayPrices.Count != dto.ExpoDanIDs.Count)
                throw new InvalidOperationException("Neki odabran dan ne postoji.");

            var asOf = DateTime.Now; // <- umesto UtcNow
            var earlyBird = asOf <= _earlyBirdDeadline;
            var haspromo = false;

            if (dto.Token.Length > 0)
            {

                Prijava? prijava = await context.Prijave
                    .Include(p => p.Korisnik)
                    .FirstOrDefaultAsync(p => p.Token == dto.Token);


                if (prijava != null)
                {
                    earlyBird = prijava.IsEarlyBird;
                    haspromo = prijava.PromoKodID != null;
                }

            }


            decimal perPerson = dayPrices
                .Select(p => earlyBird ? p * 0.90m : p)
                .Sum();

            // strože: baš oba dana
            if (dayPrices.Count == 2)
                perPerson *= 0.90m;

            decimal total = perPerson * dto.BrojOsoba;

            if (dto.BrojOsoba >= 5) total *= 0.95m;
            else if (dto.BrojOsoba >= 3) total *= 0.97m;

            if (haspromo)
            {
                total *= 0.95m;
            }


            
         
            

            return Math.Round(total, 2, MidpointRounding.AwayFromZero);
        }



        public async Task<CancelPrijavaResponseDto> CancelAsync(LoginPrijava dto)
        {
            var cancelDto = new CancelPrijavaResponseDto();

            var errorDto = new ErrorDto();

            // 1) Osnovna validacija
            if (string.IsNullOrWhiteSpace(dto.Token) || string.IsNullOrWhiteSpace(dto.Email))
            {
                errorDto.Details = "Token i email su obavezni.";
                cancelDto.Error = errorDto;
                return cancelDto;
            }

            // 2) Nađi prijavu po tokenu i emailu korisnika, ali samo ako već nije otkazana
            var prijava = await context.Prijave
                .Include(p => p.Korisnik)
                .Include(p => p.PromoKod) // kod koji je korisnik uneo
                .Include(p => p.Dani)     // da bismo eventualno obrištili dane
                .ThenInclude(pd => pd.ExpoDan)
                .FirstOrDefaultAsync(p =>
                    p.Token == dto.Token
                    && p.Korisnik.Email == dto.Email
                    && p.StatusPrijave != StatusiPrijave.Otkazana.ToString());

            if (prijava == null)
            {
                errorDto.Details = "Prijava nije pronađena ili je već otkazana.";
                cancelDto.Error = errorDto;
                return cancelDto;
            }
                

            // 3) Otvori transakciju
            await using var tx = await context.Database.BeginTransactionAsync();
            try
            {
                // 4) Obeleži prijavu kao otkazanu i onemogući token
                prijava.StatusPrijave = StatusiPrijave.Otkazana.ToString();
                
                

                // 5) Onemogući promo kod koji je generisan ovom prijavom
                var generatedPromo = await context.PromoKodovi
                    .FirstOrDefaultAsync(pk => pk.GenerisanOdPrijavaID == prijava.PrijavaID);
                if (generatedPromo != null && !generatedPromo.IsUsed)
                {
                    generatedPromo.IsUsed = true;
                }

                // 6) Snimi i commit
                await context.SaveChangesAsync();
                await tx.CommitAsync();

                // 7) Popuni odgovor
                cancelDto.IsCancelled = true;

                //cancelDto = "Prijava uspešno otkazana.";

                cancelDto.Error = errorDto;
                return cancelDto;





            }
            catch
            {

                // rollback se desi automatski pri Dispose ako ne Commit-uješ
                await tx.RollbackAsync();
                errorDto.Details = "Neočekivana greška prilikom otkazivanja prijave.";
                cancelDto.Error = errorDto;
                return cancelDto;


            }
        }

       

        public async Task<PrijavaResponseDto?> LoginAsync(LoginPrijava dto)
        {
            Korisnik? korisnik = await context.Korisnici.Include(x => x.Prijave).ThenInclude(p => p.Dani)
                .FirstOrDefaultAsync(k => k.Email == dto.Email);

            
            


            if (korisnik is not null)
            {
                Prijava? prijava = korisnik.Prijave
                    .FirstOrDefault(p => p.Token == dto.Token);

                if (prijava is null)
                {
                    return new PrijavaResponseDto
                    {
                        Error = new ErrorDto
                        {
                            Details = "Prijava nije pronađena."
                        }
                    };
                }
                else
                {
                    return new PrijavaResponseDto
                    {
                        Token = prijava.Token,
                        StatusPrijave = prijava.StatusPrijave,
                        DatumPrijave = prijava.DatumPrijave,
                        OriginalPrice = prijava.OriginalPrice,
                        FinalPrice = prijava.FinalPrice,
                        IsEarlyBird = prijava.IsEarlyBird,
                        BrojOsoba = prijava.BrojOsoba,
                        KorisnikID = prijava.KorisnikID,
                        Ime = korisnik.Ime,
                        Prezime = korisnik.Prezime,
                        // ako hoćeš da označiš i otkazane
                        IsCancelled = prijava.StatusPrijave == StatusiPrijave.Otkazana.ToString(),
                        Error = null,
                        ExpoDanIDs = prijava.Dani.Select(pd => pd.ExpoDanID).ToList(),

                    };

                }
            }
            else
            {
                return new PrijavaResponseDto
                {
                    Error = new ErrorDto
                    {
                        Details = "Korisnik sa datim email-om nije pronađen."
                    }
                };
            }

        }



        public async Task<RegistrationResponseDto> RegisterAsync(RegistracijaRequestDTO request)
        {
            var response = new RegistrationResponseDto { Error = new ErrorDto() };

            // --- Osnovne validacije ---
            if (request.BrojOsoba < 1)
                response.Error.Details += "Broj osoba mora biti veći od 0.\n";
            bool isMailTaken = false;
            // (opciono) jedinstven email - zavisi od tvojih poslovnih pravila
            Korisnik? k = await context.Korisnici.FirstOrDefaultAsync(x=>x.Email == request.Email);
            if (k == null) {
                isMailTaken = false;
            
            }
            else
            {
                List<Prijava> lista = await context.Prijave.Where(x => x.KorisnikID == k.KorisnikID).ToListAsync();

                if (lista.Any(x => x.StatusPrijave == "Aktivna"))
                {
                    isMailTaken = true;
                }

            }

            if (isMailTaken)
                response.Error.Details += "Email adresa je već registrovana.\n";

            // Učitaj izabrane dane + manifestacije
            var expoDays = await context.ExpoDani
                .Include(ed => ed.Manifestacija)
                .Where(ed => request.ExpoDanIDs.Contains(ed.ExpoDanID))
                .ToListAsync();

            if (expoDays.Count != request.ExpoDanIDs.Count)
                response.Error.Details += "Neki izabrani dan ne postoji.\n";

            // (5) Povuci cene po danu i uveri se da svaki dan ima definisanu cenu
            var pricesDict = await context.CeneDana
                
                .Where(c => request.ExpoDanIDs.Contains(c.ExpoDanID))
                .GroupBy(c => c.ExpoDanID)
                .Select(g => new { ExpoDanID = g.Key, Cena = g.Select(x => x.Cena).FirstOrDefault() })
                .ToDictionaryAsync(x => x.ExpoDanID, x => x.Cena);

            if (pricesDict.Count != request.ExpoDanIDs.Count)
                response.Error.Details += "Za neki izabrani dan nije definisana cena.\n";

            // (1) Provera kapaciteta po OSOBAMA (pre-transakcijski)
            var occupiedByPersons = await context.PrijaveDana
                
                .Where(pd => request.ExpoDanIDs.Contains(pd.ExpoDanID)
                          && pd.Prijava.StatusPrijave != StatusiPrijave.Otkazana.ToString())
                .GroupBy(pd => pd.ExpoDanID)
                .Select(g => new { ExpoDanID = g.Key, Osobe = g.Sum(pd => pd.Prijava.BrojOsoba) })
                .ToDictionaryAsync(x => x.ExpoDanID, x => x.Osobe);

            foreach (var day in expoDays)
            {
                occupiedByPersons.TryGetValue(day.ExpoDanID, out var occupied);
                var available = day.Manifestacija.MaxPosetilacaPoDanu - occupied;
                if (request.BrojOsoba > available)
                    response.Error.Details += $"Za dan {day.Datum:dd.MM.yyyy} nema dovoljno slobodnih mesta.\n";
            }

            // Promo kod validacija (samo validacija ovde; setovanje IsUsed tek u transakciji)
            PromoKod? promo = null;
            if (!string.IsNullOrWhiteSpace(request.PromoKod))
            {
                promo = await context.PromoKodovi.FirstOrDefaultAsync(pk => pk.Kod == request.PromoKod);
                if (promo == null)
                    response.Error.Details += "Nevažeći promo kod.\n";
                else if (promo.IsUsed)
                    response.Error.Details += "Promo kod je već iskorišćen.\n";
            }

            // Ako ima grešaka do sada, prekini
            if (!string.IsNullOrEmpty(response.Error.Details))
                return response;

            // --- Obračun cena (po pravilima 2.2, 2.3, 2.6 i opcioni promo 2.5) ---
            bool earlyBird = DateTime.Now <= _earlyBirdDeadline;

            decimal perPerson = request.ExpoDanIDs
                .Select(id => pricesDict[id])
                .Select(p => earlyBird ? p * 0.90m : p)
                .Sum();

            // strože: baš oba dana
            if (request.ExpoDanIDs.Count == 2)
                perPerson *= 0.90m;

            // pre promo: umnoži sa brojem osoba, pa grupni popust
            decimal originalPrice = perPerson * request.BrojOsoba;

            if (request.BrojOsoba >= 5) originalPrice *= 0.95m;   // -5%
            else if (request.BrojOsoba >= 3) originalPrice *= 0.97m; // -3%

            decimal finalPrice = originalPrice;

            // promo -5% ako validan
            if (promo != null && !promo.IsUsed)
                finalPrice *= 0.95m;

            // Zaokruživanje NA KRAJU obračuna
            originalPrice = Math.Round(originalPrice, 2, MidpointRounding.AwayFromZero);
            finalPrice = Math.Round(finalPrice, 2, MidpointRounding.AwayFromZero);

           

            // --- Transakcija ---
            await using var tx = await context.Database.BeginTransactionAsync();
            try
            {
                // (2) PONOVNA provera kapaciteta po OSOBAMA (unutar transakcije)
                var occupiedByPersons2 = await context.PrijaveDana
                    .Where(pd => request.ExpoDanIDs.Contains(pd.ExpoDanID)
                              && pd.Prijava.StatusPrijave != StatusiPrijave.Otkazana.ToString())
                    .GroupBy(pd => pd.ExpoDanID)
                    .Select(g => new { ExpoDanID = g.Key, Osobe = g.Sum(pd => pd.Prijava.BrojOsoba) })
                    .ToDictionaryAsync(x => x.ExpoDanID, x => x.Osobe);

                foreach (var day in expoDays)
                {
                    occupiedByPersons2.TryGetValue(day.ExpoDanID, out var occ);
                    var available = day.Manifestacija.MaxPosetilacaPoDanu - occ;
                    if (request.BrojOsoba > available)
                    {
                        await tx.RollbackAsync();
                        response.Error.Details = $"Za dan {day.Datum:dd.MM.yyyy} nema dovoljno slobodnih mesta.\n";
                        return response;
                    }
                }

                // Kreiraj korisnika
                var korisnik = new Korisnik
                {
                    Ime = request.Ime,
                    Prezime = request.Prezime,
                    Profesija = request.Profesija,
                    Adresa1 = request.Adresa1,
                    Adresa2 = request.Adresa2,
                    PostanskiBroj = request.PostanskiBroj,
                    Mesto = request.Mesto,
                    Drzava = request.Drzava,
                    Email = request.Email,
                    EmailPotvrdjen = request.EmailPotvrdjen
                };
                context.Korisnici.Add(korisnik);

                // Kreiraj prijavu
                var prijava = new Prijava
                {
                    Token = Guid.NewGuid().ToString("N"),
                    DatumPrijave = DateTime.Now,   // <- umesto UtcNow
                    OriginalPrice = originalPrice,  // <- DOSLEDNO: posle EB/oba/gruppe, pre promo
                    FinalPrice = finalPrice,
                    IsEarlyBird = earlyBird,
                    BrojOsoba = request.BrojOsoba,
                    Korisnik = korisnik,
                    StatusPrijave = StatusiPrijave.Aktivna.ToString()
                };

                if (promo != null && !promo.IsUsed)
                {
                    prijava.PromoKod = promo;
                    promo.IsUsed = true;
                    promo.IskoriscenOdPrijava = prijava;
                }

                context.Prijave.Add(prijava);

                // Linkuj dane
                foreach (var day in expoDays)
                {
                    context.PrijaveDana.Add(new PrijavaDan
                    {
                        Prijava = prijava,
                        ExpoDan = day
                    });
                }

                // Generiši novi promo kod za prijatelje
                var newPromo = new PromoKod
                {
                    Kod = Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper(),
                    GenerisanOdPrijava = prijava,
                    IsUsed = false
                };
                context.PromoKodovi.Add(newPromo);

                // Snimi i commit
                await context.SaveChangesAsync();
                await tx.CommitAsync();

                // Popuni odgovor

                response.Token = prijava.Token;
                response.OriginalPrice = originalPrice; // <- isti iznos kao u bazi
                response.FinalPrice = finalPrice;
                response.GeneratedPromoKod = newPromo.Kod;
                return response;
            }
            catch
            {
                await tx.RollbackAsync();
                return new RegistrationResponseDto
                {
                    Error = new ErrorDto { Details = "Neočekivana greška prilikom obrade zahteva." }
                };
            }
        }

        public async Task<PrijavaResponseDto> UpdateAsync(UpdatePrijavaDTO dto)
        {
            var resp = new PrijavaResponseDto { Error = new ErrorDto() };

            // --- Osnovne validacije ---
            if (dto.BrojOsoba < 1)
                resp.Error!.Details += "Broj osoba mora biti veći od 0.\n";
            if (dto.ExpoDanIds == null || dto.ExpoDanIds.Count == 0)
                resp.Error!.Details += "Mora biti izabran bar jedan dan.\n";
            if (!string.IsNullOrEmpty(resp.Error!.Details))
                return resp;

            // Učitaj prijavu (+ korisnika, promo, dane, manifestacije)
            var prijava = await context.Prijave
                .Include(p => p.Korisnik)
                .Include(p => p.PromoKod)
                .Include(p => p.Dani).ThenInclude(pd => pd.ExpoDan).ThenInclude(ed => ed.Manifestacija)
                .FirstOrDefaultAsync(p => p.Token == dto.Token && p.Korisnik.Email == dto.Email);

            if (prijava == null)
            {
                resp.Error!.Details = "Prijava nije pronađena.\n";
                return resp;
            }
            if (prijava.StatusPrijave == StatusiPrijave.Otkazana.ToString())
            {
                resp.Error!.Details = "Prijava je otkazana i ne može se menjati.\n";
                return resp;
            }

            // Trenutni i novi dani
            var currentDayIds = prijava.Dani.Select(d => d.ExpoDanID).ToList();
            var newDayIds = dto.ExpoDanIds.Distinct().ToList();
            var toAdd = newDayIds.Except(currentDayIds).ToList();
            var toRemove = currentDayIds.Except(newDayIds).ToList();

            // Učitaj finalni skup dana (radi kapaciteta i cena)
            var finalDayIds = newDayIds;
            var finalDays = await context.ExpoDani
                .Include(ed => ed.Manifestacija)
                .Where(ed => finalDayIds.Contains(ed.ExpoDanID))
                .ToListAsync();

            if (finalDays.Count != finalDayIds.Count)
            {
                resp.Error!.Details = "Neki izabrani dan ne postoji.\n";
                return resp;
            }

            // Cene po danu (mora postojati za svaki finalni dan)
            var pricesDict = await context.CeneDana
                .Where(c => finalDayIds.Contains(c.ExpoDanID))
                .GroupBy(c => c.ExpoDanID)
                .Select(g => new { ExpoDanID = g.Key, Cena = g.Select(x => x.Cena).FirstOrDefault() })
                .ToDictionaryAsync(x => x.ExpoDanID, x => x.Cena);

            if (pricesDict.Count != finalDayIds.Count)
            {
                resp.Error!.Details = "Za neki izabrani dan nije definisana cena.\n";
                return resp;
            }

            // --- (1) PRE-TRANSAKCIJE: kapacitet po OSOBAMA (za finalni set dana) ---
            // Računamo zauzeće drugih prijava, isključujući ovu prijavu
            var occupiedByPersons = await context.PrijaveDana
                .Where(pd => finalDayIds.Contains(pd.ExpoDanID)
                             && pd.Prijava.StatusPrijave != StatusiPrijave.Otkazana.ToString()
                             && pd.PrijavaID != prijava.PrijavaID)
                .GroupBy(pd => pd.ExpoDanID)
                .Select(g => new { ExpoDanID = g.Key, Osobe = g.Sum(pd => pd.Prijava.BrojOsoba) })
                .ToDictionaryAsync(x => x.ExpoDanID, x => x.Osobe);

            foreach (var day in finalDays)
            {
                occupiedByPersons.TryGetValue(day.ExpoDanID, out var occ);
                var available = day.Manifestacija.MaxPosetilacaPoDanu - occ;
                if (dto.BrojOsoba > available)
                    resp.Error!.Details += $"Za dan {day.Datum:dd.MM.yyyy} nema dovoljno slobodnih mesta.\n";
            }

            if (!string.IsNullOrEmpty(resp.Error!.Details))
                return resp;

            // --- TRANSakcija ---
            await using var tx = await context.Database.BeginTransactionAsync();
            try
            {
                // --- (2) PONOVNA kapacitet provera unutar transakcije ---
                var occupiedByPersons2 = await context.PrijaveDana
                    .Where(pd => finalDayIds.Contains(pd.ExpoDanID)
                                 && pd.Prijava.StatusPrijave != StatusiPrijave.Otkazana.ToString()
                                 && pd.PrijavaID != prijava.PrijavaID)
                    .GroupBy(pd => pd.ExpoDanID)
                    .Select(g => new { ExpoDanID = g.Key, Osobe = g.Sum(pd => pd.Prijava.BrojOsoba) })
                    .ToDictionaryAsync(x => x.ExpoDanID, x => x.Osobe);

                foreach (var day in finalDays)
                {
                    occupiedByPersons2.TryGetValue(day.ExpoDanID, out var occ);
                    var available = day.Manifestacija.MaxPosetilacaPoDanu - occ;
                    if (dto.BrojOsoba > available)
                    {
                        await tx.RollbackAsync();
                        resp.Error!.Details = $"Za dan {day.Datum:dd.MM.yyyy} nema dovoljno slobodnih mesta.\n";
                        return resp;
                    }
                }

                // --- Izmene linkova dana ---
                if (toRemove.Count > 0)
                {
                    var linksToRemove = await context.PrijaveDana
                        .Where(pd => pd.PrijavaID == prijava.PrijavaID && toRemove.Contains(pd.ExpoDanID))
                        .ToListAsync();
                    context.PrijaveDana.RemoveRange(linksToRemove);
                }

                if (toAdd.Count > 0)
                {
                    var daysToAdd = finalDays.Where(d => toAdd.Contains(d.ExpoDanID)).ToList();
                    foreach (var ed in daysToAdd)
                    {
                        context.PrijaveDana.Add(new PrijavaDan
                        {
                            PrijavaID = prijava.PrijavaID,
                            ExpoDanID = ed.ExpoDanID,
                            Prijava = prijava,
                            ExpoDan = ed
                        });
                    }
                }

                // --- Broj osoba ---
                prijava.BrojOsoba = dto.BrojOsoba;

                // --- Reobračun cene (2.2, 2.3, 2.6 + promo ako postoji) ---
                bool earlyBird = prijava.IsEarlyBird || prijava.DatumPrijave <= _earlyBirdDeadline;

                decimal perPerson = finalDayIds
                    .Select(id => pricesDict[id])
                    .Select(p => earlyBird ? p * 0.90m : p)
                    .Sum();

                if (finalDayIds.Count == 2)
                    perPerson *= 0.90m;

                decimal originalPrice = perPerson * prijava.BrojOsoba;

                if (prijava.BrojOsoba >= 5) originalPrice *= 0.95m;   // -5%
                else if (prijava.BrojOsoba >= 3) originalPrice *= 0.97m; // -3%

                decimal finalPrice = originalPrice;

                // Ako je na ovoj prijavi ranije bio promo kod, zadržavamo efekat -5%
                if (prijava.PromoKod != null)
                    finalPrice *= 0.95m;

                prijava.OriginalPrice = Math.Round(originalPrice, 2, MidpointRounding.AwayFromZero);
                prijava.FinalPrice = Math.Round(finalPrice, 2, MidpointRounding.AwayFromZero);
                prijava.IsEarlyBird = earlyBird;

                await context.SaveChangesAsync();
                await tx.CommitAsync();

                // Sastavi odgovor
                var updatedDayIds = await context.PrijaveDana
                    .Where(pd => pd.PrijavaID == prijava.PrijavaID)
                    .Select(pd => pd.ExpoDanID)
                    .ToListAsync();

                return new PrijavaResponseDto
                {
                    Token = prijava.Token,
                    StatusPrijave = prijava.StatusPrijave,
                    DatumPrijave = prijava.DatumPrijave,
                    OriginalPrice = prijava.OriginalPrice,
                    FinalPrice = prijava.FinalPrice,
                    IsEarlyBird = prijava.IsEarlyBird,
                    BrojOsoba = prijava.BrojOsoba,
                    KorisnikID = prijava.KorisnikID,
                    Ime = prijava.Korisnik.Ime,
                    Prezime = prijava.Korisnik.Prezime,
                    IsCancelled = prijava.StatusPrijave == StatusiPrijave.Otkazana.ToString(),
                    ExpoDanIDs = updatedDayIds,
                    Error = null
                };
            }
            catch
            {
                await tx.RollbackAsync();
                resp.Error!.Details = "Neočekivana greška prilikom ažuriranja prijave.";
                return resp;
            }
        }

    }
}