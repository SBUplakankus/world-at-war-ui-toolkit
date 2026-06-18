using System;
using System.Collections.Generic;
using UI.Views;
using NUnit.Framework;
using ViewFactory = UI.Factories.ViewFactory;

[TestFixture]
public class ViewFactoryTests
{
    private static IEnumerable<Type> ViewTypes()
    {
        yield return typeof(MainMenuView);
        yield return typeof(SoloView);
        yield return typeof(CoOpView);
        yield return typeof(MultiplayerMenuView);
        yield return typeof(ZombiesView);
        yield return typeof(MissionSelectView);
        yield return typeof(GameOptionsView);
        yield return typeof(GameVolumeView);
        yield return typeof(CreditsView);
        yield return typeof(NewGameModalView);
        yield return typeof(ResumeGameModalView);
        yield return typeof(ContentWarningModalView);
        yield return typeof(DifficultyModalView);
        yield return typeof(NoConnectionModalView);
        yield return typeof(SaveNoticeView);
    }

    [TestCaseSource(nameof(ViewTypes))]
    public void Create_AllViewTypes_Succeeds(Type viewType)
    {
        var method = typeof(ViewFactory).GetMethod("Create")!.MakeGenericMethod(viewType);
        var view = method.Invoke(null, null) as BaseView;

        Assert.That(view, Is.Not.Null, $"{viewType.Name} failed to create");
        Assert.That(view.Root, Is.Not.Null, $"{viewType.Name}.Root is null");
        Assert.That(view.Root.parent, Is.Null, $"{viewType.Name}.Root should not be attached");

        view.Activate();
        view.Deactivate();
        view.Dispose();
    }
}
