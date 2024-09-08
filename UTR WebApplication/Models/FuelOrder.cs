using System;
using System.Collections.Generic;

namespace UTR_WebApplication.Models;

public partial class FuelOrder
{
    public int FuelOrderId { get; set; }

    public int? UserId { get; set; }

    public int? FuelSupplierId { get; set; }

    public string? FuelType { get; set; }

    public decimal? Quantity { get; set; }

    public decimal? Price { get; set; }

    public string? Status { get; set; }

    public bool? NotificationSent { get; set; }

    public virtual FuelSupplier? FuelSupplier { get; set; }

    public virtual User? User { get; set; }
}
