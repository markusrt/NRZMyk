using System.ComponentModel;

namespace NRZMyk.Services.Data.Entities
{
    public enum AgeGroup
    {
        [Description("Unbekannt")]
        Unknown = 0,
        [Description("0-5")]
        ZeroToFive = 6,
        [Description("6-10")]
        SixToTen = 11,
        [Description("11-15")]
        ElevenToFifteen = 16,
        [Description("16-20")]
        SixteenToTwenty = 21,
        [Description("21-25")]
        TwentyOneToTwentyFive = 26,
        [Description("26-30")]
        TwentySixToThirty = 31,
        [Description("31-35")]
        ThirtyOneToThirtyFive = 36,
        [Description("36-40")]
        ThirtySixToForty = 41,
        [Description("41-45")]
        FortyOneToFortyFive = 46,
        [Description("46-50")]
        FortySixToFifty = 51,
        [Description("51-55")]
        FiftyOneToFiftyFive = 56,
        [Description("56-60")]
        FiftySixToSixty = 61,
        [Description("61-65")]
        SixtyOneToSixtyFive = 66,
        [Description("66-70")]
        SixtySixToSeventy = 71,
        [Description("71-75")]
        SeventyOneToSeventyFive = 76,
        [Description("76-80")]
        SeventySixToEighty = 81,
        [Description("81-85")]
        EightyOneToEightyFive = 85,
        [Description("86-90")]
        EightySixToNinety = 91,
        [Description("91+")]
        OverNinetyOne = 300

    }
}