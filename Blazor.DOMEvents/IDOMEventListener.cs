using Microsoft.JSInterop;

namespace Blazor.DOMEvents;

public interface IDomEventListener : IAsyncDisposable
{
    Guid Guid { get; }
    string Type { get; }
}