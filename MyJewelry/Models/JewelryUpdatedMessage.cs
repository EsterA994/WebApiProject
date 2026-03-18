using System;

namespace MyJewelry.Models;

public class JewelryUpdatedMessage
{
    public int UserId { get; set; }
    public string Username { get; set; }
    public string JewelryName { get; set; }
    public DateTime Timestamp { get; set; }
}

