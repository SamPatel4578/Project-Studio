using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using UTR_WebApplication.Models;

namespace UTR_WebApplication.Data;

public partial class UtrContext : DbContext
{
    public UtrContext()
    {
    }

    public UtrContext(DbContextOptions<UtrContext> options)
        : base(options)
    {
    }

    public virtual DbSet<FoodOrder> FoodOrders { get; set; }

    public virtual DbSet<FuelOrder> FuelOrders { get; set; }

    public virtual DbSet<FuelSupplier> FuelSuppliers { get; set; }

    public virtual DbSet<Inventory> Inventories { get; set; }

    public virtual DbSet<Location> Locations { get; set; }

    public virtual DbSet<LoginDetail> LoginDetails { get; set; }

    public virtual DbSet<MenuItem> MenuItems { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<OrderItem> OrderItems { get; set; }

    public virtual DbSet<PaymentDetail> PaymentDetails { get; set; }

    public virtual DbSet<Permission> Permissions { get; set; }

    public virtual DbSet<SecurityMonitoring> SecurityMonitorings { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserRole> UserRoles { get; set; }

    //    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
    //        => optionsBuilder.UseSqlServer("Data Source=SAM_PATEL27;Initial Catalog=UTR;Integrated Security=True;Encrypt=True;Trust Server Certificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FoodOrder>(entity =>
        {
            entity.HasKey(e => e.FoodOrderId).HasName("PK__Food_Ord__1F69CE2C395E436D");

            entity.ToTable("Food_Orders");

            entity.Property(e => e.FoodOrderId).HasColumnName("food_order_id");
            entity.Property(e => e.DeliveryType)
                .HasMaxLength(50)
                .HasColumnName("delivery_type");
            entity.Property(e => e.NotificationSent)
                .HasDefaultValue(false)
                .HasColumnName("notification_sent");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasColumnName("status");
            entity.Property(e => e.TotalPrice)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("total_price");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.FoodOrders)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Food_Orde__user___36B12243");
        });

        modelBuilder.Entity<FuelOrder>(entity =>
        {
            entity.HasKey(e => e.FuelOrderId).HasName("PK__Fuel_Ord__0C6E52524B4A5919");

            entity.ToTable("Fuel_Orders");

            entity.Property(e => e.FuelOrderId).HasColumnName("fuel_order_id");
            entity.Property(e => e.FuelSupplierId).HasColumnName("fuel_supplier_id");
            entity.Property(e => e.FuelType)
                .HasMaxLength(50)
                .HasColumnName("fuel_type");
            entity.Property(e => e.NotificationSent)
                .HasDefaultValue(false)
                .HasColumnName("notification_sent");
            entity.Property(e => e.Price)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("price");
            entity.Property(e => e.Quantity)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("quantity");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasColumnName("status");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.FuelSupplier).WithMany(p => p.FuelOrders)
                .HasForeignKey(d => d.FuelSupplierId)
                .HasConstraintName("FK__Fuel_Orde__fuel___32E0915F");

            entity.HasOne(d => d.User).WithMany(p => p.FuelOrders)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Fuel_Orde__user___31EC6D26");
        });

        modelBuilder.Entity<FuelSupplier>(entity =>
        {
            entity.HasKey(e => e.FuelSupplierId).HasName("PK__Fuel_Sup__B707DFBEFF486206");

            entity.ToTable("Fuel_Suppliers");

            entity.Property(e => e.FuelSupplierId).HasColumnName("fuel_supplier_id");
            entity.Property(e => e.FuelType)
                .HasMaxLength(50)
                .HasColumnName("fuel_type");
            entity.Property(e => e.SupplierName)
                .HasMaxLength(255)
                .HasColumnName("supplier_name");
        });

        modelBuilder.Entity<Inventory>(entity =>
        {
            entity.HasKey(e => e.InventoryId).HasName("PK__Inventor__B59ACC492A45145A");

            entity.ToTable("Inventory");

            entity.Property(e => e.InventoryId).HasColumnName("inventory_id");
            entity.Property(e => e.ItemName)
                .HasMaxLength(255)
                .HasColumnName("item_name");
            entity.Property(e => e.QuantityInStock).HasColumnName("quantity_in_stock");
            entity.Property(e => e.StationId).HasColumnName("station_id");
            entity.Property(e => e.Type)
                .HasMaxLength(50)
                .HasColumnName("type");

            entity.HasOne(d => d.Station).WithMany(p => p.Inventories)
                .HasForeignKey(d => d.StationId)
                .HasConstraintName("FK__Inventory__stati__45F365D3");
        });

        modelBuilder.Entity<Location>(entity =>
        {
            entity.HasKey(e => e.LocationId).HasName("PK__Location__771831EA6C037909");

            entity.Property(e => e.LocationId).HasColumnName("location_id");
            entity.Property(e => e.Address).HasColumnName("address");
            entity.Property(e => e.AvailableServices).HasColumnName("available_services");
            entity.Property(e => e.GpsCoordinates)
                .HasMaxLength(255)
                .HasColumnName("gps_coordinates");
            entity.Property(e => e.StationName)
                .HasMaxLength(255)
                .HasColumnName("station_name");
        });

        modelBuilder.Entity<LoginDetail>(entity =>
        {
            entity.HasKey(e => e.LoginId).HasName("PK__Login_De__C2C971DB971DB1DE");

            entity.ToTable("Login_Details");

            entity.Property(e => e.LoginId).HasColumnName("login_id");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(255)
                .HasColumnName("password_hash");
            entity.Property(e => e.TermsAccepted)
                .HasDefaultValue(false)
                .HasColumnName("terms_accepted");
            entity.Property(e => e.TwoFactorEnabled)
                .HasDefaultValue(false)
                .HasColumnName("two_factor_enabled");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.LoginDetails)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Login_Det__user___2C3393D0");
        });

        modelBuilder.Entity<MenuItem>(entity =>
        {
            entity.HasKey(e => e.MenuItemId).HasName("PK__Menu_Ite__973431D5E79CF8B6");

            entity.ToTable("Menu_Items");

            entity.Property(e => e.MenuItemId).HasColumnName("menu_item_id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.ImageUrl)
                .HasMaxLength(255)
                .HasColumnName("image_url");
            entity.Property(e => e.ItemName)
                .HasMaxLength(255)
                .HasColumnName("item_name");
            entity.Property(e => e.Price)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("price");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotificationId).HasName("PK__Notifica__E059842FF2F37156");

            entity.Property(e => e.NotificationId).HasColumnName("notification_id");
            entity.Property(e => e.Message).HasColumnName("message");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Notificat__user___412EB0B6");
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(e => e.OrderItemId).HasName("PK__Order_It__3764B6BC07B33A79");

            entity.ToTable("Order_Items");

            entity.Property(e => e.OrderItemId).HasColumnName("order_item_id");
            entity.Property(e => e.FoodOrderId).HasColumnName("food_order_id");
            entity.Property(e => e.ImageUrl)
                .HasMaxLength(255)
                .HasColumnName("image_url");
            entity.Property(e => e.ItemName)
                .HasMaxLength(255)
                .HasColumnName("item_name");
            entity.Property(e => e.Price)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("price");
            entity.Property(e => e.Quantity).HasColumnName("quantity");

            entity.HasOne(d => d.FoodOrder).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.FoodOrderId)
                .HasConstraintName("FK__Order_Ite__food___398D8EEE");
        });

        modelBuilder.Entity<PaymentDetail>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("PK__Payment___ED1FC9EA43CCB0AB");

            entity.ToTable("Payment_Details");

            entity.Property(e => e.PaymentId).HasColumnName("payment_id");
            entity.Property(e => e.Amount)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("amount");
            entity.Property(e => e.CardLastDigits)
                .HasMaxLength(4)
                .HasColumnName("card_last_digits");
            entity.Property(e => e.PaymentMethod)
                .HasMaxLength(50)
                .HasColumnName("payment_method");
            entity.Property(e => e.PaymentStatus)
                .HasMaxLength(50)
                .HasColumnName("payment_status");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.PaymentDetails)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Payment_D__user___3E52440B");
        });

        modelBuilder.Entity<Permission>(entity =>
        {
            entity.HasKey(e => e.PermissionId).HasName("PK__Permissi__E5331AFAB55770F5");

            entity.Property(e => e.PermissionId).HasColumnName("permission_id");
            entity.Property(e => e.PermissionName)
                .HasMaxLength(255)
                .HasColumnName("permission_name");
            entity.Property(e => e.RoleId).HasColumnName("role_id");

            entity.HasOne(d => d.Role).WithMany(p => p.Permissions)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK__Permissio__role___4BAC3F29");
        });

        modelBuilder.Entity<SecurityMonitoring>(entity =>
        {
            entity.HasKey(e => e.SecurityEventId).HasName("PK__Security__9A37EC7638F66519");

            entity.ToTable("Security_Monitoring");

            entity.Property(e => e.SecurityEventId).HasColumnName("security_event_id");
            entity.Property(e => e.EventDescription).HasColumnName("event_description");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.SecurityMonitorings)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Security___user___48CFD27E");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__B9BE370F0FF8440B");

            entity.HasIndex(e => e.Email, "UQ__Users__AB6E6164B7D5C885").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.Address).HasColumnName("address");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email");
            entity.Property(e => e.FirstName)
                .HasMaxLength(255)
                .HasColumnName("first_name");
            entity.Property(e => e.LastName)
                .HasMaxLength(255)
                .HasColumnName("last_name");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(20)
                .HasColumnName("phone_number");
            entity.Property(e => e.RoleId).HasColumnName("role_id");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK__Users__role_id__276EDEB3");
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__User_Rol__760965CC8C576366");

            entity.ToTable("User_Roles");

            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.RoleName)
                .HasMaxLength(50)
                .HasColumnName("role_name");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
