using Microsoft.AspNetCore.Mvc;

namespace DevSpot.Controllers;

public class ErrorController : Controller
{
    [Route("/Error/{statusCode}")]
    public IActionResult Error(int statusCode)
    {
        if (statusCode == 404)
        {
            return View("NotFound");
        }
        
        return View("General");
    }
}