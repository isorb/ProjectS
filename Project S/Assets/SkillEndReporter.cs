using UnityEngine;
using System.Collections;

public class SkillEndReporter : MonoBehaviour {
    GameManager gm;


    // Use this for initialization
    void Start()
    {
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
    }


    // Update is called once per frame
    void Update () {

    }

    public void SkillEndReport()
    {
        gm.SkillActionEnd();
    }
}
