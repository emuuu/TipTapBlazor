namespace TipTapBlazor.Models;

/// <summary>
/// Available table manipulation commands that can be executed on the TipTap editor.
/// </summary>
public enum TableCommands
{
    /// <summary>Insert a new table (default 3x3 with header row).</summary>
    InsertTable,

    /// <summary>Add a row above the current row.</summary>
    AddRowBefore,

    /// <summary>Add a row below the current row.</summary>
    AddRowAfter,

    /// <summary>Add a column to the left of the current column.</summary>
    AddColumnBefore,

    /// <summary>Add a column to the right of the current column.</summary>
    AddColumnAfter,

    /// <summary>Delete the current row.</summary>
    DeleteRow,

    /// <summary>Delete the current column.</summary>
    DeleteColumn,

    /// <summary>Merge the currently selected cells.</summary>
    MergeCells,

    /// <summary>Split the current merged cell.</summary>
    SplitCell,

    /// <summary>Delete the entire table.</summary>
    DeleteTable,
}
