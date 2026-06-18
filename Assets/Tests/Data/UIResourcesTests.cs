using Data;
using UI.Enums;
using NUnit.Framework;

[TestFixture]
public class UIResourcesTests
{
    [Test]
    public void DifficultyIcons_AllEntriesResolve()
    {
        foreach (var kv in UIResources.DifficultyIcons)
            Assert.That(kv.Value, Is.Not.Null, $"Missing icon for {kv.Key}");
    }

    [Test]
    public void MissionThumbnails_AllEntriesResolve()
    {
        foreach (var kv in UIResources.MissionThumbnails)
            Assert.That(kv.Value, Is.Not.Null, $"Missing thumbnail for {kv.Key}");
    }

    [Test]
    public void AudioClips_AllEntriesResolve()
    {
        foreach (var kv in UIResources.AudioClips)
            Assert.That(kv.Value, Is.Not.Null, $"Missing audio clip for {kv.Key}");
    }

    [Test]
    public void MissionTitles_AllMissionsParsed()
    {
        var titles = UIResources.MissionTitles;
        foreach (Missions mission in System.Enum.GetValues(typeof(Missions)))
            Assert.That(titles.ContainsKey(mission), $"{mission} missing from MissionTitles");
    }

    [Test]
    public void MissionDescriptions_AllMissionsParsed()
    {
        var descriptions = UIResources.MissionDescriptions;
        foreach (Missions mission in System.Enum.GetValues(typeof(Missions)))
            Assert.That(descriptions.ContainsKey(mission), $"{mission} missing from MissionDescriptions");
    }

    [Test]
    public void DifficultyDescriptions_AllDifficultiesParsed()
    {
        var descriptions = UIResources.DifficultyDescriptions;
        foreach (Difficulty difficulty in System.Enum.GetValues(typeof(Difficulty)))
            Assert.That(descriptions.ContainsKey(difficulty), $"{difficulty} missing from DifficultyDescriptions");
    }

    [Test]
    public void MessagesOfTheDay_NotEmpty()
    {
        var messages = UIResources.MessagesOfTheDay;
        Assert.That(messages, Is.Not.Null);
        Assert.That(messages.Length, Is.GreaterThan(0));
    }

    [Test]
    public void OptionCollections_AllResolve()
    {
        Assert.That(UIResources.OptionInversion, Is.Not.Null.And.Length.GreaterThan(0));
        Assert.That(UIResources.OptionStickLayout, Is.Not.Null.And.Length.GreaterThan(0));
        Assert.That(UIResources.OptionButtonLayout, Is.Not.Null.And.Length.GreaterThan(0));
        Assert.That(UIResources.OptionSensitivity, Is.Not.Null.And.Length.GreaterThan(0));
        Assert.That(UIResources.OptionTargetAssist, Is.Not.Null.And.Length.GreaterThan(0));
    }
}
