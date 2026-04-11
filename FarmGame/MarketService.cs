using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmGame
{
    /// <summary>
    /// Gestiona las ventas en el mercado.
    /// </summary>
    internal class MarketService
    {
        #region Venta de cosechas

        public (bool Success, string Message) SellCrops(Player player, string cropName, int quantity)
        {
            if (quantity <= 0) return (false, "La cantidad debe ser mayor a 0.");

            if (!player.HarvestInventory.ContainsKey(cropName) ||
                player.HarvestInventory[cropName] < quantity)
            {
                return (false,
                    $"No tienes suficientes unidades de {cropName}. " +
                    $"Disponibles: {(player.HarvestInventory.ContainsKey(cropName) ? player.HarvestInventory[cropName] : 0)}.");
            }
            
            var seed = SeedCatalog.GetByName(cropName);
            if (seed == null)
            {
                return (false, $"Error interno: definicion de '{cropName}' no encontrada.");
            }

            player.RemoveHarvest(cropName, quantity);

            int totalValue = seed.SellPrice * quantity;
            var order = new SellOrder
            {
                CropName = cropName,
                Quantity = quantity,
                TotalValue = totalValue,
                TurnsRemaining = seed.SellDelayDays,
                DayCreated = player.Day
            };
            player.PendingSales.Add(order);

            return (true,
                $"Enviaste {quantity}x {cropName} al mercado. " +
                $"Recibiras {totalValue}g en {seed.SellDelayDays} turno(s).");
        }

        public List<string> ProcessPendingSales(Player player)
        {
            var messages = new List<string>();
            var completed = new List<SellOrder>();

            foreach (var order in player.PendingSales)
            {
                order.TurnsRemaining--;

                if (order.TurnsRemaining <= 0)
                {
                    player.Gold += order.TotalValue;
                    messages.Add(
                        $"  Cobro recibido: {order.Quantity}x {order.CropName} " +
                        $"-> +{order.TotalValue}g  (Oro ahora: {player.Gold}g)");
                    completed.Add(order);
                }
            }

            foreach (var done in completed) player.PendingSales.Remove(done);

            return messages;
        }

        #endregion
    }
}