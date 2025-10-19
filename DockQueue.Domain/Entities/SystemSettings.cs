using DockQueue.Domain.Validation;
using DockQueue.Domain.ValueObjects;

namespace DockQueue.Domain.Entities
{
    public class SystemSettings
    {
        // Projeto inteiro usa uma única linha de configuração
        public int Id { get; private set; } = 1;

        public OperatingDays OperatingDays { get; private set; }  // flags
        public TimeOnly StartTime { get; private set; }           // hora inicial
        public TimeOnly EndTime { get; private set; }             // hora final (mesmo dia)
        public string TimeZone { get; private set; } = "America/Sao_Paulo";

        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }

        protected SystemSettings() { }

        public SystemSettings(OperatingDays days, TimeOnly start, TimeOnly end, string timeZone, DateTime nowUtc)
        {
            Validate(days, start, end, timeZone);
            OperatingDays = days;
            StartTime = start;
            EndTime = end;
            TimeZone = timeZone;
            CreatedAt = nowUtc;
            UpdatedAt = nowUtc;
        }

        public void Update(OperatingDays days, TimeOnly start, TimeOnly end, string timeZone, DateTime nowUtc)
        {
            Validate(days, start, end, timeZone);
            OperatingDays = days;
            StartTime = start;
            EndTime = end;
            TimeZone = timeZone;
            UpdatedAt = nowUtc;
        }

        private static void Validate(OperatingDays days, TimeOnly start, TimeOnly end, string timeZone)
        {
            DomainExceptionValidation.When(days == OperatingDays.None, "Selecione ao menos um dia de operação");
            DomainExceptionValidation.When(start >= end, "Horário inicial deve ser menor que o final");
            DomainExceptionValidation.When(string.IsNullOrWhiteSpace(timeZone), "TimeZone é obrigatório");
            DomainExceptionValidation.When(timeZone.Length > 100, "TimeZone muito longo");
        }
    }
}
