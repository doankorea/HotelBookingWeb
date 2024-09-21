using System;
using System.Collections.Generic;

namespace HotelBooking.Models;

public partial class Country
{
    public int CountryId { get; set; }

    public string? CountryName { get; set; }

    public string? CountryCode { get; set; }

    public virtual ICollection<Room> Rooms { get; set; } = new List<Room>();

    public virtual ICollection<State> States { get; set; } = new List<State>();
}
