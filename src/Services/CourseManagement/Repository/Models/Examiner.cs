using System;
using System.Collections.Generic;

namespace Repository.Models;

public partial class Examiner
{
    public long Id { get; set; }

    public string? FullName { get; set; }

    public string? Email { get; set; }
}
