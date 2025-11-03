using DockQueue.Domain.Validation;
using DockQueue.Domain.ValueObjects;

public class SystemSettings
{
    public int Id { get; private set; } = 1;

    public OperatingDays OperatingDays { get; private set; }

    public TimeOnly? StartTime { get; private set; } // agora nullable
    public TimeOnly? EndTime { get; private set; } // agora nullable

    public string TimeZone { get; private set; } = "America/Sao_Paulo";
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    protected SystemSettings() { }

    public SystemSettings(OperatingDays days, TimeOnly? start, TimeOnly? end, string timeZone, DateTime nowUtc)
    {
        Validate(days, start, end, timeZone);
        OperatingDays = days;
        StartTime = start;
        EndTime = end;
        TimeZone = timeZone;
        CreatedAt = nowUtc;
        UpdatedAt = nowUtc;
    }

    public void Update(OperatingDays days, TimeOnly? start, TimeOnly? end, string timeZone, DateTime nowUtc)
    {
        Validate(days, start, end, timeZone);
        OperatingDays = days;
        StartTime = start;
        EndTime = end;
        TimeZone = timeZone;
        UpdatedAt = nowUtc;
    }

    private static void Validate(OperatingDays days, TimeOnly? start, TimeOnly? end, string timeZone)
    {
        DomainExceptionValidation.When(days == OperatingDays.None, "Selecione ao menos um dia de operação");
        if (start.HasValue && end.HasValue)
            DomainExceptionValidation.When(start.Value >= end.Value, "Horário inicial deve ser menor que o final");
        DomainExceptionValidation.When(string.IsNullOrWhiteSpace(timeZone), "TimeZone é obrigatório");
        DomainExceptionValidation.When(timeZone.Length > 100, "TimeZone muito longo");
    }
}
