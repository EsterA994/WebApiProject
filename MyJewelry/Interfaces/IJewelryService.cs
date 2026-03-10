
using System.Linq;
using System.Collections.Generic;
using MyJewelry.Models;


namespace MyJewelry.Interfaces;

public interface IJewelryService : ICrud<Jewelry>
{

    int Count { get; }
}