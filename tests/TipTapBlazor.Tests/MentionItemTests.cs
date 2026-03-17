using TipTapBlazor.Models;

namespace TipTapBlazor.Tests;

public class MentionItemTests
{
    [Test]
    public void Properties_CanBeAssigned()
    {
        var item = new MentionItem
        {
            Id = "user-123",
            DisplayName = "John Doe",
            Category = "person",
        };

        Assert.Multiple(() =>
        {
            Assert.That(item.Id, Is.EqualTo("user-123"));
            Assert.That(item.DisplayName, Is.EqualTo("John Doe"));
            Assert.That(item.Category, Is.EqualTo("person"));
        });
    }

    [Test]
    public void Defaults_EmptyStrings()
    {
        var item = new MentionItem();

        Assert.Multiple(() =>
        {
            Assert.That(item.Id, Is.EqualTo(string.Empty));
            Assert.That(item.DisplayName, Is.EqualTo(string.Empty));
            Assert.That(item.Category, Is.Null);
        });
    }
}
