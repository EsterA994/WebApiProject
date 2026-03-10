
using System.Linq;
using System.Collections.Generic;
using MyJewelry.Models;

namespace MyJewelry.Interfaces;

public interface ICrud<T>
{
    List<T> Get();
    T Get(int id);
    int Create(T item);
    bool Delete(int id);
    bool Update(T item);
}