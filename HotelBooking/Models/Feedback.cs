﻿using System;
using System.Collections.Generic;

namespace HotelBooking.Models;

public partial class Feedback
{
    public int FeedbackId { get; set; }

    public int? ReservationId { get; set; }

    public int? Rating { get; set; }

    public string? Comment { get; set; }

    public DateTime? FeedbackDate { get; set; }

    public virtual Reservation? Reservation { get; set; }
}
