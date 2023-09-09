using System;
using System.Collections.Generic;

namespace AppnetworkCEO2023.Models;

public partial class SubscriberRegister
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Surname { get; set; } = null!;

    public string Firstname { get; set; } = null!;

    public string OtherName { get; set; } = null!;

    public DateTime Dob { get; set; }

    public string Country { get; set; } = null!;

    public string State { get; set; } = null!;

    public string City { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Religion { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string PictureName { get; set; } = null!;

    public DateTime RegisteredDate { get; set; }

    public string ReferralCode { get; set; } = null!;

    public bool CeomemberIsReg { get; set; }

    public bool ShareholderIsReg { get; set; }

    public bool CeomemberIsPaid { get; set; }

    public bool ShareholderIsPaid { get; set; }

    public virtual CeoMentor? CeoMentor { get; set; }

    public virtual CeoPayment? CeoPayment { get; set; }

    public virtual SharedHolderNok? SharedHolderNok { get; set; }

    public virtual ShareholdersPayment? ShareholdersPayment { get; set; }

    public virtual SubscriberAccount? SubscriberAccount { get; set; }

    public virtual ICollection<SubscriberReferalBonuse> SubscriberReferalBonuses { get; } = new List<SubscriberReferalBonuse>();
}
