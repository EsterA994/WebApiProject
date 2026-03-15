using System.Collections.Generic;
using System.Linq;
// using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyJewelry.Interfaces;
using MyJewelry.Models;
using MyJewelry.Services;

namespace MyJewelry.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class JewelryController : ControllerBase
{
    IJewelryService jewelryService;

    public JewelryController(IJewelryService jewelryService)
    {
        this.jewelryService = jewelryService;
    }

    [HttpGet()]
    public ActionResult<IEnumerable<Jewelry>> Get()
    {
        return jewelryService.Get();
    }

    [HttpGet("{id}")]
    public ActionResult<Jewelry> Get(int id)
    {
        var jewelry = jewelryService.Get(id);
        if (jewelry == null)
            return NotFound();
        return jewelry;
    }

    [HttpPost]
    public ActionResult Create(Jewelry newJewelry)
    {
        var jewelryId = jewelryService.Create(newJewelry);

        return CreatedAtAction(nameof(Get), new { id = jewelryId }, newJewelry);
    }

    [HttpPut("{id}")]
    public ActionResult Update(int id, Jewelry newJewelry)
    {
        if (id != newJewelry.Id)
            return BadRequest();

        var success = jewelryService.Update(newJewelry);

        if (!success)
            return NotFound();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public ActionResult Delete(int id)
    {
        var jewelry = jewelryService.Get(id);

        if (jewelry == null)
            return NotFound();
        jewelryService.Delete(id);

        return NoContent();
    }
}
