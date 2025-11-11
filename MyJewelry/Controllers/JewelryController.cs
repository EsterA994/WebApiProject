using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Collections.Generic;
using MyJewelry.Services;
using MyJewelry.Models;

namespace MyJewelry.Controllers;

[ApiController]
[Route("[controller]")]
public class JewelryController : ControllerBase
{
    private JewelryService service;

    public JewelryController()
    {
        service = new JewelryService();
    }


    [HttpGet()]
    public ActionResult<IEnumerable<Jewelry>> Get()
    {
        return service.Get();
    }

    [HttpGet("{id}")]
    public ActionResult<Jewelry> Get(int id)
    {
        var jewelry = service.Get(id);
        if (jewelry == null)
            return NotFound();
        return jewelry;
    }

    [HttpPost]
    public ActionResult Create(Jewelry newJewelry)
    {
        var postedJewelry = service.Create(newJewelry);
        return CreatedAtAction(nameof(Create), new { id = postedJewelry.Id });
    }

    [HttpPut("{id}")]
    public ActionResult Update(int id, Jewelry newJewelry)
    {
        var jewelry = service.find(id);
        if (jewelry == null)
            return NotFound();
        if (jewelry.Id != newJewelry.Id)
            return BadRequest();
        service.Update(id,newJewelry);

        return NoContent();
    }

    [HttpDelete("{id}")]
    public ActionResult Delete(int id)
    {
        var jewelry = service.find(id);
        if (jewelry == null)
            return NotFound();
        service.Delete(id);

        return NoContent();
    }

}
