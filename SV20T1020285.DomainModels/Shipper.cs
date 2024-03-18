using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV20T1020285.DomainModels
{
    /// <summary>
    /// Nhà cung cấp
    /// </summary>
    public class Shipper
    {
        public int ShipperID { get; set; }
        public string ShipperName { get; set; } = "";
        public string Phone { get; set; } = "";
    }
}
