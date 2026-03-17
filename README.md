# TipTapBlazor

Blazor Razor Class Library wrapping the [TipTap v3](https://tiptap.dev/) rich-text editor with a configurable toolbar, @mention support, and CSS theming via custom properties.

Targets **.NET 8**, **.NET 9**, and **.NET 10**. Works with Blazor Server, WebAssembly, and Blazor Web App (SSR with interactivity).

## Installation

```
dotnet add package TipTapBlazor
```

## Quick Start

```razor
@using TipTapBlazor

<TipTapEditor @bind-Value="_html" Placeholder="Start typing..." />

@code {
    private string _html = "";
}
```

No manual CSS `<link>` tags needed — the component's stylesheet is injected automatically via a Blazor JS initializer.

## Features

- Full toolbar: bold, italic, underline, strike, code, sub/superscript, headings (H1–H4), font family, text color, highlight, text alignment, bullet/ordered/task lists, blockquote, horizontal rule, code blocks with syntax highlighting, links, images, tables, undo/redo
- Two-way binding via `@bind-Value`
- @mention dropdown with async search
- Table editing (insert, add/delete rows and columns, merge/split cells)
- Syntax-highlighted code blocks via [Lowlight](https://github.com/wooorm/lowlight)
- Read-only mode
- Character count with optional max length
- Programmatic API (`GetContentAsync`, `SetContentAsync`, `FocusAsync`)
- Custom link/image dialogs via callback parameters
- WCAG AA accessible toolbar (ARIA attributes, focus-visible, keyboard navigation)
- Dark mode via `prefers-color-scheme`
- Full theming via `--ttp-*` CSS custom properties

## Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Value` | `string` | `""` | HTML content. Supports `@bind-Value`. |
| `ReadOnly` | `bool` | `false` | Hides toolbar and disables editing. |
| `Placeholder` | `string?` | `null` | Placeholder text when editor is empty. |
| `Options` | `EditorOptions` | all enabled | Controls which toolbar features are shown. |
| `Height` | `string` | `"300px"` | Height of the editor content area. |
| `CssClass` | `string?` | `null` | Additional CSS class(es) on the wrapper. |
| `MentionSearchFunc` | `Func<string, Task<IEnumerable<MentionItem>>>?` | `null` | Async search for @mention items. |
| `OnMentionClicked` | `EventCallback<MentionItem>` | — | Fired when a mention is selected. |
| `OnLinkUrlRequested` | `Func<Task<string?>>?` | `null` | Custom link dialog. Falls back to `window.prompt`. |
| `OnImageUrlRequested` | `Func<Task<string?>>?` | `null` | Custom image dialog. Falls back to `window.prompt`. |

## Configuring the Toolbar

Disable features you don't need via `EditorOptions`:

```razor
<TipTapEditor @bind-Value="_html" Options="@_options" />

@code {
    private string _html = "";

    private readonly EditorOptions _options = new()
    {
        EnableTable = false,
        EnableFontFamily = false,
        EnableSubscript = false,
        EnableSuperscript = false,
        EnableImage = false,
    };
}
```

Toolbar groups hide automatically when all features in the group are disabled.

## Mentions

```razor
<TipTapEditor @bind-Value="_html"
              Options="@(new EditorOptions { EnableMention = true })"
              MentionSearchFunc="Search"
              OnMentionClicked="OnSelected" />

@code {
    private string _html = "";

    private Task<IEnumerable<MentionItem>> Search(string query)
    {
        var results = _allItems
            .Where(m => m.DisplayName.Contains(query, StringComparison.OrdinalIgnoreCase))
            .Take(5);
        return Task.FromResult(results);
    }

    private void OnSelected(MentionItem item)
    {
        // handle selection
    }
}
```

Mention items support a `Category` property (`"person"`, `"place"`, `"tag"`) for visual styling via CSS.

## Character Count

```razor
<TipTapEditor @bind-Value="_html"
              Options="@(new EditorOptions { EnableCharacterCount = true, MaxLength = 500 })" />
```

Set `MaxLength` to `0` (default) for unlimited.

## Programmatic API

Capture the component reference to call methods from C#:

```razor
<TipTapEditor @ref="_editor" @bind-Value="_html" />

<button @onclick="ReadContent">Get Content</button>

@code {
    private TipTapEditor _editor = default!;
    private string _html = "";

    private async Task ReadContent()
    {
        var html = await _editor.GetContentAsync();
    }
}
```

Available methods: `GetContentAsync()`, `SetContentAsync(string html)`, `FocusAsync()`.

## Theming

Override CSS custom properties to match your app's design:

```css
:root {
    --ttp-primary: #2e7d32;
    --ttp-border: #c8e6c9;
    --ttp-toolbar-bg: #e8f5e9;
    --ttp-bg: #fff;
    --ttp-radius: 8px;
}
```

Or scope to a specific instance:

```html
<div style="--ttp-primary: #d32f2f;">
    <TipTapEditor @bind-Value="_html" />
</div>
```

<details>
<summary>All CSS custom properties</summary>

| Property | Description |
|----------|-------------|
| `--ttp-primary` | Accent color (buttons, links, highlights) |
| `--ttp-border` | Editor border |
| `--ttp-bg` | Editor background |
| `--ttp-toolbar-bg` | Toolbar background |
| `--ttp-toolbar-border` | Toolbar separator |
| `--ttp-hover-bg` | Button hover background |
| `--ttp-active-bg` | Active button background |
| `--ttp-font-family` | Editor font stack |
| `--ttp-font-size` | Base font size |
| `--ttp-radius` | Border radius |
| `--ttp-placeholder-color` | Placeholder text color |
| `--ttp-code-bg` | Code block background |
| `--ttp-code-fg` | Code block text color |
| `--ttp-code-inline-bg` | Inline code background |
| `--ttp-blockquote-color` | Blockquote text color |
| `--ttp-focus-ring` | Focus outline style |
| `--ttp-shadow` | Dropdown/popup shadow |
| `--ttp-mention-person-color` | Mention "person" category color |
| `--ttp-mention-place-color` | Mention "place" category color |
| `--ttp-mention-tag-color` | Mention "tag" category color |

</details>

Dark mode is supported automatically via `@media (prefers-color-scheme: dark)`.

## HTML Sanitization

This library outputs raw HTML from the editor. **You must sanitize HTML server-side** before storing or rendering user-generated content to prevent XSS attacks. Use a library like [HtmlSanitizer](https://github.com/mganss/HtmlSanitizer).

## Development

```bash
cd TipTapBlazor
npm install
npm run build        # minified bundle
npm run build:dev    # with source maps
```

Run the demo app:

```bash
dotnet run --project demo/TipTapBlazor.Demo
```

Run tests:

```bash
dotnet test
```

## License

[MIT](LICENSE)
