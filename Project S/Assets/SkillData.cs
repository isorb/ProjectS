using UnityEngine;
using System.Collections;

[System.Serializable]
public class SkillData {

    public int Id;
    public string Name;

    public int SkillPower;
    public int MpCost;
    public float SkillCastingTime;
    public bool isAbsoluteDamageToEnemy;
    public bool isRelativeDamageToEnemy;
    public bool isHealToAlly;
    public bool isStillFromEnemy;
    public bool isResurrection;

    //그래픽 요소
    public string ImageName;
    public string EffectName;
}

public class SkillQue
{
    public SkillData Skill;
    public float CastTime;
    public float CurCastTime;
    public GameObject Target;
}
