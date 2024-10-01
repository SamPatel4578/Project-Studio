using System;
using System.Collections.Generic;

namespace UTR_WebApplication.Models;

public partial class User
{
    public int UserId { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string Email { get; set; } = null!;

    public string? PhoneNumber { get; set; }

    public string? Address { get; set; }

    public int? RoleId { get; set; }

    public decimal? Points { get; set; }
    public string? Status { get; set; }
    public virtual ICollection<FoodOrder> FoodOrders { get; set; } = new List<FoodOrder>();

    public virtual ICollection<FuelOrder> FuelOrders { get; set; } = new List<FuelOrder>();

    public virtual ICollection<LoginDetail> LoginDetails { get; set; } = new List<LoginDetail>();

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual ICollection<PaymentDetail> PaymentDetails { get; set; } = new List<PaymentDetail>();

    public virtual UserRole? Role { get; set; }

    public virtual ICollection<SecurityMonitoring> SecurityMonitorings { get; set; } = new List<SecurityMonitoring>();
}
