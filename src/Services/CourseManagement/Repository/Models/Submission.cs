using System;
using System.Collections.Generic;

namespace Repository.Models;

public partial class Submission
{
    public long Id { get; set; }

    public long? ExamId { get; set; }

    public Guid? SubmissionId { get; set; }

    public string? StudentCode { get; set; }

    public string? Status { get; set; }

    public double? TotalScore { get; set; }

    public long? ExaminerId { get; set; }

    public virtual ICollection<CriterionScore> CriterionScores { get; set; } = new List<CriterionScore>();

    public virtual Exam? Exam { get; set; }

    public virtual Examiner? Examiner { get; set; }

    public virtual ICollection<Violation> Violations { get; set; } = new List<Violation>();
}
