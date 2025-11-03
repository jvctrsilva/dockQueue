using DockQueue.Client.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using DockQueue.Client.Shared;


namespace DockQueue.Client.ViewModels
{
    public class OperatingScheduleViewModel
    {
        private readonly SystemSettingsApi _api;
        private readonly IAuthorizationService _authz;
        private readonly AuthenticationStateProvider _auth;

        public OperatingScheduleViewModel(SystemSettingsApi api, AuthenticationStateProvider auth, IAuthorizationService authz)
        {
            _api = api;
            _auth = auth;
            _authz = authz;
        }

        public bool IsLoading { get; private set; }
        public string? Error { get; private set; }

        public SettingsDto? Current { get; private set; }
        public UpdateSettingsDto Edit { get; private set; } = new();

        public List<DayItem> Days { get; private set; } = new();

        public bool CanEdit { get; private set; }

        public async Task LoadAsync()
        {
            try
            {
                IsLoading = true;
                Error = null;

                var state = await _auth.GetAuthenticationStateAsync();
                var user = state.User;
                var res = await _authz.AuthorizeAsync(user, null, "Screen:SettingsEdit");
                CanEdit = res.Succeeded || user.IsInRole("Admin");

                Current = await _api.GetAsync();

                if (Current is null)
                {
                    Edit = new UpdateSettingsDto
                    {
                        OperatingDays = OperatingDays.Weekdays,
                        StartTime = null,
                        EndTime = null,
                        TimeZone = "America/Sao_Paulo"
                    };
                }
                else
                {
                    Edit = new UpdateSettingsDto
                    {
                        OperatingDays = Current.OperatingDays,
                        StartTime = Current.StartTime,
                        EndTime = Current.EndTime,
                        TimeZone = Current.TimeZone
                    };
                }


                BuildDaysListFromFlags(Edit.OperatingDays);
            }
            catch (Exception ex) { Error = ex.Message; }
            finally { IsLoading = false; }
        }

        public async Task SaveAsync()
        {
            try
            {
                IsLoading = true;
                Error = null;

                Edit.OperatingDays = ComposeFlagsFromDays();

                var saved = await _api.UpsertAsync(Edit);

                Current = saved;
                BuildDaysListFromFlags(saved.OperatingDays);
                Edit.StartTime = saved.StartTime;
                Edit.EndTime = saved.EndTime;
                Edit.TimeZone = saved.TimeZone;
            }
            catch (Exception ex) { Error = ex.Message; }
            finally { IsLoading = false; }
        }

        private void BuildDaysListFromFlags(OperatingDays flags)
        {
            Days = new List<DayItem>
            {
                new("Seg", OperatingDays.Monday,    Has(flags, OperatingDays.Monday)),
                new("Ter", OperatingDays.Tuesday,   Has(flags, OperatingDays.Tuesday)),
                new("Qua", OperatingDays.Wednesday, Has(flags, OperatingDays.Wednesday)),
                new("Qui", OperatingDays.Thursday,  Has(flags, OperatingDays.Thursday)),
                new("Sex", OperatingDays.Friday,    Has(flags, OperatingDays.Friday)),
                new("Sáb", OperatingDays.Saturday,  Has(flags, OperatingDays.Saturday)),
                new("Dom", OperatingDays.Sunday,    Has(flags, OperatingDays.Sunday))
            };
        }


        private OperatingDays ComposeFlagsFromDays()
            => Days.Where(d => d.Selected).Aggregate(OperatingDays.None, (acc, d) => acc | d.Flag);

        private static bool Has(OperatingDays value, OperatingDays flag) => (value & flag) == flag;

        public class DayItem
        {
            public string Label { get; }
            public OperatingDays Flag { get; }
            public bool Selected { get; set; }
            public DayItem(string label,OperatingDays flag, bool selected)
            { Label = label; Flag = flag; Selected = selected; }
        }
    }
}
