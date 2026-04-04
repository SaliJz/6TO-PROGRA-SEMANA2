using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmGame
{
    /// <summary>
    /// Estado completo del jugador.
    /// </summary>
    internal class Player
    {
        #region Datos Base del Jugador

        public string Name { get; set; }
        public int Gold { get; set; }
        public int Day { get; set; } = 1; // Día actual de la partida (comienza en 1).
        public List<FarmPlot> Farm { get; set; } = new List<FarmPlot>();

        #endregion

        #region Datos de Inventario

        public Dictionary<string, int> SeedInventory { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, int> HarvestInventory { get; set; } = new Dictionary<string, int>();

        #endregion

        #region Datos de Mercado

        public List<SellOrder> PendingSales { get; set; } = new List<SellOrder>();
        public int TotalPlots => Farm.Count;
        public int EmptyPlots => Farm.Count(p => p.IsEmpty);
        public int ReadyPlots => Farm.Count(p => p.IsReady);
        public int TotalSeeds => SeedInventory.Values.Sum();
        public int TotalHarvested => HarvestInventory.Values.Sum();

        #endregion

        #region Logica de Inventario

        public void AddSeeds(string seedName, int quantity)
        {
            if (!SeedInventory.ContainsKey(seedName)) SeedInventory[seedName] = 0;
            SeedInventory[seedName] += quantity;
        }

        public bool RemoveSeeds(string seedName, int quantity)
        {
            if (!SeedInventory.ContainsKey(seedName) || SeedInventory[seedName] < quantity)
            {
                return false;
            }

            SeedInventory[seedName] -= quantity;
            
            if (SeedInventory[seedName] == 0) SeedInventory.Remove(seedName);
            return true;
        }

        public void AddHarvest(string cropName, int quantity)
        {
            if (!HarvestInventory.ContainsKey(cropName)) HarvestInventory[cropName] = 0;
            HarvestInventory[cropName] += quantity;
        }

        public bool RemoveHarvest(string cropName, int quantity)
        {
            if (!HarvestInventory.ContainsKey(cropName) || HarvestInventory[cropName] < quantity)
            {
                return false;
            }
            
            HarvestInventory[cropName] -= quantity;
            
            if (HarvestInventory[cropName] == 0) HarvestInventory.Remove(cropName);
            return true;
        }

        #endregion
    }
}