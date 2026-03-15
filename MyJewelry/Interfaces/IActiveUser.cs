using Microsoft.AspNetCore.Http;
using MyJewelry.Models;

namespace MyJewelry.Interfaces;

public interface IActiveUser
{
    User ActiveUser { get; }
}
