using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace ProyectoSistemas.Models
{

    public class MMU
    {
        public int FrameCount { get; set; } // Número de marcos en memoria física
        public List<int> Pages { get; set; } // Lista de páginas solicitadas
        public List<int> Frames { get; set; } // Lista para representar los marcos en FIFO
        public List<string> ProcessLogs { get; private set; } // Logs del proceso

       

        public MMU(int frameCount)
        {
            FrameCount = frameCount;
            Pages = new List<int>();
            Frames = new List<int>();
            ProcessLogs = new List<string>();


        }
        public void FIFO()
        {
            Frames.Clear(); // Limpiar los marcos antes de comenzar
            ProcessLogs.Clear(); // Limpiar los logs antes de comenzar
            ProcessLogs.Add("Simulación FIFO:");

            foreach (int page in Pages)
            {
                if (!Frames.Contains(page)) // Si la página no está en los marcos
                {
                    if (Frames.Count == FrameCount) // Si los marcos están llenos
                    {
                        int removedPage = Frames[0]; // Sacar la más antigua (FIFO)
                        Frames.RemoveAt(0); // Eliminarla de la lista
                        ProcessLogs.Add($"Página {removedPage} reemplazada por {page}");
                    }

                    Frames.Add(page); // Agregar la nueva página al final
                    ProcessLogs.Add($"Cargamos la página {page}: {string.Join(", ", Frames)}");
                }
                else
                {
                    ProcessLogs.Add($"Página {page} ya está en memoria: {string.Join(", ", Frames)}");
                }
            }

            ProcessLogs.Add($"Estado final de los marcos: {string.Join(", ", Frames)}");
        }
        public void Optimal()
        {
            Frames.Clear(); // Limpiar los marcos antes de comenzar
            ProcessLogs.Clear(); // Limpiar los logs antes de comenzar
            ProcessLogs.Add("Simulación Óptima:");

            List<int> frames = new List<int>(); // Lista para manejar los marcos

            for (int i = 0; i < Pages.Count; i++)
            {
                int currentPage = Pages[i];

                if (!frames.Contains(currentPage)) // Si la página no está en los marcos
                {
                    if (frames.Count == FrameCount) // Si los marcos están llenos
                    {
                        // Usar la lógica de reemplazo óptimo
                        int pageToReplace = -1;
                        int farthestUse = -1;

                        // Buscar la página a reemplazar
                        foreach (int frame in frames)
                        {
                            // Buscar el próximo uso de esta página
                            int nextUse = Pages.Skip(i + 1).ToList().IndexOf(frame);

                            if (nextUse == -1) // Si la página no se usará más
                            {
                                pageToReplace = frame;
                                break;
                            }
                            else if (nextUse > farthestUse) // Si esta página se usará más tarde que las demás
                            {
                                farthestUse = nextUse;
                                pageToReplace = frame;
                            }
                        }

                        // Reemplazar la página más óptima
                        frames.Remove(pageToReplace);
                        ProcessLogs.Add($"Página {pageToReplace} reemplazada por {currentPage}");
                    }

                    // Agregar la nueva página al marco
                    frames.Add(currentPage);
                    ProcessLogs.Add($"Cargamos la página {currentPage}: {string.Join(", ", frames)}");
                }
                else
                {
                    ProcessLogs.Add($"Página {currentPage} ya está en memoria: {string.Join(", ", frames)}");
                }
            }

            // Actualizar los marcos finales
            Frames = new List<int>(frames);
            ProcessLogs.Add($"Estado final de los marcos: {string.Join(", ", Frames)}");
        }
        // Implementación del Algoritmo NRU
        public void NRU()
        {
            Frames.Clear(); // Limpiar los marcos antes de comenzar
            ProcessLogs.Clear(); // Limpiar los logs antes de comenzar
            ProcessLogs.Add("Simulación NRU:");

            // Representamos los marcos como una lista de páginas con bits R y M
            List<Page> frames = new List<Page>();

            foreach (int currentPage in Pages)
            {
                var pageInMemory = frames.FirstOrDefault(p => p.PageNumber == currentPage);

                if (pageInMemory == null) // Si la página no está en memoria
                {
                    if (frames.Count == FrameCount) // Si los marcos están llenos
                    {
                        // Reemplazar según los bits R y M
                        Page pageToReplace = SelectPageToReplace(frames);
                        frames.Remove(pageToReplace);
                        ProcessLogs.Add($"Página {pageToReplace.PageNumber} reemplazada por {currentPage}");
                    }

                    // Agregar la nueva página
                    frames.Add(new Page { PageNumber = currentPage, R = true, M = false });
                    ProcessLogs.Add($"Cargamos la página {currentPage}: {string.Join(", ", frames.Select(p => p.PageNumber))}");
                }
                else
                {
                    // Si la página ya está en memoria, actualizamos su bit R
                    pageInMemory.R = true;
                    ProcessLogs.Add($"Página {currentPage} ya está en memoria: {string.Join(", ", frames.Select(p => p.PageNumber))}");
                }
            }

            // Actualizar el estado final de los marcos
            Frames = frames.Select(p => p.PageNumber).ToList();
            ProcessLogs.Add($"Estado final de los marcos: {string.Join(", ", Frames)}");
        }

        private Page SelectPageToReplace(List<Page> frames)
        {
            // Buscar la página con R=0 y M=0 (no referenciada, no modificada)
            var candidate = frames.FirstOrDefault(p => !p.R && !p.M);
            if (candidate != null) return candidate;

            // Si no hay ninguna con R=0 y M=0, buscar con R=0 y M=1
            candidate = frames.FirstOrDefault(p => !p.R && p.M);
            if (candidate != null) return candidate;

            // Si no, buscar con R=1 y M=0
            candidate = frames.FirstOrDefault(p => p.R && !p.M);
            if (candidate != null) return candidate;

            // Finalmente, reemplazar la que tiene R=1 y M=1
            return frames.FirstOrDefault(p => p.R && p.M);
        }

        private class Page
        {
            public int PageNumber { get; set; }
            public bool R { get; set; }
            public bool M { get; set; }
        }











       
    }



}
