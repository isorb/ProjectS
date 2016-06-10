using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SkillPlayer : MonoBehaviour {
    GameManager gm;
    string User_img;
    string Effect_prefab;
    string Target_img;

    Dictionary<string, Sprite> CharacterSprites = new Dictionary<string, Sprite>();

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void ReadySprites ()
    {
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();

        foreach (CharacterStat c in gm.PcPnls.GetComponent<DeployManager>().Characters)
        {
            CharacterSprites.Add(c.Skill_1.ImageName, Resources.Load<Sprite>("Sprites/skill/" + c.Skill_1.ImageName));            
        }

        foreach (CharacterStat c in gm.EnemyPnls.GetComponent<DeployManager>().Characters)
        {
            if (!CharacterSprites.ContainsKey(c.thumb))
                CharacterSprites.Add(c.thumb, Resources.Load<Sprite>("Sprites/skill/" + c.thumb));
        }

        Debug.Log("charSprite count is " + CharacterSprites.Count);
    }

    public void Play (SkillData skill, GameObject target)
    {
        User_img = skill.ImageName;
        Target_img = target.GetComponent<DataHandler>().Character.thumb;
        //Effect_prefab = skill.EffectName;        

        gameObject.transform.FindChild("PC_Use_Skill/Body/PC").gameObject.GetComponent<UI2DSprite>().sprite2D = CharacterSprites[User_img];
        gameObject.transform.FindChild("PC_Use_Skill/Body/Enemy").gameObject.GetComponent<UI2DSprite>().sprite2D = CharacterSprites[Target_img];


        Debug.Log("호출됨!");
        gameObject.transform.FindChild("PC_Use_Skill/Body").GetComponent<Animator>().SetTrigger("SkillUse");
    }
}
