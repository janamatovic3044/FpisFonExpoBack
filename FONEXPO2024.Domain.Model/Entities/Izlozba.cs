using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FONEXPO2024.Domain.Model.Entities
{
    public class Izlozba
    {
        [Key]
        public int IzlozbaID { get; set; }

        public int ExpoDanID { get; set; }
        public ExpoDan ExpoDan { get; set; } = null!;

        public TimeSpan VremeOtvaranja { get; set; }
        public TimeSpan VremeZatvaranja { get; set; }

        [Required]
        public string Umetnik { get; set; } = null!;
    }
}
