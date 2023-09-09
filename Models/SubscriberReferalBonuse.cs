using System;
using System.Collections.Generic;

namespace AppnetworkCEO2023.Models;

public partial class SubscriberReferalBonuse
{
    public int Id { get; set; }

    public int OwnerId { get; set; }

    public int Amount { get; set; }

    public int BonusFrom { get; set; }

    public DateTime BonusDate { get; set; }

    public virtual SubscriberRegister Owner { get; set; } = null!;
}
