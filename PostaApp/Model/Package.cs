using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostaApp.Model
{
    public enum PackageStatus { Feldolgozás_alatt, Kiszállítás_alatt, Kiszállítva, Törölve }

    public class Package
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = "";
        public DateTime ShipDate { get; set; } = DateTime.Now;
        public string OriginCity { get; set; } = "";
        public string DestinationCity { get; set; } = "";
        public PackageStatus Status { get; set; } = PackageStatus.Feldolgozás_alatt;
        public double Price { get; set; }
        public int DaysUntilDelivery { get; set; }
    }
}
