using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace TipTapBlazor;

/// <summary>
/// JavaScript interop wrapper for the TipTap editor module.
/// Handles lazy module import and provides typed methods for all editor operations.
/// </summary>
public class TipTapInterop : IAsyncDisposable
{
    private readonly Lazy<Task<IJSObjectReference>> _moduleTask;

    /// <summary>Creates a new interop instance using the specified JS runtime.</summary>
    public TipTapInterop(IJSRuntime jsRuntime)
    {
        _moduleTask = new Lazy<Task<IJSObjectReference>>(() =>
            jsRuntime.InvokeAsync<IJSObjectReference>(
                "import", "./_content/TipTapBlazor/js/tiptap-interop.js").AsTask());
    }

    internal async ValueTask CreateAsync(ElementReference element, string optionsJson, DotNetObjectReference<TipTapEditor> dotNetRef)
    {
        var module = await _moduleTask.Value;
        await module.InvokeVoidAsync("create", element, optionsJson, dotNetRef);
    }

    internal async ValueTask DestroyAsync(ElementReference element)
    {
        var module = await _moduleTask.Value;
        await module.InvokeVoidAsync("destroy", element);
    }

    internal async ValueTask<string> GetContentAsync(ElementReference element)
    {
        var module = await _moduleTask.Value;
        return await module.InvokeAsync<string>("getContent", element);
    }

    internal async ValueTask SetContentAsync(ElementReference element, string html)
    {
        var module = await _moduleTask.Value;
        await module.InvokeVoidAsync("setContent", element, html);
    }

    internal async ValueTask SetEditableAsync(ElementReference element, bool editable)
    {
        var module = await _moduleTask.Value;
        await module.InvokeVoidAsync("setEditable", element, editable);
    }

    internal async ValueTask FocusAsync(ElementReference element)
    {
        var module = await _moduleTask.Value;
        await module.InvokeVoidAsync("focus", element);
    }

    internal async ValueTask ExecuteCommandAsync(ElementReference element, string command, string? argsJson = null)
    {
        var module = await _moduleTask.Value;
        await module.InvokeVoidAsync("executeCommand", element, command, argsJson);
    }

    internal async ValueTask<string> GetActiveFormatsAsync(ElementReference element)
    {
        var module = await _moduleTask.Value;
        return await module.InvokeAsync<string>("getActiveFormats", element);
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        if (_moduleTask.IsValueCreated)
        {
            try
            {
                var module = await _moduleTask.Value;
                await module.DisposeAsync();
            }
            catch (JSDisconnectedException)
            {
                // Circuit disconnected, safe to ignore
            }
            catch (ObjectDisposedException)
            {
                // Already disposed
            }
        }
    }
}
