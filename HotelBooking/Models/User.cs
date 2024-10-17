using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HotelBooking.Models;

public partial class User
{
    public int UserId { get; set; }
    [Required]
    [EmailAddress]
    [StringLength(100)]
    [Remote(action: "VerifyEmail", controller: "Access")]
    public string? Email { get; set; }

    public string? PasswordHash { get; set; }

    public string? Role { get; set; }

    public DateTime? CreatedAt { get; set; }

    public bool IsActive { get; set; }
    public int? HotelID { get; set; }
    public virtual Hotel? Hotel { get; set; }
    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}
