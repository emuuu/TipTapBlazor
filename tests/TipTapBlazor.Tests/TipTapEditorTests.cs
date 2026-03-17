using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using TipTapBlazor.Models;

namespace TipTapBlazor.Tests;

public class TipTapEditorTests : BunitContext
{
    private BunitJSModuleInterop _moduleInterop = default!;

    [SetUp]
    public void Setup()
    {
        _moduleInterop = JSInterop.SetupModule("./_content/TipTapBlazor/js/tiptap-interop.js");
        _moduleInterop.Mode = JSRuntimeMode.Loose;
    }

    [Test]
    public void DefaultRender_HasEditorWrapper()
    {
        var cut = Render<TipTapEditor>();

        var wrapper = cut.Find(".ttp-editor-wrapper");
        Assert.That(wrapper, Is.Not.Null);
    }

    [Test]
    public void DefaultRender_HasToolbar()
    {
        var cut = Render<TipTapEditor>();

        var toolbar = cut.Find(".ttp-toolbar");
        Assert.That(toolbar, Is.Not.Null);
    }

    [Test]
    public void DefaultRender_ToolbarHasRoleAttribute()
    {
        var cut = Render<TipTapEditor>();

        var toolbar = cut.Find(".ttp-toolbar");
        Assert.That(toolbar.GetAttribute("role"), Is.EqualTo("toolbar"));
    }

    [Test]
    public void DefaultRender_HasEditorContent()
    {
        var cut = Render<TipTapEditor>();

        var content = cut.Find(".ttp-editor-content");
        Assert.That(content, Is.Not.Null);
    }

    [Test]
    public void ReadOnly_HidesToolbar()
    {
        var cut = Render<TipTapEditor>(p => p.Add(e => e.ReadOnly, true));

        var wrapper = cut.Find(".ttp-editor-wrapper");
        Assert.That(wrapper.ClassList, Does.Contain("ttp-readonly"));
        Assert.Throws<Bunit.ElementNotFoundException>(() => cut.Find(".ttp-toolbar"));
    }

    [Test]
    public void CssClass_AppliedToWrapper()
    {
        var cut = Render<TipTapEditor>(p => p.Add(e => e.CssClass, "my-custom-class"));

        var wrapper = cut.Find(".ttp-editor-wrapper");
        Assert.That(wrapper.ClassList, Does.Contain("my-custom-class"));
    }

    [Test]
    public void Height_AppliedToEditorContent()
    {
        var cut = Render<TipTapEditor>(p => p.Add(e => e.Height, "500px"));

        var content = cut.Find(".ttp-editor-content");
        Assert.That(content.GetAttribute("style"), Does.Contain("500px"));
    }

    [Test]
    public void DisabledFeatures_HideToolbarButtons()
    {
        var options = new EditorOptions
        {
            EnableBold = false,
            EnableItalic = false,
            EnableUnderline = false,
            EnableStrike = false,
            EnableCode = false,
            EnableSubscript = false,
            EnableSuperscript = false,
        };

        var cut = Render<TipTapEditor>(p => p.Add(e => e.Options, options));

        Assert.Throws<Bunit.ElementNotFoundException>(() =>
            cut.Find("button[title='Bold']"));
    }

    [Test]
    public void CharacterCount_ShowsFooterWhenEnabled()
    {
        var options = new EditorOptions { EnableCharacterCount = true };

        var cut = Render<TipTapEditor>(p => p.Add(e => e.Options, options));

        var footer = cut.Find(".ttp-footer");
        Assert.That(footer, Is.Not.Null);
    }

    [Test]
    public void CharacterCount_HidesFooterWhenDisabled()
    {
        var options = new EditorOptions { EnableCharacterCount = false };

        var cut = Render<TipTapEditor>(p => p.Add(e => e.Options, options));

        Assert.Throws<Bunit.ElementNotFoundException>(() => cut.Find(".ttp-footer"));
    }

    [Test]
    public void TableGroup_RenderWhenEnabled()
    {
        var options = new EditorOptions { EnableTable = true };

        var cut = Render<TipTapEditor>(p => p.Add(e => e.Options, options));

        var tableButton = cut.Find("button[title='Table']");
        Assert.That(tableButton, Is.Not.Null);
    }

    [Test]
    public void EditorContent_HasUniqueId()
    {
        var cut = Render<TipTapEditor>();

        var content = cut.Find(".ttp-editor-content");
        var id = content.GetAttribute("id");
        Assert.That(id, Does.StartWith("ttp-"));
    }

    [Test]
    public void ToggleButtons_HaveAriaPressed()
    {
        var cut = Render<TipTapEditor>();

        var boldButton = cut.Find("button[aria-label='Bold']");
        Assert.That(boldButton.GetAttribute("aria-pressed"), Is.EqualTo("false"));
    }

    [Test]
    public void HeadingDropdown_HasAriaLabel()
    {
        var cut = Render<TipTapEditor>();

        var select = cut.Find("select[aria-label='Block type']");
        Assert.That(select, Is.Not.Null);
    }

    [Test]
    public void OnContentChanged_FiresValueChanged()
    {
        string? receivedValue = null;
        var cut = Render<TipTapEditor>(p => p
            .Add(e => e.Value, "")
            .Add(e => e.ValueChanged, new EventCallback<string>(null, (Action<string>)(v => receivedValue = v))));

        cut.InvokeAsync(() =>
            cut.Instance.OnContentChanged("<p>hello</p>"));

        Assert.That(receivedValue, Is.EqualTo("<p>hello</p>"));
    }

    [Test]
    public void OnCharacterCountChanged_UpdatesFooter()
    {
        var options = new EditorOptions { EnableCharacterCount = true };

        var cut = Render<TipTapEditor>(p => p.Add(e => e.Options, options));

        cut.InvokeAsync(() =>
            cut.Instance.OnCharacterCountChanged(42));

        var footer = cut.Find(".ttp-footer span");
        Assert.That(footer.TextContent, Does.Contain("42"));
    }

    [Test]
    public void OnSelectionChanged_UpdatesActiveFormats()
    {
        var cut = Render<TipTapEditor>();

        var formatsJson = """{"bold":true,"italic":false,"underline":false,"strike":false,"code":false,"subscript":false,"superscript":false,"blockquote":false,"bulletList":false,"orderedList":false,"taskList":false,"codeBlock":false,"heading1":false,"heading2":false,"heading3":false,"heading4":false,"link":false,"highlight":false,"alignLeft":false,"alignCenter":false,"alignRight":false,"alignJustify":false,"table":false,"fontFamily":""}""";

        cut.InvokeAsync(() =>
            cut.Instance.OnSelectionChanged(formatsJson));

        var boldButton = cut.Find("button[aria-label='Bold']");
        Assert.That(boldButton.ClassList, Does.Contain("active"));
    }

    [Test]
    public void OnSelectionChanged_SkipsRerenderWhenUnchanged()
    {
        var cut = Render<TipTapEditor>();

        var formatsJson = """{"bold":false,"italic":false,"fontFamily":""}""";

        // First call sets the formats
        cut.InvokeAsync(() => cut.Instance.OnSelectionChanged(formatsJson));
        var renderCount1 = cut.RenderCount;

        // Second call with same formats should not trigger re-render
        cut.InvokeAsync(() => cut.Instance.OnSelectionChanged(formatsJson));
        var renderCount2 = cut.RenderCount;

        Assert.That(renderCount2, Is.EqualTo(renderCount1));
    }

    [Test]
    public void TableMenu_ClosesOnOverlayClick()
    {
        var options = new EditorOptions { EnableTable = true };
        var cut = Render<TipTapEditor>(p => p.Add(e => e.Options, options));

        // Open table menu
        var tableButton = cut.Find("button[title='Table']");
        tableButton.Click();

        // Verify menu is open
        Assert.DoesNotThrow(() => cut.Find(".ttp-table-menu"));

        // Click overlay
        var overlay = cut.Find(".ttp-menu-overlay");
        overlay.Click();

        // Verify menu is closed
        Assert.Throws<Bunit.ElementNotFoundException>(() => cut.Find(".ttp-table-menu"));
    }

    [Test]
    public void OnMentionSearch_ReturnsSerializedItems()
    {
        var items = new List<MentionItem>
        {
            new() { Id = "1", DisplayName = "Alice", Category = "person" },
            new() { Id = "2", DisplayName = "Bob" },
        };

        var cut = Render<TipTapEditor>(p => p
            .Add(e => e.MentionSearchFunc, _ => Task.FromResult<IEnumerable<MentionItem>>(items)));

        string? result = null;
        cut.InvokeAsync(async () =>
            result = await cut.Instance.OnMentionSearch("al"));

        Assert.That(result, Is.Not.Null);
        Assert.That(result, Does.Contain("Alice"));
        Assert.That(result, Does.Contain("Bob"));
    }

    [Test]
    public void OnMentionSearch_ReturnsEmptyArrayWhenNoFunc()
    {
        var cut = Render<TipTapEditor>();

        string? result = null;
        cut.InvokeAsync(async () =>
            result = await cut.Instance.OnMentionSearch("test"));

        Assert.That(result, Is.EqualTo("[]"));
    }

    [Test]
    public void OnMentionSelected_FiresOnMentionClicked()
    {
        MentionItem? received = null;
        var cut = Render<TipTapEditor>(p => p
            .Add(e => e.OnMentionClicked, new EventCallback<MentionItem>(null, (Action<MentionItem>)(item => received = item))));

        var json = """{"id":"user-1","displayName":"Alice","category":"person"}""";

        cut.InvokeAsync(() =>
            cut.Instance.OnMentionSelected(json));

        Assert.That(received, Is.Not.Null);
        Assert.That(received!.Id, Is.EqualTo("user-1"));
        Assert.That(received.DisplayName, Is.EqualTo("Alice"));
        Assert.That(received.Category, Is.EqualTo("person"));
    }

    [Test]
    public void TableButton_HasAriaHaspopup()
    {
        var options = new EditorOptions { EnableTable = true };
        var cut = Render<TipTapEditor>(p => p.Add(e => e.Options, options));

        var tableButton = cut.Find("button[title='Table']");
        Assert.That(tableButton.GetAttribute("aria-haspopup"), Is.EqualTo("menu"));
        Assert.That(tableButton.GetAttribute("aria-expanded"), Is.EqualTo("false"));
    }

    [Test]
    public void TableMenu_ClosesOnEscapeKey()
    {
        var options = new EditorOptions { EnableTable = true };
        var cut = Render<TipTapEditor>(p => p.Add(e => e.Options, options));

        // Open table menu
        cut.Find("button[title='Table']").Click();
        Assert.DoesNotThrow(() => cut.Find(".ttp-table-menu"));

        // Press Escape
        cut.Find(".ttp-table-menu").KeyDown(key: "Escape");

        // Verify menu is closed
        Assert.Throws<Bunit.ElementNotFoundException>(() => cut.Find(".ttp-table-menu"));
    }

    [Test]
    public async Task DisposeAsync_DoesNotThrow()
    {
        var cut = Render<TipTapEditor>();

        await cut.Instance.DisposeAsync();
    }
}
