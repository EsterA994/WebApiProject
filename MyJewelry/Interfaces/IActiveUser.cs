using MyJewelry.Models;
using Microsoft.AspNetCore.Http;


namespace MyJewelry.Interfaces;

public interface IActiveUser
{
    User ActiveUser { get; }
}