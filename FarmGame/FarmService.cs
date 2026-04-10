using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmGame
{
    /// <summary>
    /// Gestiona las acciones sobre la granja (plantar y cosechar).
    /// </summary>
    internal class FarmService
    {
        #region Plantar y Cosechar

        public (bool Success, string Message) PlantSeed(Player player, int plotId, string seedName)
        {
            var plot = player.Farm.FirstOrDefault(p => p.Id == plotId);
            if (plot == null) return (false, $"La parcela {plotId} no existe.");
            if (!plot.IsEmpty)
            {
                return (false, $"La parcela {plotId} ya tiene un cultivo ({plot.Seed?.Name ?? "?"}).");
            }

            if (!player.SeedInventory.ContainsKey(seedName) || player.SeedInventory[seedName] == 0)
            {
                return (false, $"No tienes semillas de {seedName} en tu inventario.");
            }
            
            var seed = SeedCatalog.GetByName(seedName);
            if (seed == null) return (false, $"Error interno: definicion de '{seedName}' no encontrada.");

            player.RemoveSeeds(seedName, 1);
            plot.Seed = seed;
            plot.State = PlotState.Growing;
            plot.DaysPlanted = 0;

            return (true,
                $"Plantaste {seed.Name} en parcela #{plotId}. " +
                $"Lista para cosechar en {seed.GrowthDays} dia(s).");
        }

        public (bool Success, string Message) HarvestPlot(Player player, int plotId)
        {
            var plot = player.Farm.FirstOrDefault(p => p.Id == plotId);
            if (plot == null) return (false, $"La parcela {plotId} no existe.");

            if (plot.State == PlotState.Empty)
            {
                return (false, $"La parcela {plotId} esta vacia, no hay nada que cosechar.");
            }

            if (plot.State == PlotState.Growing)
            {
                return (false,
                    $"La parcela {plotId} todavia esta creciendo. " +
                    $"Faltan {plot.DaysRemaining} dia(s).");
            }
            
            string cropName = plot.Seed.Name;
            player.AddHarvest(cropName, 1);
            
            plot.State = PlotState.Empty;
            plot.Seed = null;
            plot.DaysPlanted = 0;

            return (true, $"Cosechaste 1x {cropName} de la parcela #{plotId}.");
        }

        public void AdvanceGrowth(Player player)
        {
            foreach (var plot in player.Farm.Where(p => p.State == PlotState.Growing))
            {
                plot.DaysPlanted++;
                if (plot.DaysPlanted >= plot.Seed.GrowthDays)
                {
                    plot.State = PlotState.ReadyToHarvest;
                }
            }
        }

        #endregion
    }
}