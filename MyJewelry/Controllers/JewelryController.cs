using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Collections.Generic;
using MyJewelry.Services;
using MyJewelry.Models;
using MyJewelry.Interfaces;

namespace MyJewelry.Controllers;

[ApiController]
[Route("[controller]")]
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
        var postedJewelry = jewelryService.Create(newJewelry);
        return CreatedAtAction(nameof(Create), new { id = postedJewelry.Id });
    }

    [HttpPut("{id}")]
    public ActionResult Update(int id, Jewelry newJewelry)
    {
        var jewelry = jewelryService.find(id);
        if (jewelry == null)
            return NotFound();
        if (jewelry.Id != newJewelry.Id)
            return BadRequest();
        jewelryService.Update(id,newJewelry);

        return NoContent();
    }

    [HttpDelete("{id}")]
    public ActionResult Delete(int id)
    {
        var jewelry = jewelryService.find(id);
        
        if (jewelry == null)
            return NotFound();
        jewelryService.Delete(id);

        return NoContent();
    }

}
