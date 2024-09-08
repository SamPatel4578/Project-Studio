using System;
using System.Collections.Generic;

namespace UTR_WebApplication.Models;

public partial class FoodOrder
{
    public int FoodOrderId { get; set; }

    public int? UserId { get; set; }

    public decimal? TotalPrice { get; set; }

    public string? Status { get; set; }

    public string? DeliveryType { get; set; }

    public bool? NotificationSent { get; set; }

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual User? User { get; set; }
}
