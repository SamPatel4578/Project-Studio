using System;
using System.Collections.Generic;

namespace UTR_WebApplication.Models;

public partial class PaymentDetail
{
    public int PaymentId { get; set; }

    public int? UserId { get; set; }

    public string? PaymentMethod { get; set; }

    public string? CardLastDigits { get; set; }

    public string? PaymentStatus { get; set; }

    public decimal? Amount { get; set; }

    public virtual User? User { get; set; }
}
