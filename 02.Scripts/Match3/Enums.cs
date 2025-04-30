using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// Bonus types
/// </summary>
[Flags]
public enum BonusType
{
    None,
    DestroyWholeRowColumn,
    DestroyWholeRow,
    DestroyWholeColumn,
    DestroySurrounding,
    Count,
}


public static class BonusTypeUtilities
{
    /// <summary>
    /// Helper method to check for specific bonus type
    /// </summary>
    /// <param name="bt"></param>
    /// <returns></returns>
    /// 
    public static bool ContainsDestroyWholeRowColumn(BonusType bt)
    {
        return (bt & BonusType.DestroyWholeRowColumn) == BonusType.DestroyWholeRowColumn;
    }

    public static bool ContainsBonus(BonusType bt)
    {
        return bt != BonusType.None;
    }

    //bonusType값을 순차적으로 넘겨주는함수
    public static BonusType GetBonusType(int index)
    {
        switch(index)
        {

            case 0:
                return BonusType.DestroyWholeRow;
            case 1:
                return BonusType.DestroyWholeColumn;
            case 2:
                return BonusType.DestroySurrounding;
            case 3:
                return BonusType.DestroyWholeRowColumn;
            default:
                throw new ArgumentOutOfRangeException("index");
        }
    }



}


/// <summary>
/// Our simple game state
/// </summary>
public enum GameState
{
    None,
    SelectionStarted,
    Animating,
    GameOver,
    Paused,
}
