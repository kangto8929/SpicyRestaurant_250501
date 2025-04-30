using UnityEngine;

[CreateAssetMenu(fileName = "ConstantsSO", menuName = "Scriptable Objects/ConstantsSO")]
public class ConstantsSO : ScriptableObject
{
    [Header("가로세로")]
    public int Rows;
    public int Columns;
    [Header("애니메이션의 지속 시간")]
    public float AnimationDuration;
    [Header("이동 애니메이션의 최소 지속 시간")]
    public float MoveAnimationMinDuration;
    [Header("폭발 애니메이션의 지속 시간")]
    public float ExplosionDuration;
    [Header("폭발 애니메이션의 최소 지속 시간")]
    public float WaitBeforePotentialMatchesCheck;
    [Header("힌트 투명도 애니메이션의 프레임 간격")]
    public float OpacityAnimationFrameDelay;
    [Header("매치로 간주되는 최소 매치, 보너스 생성 수")]
    public int MinimumMatches;
    public int MinimumMatchesForBonus;
    [Header("게임 시간 제한")]
    public float GameTimeLimit;
}
