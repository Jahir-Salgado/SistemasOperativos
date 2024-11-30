using ProyectoSistemas.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProyectoSistemas.Controllers
{
    public class MMUController : Controller
    {
        // GET: MMU
        public ActionResult EmulateMMU()
        {
            return View();
        }

        [HttpPost]
        public ActionResult RunAlgorithm(string pages, int frames, string algorithm, int virtualMemory)
        {
            MMU mmu = new MMU(frames);
            mmu.Pages = pages.Split(',').Select(int.Parse).ToList(); // Secuencia de páginas

            // Validaciones
            if (mmu.Pages.Any(page => page > virtualMemory || page < 1) || virtualMemory < mmu.Pages.Max())
            {
                ViewBag.Error = "Asegúrate de que el número de páginas (memoria virtual) sea mayor o igual al valor máximo de la secuencia de referencias y que las referencias estén dentro del rango.";
                return View("EmulateMMU");
            }

            // Ejecutar el algoritmo
            switch (algorithm)
            {
                case "FIFO":
                    mmu.FIFO();
                    break;
                case "Optimal":
                    mmu.Optimal();
                    break;
                case "NRU":
                    mmu.NRU();
                    break;
                case "SecondChance":
                    mmu.SecondChance();
                    break;
                case "Clock":
                    mmu.Clock();
                    break;
            }

            // Validar que FrameStates no sea null o esté vacío
            if (mmu.FrameStates == null || mmu.FrameStates.Count == 0)
            {
                ViewBag.Error = "Hubo un problema al generar los estados de los marcos. Por favor, verifica los datos ingresados o el algoritmo seleccionado.";
                return View("EmulateMMU");
            }

            // Cálculo del rendimiento
            int totalReferences = mmu.Pages.Count;
            int pageFaults = mmu.PageFaults; // Usar el contador de fallos de la clase MMU
            double performance = 1 - ((double)pageFaults / totalReferences);

            // Enviar resultados a la vista
            ViewBag.ProcessLogs = mmu.ProcessLogs;
            ViewBag.Frames = mmu.Frames; // Estado final de los marcos
            ViewBag.FrameStates = mmu.FrameStates; // Estados dinámicos de los marcos
            ViewBag.Pages = mmu.Pages; // Secuencia de referencias de páginas
            ViewBag.PageFaults = pageFaults; // Total de fallos
            ViewBag.Performance = performance * 100; // Porcentaje de rendimiento

            return View("EmulateMMU");
        }


    }
}