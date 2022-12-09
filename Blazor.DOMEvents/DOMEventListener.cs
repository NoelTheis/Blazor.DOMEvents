using Microsoft.JSInterop;

namespace Blazor.DOMEvents;

public class DomEventListener<TEventArgs> : IDomEventListener where TEventArgs : EventArgs
{
    private readonly IJSObjectReference module;
    private readonly IJSObjectReference targetElementRef;
    private readonly IJSObjectReference jsListenerReference;

    private volatile bool disposed;

    public event Func<object, TEventArgs, Task>? EventRaised;
    public Guid Guid { get; } = Guid.NewGuid();
    public string Type { get; }
   
    private EventArgsConverter<TEventArgs> Converter { get; }

    internal DomEventListener(
        IJSObjectReference module,
        IJSObjectReference targetElementRef,
        IJSObjectReference jsListenerReference,
        string type,
        EventArgsConverter<TEventArgs> converter)
    {
        this.module = module;
        this.targetElementRef = targetElementRef;
        this.jsListenerReference = jsListenerReference;

        Type = type;
        Converter = converter;
        Converter.ResultHandler = EventArgsReceived;
    }

    private async Task EventArgsReceived(TEventArgs eventArgs)
    {
        if (EventRaised is not null)
            await EventRaised.Raise(this, eventArgs);
    }

    /// <summary>
    /// Removes this <see cref="IDomEventListener"/>s EventListenerOrEventListenerObject from its EventTarget by invoking removeEventListener.<br/>
    /// Then disposes the <see cref="IJSObjectReference"/> to its EventListenerOrEventListenerObject.
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        if (disposed)
            return;
        disposed = true;

        await module.InvokeVoidAsync(
            "removeEventListener",
            new object?[]
            {
                        targetElementRef,
                        Type,
                        jsListenerReference
            });

        await jsListenerReference.DisposeAsync();  
    }
}
