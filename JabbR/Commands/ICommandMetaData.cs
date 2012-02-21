
namespace JabbR.Commands
{
    public interface ICommandMetaData
    {
        string Name { get; }
        string Usage { get; }
        float Weight { get; }
    }
}