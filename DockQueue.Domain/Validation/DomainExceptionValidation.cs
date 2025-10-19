namespace DockQueue.Domain.Validation

{
    public class DomainExceptionValidation : Exception
    {
        public DomainExceptionValidation(string error) : base(error)
        {
        }
        public static void When(bool hasError, string error)
        {
            if (hasError)
                throw new DomainExceptionValidation(error);
        }

        public class EntityNotFoundException : Exception
        {
            public EntityNotFoundException() { }

            public EntityNotFoundException(string message) : base(message) { }

            public EntityNotFoundException(string message, Exception innerException) : base(message, innerException) { }
        }
    }

}
