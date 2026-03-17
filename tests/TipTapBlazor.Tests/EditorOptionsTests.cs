using TipTapBlazor.Models;

namespace TipTapBlazor.Tests;

public class EditorOptionsTests
{
    [Test]
    public void Defaults_AllFormattingEnabled()
    {
        var options = new EditorOptions();

        Assert.Multiple(() =>
        {
            Assert.That(options.EnableBold, Is.True);
            Assert.That(options.EnableItalic, Is.True);
            Assert.That(options.EnableUnderline, Is.True);
            Assert.That(options.EnableStrike, Is.True);
            Assert.That(options.EnableCode, Is.True);
            Assert.That(options.EnableSubscript, Is.True);
            Assert.That(options.EnableSuperscript, Is.True);
        });
    }

    [Test]
    public void Defaults_AllBlocksEnabled()
    {
        var options = new EditorOptions();

        Assert.Multiple(() =>
        {
            Assert.That(options.EnableBlockquote, Is.True);
            Assert.That(options.EnableBulletList, Is.True);
            Assert.That(options.EnableOrderedList, Is.True);
            Assert.That(options.EnableTaskList, Is.True);
            Assert.That(options.EnableCodeBlock, Is.True);
            Assert.That(options.EnableHorizontalRule, Is.True);
            Assert.That(options.EnableHeading, Is.True);
        });
    }

    [Test]
    public void Defaults_RichFeaturesEnabled()
    {
        var options = new EditorOptions();

        Assert.Multiple(() =>
        {
            Assert.That(options.EnableTextAlign, Is.True);
            Assert.That(options.EnableColor, Is.True);
            Assert.That(options.EnableHighlight, Is.True);
            Assert.That(options.EnableFontFamily, Is.True);
            Assert.That(options.EnableLink, Is.True);
            Assert.That(options.EnableImage, Is.True);
            Assert.That(options.EnableTable, Is.True);
            Assert.That(options.EnablePlaceholder, Is.True);
            Assert.That(options.EnableUndoRedo, Is.True);
        });
    }

    [Test]
    public void Defaults_OptionalFeaturesDisabled()
    {
        var options = new EditorOptions();

        Assert.Multiple(() =>
        {
            Assert.That(options.EnableMention, Is.False);
            Assert.That(options.EnableCharacterCount, Is.False);
        });
    }

    [Test]
    public void Defaults_NumericValuesZero()
    {
        var options = new EditorOptions();

        Assert.That(options.MaxLength, Is.EqualTo(0));
    }

    [Test]
    public void Defaults_PlaceholderNull()
    {
        var options = new EditorOptions();

        Assert.That(options.Placeholder, Is.Null);
    }
}
