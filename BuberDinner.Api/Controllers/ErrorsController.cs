using Microsoft.AspNetCore.Mvc;

namespace BuberDinner.Api.Controllers;

public class ErrorsController : ControllerBase
{
    [Route("error")]
    public ActionResult Error()
    {
        return Problem();
    }
}