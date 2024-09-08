using System;
using System.Collections.Generic;

namespace UTR_WebApplication.Models;

public partial class LoginDetail
{
    public int LoginId { get; set; }

    public int? UserId { get; set; }

    public string? PasswordHash { get; set; }

    public bool? TwoFactorEnabled { get; set; }

    public bool? TermsAccepted { get; set; }

    public virtual User? User { get; set; }
}
