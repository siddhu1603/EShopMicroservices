namespace BuildingBlocks.Exceptions
{
    internal class InternalServerException : Exception
    {
        public InternalServerException(string message) : base(message)
        {
            
        }
        public InternalServerException(string message, string details) : base(message)
        {
            details = details;
        }
        public string? Details { get; }
    }
}
