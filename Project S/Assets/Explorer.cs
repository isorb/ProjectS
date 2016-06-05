using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Explorer : MonoBehaviour {

    public List<Stage> Stages = new List<Stage>();
    public List<int> ClearStages = new List<int>();

    private List<GameObject> stageObjs = new List<GameObject>();

	// Use this for initialization
	void Start () {
        stageObjs.AddRange(GameObject.FindGameObjectsWithTag("StageBtn"));
        ClearStages.Add(0);

        StageDeploy();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void StageDeploy ()
    {
        for(int i = 0; i < Stages.Count; i++)
        {
            Debug.Log("count");
            stageObjs[i].transform.localPosition = new Vector3(Stages[i].Position_x, Stages[i].position_y);
            stageObjs[i].GetComponent<StageDataHandler>().StageInfo = Stages[i];
        }
    }
    void ClearStageStateChange ()
    {

    }
}

[System.Serializable]
public class Stage
{
    public string StageName;
    public int StageId;
    public int[] OpenConditions;
    public string IconName;

    public string ParentArea;
    public float Position_x, position_y;

    public int EnemyGroupID;
    public int EnemyLevel;
    public int Exp;
}
