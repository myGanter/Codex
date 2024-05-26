namespace CodexCQRS.Exceptions
{
    public class DecorateInitException : Exception
    {
        public DecorateInitException(string exception) : base(exception)
        { }
    }
}
