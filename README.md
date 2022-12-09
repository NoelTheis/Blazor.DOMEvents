# Blazor.DOMEvents

## Description
This project contains a razor class library with the goal to provide methods that allow you to easily attach event listeners to your DOM in both Blazor Server and WASM.

For most purposes, attaching event listeners directly to your components or their html elements is sufficient. But in some cases it can be beneficial to attach directly to your `document` or `window`.

## Why not do it yourself?
While it is not complicated to achieve this goal yourself, it does involve a bit of javascript and fiddling with deserialization that you should not have to deal with. Most annoyingly, the event arguments you receive in javascript can contain circular references and a lot of information that might not be relevant to you.

The approach I took was to only serialize those properties of the event argument on the javascript side that are also present in the type of `EventArgs` your're expecting on the C# side.

## Targets
    .Net 6
## Install
    PM> Install-Package Blazor.DOMEvents
## Usage
Add services.
```csharp
builder.Services.AddDOMEventManagement();
```
Inject the manager into any component.
```csharp
@using Blazor.DOMEvents

@inject DOMEventManager dom
```

Request the `document` or `window`.
```csharp
var doc = await dom.Document.Value;
var window = await dom.Window.Value;
```
Attach an event listener.
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

For the serialization to work, `AddEventListener<TEventArgs>(string eventType)` passes an instance of `TEventArgs` to the js code, with which it figures out which properties of the event arguments to keep. If `TEventArgs` posses a public parameterless constructor, the instance is created for you.

If not, you should provide an instance yourself by calling:

```AddEventListener<TEventArgs>(string eventType,TEventArgs template)```

## Demo
[WASM](https://noeltheis.github.io/Blazor.DOMEvents/)

## License
[MIT](LICENSE)