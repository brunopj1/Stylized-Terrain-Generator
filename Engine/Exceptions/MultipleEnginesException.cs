namespace Engine.Exceptions;

internal class MultipleEnginesException : Exception
{
    public MultipleEnginesException()
        : base("Only one instance of EngineBase can be created at a time.")
    {
    }
}