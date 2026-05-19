using FluentAssertions;
using SharedServices.Models.TopEleven;

namespace TopElevenStats.Tests;

public class SeasonStatsTests
{
    private static TopElevenSeasonStats CreateValidStats() => new()
    {
        Season = 1,
        AvgRating = 7.5m,
        MatchesPlayed = 20,
        WinRatioWith = 65,
        WinRatioWithout = 40,
        TeamAverage = 55,
        Assists = 3,
        KeyPasses = 1.5m,
        Injuries = 0,
        YellowCards = 2,
        RedCards = 0,
    };

    [Fact]
    public void AvgRating_WhenSetToValidValue_ShouldBeWithinRange()
    {
        var stats = CreateValidStats();
        stats.AvgRating = 7.5m;

        stats.AvgRating.Should().BeGreaterThanOrEqualTo(0m)
            .And.BeLessThanOrEqualTo(10m);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(5)]
    [InlineData(10)]
    public void AvgRating_BoundaryValues_AreAccepted(decimal rating)
    {
        var stats = CreateValidStats();
        stats.AvgRating = rating;

        stats.AvgRating.Should().Be(rating);
    }

    [Fact]
    public void MatchesPlayed_WhenSet_ShouldBeNonNegative()
    {
        var stats = CreateValidStats();
        stats.MatchesPlayed = 38;

        stats.MatchesPlayed.Should().BeGreaterThanOrEqualTo(0);
    }

    [Fact]
    public void MatchesPlayed_WhenZero_IsValid()
    {
        var stats = CreateValidStats();
        stats.MatchesPlayed = 0;

        stats.MatchesPlayed.Should().Be(0);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(50)]
    [InlineData(100)]
    public void WinRatioWith_BoundaryValues_AreStoredCorrectly(int ratio)
    {
        var stats = CreateValidStats();
        stats.WinRatioWith = ratio;

        stats.WinRatioWith.Should().Be(ratio)
            .And.BeGreaterThanOrEqualTo(0)
            .And.BeLessThanOrEqualTo(100);
    }

    [Fact]
    public void WinRatioWith_AndWinRatioWithout_AreIndependentProperties()
    {
        var stats = CreateValidStats();
        stats.WinRatioWith = 80;
        stats.WinRatioWithout = 30;

        stats.WinRatioWith.Should().Be(80);
        stats.WinRatioWithout.Should().Be(30);
    }

    [Fact]
    public void CleanSheets_IsNullableAndDefaultsToNull()
    {
        var stats = CreateValidStats();

        stats.CleanSheets.Should().BeNull();
    }

    [Fact]
    public void Goals_IsNullableAndDefaultsToNull()
    {
        var stats = CreateValidStats();

        stats.Goals.Should().BeNull();
    }

    [Fact]
    public void Xg90_IsNullableAndDefaultsToNull()
    {
        var stats = CreateValidStats();

        stats.Xg90.Should().BeNull();
    }

    [Fact]
    public void GoalkeeperStats_CleanSheets_CanBeSet()
    {
        var stats = CreateValidStats();
        stats.CleanSheets = 12;

        stats.CleanSheets.Should().Be(12);
        stats.Goals.Should().BeNull("GoalKeeper stats should not have goals by default");
    }

    [Fact]
    public void OutfieldStats_GoalsAndXg90_CanBeSetIndependently()
    {
        var stats = CreateValidStats();
        stats.Goals = 15;
        stats.Xg90 = 0.62m;

        stats.Goals.Should().Be(15);
        stats.Xg90.Should().Be(0.62m);
        stats.CleanSheets.Should().BeNull();
    }

    [Fact]
    public void SeasonNumber_IsStoredCorrectly()
    {
        var stats = CreateValidStats();
        stats.Season = 5;

        stats.Season.Should().Be(5);
    }

    [Fact]
    public void CardStats_AreStoredCorrectly()
    {
        var stats = CreateValidStats();
        stats.YellowCards = 4;
        stats.RedCards = 1;

        stats.YellowCards.Should().Be(4);
        stats.RedCards.Should().Be(1);
    }
}
