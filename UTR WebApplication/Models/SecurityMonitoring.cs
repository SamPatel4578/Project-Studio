using System;
using System.Collections.Generic;

namespace UTR_WebApplication.Models;

public partial class SecurityMonitoring
{
    public int SecurityEventId { get; set; }

    public int? UserId { get; set; }

    public string? EventDescription { get; set; }

    public virtual User? User { get; set; }
}
