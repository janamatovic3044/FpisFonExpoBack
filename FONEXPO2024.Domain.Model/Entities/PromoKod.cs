using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FONEXPO2024.Domain.Model.Entities
{
    public class PromoKod
    {
        [Key]
        public int PromoKodID { get; set; }

        [Required]
        public string Kod { get; set; } = null!;
        public bool IsUsed { get; set; }

        public int GenerisanOdPrijavaID { get; set; }
        public Prijava GenerisanOdPrijava { get; set; } = null!;

        public int? IskoriscenOdPrijavaID { get; set; }
        public Prijava? IskoriscenOdPrijava { get; set; }
    }
}

