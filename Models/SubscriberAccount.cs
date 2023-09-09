using System;
using System.Collections.Generic;

namespace AppnetworkCEO2023.Models;

public partial class SubscriberAccount
{
    public int Id { get; set; }

    public string? AccountName { get; set; }

    public string AccoutNumber { get; set; } = null!;

    public string BankName { get; set; } = null!;

    public virtual SubscriberRegister IdNavigation { get; set; } = null!;
}
