using System.Text.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using TipTapBlazor.Models;

namespace TipTapBlazor;

/// <summary>
/// A Blazor component wrapping the TipTap rich-text editor with a configurable toolbar.
/// Supports two-way binding via <see cref="Value"/>/<see cref="ValueChanged"/>.
/// </summary>
public partial class TipTapEditor : ComponentBase, IAsyncDisposable
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    [Inject] private IJSRuntime JsRuntime { get; set; } = default!;

    /// <summary>The HTML content of the editor. Supports two-way binding via @bind-Value.</summary>
    [Parameter] public string Value { get; set; } = string.Empty;

    /// <summary>Callback fired when the editor content changes.</summary>
    [Parameter] public EventCallback<string> ValueChanged { get; set; }

    /// <summary>When true, hides the toolbar and disables editing.</summary>
    [Parameter] public bool ReadOnly { get; set; }

    /// <summary>Placeholder text shown when the editor is empty. Overrides <see cref="EditorOptions.Placeholder"/>.</summary>
    [Parameter] public string? Placeholder { get; set; }

    /// <summary>Configuration options controlling which editor features are enabled.</summary>
    [Parameter] public EditorOptions Options { get; set; } = new();

    /// <summary>Async function called when the user types an @mention query. Return matching items.</summary>
    [Parameter] public Func<string, Task<IEnumerable<MentionItem>>>? MentionSearchFunc { get; set; }

    /// <summary>Callback fired when a mention item is selected from the dropdown.</summary>
    [Parameter] public EventCallback<MentionItem> OnMentionClicked { get; set; }

    /// <summary>
    /// Optional callback to provide a link URL. Called when the link button is clicked.
    /// Should return a URL string, or null to cancel. If not set, a browser prompt is used as fallback.
    /// </summary>
    [Parameter] public Func<Task<string?>>? OnLinkUrlRequested { get; set; }

    /// <summary>
    /// Optional callback to provide an image URL. Called when the image button is clicked.
    /// Should return an image URL string, or null to cancel. If not set, a browser prompt is used as fallback.
    /// </summary>
    [Parameter] public Func<Task<string?>>? OnImageUrlRequested { get; set; }

    /// <summary>Height of the editor content area. Default: "300px".</summary>
    [Parameter] public string Height { get; set; } = "300px";

    /// <summary>Additional CSS class(es) applied to the editor wrapper element.</summary>
    [Parameter] public string? CssClass { get; set; }

    /// <summary>Captures unmatched HTML attributes and applies them to the wrapper element.</summary>
    [Parameter(CaptureUnmatchedValues = true)] public Dictionary<string, object>? UserAttributes { get; set; }

    private ElementReference _editorElement;
    private TipTapInterop? _interop;
    private DotNetObjectReference<TipTapEditor>? _dotNetRef;
    private readonly string _editorId = $"ttp-{Guid.NewGuid():N}";
    private Dictionary<string, bool> _activeFormats = new();
    private ToolbarFeatures _toolbarFeatures = new();
    private bool _showTableMenu;
    private string _textColor = "#000000";
    private string _currentFontFamily = "";
    private int _characterCount;
    private bool _initialized;
    private string _currentValue = string.Empty;
    private bool _lastReadOnly;

    private string HeightStyle => string.IsNullOrEmpty(Height) ? "" : $"height: {Height}";
    private string _readOnlyClass => ReadOnly ? "ttp-readonly" : "";

    private string CurrentHeadingLevel =>
        IsActive("heading1") ? "1" :
        IsActive("heading2") ? "2" :
        IsActive("heading3") ? "3" :
        IsActive("heading4") ? "4" : "0";

    /// <inheritdoc />
    protected override async Task OnParametersSetAsync()
    {
        _toolbarFeatures = ToolbarFeatures.FromOptions(Options);

        if (!_initialized || _interop is null) return;

        if (ReadOnly != _lastReadOnly)
        {
            _lastReadOnly = ReadOnly;
            await _interop.SetEditableAsync(_editorElement, !ReadOnly);
        }

        if (Value != _currentValue)
        {
            _currentValue = Value;
            await _interop.SetContentAsync(_editorElement, Value);
        }
    }

    /// <inheritdoc />
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _interop = new TipTapInterop(JsRuntime);
            _dotNetRef = DotNetObjectReference.Create(this);

            _currentValue = Value;
            _lastReadOnly = ReadOnly;

            var options = new
            {
                initialContent = Value,
                readOnly = ReadOnly,
                placeholder = Placeholder ?? Options.Placeholder,
                maxLength = Options.MaxLength,
                enableBold = Options.EnableBold,
                enableItalic = Options.EnableItalic,
                enableUnderline = Options.EnableUnderline,
                enableStrike = Options.EnableStrike,
                enableCode = Options.EnableCode,
                enableSubscript = Options.EnableSubscript,
                enableSuperscript = Options.EnableSuperscript,
                enableBlockquote = Options.EnableBlockquote,
                enableBulletList = Options.EnableBulletList,
                enableOrderedList = Options.EnableOrderedList,
                enableTaskList = Options.EnableTaskList,
                enableCodeBlock = Options.EnableCodeBlock,
                enableHorizontalRule = Options.EnableHorizontalRule,
                enableHeading = Options.EnableHeading,
                enableTextAlign = Options.EnableTextAlign,
                enableColor = Options.EnableColor,
                enableHighlight = Options.EnableHighlight,
                enableFontFamily = Options.EnableFontFamily,
                enableLink = Options.EnableLink,
                enableImage = Options.EnableImage,
                enableTable = Options.EnableTable,
                enableMention = Options.EnableMention,
                enableCharacterCount = Options.EnableCharacterCount,
                enablePlaceholder = Options.EnablePlaceholder,
                enableUndoRedo = Options.EnableUndoRedo,
            };

            var optionsJson = JsonSerializer.Serialize(options, JsonOptions);
            await _interop.CreateAsync(_editorElement, optionsJson, _dotNetRef);
            _initialized = true;
        }
    }

    /// <summary>Called from JS when the editor content changes. Do not call directly.</summary>
    [JSInvokable]
    public async Task OnContentChanged(string html)
    {
        _currentValue = html;
        await ValueChanged.InvokeAsync(html);
    }

    /// <summary>Called from JS when the character count changes. Do not call directly.</summary>
    [JSInvokable]
    public void OnCharacterCountChanged(int count)
    {
        _characterCount = count;
        StateHasChanged();
    }

    /// <summary>Called from JS to search for mention items. Do not call directly.</summary>
    [JSInvokable]
    public async Task<string> OnMentionSearch(string query)
    {
        if (MentionSearchFunc is null)
            return "[]";

        var items = await MentionSearchFunc(query);
        return JsonSerializer.Serialize(items, JsonOptions);
    }

    /// <summary>Called from JS when a mention item is selected. Do not call directly.</summary>
    [JSInvokable]
    public async Task OnMentionSelected(string mentionItemJson)
    {
        var item = JsonSerializer.Deserialize<MentionItem>(mentionItemJson, JsonOptions);

        if (item is not null)
        {
            await OnMentionClicked.InvokeAsync(item);
        }
    }

    /// <summary>Called from JS when the text selection or active formats change. Do not call directly.</summary>
    [JSInvokable]
    public void OnSelectionChanged(string activeFormatsJson)
    {
        using var doc = JsonDocument.Parse(activeFormatsJson);
        var root = doc.RootElement;

        var formats = new Dictionary<string, bool>();
        var fontFamily = "";

        foreach (var prop in root.EnumerateObject())
        {
            if (prop.Value.ValueKind is JsonValueKind.True or JsonValueKind.False)
            {
                formats[prop.Name] = prop.Value.GetBoolean();
            }
            else if (prop.Name == "fontFamily" && prop.Value.ValueKind == JsonValueKind.String)
            {
                fontFamily = prop.Value.GetString() ?? "";
            }
        }

        if (!FormatsEqual(_activeFormats, formats) || _currentFontFamily != fontFamily)
        {
            _activeFormats = formats;
            _currentFontFamily = fontFamily;
            StateHasChanged();
        }
    }

    private static bool FormatsEqual(Dictionary<string, bool> a, Dictionary<string, bool> b)
    {
        if (a.Count != b.Count) return false;
        foreach (var (key, value) in a)
        {
            if (!b.TryGetValue(key, out var other) || value != other) return false;
        }
        return true;
    }

    private async Task ExecuteCommand(string command, object? args = null)
    {
        if (_interop is null || !_initialized) return;

        var argsJson = args is not null
            ? JsonSerializer.Serialize(args, JsonOptions)
            : null;

        await _interop.ExecuteCommandAsync(_editorElement, command, argsJson);
    }

    private async Task OnHeadingChanged(ChangeEventArgs e)
    {
        var value = e.Value?.ToString();
        if (int.TryParse(value, out var level) && level > 0)
        {
            await ExecuteCommand("setHeading", new { level });
        }
        else
        {
            await ExecuteCommand("setParagraph");
        }
    }

    private async Task OnFontFamilyChanged(ChangeEventArgs e)
    {
        var value = e.Value?.ToString();
        if (string.IsNullOrEmpty(value))
        {
            await ExecuteCommand("unsetFontFamily");
        }
        else
        {
            await ExecuteCommand("setFontFamily", new { fontFamily = value });
        }
    }

    private async Task OnTextColorChanged(ChangeEventArgs e)
    {
        var value = e.Value?.ToString();
        if (!string.IsNullOrEmpty(value))
        {
            _textColor = value;
            await ExecuteCommand("setColor", new { color = value });
        }
    }

    private async Task SetTextAlign(string align)
    {
        await ExecuteCommand("setTextAlign", new { align });
    }

    private async Task OnLinkClicked()
    {
        if (IsActive("link"))
        {
            await ExecuteCommand("unsetLink");
            return;
        }

        string? url;
        if (OnLinkUrlRequested is not null)
        {
            url = await OnLinkUrlRequested();
        }
        else
        {
            url = await JsRuntime.InvokeAsync<string?>("prompt", "Enter link URL:", "https://");
        }

        if (!string.IsNullOrWhiteSpace(url))
        {
            await ExecuteCommand("setLink", new { href = url, target = "_blank" });
        }
    }

    private async Task OnImageClicked()
    {
        string? src;
        if (OnImageUrlRequested is not null)
        {
            src = await OnImageUrlRequested();
        }
        else
        {
            src = await JsRuntime.InvokeAsync<string?>("prompt", "Enter image URL:");
        }

        if (!string.IsNullOrWhiteSpace(src))
        {
            await ExecuteCommand("setImage", new { src, alt = "" });
        }
    }

    private void ToggleTableMenu()
    {
        _showTableMenu = !_showTableMenu;
    }

    private void CloseTableMenu()
    {
        _showTableMenu = false;
    }

    private void OnTableMenuKeyDown(KeyboardEventArgs e)
    {
        if (e.Key == "Escape")
        {
            _showTableMenu = false;
        }
    }

    private async Task ExecuteTableCommand(TableCommands command)
    {
        _showTableMenu = false;
        var commandName = command switch
        {
            TableCommands.InsertTable => "insertTable",
            TableCommands.AddRowBefore => "addRowBefore",
            TableCommands.AddRowAfter => "addRowAfter",
            TableCommands.AddColumnBefore => "addColumnBefore",
            TableCommands.AddColumnAfter => "addColumnAfter",
            TableCommands.DeleteRow => "deleteRow",
            TableCommands.DeleteColumn => "deleteColumn",
            TableCommands.MergeCells => "mergeCells",
            TableCommands.SplitCell => "splitCell",
            TableCommands.DeleteTable => "deleteTable",
            _ => throw new ArgumentOutOfRangeException(nameof(command)),
        };
        await ExecuteCommand(commandName);
    }

    // Toolbar command handlers
    private Task CmdUndo() => ExecuteCommand("undo");
    private Task CmdRedo() => ExecuteCommand("redo");
    private Task CmdToggleBold() => ExecuteCommand("toggleBold");
    private Task CmdToggleItalic() => ExecuteCommand("toggleItalic");
    private Task CmdToggleUnderline() => ExecuteCommand("toggleUnderline");
    private Task CmdToggleStrike() => ExecuteCommand("toggleStrike");
    private Task CmdToggleCode() => ExecuteCommand("toggleCode");
    private Task CmdToggleSubscript() => ExecuteCommand("toggleSubscript");
    private Task CmdToggleSuperscript() => ExecuteCommand("toggleSuperscript");
    private Task CmdToggleHighlight() => ExecuteCommand("toggleHighlight");
    private Task CmdAlignLeft() => SetTextAlign("left");
    private Task CmdAlignCenter() => SetTextAlign("center");
    private Task CmdAlignRight() => SetTextAlign("right");
    private Task CmdAlignJustify() => SetTextAlign("justify");
    private Task CmdToggleBulletList() => ExecuteCommand("toggleBulletList");
    private Task CmdToggleOrderedList() => ExecuteCommand("toggleOrderedList");
    private Task CmdToggleTaskList() => ExecuteCommand("toggleTaskList");
    private Task CmdToggleBlockquote() => ExecuteCommand("toggleBlockquote");
    private Task CmdSetHorizontalRule() => ExecuteCommand("setHorizontalRule");
    private Task CmdToggleCodeBlock() => ExecuteCommand("toggleCodeBlock");
    private Task CmdInsertTable() => ExecuteTableCommand(TableCommands.InsertTable);
    private Task CmdAddRowBefore() => ExecuteTableCommand(TableCommands.AddRowBefore);
    private Task CmdAddRowAfter() => ExecuteTableCommand(TableCommands.AddRowAfter);
    private Task CmdAddColumnBefore() => ExecuteTableCommand(TableCommands.AddColumnBefore);
    private Task CmdAddColumnAfter() => ExecuteTableCommand(TableCommands.AddColumnAfter);
    private Task CmdDeleteRow() => ExecuteTableCommand(TableCommands.DeleteRow);
    private Task CmdDeleteColumn() => ExecuteTableCommand(TableCommands.DeleteColumn);
    private Task CmdMergeCells() => ExecuteTableCommand(TableCommands.MergeCells);
    private Task CmdSplitCell() => ExecuteTableCommand(TableCommands.SplitCell);
    private Task CmdDeleteTable() => ExecuteTableCommand(TableCommands.DeleteTable);

    private bool IsActive(string format)
    {
        return _activeFormats.TryGetValue(format, out var active) && active;
    }

    private string IsActiveStr(string format)
    {
        return IsActive(format) ? "true" : "false";
    }

    private string ActiveClass(string format)
    {
        return IsActive(format) ? "active" : "";
    }

    /// <summary>
    /// Programmatically get the current HTML content from the editor.
    /// </summary>
    public async ValueTask<string> GetContentAsync()
    {
        if (_interop is null || !_initialized) return string.Empty;
        return await _interop.GetContentAsync(_editorElement);
    }

    /// <summary>
    /// Programmatically set the editor content.
    /// </summary>
    public async ValueTask SetContentAsync(string html)
    {
        if (_interop is null || !_initialized) return;
        _currentValue = html;
        await _interop.SetContentAsync(_editorElement, html);
    }

    /// <summary>
    /// Programmatically focus the editor.
    /// </summary>
    public async ValueTask FocusAsync()
    {
        if (_interop is null || !_initialized) return;
        await _interop.FocusAsync(_editorElement);
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        if (_interop is not null && _initialized)
        {
            try
            {
                await _interop.DestroyAsync(_editorElement);
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

        _dotNetRef?.Dispose();

        if (_interop is not null)
        {
            try
            {
                await _interop.DisposeAsync();
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
