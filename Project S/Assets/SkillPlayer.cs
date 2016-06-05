using UnityEngine;
using System.Collections;

public class SkillPlayer : MonoBehaviour {
    GameManager gm;


	// Use this for initialization
	void Start () {
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Play (SkillData skill, GameObject target)
    {
        Debug.Log("호출됨!");
        gameObject.transform.FindChild("Char01_Skill_1/Body").GetComponent<Animator>().SetTrigger("SkillUse");
    }
}
