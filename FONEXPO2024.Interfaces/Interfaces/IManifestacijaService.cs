using FONEXPO2024.Domain.Model.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FONEXPO2024.Interfaces.Interfaces
{
    public interface IManifestacijaService
    {
        Task<ManifestacijaInfoDto> GetManifestacijaInfo();
    }
}
