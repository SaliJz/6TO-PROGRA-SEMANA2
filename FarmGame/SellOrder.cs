using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmGame
{
    /// <summary>
    /// Representa una orden de venta pendiente en el mercado.
    /// El dinero se cobra una vez que TurnsRemaining llega a 0.
    /// </summary>
    internal class SellOrder
    {
        #region Datos Base de la Orden de Venta

        public string CropName { get; set; }
        public int Quantity { get; set; }
        public int TotalValue { get; set; }
        public int TurnsRemaining { get; set; }
        public int DayCreated { get; set; }
        public string DisplayInfo =>
            $"{Quantity} x {CropName} => {TotalValue}g " +
            $"(en {TurnsRemaining} turno{(TurnsRemaining != 1 ? "s" : "")})";

        #endregion
    }
}