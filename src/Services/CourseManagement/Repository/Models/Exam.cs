using System;
using System.Collections.Generic;

namespace Repository.Models;

public partial class Exam
{
    public long Id { get; set; }

    public long? SubjectId { get; set; }

    public long? SemesterId { get; set; }

    public string? Title { get; set; }

    public string? Status { get; set; }

    public DateTime? StartTime { get; set; }

    public DateTime? EndTime { get; set; }

    public virtual ICollection<Rubric> Rubrics { get; set; } = new List<Rubric>();

    public virtual Semester? Semester { get; set; }

    public virtual Subject? Subject { get; set; }

    public virtual ICollection<Submission> Submissions { get; set; } = new List<Submission>();
}
