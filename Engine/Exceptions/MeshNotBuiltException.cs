namespace Engine.Exceptions;
internal class MeshNotBuiltException : Exception
{
    public MeshNotBuiltException()
        : base("The mesh has not been built.")
    {
    }
}
