using System;
using System.Collections.Generic;

namespace AppnetworkCEO2023.Models;

public partial class ShareholdersPayment
{
    public int Id { get; set; }

    public string? PaymentJson { get; set; }

    public DateTime PaymentDate { get; set; }

    public bool? Isconfirmed { get; set; }

    public int Amount { get; set; }

    public string? TransacRefId { get; set; }

    public virtual SubscriberRegister IdNavigation { get; set; } = null!;
}
