using System;
using System.Collections.Generic;

namespace UTR_WebApplication.Models;

public partial class Permission
{
    public int PermissionId { get; set; }

    public int? RoleId { get; set; }

    public string? PermissionName { get; set; }

    public virtual UserRole? Role { get; set; }
}
