using System;
using System.Collections.Generic;

namespace Repository.Models;

public partial class Submission
{
    public long Id { get; set; }

    public long? ExamId { get; set; }

    public string? StudentCode { get; set; }

    public string? FileUrl { get; set; }

    public string? Status { get; set; }

    public double? TotalScore { get; set; }

    public virtual Exam? Exam { get; set; }

    public virtual ICollection<Violation> Violations { get; set; } = new List<Violation>();
}
