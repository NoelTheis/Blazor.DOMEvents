# Blazor.DOMEvents

## Description
This project contains a razor class library with the goal to provide methods that allow you to easily attach event listeners to your DOM in both Blazor Server and WASM.

For most purposes, attaching event listeners directly to your components or their html elements is sufficient. But in some cases it can be beneficial to attach directly to your `document` or `window`.

## Demo
[WASM](https://noeltheis.github.io/Blazor.DOMEvents/)

## Why not do it yourself?
While it is not complicated to achieve this goal yourself, it does involve a bit of javascript and fiddling with deserialization that you should not have to deal with. Most annoyingly, the event arguments you receive in javascript can contain circular references and a lot of information that might not be relevant to you.

The approach I took was to only serialize those properties of the event argument on the javascript side that are also present in the type of `EventArgs` your're expecting on the C# side.

## Targets
    .Net 6
## Install
    PM> Install-Package Blazor.DOMEvents
## Usage
### Add service
```csharp
builder.Services.AddDOMEventManagement();
```
### Inject DOMEventManager
```csharp
@using Blazor.DOMEvents

@inject DOMEventManager dom
```
### Access event targets
`DOMEventManager` provides two instances of `DOMEventTarget` for the `document` and `window`.
```csharp
var doc = await dom.Document.Value;
var window = await dom.Window.Value;
```
### Adding event listeners
Create a `DomEventListener` by calling `AddEventListener` on an `DOMEventTarget`
```csharp
var listener = await doc.AddEventListener<KeyboardEventArgs>("keydown");
```
Add an event handler to the listener.
```csharp
listener.EventRaised += async (s, e) =>
{
    KeyPressed = e.Key;
    await InvokeAsync(StateHasChanged);
};
```

For the serialization to work, `AddEventListener<TEventArgs>(string eventType)` passes an instance of `TEventArgs` to the js code, with which it figures out which properties of the event arguments to keep. If `TEventArgs` posses a public parameterless constructor, the instance is created through reflection.

If this does not work, you can provide an instance yourself by calling:

`AddEventListener<TEventArgs>(string eventType,TEventArgs template)`

### Removing event listeners 

Usually your component should only need to implement `IAsyncDisposable` 
and dispose the injected `DOMEventManager`

```csharp
@implements IAsyncDisposable
...
@code {
    public async ValueTask DisposeAsync()
    {
        await dom.DisposeAsync();
    }
}
```

If you want to remove event listeners manually, simply dispose the respective `DomEventListener` instance.

#### Explication
`DOMEventManager`, `DOMEventTarget` and `DomEventListener` all implement `IAsyncDisposable`.

On `DisposeAsync`:

`DOMEventManager` will dispose its `DOMEventTarget` instances, `document` and `window`.

`DOMEventTarget` will keep track of and dispose all of its non-disposed `DomEventListener` instances.

`DomEventListener` will invoke Javascript code to call `target.removeEventListener`.



## License
[MIT](LICENSE)