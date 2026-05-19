using FluentAssertions;

namespace TopElevenStats.Tests;

/// <summary>
/// A simple helper that decides which of two decimal stat values is "better".
/// For TopEleven, higher is always better for rating, goals, and assists.
/// </summary>
public static class StatComparer
{
    /// <summary>
    /// Returns the better (higher) of two stat values.
    /// </summary>
    public static decimal BetterValue(decimal a, decimal b) => a >= b ? a : b;

    /// <summary>
    /// Returns 1 if <paramref name="a"/> is better, -1 if <paramref name="b"/> is better,
    /// and 0 if they are equal.
    /// </summary>
    public static int Compare(decimal a, decimal b)
    {
        if (a > b) return 1;
        if (a < b) return -1;
        return 0;
    }

    /// <summary>
    /// Returns the label ("Player A", "Player B", or "Draw") for the better value.
    /// </summary>
    public static string WinnerLabel(decimal a, decimal b, string labelA = "Player A", string labelB = "Player B")
    {
        return Compare(a, b) switch
        {
            1 => labelA,
            -1 => labelB,
            _ => "Draw"
        };
    }
}

public class CompareLogicTests
{
    // --- BetterValue ---

    [Fact]
    public void BetterValue_WhenFirstIsHigher_ReturnsFirst()
    {
        var result = StatComparer.BetterValue(8.5m, 7.2m);

        result.Should().Be(8.5m);
    }

    [Fact]
    public void BetterValue_WhenSecondIsHigher_ReturnsSecond()
    {
        var result = StatComparer.BetterValue(6.0m, 9.1m);

        result.Should().Be(9.1m);
    }

    [Fact]
    public void BetterValue_WhenEqual_ReturnsThatValue()
    {
        var result = StatComparer.BetterValue(7.5m, 7.5m);

        result.Should().Be(7.5m);
    }

    [Fact]
    public void BetterValue_WithZeroStats_ReturnsZero()
    {
        var result = StatComparer.BetterValue(0m, 0m);

        result.Should().Be(0m);
    }

    // --- Compare ---

    [Fact]
    public void Compare_WhenAIsGreater_ReturnsPositive()
    {
        var result = StatComparer.Compare(10m, 8m);

        result.Should().Be(1);
    }

    [Fact]
    public void Compare_WhenBIsGreater_ReturnsNegative()
    {
        var result = StatComparer.Compare(5m, 9m);

        result.Should().Be(-1);
    }

    [Fact]
    public void Compare_WhenEqual_ReturnsZero()
    {
        var result = StatComparer.Compare(7m, 7m);

        result.Should().Be(0);
    }

    // --- Rating comparison scenarios ---

    [Theory]
    [InlineData(9.2, 7.8, 1)]   // better rating wins
    [InlineData(6.0, 8.5, -1)]  // worse rating loses
    [InlineData(7.5, 7.5, 0)]   // equal is draw
    public void Compare_AvgRating_ReturnsCorrectResult(double a, double b, int expected)
    {
        var result = StatComparer.Compare((decimal)a, (decimal)b);

        result.Should().Be(expected);
    }

    // --- Goals comparison scenarios ---

    [Theory]
    [InlineData(20, 15, 1)]
    [InlineData(5, 12, -1)]
    [InlineData(8, 8, 0)]
    public void Compare_Goals_ReturnsCorrectResult(int goalsA, int goalsB, int expected)
    {
        var result = StatComparer.Compare(goalsA, goalsB);

        result.Should().Be(expected);
    }

    // --- Assists comparison scenarios ---

    [Theory]
    [InlineData(10, 6, 1)]
    [InlineData(3, 11, -1)]
    [InlineData(5, 5, 0)]
    public void Compare_Assists_ReturnsCorrectResult(int assistsA, int assistsB, int expected)
    {
        var result = StatComparer.Compare(assistsA, assistsB);

        result.Should().Be(expected);
    }

    // --- WinnerLabel ---

    [Fact]
    public void WinnerLabel_WhenAIsBetter_ReturnsPlayerALabel()
    {
        var label = StatComparer.WinnerLabel(9.0m, 7.0m);

        label.Should().Be("Player A");
    }

    [Fact]
    public void WinnerLabel_WhenBIsBetter_ReturnsPlayerBLabel()
    {
        var label = StatComparer.WinnerLabel(5.0m, 8.0m);

        label.Should().Be("Player B");
    }

    [Fact]
    public void WinnerLabel_WhenEqual_ReturnsDraw()
    {
        var label = StatComparer.WinnerLabel(7.0m, 7.0m);

        label.Should().Be("Draw");
    }

    [Fact]
    public void WinnerLabel_WithCustomLabels_UsesProvidedNames()
    {
        var label = StatComparer.WinnerLabel(8.0m, 6.5m, labelA: "Messi", labelB: "Ronaldo");

        label.Should().Be("Messi");
    }

    [Fact]
    public void BetterValue_IsConsistentWithCompare()
    {
        decimal a = 8.5m, b = 7.0m;
        var better = StatComparer.BetterValue(a, b);
        var comparison = StatComparer.Compare(a, b);

        // If Compare says A wins, BetterValue should return A
        if (comparison == 1) better.Should().Be(a);
        else if (comparison == -1) better.Should().Be(b);
        else better.Should().Be(a); // equal: either is fine, method returns a
    }
}
