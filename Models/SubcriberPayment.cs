using System;
using System.Collections.Generic;

namespace AppnetworkCEO2023.Models;

public partial class SubcriberPayment
{
    public int Id { get; set; }

    public string ReferralCode { get; set; } = null!;

    public string? CeoPaymentJson { get; set; }

    public string? ShareholderPaymentJson { get; set; }

    public DateTime CeoPaymentDate { get; set; }

    public DateTime ShareholderPaymentDate { get; set; }

    public bool? CeoIsconfrimed { get; set; }

    public bool? ShareholderIsconfrimed { get; set; }

    public int Amount { get; set; }

    public string? ShareholderTransacRefId { get; set; }

    public string? CeoTransacRefId { get; set; }

    public virtual SubscriberRegister IdNavigation { get; set; } = null!;
}
