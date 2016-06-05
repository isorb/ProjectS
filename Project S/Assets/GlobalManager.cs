using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GlobalManager : MonoBehaviour {

    public List<int> Members = new List<int>();
    public GameObject MembersPanel;

    public GameObject RootScene;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void StartBattle (int enemyGroupId, int enemyLevel, int[] members)
    {
        gameObject.GetComponent<GameManager>().SetBattleStart(enemyGroupId, enemyLevel, members);
        RootScene.SetActive(false);
    }

    public void ReturnBattle ()
    {
        gameObject.GetComponent<GameManager>().ResetGameManager();
        RootScene.SetActive(true);
    }
}
