using System;
using System.Collections.Generic;

namespace HotelBooking.Models;

public partial class Amenity
{
    public int AmenityId { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public bool? IsActive { get; set; }

    public virtual ICollection<RoomType> RoomTypes { get; set; } = new List<RoomType>();
}
