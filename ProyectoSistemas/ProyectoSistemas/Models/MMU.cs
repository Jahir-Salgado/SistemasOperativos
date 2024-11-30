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
        public int PageFaults { get; private set; } // Contador de fallos de página
        public List<List<int?>> FrameStates { get; private set; } // Lista para almacenar los estados de los marcos



        public MMU(int frameCount)
        {
            FrameCount = frameCount;
            Pages = new List<int>();
            Frames = new List<int>();
            ProcessLogs = new List<string>();
            FrameStates = new List<List<int?>>(); // Inicializar lista de estados de marcos

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
                    PageFaults++; // Incrementar el contador de fallos de página
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
                // Registrar el estado actual de los marcos en cada paso para mostrarlos graficamente
                FrameStates.Add(new List<int?>(Frames.Select(f => (int?)f)));
                while (FrameStates.Last().Count < FrameCount)
                {
                    FrameStates.Last().Add(null); // Rellenar con null si faltan marcos
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
            int? lastReplacedPage = null; // Para rastrear la última página reemplazada

            for (int i = 0; i < Pages.Count; i++)
            {
                int currentPage = Pages[i];

                if (!frames.Contains(currentPage)) // Si la página no está en los marcos
                {
                    PageFaults++; // Incrementar el contador de fallos de página
                    if (frames.Count == FrameCount) // Si los marcos están llenos
                    {
                        // Usar la lógica de reemplazo óptimo
                        int pageToReplace = -1;
                        int farthestUse = -1;
                        bool replacedUsingFIFO = false;

                        // Buscar la página a reemplazar
                        foreach (int frame in frames)
                        {
                            // Evitar seleccionar la página recién reemplazada
                            if (lastReplacedPage != null && frame == lastReplacedPage)
                            {
                                continue;
                            }

                            // Buscar el próximo uso de esta página
                            int nextUse = Pages.Skip(i + 1).ToList().IndexOf(frame);

                            if (nextUse == -1) // Si la página no se usará más
                            {
                                pageToReplace = frame;
                                replacedUsingFIFO = false;
                                break;
                            }
                            else if (nextUse > farthestUse) // Si esta página se usará más tarde que las demás
                            {
                                farthestUse = nextUse;
                                pageToReplace = frame;
                            }
                        }

                        // Si no se puede decidir de forma óptima (todas las páginas tienen uso futuro incierto)
                        if (pageToReplace == -1 || replacedUsingFIFO)
                        {
                            // Aplicar FIFO, excluyendo el último reemplazo
                            foreach (int frame in frames)
                            {
                                if (lastReplacedPage != null && frame == lastReplacedPage)
                                {
                                    continue; // Excluir el último reemplazo
                                }

                                pageToReplace = frame;
                                break;
                            }
                        }

                        // Reemplazar la página más óptima
                        frames.Remove(pageToReplace);
                        ProcessLogs.Add($"Página {pageToReplace} reemplazada por {currentPage}");
                        lastReplacedPage = pageToReplace; // Actualizar la última página reemplazada
                    }

                    // Agregar la nueva página al marco
                    frames.Add(currentPage);
                    ProcessLogs.Add($"Cargamos la página {currentPage}: {string.Join(", ", frames)}");
                }
                else
                {
                    ProcessLogs.Add($"Página {currentPage} ya está en memoria: {string.Join(", ", frames)}");
                }

                // **Agregar el registro del estado de los marcos**
                FrameStates.Add(new List<int?>(frames.Select(f => (int?)f)));
                while (FrameStates.Last().Count < FrameCount)
                {
                    FrameStates.Last().Add(null); // Rellenar con null si faltan marcos
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
                    PageFaults++; // Incrementar el contador de fallos de página
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
                // **Agregar el registro del estado de los marcos **
                FrameStates.Add(new List<int?>(frames.Select(p => (int?)p.PageNumber)));
                while (FrameStates.Last().Count < FrameCount)
                {
                    FrameStates.Last().Add(null); // Rellenar con null si faltan marcos
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
        public void SecondChance()
        {
            ProcessLogs.Clear(); // Limpiar los logs antes de comenzar
            ProcessLogs.Add("Simulación de Segunda Oportunidad:");

            // Inicializar los marcos como una lista de objetos "Page"
            List<Page> frames = new List<Page>();
            int pointer = 0; // Puntero para indicar la posición actual en los marcos

            foreach (int currentPage in Pages)
            {
                // Verificar si la página ya está en memoria
                var pageInMemory = frames.FirstOrDefault(p => p.PageNumber == currentPage);

                if (pageInMemory != null)
                {
                    // Si la página ya está en memoria, marcamos su bit de referencia
                    pageInMemory.R = true;
                    ProcessLogs.Add($"Página {currentPage} ya está en memoria. Marcamos su bit de referencia.");
                }
                else
                {
                    // Si la página no está en memoria, debemos cargarla
                    PageFaults++; // Incrementar el contador de fallos de página (página no estaba en memoria)
                    if (frames.Count < FrameCount)
                    {
                        // Hay espacio disponible, cargamos la página directamente
                        frames.Add(new Page { PageNumber = currentPage, R = true, M = false });
                        ProcessLogs.Add($"Cargamos la página {currentPage}: {string.Join(", ", frames.Select(p => p.PageNumber))}");
                    }
                    else
                    {
                        // No hay espacio disponible, aplicamos el algoritmo de segunda oportunidad
                        while (true)
                        {
                            var candidatePage = frames[pointer];

                            if (!candidatePage.R)
                            {
                                // Reemplazamos la página con bit de referencia en 0
                                ProcessLogs.Add($"Página {candidatePage.PageNumber} reemplazada por {currentPage}");
                                frames[pointer] = new Page { PageNumber = currentPage, R = true, M = false };
                                pointer = (pointer + 1) % FrameCount; // Avanzar el puntero circularmente
                                break;
                            }
                            else
                            {
                                // La página recibe una segunda oportunidad: limpiamos su bit de referencia
                                candidatePage.R = false;
                                ProcessLogs.Add($"Página {candidatePage.PageNumber} recibe segunda oportunidad.");
                                pointer = (pointer + 1) % FrameCount; // Avanzar el puntero circularmente
                            }
                        }
                    }
                }
                // **Agregar el registro del estado de los marcos **
                FrameStates.Add(new List<int?>(frames.Select(p => (int?)p.PageNumber)));
                while (FrameStates.Last().Count < FrameCount)
                {
                    FrameStates.Last().Add(null); // Rellenar con null si faltan marcos
                }
            }

            // Actualizar los marcos finales en el atributo Frames para consistencia
            Frames = frames.Select(p => p.PageNumber).ToList();
            ProcessLogs.Add($"Estado final de los marcos: {string.Join(", ", Frames)}");
        }

        public void Clock()
        {
            Frames.Clear(); // Limpiar los marcos antes de comenzar
            ProcessLogs.Clear(); // Limpiar los logs antes de comenzar
            ProcessLogs.Add("Simulación Algoritmo de Reloj:");

            List<Page> frames = new List<Page>(); // Lista circular para los marcos
            int clockPointer = 0; // Puntero del reloj

            foreach (int currentPage in Pages)
            {
                // Verificar si la página ya está en memoria
                var pageInMemory = frames.FirstOrDefault(p => p.PageNumber == currentPage);

                if (pageInMemory != null)
                {
                    // La página ya está en memoria, actualizamos su bit de referencia
                    pageInMemory.R = true;
                    ProcessLogs.Add($"Página {currentPage} ya está en memoria. Marcamos su bit de referencia.");
                }
                else
                {
                    // La página no está en memoria, se debe cargar
                    PageFaults++; // Incrementar el contador de fallos de página (página no estaba en memoria)
                    if (frames.Count < FrameCount)
                    {
                        // Si hay espacio disponible, cargamos la página directamente
                        frames.Add(new Page { PageNumber = currentPage, R = true, M = false });
                        ProcessLogs.Add($"Cargamos la página {currentPage}: {string.Join(", ", frames.Select(p => p.PageNumber))}");
                    }
                    else
                    {
                        // No hay espacio disponible, aplicamos el algoritmo de reloj
                        while (true)
                        {
                            var candidatePage = frames[clockPointer];

                            if (!candidatePage.R)
                            {
                                // Reemplazamos la página con bit de referencia en 0
                                ProcessLogs.Add($"Página {candidatePage.PageNumber} reemplazada por {currentPage}");
                                frames[clockPointer] = new Page { PageNumber = currentPage, R = true, M = false };
                                clockPointer = (clockPointer + 1) % FrameCount; // Avanzar el puntero circularmente
                                break;
                            }
                            else
                            {
                                // La página recibe una segunda oportunidad: limpiamos su bit de referencia
                                candidatePage.R = false;
                                ProcessLogs.Add($"Página {candidatePage.PageNumber} recibe segunda oportunidad.");
                                clockPointer = (clockPointer + 1) % FrameCount; // Avanzar el puntero circularmente
                            }
                        }
                    }
                }
                // **Agregar el registro del estado de los marcos**
                FrameStates.Add(new List<int?>(frames.Select(p => (int?)p.PageNumber)));
                while (FrameStates.Last().Count < FrameCount)
                {
                    FrameStates.Last().Add(null); // Rellenar con null si faltan marcos
                }
            }

            // Actualizar el estado final de los marcos
            Frames = frames.Select(p => p.PageNumber).ToList();
            ProcessLogs.Add($"Estado final de los marcos: {string.Join(", ", Frames)}");
        }

















    }



}
