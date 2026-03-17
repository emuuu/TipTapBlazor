namespace TipTapBlazor.Models;

/// <summary>
/// Represents a mention suggestion item displayed in the mention dropdown.
/// </summary>
public class MentionItem
{
    /// <summary>Unique identifier for the mention. Stored in the document as the mention node's id attribute.</summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>Display name shown in the mention dropdown and rendered in the editor.</summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>Optional category for visual styling (e.g. "person", "place", "tag"). Maps to a data-category CSS attribute.</summary>
    public string? Category { get; set; }
}
