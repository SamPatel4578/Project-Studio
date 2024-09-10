using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace UTR_WebApplication.Models;

public partial class PaymentDetail
{
    public int PaymentId { get; set; }

    public int? UserId { get; set; }

    [Column("cardholder_name")]
    public string CardholderName { get; set; }

    public string? PaymentMethod { get; set; }

    [Column("card_last_digits")]
    public string? CardLastDigits { get; set; }

    [Column("payment_status")]
    public string? PaymentStatus { get; set; }

    [Column("amount")]
    public decimal? Amount { get; set; }

    public virtual User? User { get; set; }
}
