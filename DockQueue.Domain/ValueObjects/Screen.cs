namespace DockQueue.Domain.ValueObjects
{
	// Flags permitem combinar permissões de visualização sem criar muitas linhas no banco.
	[Flags]
	public enum Screen
	{
		None = 0,
		Dashboard = 1 << 0,
		QueueView = 1 << 1,
		BoxesView = 1 << 2,
		StatusAdmin = 1 << 3,
		Settings = 1 << 4,
		Reports = 1 << 5,

		// Atalho para “tudo liberado” — útil para ADMIN.
		All = Dashboard | QueueView | BoxesView | StatusAdmin | Settings | Reports
	}
}
