namespace Todo.Web.Services;

public interface ILoadingService
{
    bool IsLoading { get; set; }

    event Action<bool>? OnChange;
}

public class LoadingService : ILoadingService
{
    private bool _isLoading;
    public bool IsLoading { get => _isLoading; set { _isLoading = value; NotifyStateChanged(value); } }

    public event Action<bool>? OnChange;

    private void NotifyStateChanged(bool isLoading) => OnChange?.Invoke(isLoading);
}
