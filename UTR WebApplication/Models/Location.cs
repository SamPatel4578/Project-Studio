using System;
using System.Collections.Generic;

namespace UTR_WebApplication.Models;

public partial class Location
{
    public int LocationId { get; set; }

    public string? StationName { get; set; }

    public string? Address { get; set; }

    public string? GpsCoordinates { get; set; }

    public string? AvailableServices { get; set; }

    public virtual ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();
}
