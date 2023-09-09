using System;
using System.Collections.Generic;

namespace AppnetworkCEO2023.Models;

public partial class CeoMentor
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Surname { get; set; } = null!;

    public string Othernames { get; set; } = null!;

    public DateTime Dob { get; set; }

    public string Country { get; set; } = null!;

    public string City { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string Address { get; set; } = null!;

    public virtual SubscriberRegister IdNavigation { get; set; } = null!;
}
