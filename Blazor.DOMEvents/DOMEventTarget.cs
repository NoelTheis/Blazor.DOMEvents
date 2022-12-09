using Microsoft.JSInterop;
using System.Reflection;
using System.Runtime.Serialization;

namespace Blazor.DOMEvents;

public class DOMEventTarget : IAsyncDisposable
{
    private readonly IJSObjectReference targetElementRef;
    private readonly IJSObjectReference module;

    private volatile bool disposed;

    private Dictionary<Guid, IDomEventListener> Listeners { get; } = new();

    public DOMEventTarget(
        IJSObjectReference module, 
        IJSObjectReference targetElementRef)
    {
        this.targetElementRef = targetElementRef;
        this.module = module;
    }

    /// <summary>
    /// Like <see cref="AddEventListener{TEventArgs}(string, TEventArgs)"/> but tries to build <br/>
    /// the template of type<typeparamref name="TEventArgs"/> through <see cref="FormatterServices.GetUninitializedObject"/>.
    /// </summary>
    /// <remarks>
    /// Use <see cref="AddEventListener{TEventArgs}(string, TEventArgs)"/> if the template creation fails.
    /// </remarks>
    public ValueTask<DomEventListener<TEventArgs>> AddEventListener<TEventArgs>(
        string eventType) where TEventArgs : EventArgs
    {
        var ctor = typeof(TEventArgs).GetConstructor(BindingFlags.Public | BindingFlags.Instance, Type.EmptyTypes);

        if (ctor is null)
            throw new Exception($"No parameterless public constructor for type {typeof(TEventArgs).FullName} found, consider providing a template");

        var instance = ctor.Invoke(null);

        return AddEventListener(eventType, (TEventArgs)instance);
    }

    /// <summary>
    /// Adds an event listener to its EventTarget by invoking AddEventListener.
    /// </summary>
    /// <returns>
    /// A <see cref="DomEventListener{TEventArgs}"/> that invokes <see cref="DomEventListener{TEventArgs}.EventRaised"/>
    /// when the <paramref name="eventType"/> is invoked.
    /// </returns>
    public async ValueTask<DomEventListener<TEventArgs>> AddEventListener<TEventArgs>(
        string eventType,
        TEventArgs template) where TEventArgs : EventArgs
    {
        var converter = new EventArgsConverter<TEventArgs>();
        var wrapper = new JSInvokableWrapper<string>(converter.Convert);
        var dotNetRef = wrapper.CreateReference();

        var jsListenerRef = await module.InvokeAsync<IJSObjectReference>(
            "addEventListener",
            new object?[]
            {
                eventType,
                template,
                dotNetRef
            });

        var listener = new DomEventListener<TEventArgs>(
            module,
            targetElementRef,
            jsListenerRef,
            eventType,
            converter);

        Listeners.Add(listener.Guid, listener);

        return listener;
    }

    /// <summary>
    /// Disposes this <see cref="DOMEventTarget"/>s <see cref="IDomEventListener"/>s.
    /// </summary>
    private async Task DisposeListeners()
    {
        foreach (var listener in Listeners.Values)
        {
            await listener.DisposeAsync();
            Listeners.Remove(listener.Guid);
        }            
    }

    /// <summary>
    /// Disposes this <see cref="DOMEventTarget"/>s <see cref="IDomEventListener"/>s.
    /// Then disposes the <see cref="IJSObjectReference"/> to its EventTarget.
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        if (disposed)
            return;
        disposed = true;

        await DisposeListeners();
        await targetElementRef.DisposeAsync();
    }
} 
