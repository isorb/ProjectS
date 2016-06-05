using UnityEngine;
using System.Collections;

public class StageDataHandler : MonoBehaviour {

    public Stage StageInfo;
    public GameObject GameManager;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void StageSelectReport ()
    {
        GameManager.GetComponent<GlobalManager>().StartBattle
            (
            StageInfo.EnemyGroupID, 
            StageInfo.EnemyLevel, 
            GameManager.GetComponent<GlobalManager>().Members.ToArray()
            );
    }
}
