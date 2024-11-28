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
        public ActionResult RunAlgorithm(string pages, int frames, string algorithm)
        {
            MMU mmu = new MMU(frames);
            mmu.Pages = pages.Split(',').Select(int.Parse).ToList(); // Convertir la secuencia de páginas

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
            }

            // Enviar los resultados a la vista
            ViewBag.ProcessLogs = mmu.ProcessLogs; // Logs del proceso
            ViewBag.Frames = mmu.Frames; // Estado final de los marcos
            ViewBag.Algorithm = algorithm;

            return View("Results");
        }
    }
}