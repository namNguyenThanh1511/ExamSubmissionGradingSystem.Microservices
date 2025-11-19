using System;
using System.Collections.Generic;

namespace Repository.Models;

public partial class Semester
{
    public long Id { get; set; }

    public string? Name { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public virtual ICollection<Exam> Exams { get; set; } = new List<Exam>();
}
