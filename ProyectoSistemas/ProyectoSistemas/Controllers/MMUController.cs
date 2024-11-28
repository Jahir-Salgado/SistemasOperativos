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

            if (mmu.Pages.Any(page => page > virtualMemory || page < 1))
            {
                ViewBag.Error = "Algunas referencias de página están fuera del rango de la memoria virtual.";
                return View("EmulateMMU"); // Esto reinicia la vista
            }
            if (virtualMemory < mmu.Pages.Max())
            {
                ViewBag.Error = "El número de páginas (memoria virtual) debe ser mayor o igual al valor máximo de la secuencia de referencias.";
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

            // Cálculo del rendimiento
            int totalReferences = mmu.Pages.Count;
            int pageFaults = mmu.PageFaults; // Usar el contador de fallos de la clase MMU
            double performance = 1 - ((double)pageFaults / totalReferences);

            // Enviar resultados a la vista
            ViewBag.ProcessLogs = mmu.ProcessLogs;
            ViewBag.Frames = mmu.Frames;
            ViewBag.PageFaults = pageFaults;
            ViewBag.Performance = performance * 100;

            return View("EmulateMMU");
        }

    }
}