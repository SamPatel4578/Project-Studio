using System;
using System.Collections.Generic;

namespace UTR_WebApplication.Models;

public partial class Inventory
{
    public int InventoryId { get; set; }

    public string? ItemName { get; set; }

    public int? QuantityInStock { get; set; }

    public string? Type { get; set; }

    public int? StationId { get; set; }

    public virtual Location? Station { get; set; }
}
