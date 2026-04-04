using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmGame
{
    /// <summary>Estado posible de una parcela en la granja.</summary>
    public enum PlotState
    {
        Empty,
        Growing,
        ReadyToHarvest
    }

    /// <summary>
    /// Representa una parcela individual dentro de la granja del jugador.
    /// </summary>
    internal class FarmPlot
    {
        #region Datos Base de la Parcela

        public int Id { get; set; } // Identificador único de la parcela.
        public PlotState State { get; set; } = PlotState.Empty;
        public SeedDefinition Seed { get; set; }
        public int DaysPlanted { get; set; }

        #endregion

        #region Datos de Crecimiento

        public bool IsEmpty => State == PlotState.Empty;
        public bool IsReady => State == PlotState.ReadyToHarvest;

        public int DaysRemaining
        {
            get
            {
                if (Seed == null || State != PlotState.Growing) return 0;
                int remaining = Seed.GrowthDays - DaysPlanted;
                return remaining < 0 ? 0 : remaining;
            }
        }

        public string DisplayStatus
        {
            get
            {
                switch (State)
                {
                    case PlotState.Empty: 
                        return "[Vacía]";
                    case PlotState.Growing: 
                        return $"[Creciendo - {DaysRemaining}d]";
                    case PlotState.ReadyToHarvest: 
                        return "[¡LISTA!]";
                    default: 
                        return "[???]";
                }
            }
        }

        #endregion
    }
}