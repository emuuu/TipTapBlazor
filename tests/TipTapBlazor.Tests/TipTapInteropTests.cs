using Bunit;
using Microsoft.JSInterop;

namespace TipTapBlazor.Tests;

public class TipTapInteropTests : BunitContext
{
    [Test]
    public void Constructor_DoesNotImportModuleImmediately()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;

        var interop = new TipTapInterop(JSInterop.JSRuntime);

        // No JS calls should have been made yet (lazy import)
        Assert.That(JSInterop.Invocations, Is.Empty);
    }

    [Test]
    public async Task GetContentAsync_InvokesCorrectJsFunction()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        var moduleHandler = JSInterop.SetupModule("./_content/TipTapBlazor/js/tiptap-interop.js");
        moduleHandler.Mode = JSRuntimeMode.Loose;

        var interop = new TipTapInterop(JSInterop.JSRuntime);

        await interop.GetContentAsync(default);

        Assert.That(moduleHandler.Invocations["getContent"], Has.Count.EqualTo(1));
    }

    [Test]
    public async Task SetContentAsync_InvokesCorrectJsFunction()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        var moduleHandler = JSInterop.SetupModule("./_content/TipTapBlazor/js/tiptap-interop.js");
        moduleHandler.Mode = JSRuntimeMode.Loose;

        var interop = new TipTapInterop(JSInterop.JSRuntime);

        await interop.SetContentAsync(default, "<p>hello</p>");

        Assert.That(moduleHandler.Invocations["setContent"], Has.Count.EqualTo(1));
    }

    [Test]
    public async Task FocusAsync_InvokesCorrectJsFunction()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        var moduleHandler = JSInterop.SetupModule("./_content/TipTapBlazor/js/tiptap-interop.js");
        moduleHandler.Mode = JSRuntimeMode.Loose;

        var interop = new TipTapInterop(JSInterop.JSRuntime);

        await interop.FocusAsync(default);

        Assert.That(moduleHandler.Invocations["focus"], Has.Count.EqualTo(1));
    }

    [Test]
    public async Task DisposeAsync_DisposesModuleWhenCreated()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        var moduleHandler = JSInterop.SetupModule("./_content/TipTapBlazor/js/tiptap-interop.js");
        moduleHandler.Mode = JSRuntimeMode.Loose;

        var interop = new TipTapInterop(JSInterop.JSRuntime);

        // Trigger module import
        await interop.GetContentAsync(default);

        // Dispose should not throw
        await interop.DisposeAsync();
    }

    [Test]
    public async Task DisposeAsync_DoesNothingWhenModuleNotCreated()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;

        var interop = new TipTapInterop(JSInterop.JSRuntime);

        // Dispose without ever triggering module import — should not throw
        await interop.DisposeAsync();
    }
}
