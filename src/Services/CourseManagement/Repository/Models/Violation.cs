using System;
using System.Collections.Generic;

namespace Repository.Models;

public partial class Violation
{
    public long Id { get; set; }

    public long? SubmissionId { get; set; }

    public string? Type { get; set; }

    public string? Description { get; set; }

    public bool? Verified { get; set; }

    public virtual Submission? Submission { get; set; }
}
