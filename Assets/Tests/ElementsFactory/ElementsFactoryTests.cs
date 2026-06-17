using UI.Constants;
using UI.Factories;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UIElements;

[TestFixture]
public class ElementsFactoryTests
{
    [Test]
    public void ScreenLayout_AllElementsResolve()
    {
        var root = new VisualElement();
        root.Add(new Label { name = Elements.MenuName });
        root.Add(new Label { name = Elements.UsernameLabel });
        root.Add(new Label { name = Elements.VersionLabel });
        root.Add(new Button { name = Elements.BackBtn });
        root.Add(new VisualElement { name = Elements.ScreenViewContainer });

        var result = ElementsFactory.ScreenLayout(root);

        Assert.That(result.Header, Is.Not.Null);
        Assert.That(result.Username, Is.Not.Null);
        Assert.That(result.GameVersion, Is.Not.Null);
        Assert.That(result.BackButton, Is.Not.Null);
        Assert.That(result.ScreenViewContainer, Is.Not.Null);
    }

    [Test]
    public void ModalLayout_AllElementsResolve()
    {
        var root = new VisualElement();
        root.Add(new Label { name = Elements.ModalHeader });
        root.Add(new VisualElement { name = Elements.ModalViewContainer });
        root.Add(new VisualElement { name = Elements.ModalContainer });

        var result = ElementsFactory.ModalLayout(root);

        Assert.That(result.Header, Is.Not.Null);
        Assert.That(result.ModalViewContainer, Is.Not.Null);
        Assert.That(result.ModalContainer, Is.Not.Null);
    }

    [Test]
    public void MainMenu_AllElementsResolve()
    {
        var root = CloneUxml("Views/MainMenuView");
        var result = ElementsFactory.MainMenu(root);

        Assert.That(result.SoloButton, Is.Not.Null);
        Assert.That(result.CoOpButton, Is.Not.Null);
        Assert.That(result.ZombiesButton, Is.Not.Null);
        Assert.That(result.MultiplayerButton, Is.Not.Null);
        Assert.That(result.OptionsButton, Is.Not.Null);
        Assert.That(result.CreditsButton, Is.Not.Null);
        Assert.That(result.MessageLabel, Is.Not.Null);
    }

    [Test]
    public void SoloView_AllElementsResolve()
    {
        var root = CloneUxml("Views/SoloView");
        var result = ElementsFactory.SoloScreen(root);

        Assert.That(result.ResumeButton, Is.Not.Null);
        Assert.That(result.NewGameButton, Is.Not.Null);
        Assert.That(result.MissionSelectButton, Is.Not.Null);
    }

    [Test]
    public void CoOpView_AllElementsResolve()
    {
        var root = CloneUxml("Views/CoOpView");
        var result = ElementsFactory.CoOpScreen(root);

        Assert.That(result.OnlineButton, Is.Not.Null);
        Assert.That(result.SplitScreenButton, Is.Not.Null);
        Assert.That(result.LanButton, Is.Not.Null);
    }

    [Test]
    public void MultiplayerMenu_AllElementsResolve()
    {
        var root = CloneUxml("Views/MultiplayerMenuView");
        var result = ElementsFactory.MultiplayerMenu(root);

        Assert.That(result.OnlineButton, Is.Not.Null);
        Assert.That(result.SplitScreenButton, Is.Not.Null);
        Assert.That(result.LanPartyButton, Is.Not.Null);
        Assert.That(result.OptionsButton, Is.Not.Null);
        Assert.That(result.MessageOfTheDay, Is.Not.Null);
    }

    [Test]
    public void ZombiesView_AllElementsResolve()
    {
        var root = CloneUxml("Views/ZombiesView");
        var result = ElementsFactory.ZombiesScreen(root);

        Assert.That(result.ResumeButton, Is.Not.Null);
        Assert.That(result.NewGameButton, Is.Not.Null);
    }

    [Test]
    public void CreditsView_AllElementsResolve()
    {
        var root = CloneUxml("Views/CreditsView");
        var result = ElementsFactory.CreditsScreen(root);

        Assert.That(result.CreditsLabel, Is.Not.Null);
    }

    [Test]
    public void MissionSelect_AllElementsResolve()
    {
        var root = CloneUxml("Views/MissionSelectView");
        var result = ElementsFactory.MissionSelect(root);

        Assert.That(result.Thumbnail, Is.Not.Null);
        Assert.That(result.Header, Is.Not.Null);
        Assert.That(result.Description, Is.Not.Null);
        Assert.That(result.SemperFi, Is.Not.Null);
        Assert.That(result.LittleResistance, Is.Not.Null);
        Assert.That(result.HardLanding, Is.Not.Null);
        Assert.That(result.Vendetta, Is.Not.Null);
        Assert.That(result.TheirLandTheirBlood, Is.Not.Null);
        Assert.That(result.BurnEmOut, Is.Not.Null);
        Assert.That(result.Relentless, Is.Not.Null);
        Assert.That(result.BloodAndIron, Is.Not.Null);
        Assert.That(result.RingOfSteel, Is.Not.Null);
        Assert.That(result.Eviction, Is.Not.Null);
        Assert.That(result.BlackCats, Is.Not.Null);
        Assert.That(result.BlowtorchAndCorkscrew, Is.Not.Null);
        Assert.That(result.BreakingPoint, Is.Not.Null);
        Assert.That(result.HeartOfTheReich, Is.Not.Null);
        Assert.That(result.Downfall, Is.Not.Null);
    }

    [Test]
    public void GameOptions_AllElementsResolve()
    {
        var root = CloneUxml("Views/GameOptionsView");
        var result = ElementsFactory.GameOptions(root);

        Assert.That(result.LookInversionButton, Is.Not.Null);
        Assert.That(result.LookInversionLabel, Is.Not.Null);
        Assert.That(result.StickLayoutButton, Is.Not.Null);
        Assert.That(result.StickLayoutLabel, Is.Not.Null);
        Assert.That(result.ButtonLayoutButton, Is.Not.Null);
        Assert.That(result.ButtonLayoutLabel, Is.Not.Null);
        Assert.That(result.SensitivityButton, Is.Not.Null);
        Assert.That(result.SensitivityLabel, Is.Not.Null);
        Assert.That(result.TargetAssistButton, Is.Not.Null);
        Assert.That(result.TargetAssistLabel, Is.Not.Null);
        Assert.That(result.PlayerNameButton, Is.Not.Null);
        Assert.That(result.PlayerNameLabel, Is.Not.Null);
        Assert.That(result.GameVolumeButton, Is.Not.Null);
    }

    [Test]
    public void GameVolume_AllElementsResolve()
    {
        var root = CloneUxml("Views/GameVolumeView");
        var result = ElementsFactory.GameVolume(root);

        Assert.That(result.VoiceButton, Is.Not.Null);
        Assert.That(result.VoiceSlider, Is.Not.Null);
        Assert.That(result.MusicButton, Is.Not.Null);
        Assert.That(result.MusicSlider, Is.Not.Null);
        Assert.That(result.SfxButton, Is.Not.Null);
        Assert.That(result.SfxSlider, Is.Not.Null);
        Assert.That(result.CinematicsButton, Is.Not.Null);
        Assert.That(result.CinematicsSlider, Is.Not.Null);
        Assert.That(result.MasterButton, Is.Not.Null);
        Assert.That(result.MasterSlider, Is.Not.Null);
        Assert.That(result.VoipButton, Is.Not.Null);
        Assert.That(result.VoipSlider, Is.Not.Null);
    }

    [Test]
    public void NewGameModalView_AllElementsResolve()
    {
        var root = CloneUxml("Views/NewGameModalView");
        var result = ElementsFactory.NewGameModal(root);

        Assert.That(result.Yes, Is.Not.Null);
        Assert.That(result.No, Is.Not.Null);
    }

    [Test]
    public void ResumeGameModalView_AllElementsResolve()
    {
        var root = CloneUxml("Views/ResumeGameModalView");
        var result = ElementsFactory.ResumeGameModal(root);

        Assert.That(result.Yes, Is.Not.Null);
        Assert.That(result.No, Is.Not.Null);
    }

    [Test]
    public void DifficultyModalView_AllElementsResolve()
    {
        var root = CloneUxml("Views/DifficultyModalView");
        var result = ElementsFactory.DifficultyModal(root);

        Assert.That(result.Recruit, Is.Not.Null);
        Assert.That(result.Regular, Is.Not.Null);
        Assert.That(result.Hardened, Is.Not.Null);
        Assert.That(result.Veteran, Is.Not.Null);
        Assert.That(result.Icon, Is.Not.Null);
        Assert.That(result.Description, Is.Not.Null);
    }

    [Test]
    public void NoConnectionModalView_AllElementsResolve()
    {
        var root = CloneUxml("Views/NoConnectionModalView");
        var result = ElementsFactory.NoConnectionModal(root);

        Assert.That(result.Notice, Is.Not.Null);
        Assert.That(result.Icon, Is.Not.Null);
        Assert.That(result.Ok, Is.Not.Null);
    }

    [Test]
    public void ContentWarningModalView_AllElementsResolve()
    {
        var root = CloneUxml("Views/ContentWarningModalView");
        var result = ElementsFactory.NoticeModal(root);

        Assert.That(result.Notice, Is.Not.Null);
        Assert.That(result.Ok, Is.Not.Null);
    }

    [Test]
    public void SaveNoticeView_AllElementsResolve()
    {
        var root = CloneUxml("Views/SaveNoticeView");
        var result = ElementsFactory.NoticeModal(root);

        Assert.That(result.Notice, Is.Not.Null);
        Assert.That(result.Ok, Is.Not.Null);
    }

    private static VisualElement CloneUxml(string path)
    {
        var asset = Resources.Load<VisualTreeAsset>(path);
        Assert.That(asset, Is.Not.Null, $"Missing UXML at '{path}'. Is it in a Resources folder?");
        return asset.CloneTree();
    }
}
