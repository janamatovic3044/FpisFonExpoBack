using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FONEXPO2024.Domain.Model.Entities
{
    public class CenaDana
    {
        [Key]
        public int CenaDanaID { get; set; }

        public int ExpoDanID { get; set; }
        public ExpoDan ExpoDan { get; set; } = null!;

        [Column(TypeName = "decimal(10,2)")]
        public decimal Cena { get; set; }
    }
}
