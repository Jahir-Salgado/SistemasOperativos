﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProyectoSistemas.Controllers
{
    public class SettingsController : Controller
    {
        // GET: Settings
        public ActionResult ConfigureOS()
        {
            return View();
        }
    }
}