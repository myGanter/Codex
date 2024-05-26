namespace CodexCQRS.Exceptions
{
    public class DispatchException : Exception
    {
        public DispatchException(string text) : base(text) 
        { }
    }
}
