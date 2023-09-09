using System;
using System.Collections.Generic;

namespace AppnetworkCEO2023.Models;

public partial class SubcriberBonu
{
    public int Id { get; set; }

    public string RefererCode { get; set; } = null!;

    public bool CeoBonus { get; set; }

    public int Amount { get; set; }

    public DateTime BonusDate { get; set; }

    public virtual SubscriberRegister IdNavigation { get; set; } = null!;
}
