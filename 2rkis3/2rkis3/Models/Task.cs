using System;
using System.Collections.Generic;

namespace _2rkis3.Models;

public partial class Task
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public DateOnly Duedata { get; set; }

    public bool? Iscompleted { get; set; }
}
