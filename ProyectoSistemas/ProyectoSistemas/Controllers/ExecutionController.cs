using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProyectoSistemas.Controllers
{
    public class ExecutionController : Controller
    {
        // GET: Execution
        public ActionResult DefineExecutionOrder()
        {
            return View();
        }
    }
}