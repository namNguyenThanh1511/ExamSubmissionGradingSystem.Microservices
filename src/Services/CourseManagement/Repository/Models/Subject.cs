using System;
using System.Collections.Generic;

namespace Repository.Models;

public partial class Subject
{
    public long Id { get; set; }

    public string? Code { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<Exam> Exams { get; set; } = new List<Exam>();
}
