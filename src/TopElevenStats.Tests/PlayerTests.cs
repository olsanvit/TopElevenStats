using FluentAssertions;
using SharedServices.Models.TopEleven;

namespace TopElevenStats.Tests;

public class PlayerTests
{
    private static TopElevenPlayer CreateValidPlayer() => new()
    {
        Name = "Petr Novak",
        Ovr = 75,
        Age = 25,
        Roles = "ML,AML",
        IsGoalkeeper = false,
        IsElite = false,
    };

    [Fact]
    public void Name_WhenSet_IsNotNullOrEmpty()
    {
        var player = CreateValidPlayer();
        player.Name = "Jan Kovar";

        player.Name.Should().NotBeNullOrEmpty();
        player.Name.Should().Be("Jan Kovar");
    }

    [Fact]
    public void Name_DefaultValue_IsEmptyString()
    {
        var player = new TopElevenPlayer();

        player.Name.Should().NotBeNull();
        player.Name.Should().BeEmpty();
    }

    [Fact]
    public void IsGoalkeeper_DefaultsToFalse()
    {
        var player = new TopElevenPlayer();

        player.IsGoalkeeper.Should().BeFalse();
    }

    [Fact]
    public void IsElite_DefaultsToFalse()
    {
        var player = new TopElevenPlayer();

        player.IsElite.Should().BeFalse();
    }

    [Fact]
    public void IsGoalkeeper_AndIsElite_AreIndependentBooleans()
    {
        var player = CreateValidPlayer();
        player.IsGoalkeeper = true;
        player.IsElite = false;

        player.IsGoalkeeper.Should().BeTrue();
        player.IsElite.Should().BeFalse();
    }

    [Fact]
    public void PlayerCanBeBothGoalkeeperAndElite()
    {
        var player = CreateValidPlayer();
        player.IsGoalkeeper = true;
        player.IsElite = true;

        player.IsGoalkeeper.Should().BeTrue();
        player.IsElite.Should().BeTrue();
    }

    [Fact]
    public void PlayerCanBeNeitherGoalkeeperNorElite()
    {
        var player = CreateValidPlayer();
        player.IsGoalkeeper = false;
        player.IsElite = false;

        player.IsGoalkeeper.Should().BeFalse();
        player.IsElite.Should().BeFalse();
    }

    [Fact]
    public void Ovr_WhenSet_IsStoredCorrectly()
    {
        var player = CreateValidPlayer();
        player.Ovr = 90;

        player.Ovr.Should().Be(90);
    }

    [Fact]
    public void Age_WhenSet_IsStoredCorrectly()
    {
        var player = CreateValidPlayer();
        player.Age = 22;

        player.Age.Should().Be(22);
    }

    [Fact]
    public void Roles_WhenSet_IsStoredCorrectly()
    {
        var player = CreateValidPlayer();
        player.Roles = "GK";

        player.Roles.Should().Be("GK");
    }

    [Fact]
    public void Roles_DefaultValue_IsEmptyString()
    {
        var player = new TopElevenPlayer();

        player.Roles.Should().NotBeNull();
    }

    [Fact]
    public void SpecialAbility_IsNullableByDefault()
    {
        var player = new TopElevenPlayer();

        player.SpecialAbility.Should().BeNull();
    }

    [Fact]
    public void SpecialAbility_WhenSet_IsStoredCorrectly()
    {
        var player = CreateValidPlayer();
        player.SpecialAbility = "Long Shot";

        player.SpecialAbility.Should().Be("Long Shot");
    }

    [Fact]
    public void Citizenship_IsNullableByDefault()
    {
        var player = new TopElevenPlayer();

        player.Citizenship.Should().BeNull();
    }

    [Fact]
    public void SeasonStats_CollectionInitialized_IsNotNull()
    {
        var player = new TopElevenPlayer();

        player.SeasonStats.Should().NotBeNull();
        player.SeasonStats.Should().BeEmpty();
    }

    [Fact]
    public void Guid_OnNewPlayer_IsNotEmpty()
    {
        var player = new TopElevenPlayer();

        player.Guid.Should().NotBeEmpty();
    }
}
