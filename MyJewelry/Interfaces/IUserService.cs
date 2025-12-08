
using System.Linq;
using System.Collections.Generic;
using MyJewelry.Models;


namespace MyJewelry.Interfaces;

public interface IUserService
{

    public List<User> Get();

    public User find(int id);

    public User Get(int id);

    public User Create(User newUser);

    public bool Update(int id, User newUser);

    public bool Delete(int id);
}