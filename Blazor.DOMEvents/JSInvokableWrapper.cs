using Microsoft.JSInterop;

namespace Blazor.DOMEvents;

internal class JSInvokableWrapper<T>
{
    private Func<T, Task> Func { get; }

    public DotNetObjectReference<JSInvokableWrapper<T>> CreateReference()
    {
        return DotNetObjectReference.Create(this);
    }

    public JSInvokableWrapper(
        Func<T, Task> func)
    {
        Func = func;
    }

    [JSInvokable]
    public async Task Invoke(T argument)
    {
        await Func.Invoke(argument);
    }
}