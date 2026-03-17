using TipTapBlazor.Models;

namespace TipTapBlazor.Tests;

public class ToolbarFeaturesTests
{
    [Test]
    public void FromOptions_DefaultOptions_AllGroupsVisible()
    {
        var options = new EditorOptions();
        var features = ToolbarFeatures.FromOptions(options);

        Assert.Multiple(() =>
        {
            Assert.That(features.ShowFormattingGroup, Is.True);
            Assert.That(features.ShowHeadingGroup, Is.True);
            Assert.That(features.ShowAlignmentGroup, Is.True);
            Assert.That(features.ShowListGroup, Is.True);
            Assert.That(features.ShowColorGroup, Is.True);
            Assert.That(features.ShowFontGroup, Is.True);
            Assert.That(features.ShowInsertGroup, Is.True);
            Assert.That(features.ShowTableGroup, Is.True);
            Assert.That(features.ShowHistoryGroup, Is.True);
        });
    }

    [Test]
    public void FromOptions_AllDisabled_NoGroupsVisible()
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
            EnableHeading = false,
            EnableTextAlign = false,
            EnableBulletList = false,
            EnableOrderedList = false,
            EnableTaskList = false,
            EnableBlockquote = false,
            EnableColor = false,
            EnableHighlight = false,
            EnableFontFamily = false,
            EnableLink = false,
            EnableImage = false,
            EnableHorizontalRule = false,
            EnableCodeBlock = false,
            EnableTable = false,
            EnableUndoRedo = false,
        };

        var features = ToolbarFeatures.FromOptions(options);

        Assert.Multiple(() =>
        {
            Assert.That(features.ShowFormattingGroup, Is.False);
            Assert.That(features.ShowHeadingGroup, Is.False);
            Assert.That(features.ShowAlignmentGroup, Is.False);
            Assert.That(features.ShowListGroup, Is.False);
            Assert.That(features.ShowColorGroup, Is.False);
            Assert.That(features.ShowFontGroup, Is.False);
            Assert.That(features.ShowInsertGroup, Is.False);
            Assert.That(features.ShowTableGroup, Is.False);
            Assert.That(features.ShowHistoryGroup, Is.False);
        });
    }

    [Test]
    public void FromOptions_OnlyBold_ShowsFormattingGroup()
    {
        var options = new EditorOptions
        {
            EnableBold = true,
            EnableItalic = false,
            EnableUnderline = false,
            EnableStrike = false,
            EnableCode = false,
            EnableSubscript = false,
            EnableSuperscript = false,
        };

        var features = ToolbarFeatures.FromOptions(options);

        Assert.That(features.ShowFormattingGroup, Is.True);
    }
}
