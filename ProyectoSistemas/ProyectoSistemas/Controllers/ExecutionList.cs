using System;
using System.Collections.Generic;

namespace ProyectoSistemas.Models
{
    public class ExecutionList
    {
        public int ProgramId { get; set; } // ID del programa
        public string ProgramName { get; set; } // Nombre del programa
        public int Priority { get; set; } // Prioridad del programa (1 = alta, 2 = media, 3 = baja)
        public string Status { get; set; } // Estado del programa (e.g., "Pendiente", "En ejecución", "Finalizado")

        public static List<ExecutionList> GetSamplePrograms()
        {
            // Lista de programas de ejemplo
            return new List<ExecutionList>
            {
                new ExecutionList { ProgramId = 1, ProgramName = "Programa A", Priority = 1, Status = "Pendiente" },
                new ExecutionList { ProgramId = 2, ProgramName = "Programa B", Priority = 2, Status = "Pendiente" },
                new ExecutionList { ProgramId = 3, ProgramName = "Programa C", Priority = 3, Status = "Pendiente" },
                new ExecutionList { ProgramId = 4, ProgramName = "Programa D", Priority = 1, Status = "Pendiente" }
            };
        }
    }
}
