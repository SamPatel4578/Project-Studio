using System;
using System.Collections.Generic;

namespace UTR_WebApplication.Models;

public partial class MenuItem
{
    public int MenuItemId { get; set; }

    public string? ItemName { get; set; }

    public string? Description { get; set; }

    public decimal? Price { get; set; }

    public string? ImageUrl { get; set; }
}
