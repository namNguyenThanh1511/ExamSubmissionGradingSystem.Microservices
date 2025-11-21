using System;
using System.Collections.Generic;

namespace Repository.Models;

public partial class RubricCriterion
{
    public long Id { get; set; }

    public long? RubricId { get; set; }

    public string? CriterionName { get; set; }

    public double? MaxScore { get; set; }

    public virtual ICollection<CriterionScore> CriterionScores { get; set; } = new List<CriterionScore>();

    public virtual Rubric? Rubric { get; set; }
}
