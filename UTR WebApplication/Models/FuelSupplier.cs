using System;
using System.Collections.Generic;

namespace UTR_WebApplication.Models;

public partial class FuelSupplier
{
    public int FuelSupplierId { get; set; }

    public string? SupplierName { get; set; }

    public string? FuelType { get; set; }

    public virtual ICollection<FuelOrder> FuelOrders { get; set; } = new List<FuelOrder>();
}
