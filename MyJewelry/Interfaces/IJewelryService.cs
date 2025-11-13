
using System.Linq;
using System.Collections.Generic;
using MyJewelry.Models;


namespace MyJewelry.Interfaces;

public interface IJewelryService
{

    public List<Jewelry> Get();

    public Jewelry find(int id);

    public Jewelry Get(int id);

    public Jewelry Create(Jewelry newJewelry);

    public bool Update(int id, Jewelry newJewelry);

    public bool Delete(int id);
}