using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmGame
{
    /// <summary>
    /// Define las propiedades de un tipo de semilla disponible en la tienda.
    /// </summary>
    internal class SeedDefinition
    {
        #region Datos Base de la Semilla

        public string Name { get; set; }
        public string Icon { get; set; }
        public int BuyCost { get; set; }
        public int GrowthDays { get; set; }
        public int SellPrice { get; set; }
        public int SellDelayDays { get; set; }
        public string Description { get; set; }
        public int NetProfit => SellPrice - BuyCost;

        #endregion
    }
}