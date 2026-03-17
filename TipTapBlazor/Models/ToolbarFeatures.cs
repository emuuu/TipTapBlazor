namespace TipTapBlazor.Models;

/// <summary>
/// Determines which toolbar groups are visible based on the current <see cref="EditorOptions"/>.
/// </summary>
public class ToolbarFeatures
{
    /// <summary>Shows the formatting group (bold, italic, underline, strike, code, sub/superscript).</summary>
    public bool ShowFormattingGroup { get; init; }

    /// <summary>Shows the heading dropdown.</summary>
    public bool ShowHeadingGroup { get; init; }

    /// <summary>Shows the text alignment buttons.</summary>
    public bool ShowAlignmentGroup { get; init; }

    /// <summary>Shows the list and blockquote buttons.</summary>
    public bool ShowListGroup { get; init; }

    /// <summary>Shows the color and highlight buttons.</summary>
    public bool ShowColorGroup { get; init; }

    /// <summary>Shows the font family dropdown.</summary>
    public bool ShowFontGroup { get; init; }

    /// <summary>Shows the insert group (link, image, horizontal rule, code block).</summary>
    public bool ShowInsertGroup { get; init; }

    /// <summary>Shows the table toolbar group.</summary>
    public bool ShowTableGroup { get; init; }

    /// <summary>Shows the undo/redo buttons.</summary>
    public bool ShowHistoryGroup { get; init; }

    /// <summary>
    /// Creates a <see cref="ToolbarFeatures"/> instance by evaluating which groups have at least one enabled feature.
    /// </summary>
    public static ToolbarFeatures FromOptions(EditorOptions options)
    {
        return new ToolbarFeatures
        {
            ShowFormattingGroup = options.EnableBold || options.EnableItalic || options.EnableUnderline
                                  || options.EnableStrike || options.EnableCode
                                  || options.EnableSubscript || options.EnableSuperscript,
            ShowHeadingGroup = options.EnableHeading,
            ShowAlignmentGroup = options.EnableTextAlign,
            ShowListGroup = options.EnableBulletList || options.EnableOrderedList
                            || options.EnableTaskList || options.EnableBlockquote,
            ShowColorGroup = options.EnableColor || options.EnableHighlight,
            ShowFontGroup = options.EnableFontFamily,
            ShowInsertGroup = options.EnableLink || options.EnableImage
                              || options.EnableHorizontalRule || options.EnableCodeBlock,
            ShowTableGroup = options.EnableTable,
            ShowHistoryGroup = options.EnableUndoRedo,
        };
    }
}
