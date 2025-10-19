namespace DockQueue.Application.DTOs
{
	public class StatusDto
	{
		public int Id { get; set; }
		public string Code { get; set; } = default!;
		public string Name { get; set; } = default!;
		public string? Description { get; set; }
		public int DisplayOrder { get; set; }
		public bool IsDefault { get; set; }
		public bool IsTerminal { get; set; }
		public bool Active { get; set; }
		public DateTime CreatedAt { get; set; }
	}

	public class CreateStatusDto
	{
		public string Code { get; set; } = default!;
		public string Name { get; set; } = default!;
		public string? Description { get; set; }
		public int DisplayOrder { get; set; } = 0;
		public bool IsDefault { get; set; } = false;
		public bool IsTerminal { get; set; } = false;
		public bool Active { get; set; } = true;
	}

	public class UpdateStatusDto
	{
		public string Code { get; set; } = default!;
		public string Name { get; set; } = default!;
		public string? Description { get; set; }
		public int DisplayOrder { get; set; }
		public bool IsDefault { get; set; }
		public bool IsTerminal { get; set; }
		public bool Active { get; set; }
	}
}
