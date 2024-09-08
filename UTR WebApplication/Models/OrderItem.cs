using System;
using System.Collections.Generic;

namespace UTR_WebApplication.Models;

public partial class OrderItem
{
    public int OrderItemId { get; set; }

    public int? FoodOrderId { get; set; }

    public string? ItemName { get; set; }

    public int? Quantity { get; set; }

    public decimal? Price { get; set; }

    public string? ImageUrl { get; set; }

    public virtual FoodOrder? FoodOrder { get; set; }
}
