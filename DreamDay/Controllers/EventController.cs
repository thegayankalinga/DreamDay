﻿using Microsoft.AspNetCore.Mvc;

namespace DreamDay.Controllers;

public class EventController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}