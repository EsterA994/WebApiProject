using System.Collections.Generic;
using MyJewelry.Models;

namespace MyJewelry.Interfaces
{
    public interface IUserRepository
    {
        List<User> GetAll();
        User Get(int id);
        void Add(User item);
        void Delete(int id);
        void Update(User item);
        int Count { get; }
    }
}
