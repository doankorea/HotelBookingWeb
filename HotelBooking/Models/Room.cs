using System;
using System.Collections.Generic;

namespace HotelBooking.Models;

public partial class Room
{
    public int RoomId { get; set; }

    public string? RoomName { get; set; }

    public int? RoomTypeId { get; set; }

    public decimal? Price { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? Image { get; set; }

    public int? CountryId { get; set; }

    public virtual Country? Country { get; set; }

    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();

    public virtual RoomType? RoomType { get; set; }
}
