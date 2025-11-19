using System;
using System.Collections.Generic;

namespace Repository.Models;

public partial class Rubric
{
    public long Id { get; set; }

    public string? Name { get; set; }

    public long? ExamId { get; set; }

    public virtual Exam? Exam { get; set; }

    public virtual ICollection<RubricCriterion> RubricCriteria { get; set; } = new List<RubricCriterion>();
}
