using FONEXPO2024.Domain.Model.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FONEXPO2024.Interfaces.Interfaces
{
    public interface IRegistracijaService
    {
        Task<CancelPrijavaResponseDto> CancelAsync(LoginPrijava dto);
        Task<decimal> IzracunajCenu(CenaRacunanjeDTO dto);
        Task<PrijavaResponseDto?> LoginAsync(LoginPrijava dto);
        Task<RegistrationResponseDto> RegisterAsync(RegistracijaRequestDTO request);
        Task<PrijavaResponseDto> UpdateAsync(UpdatePrijavaDTO dto);
    }
}
