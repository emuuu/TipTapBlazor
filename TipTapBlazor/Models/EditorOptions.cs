namespace TipTapBlazor.Models;

/// <summary>
/// Configuration options for the TipTap editor instance.
/// Controls which extensions and toolbar features are enabled.
/// </summary>
public class EditorOptions
{
    /// <summary>Enables the bold formatting button.</summary>
    public bool EnableBold { get; set; } = true;

    /// <summary>Enables the italic formatting button.</summary>
    public bool EnableItalic { get; set; } = true;

    /// <summary>Enables the underline formatting button.</summary>
    public bool EnableUnderline { get; set; } = true;

    /// <summary>Enables the strikethrough formatting button.</summary>
    public bool EnableStrike { get; set; } = true;

    /// <summary>Enables the inline code formatting button.</summary>
    public bool EnableCode { get; set; } = true;

    /// <summary>Enables the subscript formatting button.</summary>
    public bool EnableSubscript { get; set; } = true;

    /// <summary>Enables the superscript formatting button.</summary>
    public bool EnableSuperscript { get; set; } = true;

    /// <summary>Enables the blockquote button.</summary>
    public bool EnableBlockquote { get; set; } = true;

    /// <summary>Enables the bullet list button.</summary>
    public bool EnableBulletList { get; set; } = true;

    /// <summary>Enables the ordered (numbered) list button.</summary>
    public bool EnableOrderedList { get; set; } = true;

    /// <summary>Enables the task/checkbox list button.</summary>
    public bool EnableTaskList { get; set; } = true;

    /// <summary>Enables the code block button with syntax highlighting via lowlight.</summary>
    public bool EnableCodeBlock { get; set; } = true;

    /// <summary>Enables the horizontal rule button.</summary>
    public bool EnableHorizontalRule { get; set; } = true;

    /// <summary>Enables the heading dropdown (H1-H4).</summary>
    public bool EnableHeading { get; set; } = true;

    /// <summary>Enables text alignment buttons (left, center, right, justify).</summary>
    public bool EnableTextAlign { get; set; } = true;

    /// <summary>Enables the text color picker.</summary>
    public bool EnableColor { get; set; } = true;

    /// <summary>Enables the text highlight button.</summary>
    public bool EnableHighlight { get; set; } = true;

    /// <summary>Enables the font family dropdown.</summary>
    public bool EnableFontFamily { get; set; } = true;

    /// <summary>Enables the link button.</summary>
    public bool EnableLink { get; set; } = true;

    /// <summary>Enables the image insertion button.</summary>
    public bool EnableImage { get; set; } = true;

    /// <summary>Enables the table toolbar group with insert/edit commands.</summary>
    public bool EnableTable { get; set; } = true;

    /// <summary>Enables @mention support. Requires <see cref="TipTapEditor.MentionSearchFunc"/> to be set.</summary>
    public bool EnableMention { get; set; } = false;

    /// <summary>Enables the character count display in the editor footer.</summary>
    public bool EnableCharacterCount { get; set; } = false;

    /// <summary>Enables placeholder text when the editor is empty.</summary>
    public bool EnablePlaceholder { get; set; } = true;

    /// <summary>Enables undo/redo buttons in the toolbar.</summary>
    public bool EnableUndoRedo { get; set; } = true;

    /// <summary>Placeholder text shown when the editor is empty. Also configurable via the component's Placeholder parameter.</summary>
    public string? Placeholder { get; set; }

    /// <summary>Maximum character count. 0 means no limit. Only effective when <see cref="EnableCharacterCount"/> is true.</summary>
    public int MaxLength { get; set; }
}
