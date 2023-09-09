using System;
using System.Collections.Generic;

namespace AppnetworkCEO2023.Models;

public partial class ReferalRemittance
{
    public int RemittanceId { get; set; }

    public string RefererId { get; set; } = null!;

    public string SubscriberId { get; set; } = null!;

    public decimal Amount { get; set; }

    public DateTime RemittedDate { get; set; }
}
