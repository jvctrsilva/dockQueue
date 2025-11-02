namespace DockQueue.Domain.ValueObjects
{
	// Flags permitem combinar permissões de visualização sem criar muitas linhas no banco.
	[Flags]
	public enum Screen
	{
		None = 0,
		UsersView = 1 << 0,
		QueueView = 1 << 1,
		BoxesView = 1 << 2,
		StatusView = 1 << 3,
		SettingsView = 1 << 4,
		PermissionsView = 1 << 5,

        // Atalho para “tudo liberado” — útil para ADMIN.
        All = UsersView | QueueView | BoxesView | StatusView | SettingsView | PermissionsView
    }
}
