// IBossSkill.cs
using System.Collections;
using UnityEngine;

public enum SkillCategory { Melee, Ranged, Unrestricted }

public interface IBossSkill
{
    bool IsAvailable { get; }
    float Cooldown { get; }
    float RangeMin { get; }
    float RangeMax { get; }
    float Weight { get; }
    SkillCategory Category { get; }

    IEnumerator ExecuteSkill();
    void OnSkillFinished();
}
