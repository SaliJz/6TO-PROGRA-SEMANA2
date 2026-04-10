using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmGame
{
    /// <summary>
    /// Gestiona las compras en la tienda (semillas y terreno).
    /// </summary>
    internal class ShopService
    {
        #region Datos Base de la Tienda

        public const int ExpansionCost = 50;
        public const int ExpansionPlots = 4;

        #endregion

        #region Compra de semillas

        public (bool Success, string Message) BuySeeds(Player player, string seedName, int quantity)
        {
            if (quantity <= 0) return (false, "La cantidad debe ser mayor a 0.");

            var seed = SeedCatalog.GetByName(seedName);
            if (seed == null) return (false, $"Semilla '{seedName}' no encontrada en el catalogo.");

            int totalCost = seed.BuyCost * quantity;
            if (player.Gold < totalCost)
            {
                return (false,
                    $"Oro insuficiente. Necesitas {totalCost}g, " +
                    $"tienes {player.Gold}g.");
            }

            player.Gold -= totalCost;
            player.AddSeeds(seedName, quantity);

            return (true,
                $"Compraste {quantity} x {seed.Name} por {totalCost}g. " +
                $"Oro restante: {player.Gold}g.");
        }

        #endregion

        #region Expansión de granja

        public (bool Success, string Message) ExpandFarm(Player player)
        {
            if (player.Gold < ExpansionCost)
            { 
                return (false,
                    $"Oro insuficiente. Necesitas {ExpansionCost}g, " +
                    $"tienes {player.Gold}g.");
            }

            player.Gold -= ExpansionCost;
            int nextId = player.Farm.Count + 1;

            for (int i = 0; i < ExpansionPlots; i++)
            {
                player.Farm.Add(new FarmPlot { Id = nextId + i });
            }

            return (true,
                $"Granja expandida: +{ExpansionPlots} parcelas por {ExpansionCost}g. " +
                $"Total de parcelas: {player.TotalPlots}. Oro restante: {player.Gold}g.");
        }

        #endregion
    }
}