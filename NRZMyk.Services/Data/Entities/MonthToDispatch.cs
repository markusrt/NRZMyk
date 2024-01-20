using System.ComponentModel;

namespace NRZMyk.Services.Data.Entities;

public enum MonthToDispatch
{
    [Description("-")]
    None = 0,
    [Description("Januar")]
    January = 1,
    [Description("Februar")]
    February = 2,
    [Description("März")]
    March = 3,
    [Description("April")]
    April = 4,
    [Description("Mai")]
    May = 5,
    [Description("Juni")]
    June = 6,
    [Description("Juli")]
    July = 7,
    [Description("August")]
    August = 8,
    [Description("September")]
    September = 9,
    [Description("Oktober")]
    October = 10,
    [Description("November")]
    November = 11,
    [Description("Dezember")]
    December = 12
}