namespace Engine.Exceptions;

internal class MultipleEnginesException : Exception
{
    public MultipleEnginesException()
        : base("Only one instance of AEngineBase can be created at a time.")
    {
    }
}
