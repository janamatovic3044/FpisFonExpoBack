using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FONEXPO2024.Domain.Model.Entities
{
    public class PrijavaDan
    {
        public int PrijavaID { get; set; }
        public Prijava Prijava { get; set; } = null!;

        public int ExpoDanID { get; set; }
        public ExpoDan ExpoDan { get; set; } = null!;
    }

}
