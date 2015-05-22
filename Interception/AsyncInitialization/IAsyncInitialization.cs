namespace AsyncInitialization.Interception.AsyncInitialization
{
    using System.Threading.Tasks;

    public interface IAsyncInitialization
    {
        Task InitializeAsync();
    }
}