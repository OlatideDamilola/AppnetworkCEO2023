using System;
using System.Collections.Generic;

namespace AppnetworkCEO2023.Models;

public partial class ShareholderBonu
{
    public int Id { get; set; }

    public string RefererCode { get; set; } = null!;

    public int Amount { get; set; }

    public DateTime BonusDate { get; set; }

    public string? JsonData { get; set; }

    public virtual SubscriberRegister IdNavigation { get; set; } = null!;
}
