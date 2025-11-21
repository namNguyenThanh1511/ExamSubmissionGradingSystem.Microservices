using System;
using System.Collections.Generic;

namespace Repository.Models;

public partial class CriterionScore
{
    public long Id { get; set; }

    public long SubmissionId { get; set; }

    public long RubricCriterionId { get; set; }

    public double? Score { get; set; }

    public string? Comment { get; set; }

    public virtual RubricCriterion RubricCriterion { get; set; } = null!;

    public virtual Submission Submission { get; set; } = null!;
}
