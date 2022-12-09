using Microsoft.JSInterop;

namespace Blazor.DOMEvents;

public class DOMEventManager : IAsyncDisposable
{
    private readonly Lazy<Task<IJSObjectReference>> moduleTask;

    public Lazy<Task<DOMEventTarget>> Document { get; }
    public Lazy<Task<DOMEventTarget>> Window { get; }

    public DOMEventManager(IJSRuntime jsRuntime)
    {
        moduleTask = new(() => jsRuntime.InvokeAsync<IJSObjectReference>(
            "import", "./_content/Blazor.DOMEvents/BlazorDomEvents.js").AsTask());

        Document = new(async () =>
        {
            var module = await moduleTask.Value;
            var document = await module.InvokeAsync<IJSObjectReference>("getDocument");
            return new(module, document);
        });

        Window = new(async () =>
        {
            var module = await moduleTask.Value;
            var window = await module.InvokeAsync<IJSObjectReference>("getWindow");
            return new(module, window);
        });
    }

    public async ValueTask DisposeAsync()
    {
        if (Document.IsValueCreated)
        {
            var document = await Document.Value;
            await document.DisposeAsync();
        }

        if (Window.IsValueCreated)
        {
            var window = await Window.Value;
            await window.DisposeAsync();
        }

        if (moduleTask.IsValueCreated)
        {
            var module = await moduleTask.Value;
            await module.DisposeAsync();
        }
    }
}