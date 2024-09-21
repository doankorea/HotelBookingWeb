using System;
using System.Collections.Generic;

namespace HotelBooking.Models;

public partial class State
{
    public int StateId { get; set; }

    public string? StateName { get; set; }

    public int? CountryId { get; set; }

    public virtual Country? Country { get; set; }
}
