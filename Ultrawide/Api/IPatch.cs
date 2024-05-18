namespace Ultrawide.Api;

internal interface IPatch : IConfigurable
{
    /// <summary>
    /// The patch itself
    /// </summary>
    public void Commit();
}
