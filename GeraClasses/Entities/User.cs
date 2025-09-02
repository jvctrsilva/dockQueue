using System;
using System.Collections.Generic;

namespace DockQueue.Domain.Entities;

public partial class User
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Number { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Role { get; set; } = null!;

    public DateTime CreatedAt { get; set; }
}
