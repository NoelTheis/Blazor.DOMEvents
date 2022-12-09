using System.Text.Json;

namespace Blazor.DOMEvents;

internal class EventArgsConverter<TEventArgs>
{
    public Func<TEventArgs, Task>? ResultHandler { get; set; }

    public async Task Convert(string stringifiedEventArgs)
    {
        if (ResultHandler is null)
            return;

        var deserialized = JsonSerializer.Deserialize<TEventArgs>(
            stringifiedEventArgs,
            new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true
            });

        if (deserialized is null)
            throw new Exception();

        await ResultHandler(deserialized);
    }
}
