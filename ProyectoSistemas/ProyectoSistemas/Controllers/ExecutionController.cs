using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProyectoSistemas.Models; // Asegúrate de incluir este namespace para acceder a la clase ExecutionList

namespace ProyectoSistemas.Controllers
{
    public class ExecutionController : Controller
    {
        // GET: Execution/DefineExecutionOrder
        public ActionResult DefineExecutionOrder()
        {
            // Obtener la lista de programas de ejemplo
            List<ExecutionList> programs = ExecutionList.GetSamplePrograms();

            // Ordenar la lista de programas según la prioridad
            programs.Sort((x, y) => x.Priority.CompareTo(y.Priority));

            // Enviar la lista ordenada a la vista
            return View(programs);
        }
    }
}
