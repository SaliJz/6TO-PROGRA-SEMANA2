using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmGame
{
    /// <summary>
    /// Catálogo estático con todas las semillas disponibles.
    /// </summary>
    internal class SeedCatalog
    {
        //  Tabla de semillas
        // Nombre | Costo | Tiempo de crecimiento | Precio de venta | Tiempo de demora de venta
        public static readonly IReadOnlyList<SeedDefinition> All =
            new List<SeedDefinition>
            {
                new SeedDefinition
                {
                    Name         = "Trigo",
                    Icon         = "[T]",
                    BuyCost      = 5,
                    GrowthDays   = 1,
                    SellPrice    = 12,
                    SellDelayDays = 1,
                    Description  = "Crece en 1 dia. Mercado rapido. Ganancia baja."
                },
                new SeedDefinition
                {
                    Name         = "Zanahoria",
                    Icon         = "[Z]",
                    BuyCost      = 8,
                    GrowthDays   = 2,
                    SellPrice    = 22,
                    SellDelayDays = 2,
                    Description  = "Equilibrio entre costo, tiempo y ganancia."
                },
                new SeedDefinition
                {
                    Name         = "Tomate",
                    Icon         = "[O]",
                    BuyCost      = 15,
                    GrowthDays   = 3,
                    SellPrice    = 45,
                    SellDelayDays = 3,
                    Description  = "Alta inversion y alta recompensa."
                },
                new SeedDefinition
                {
                    Name         = "Maiz",
                    Icon         = "[M]",
                    BuyCost      = 12,
                    GrowthDays   = 4,
                    SellPrice    = 40,
                    SellDelayDays = 2,
                    Description  = "Lento de crecer, mercado moderado."
                },
                new SeedDefinition
                {
                    Name         = "Fresa",
                    Icon         = "[F]",
                    BuyCost      = 20,
                    GrowthDays   = 2,
                    SellPrice    = 60,
                    SellDelayDays = 4,
                    Description  = "Crece rapido pero el mercado es muy lento."
                }
            };

        // Busca una semilla por nombre.
        public static SeedDefinition GetByName(string name)
        {
            return All.FirstOrDefault(
                s => s.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        // Busca una semilla por índice (1-base).
        public static SeedDefinition GetByIndex(int oneBased)
        {
            int idx = oneBased - 1;
            return (idx >= 0 && idx < All.Count) ? All[idx] : null;
        }
    }
}