using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DeployManager : MonoBehaviour {
    GameObject gm;
    
    public List<CharacterStat> Characters = new List<CharacterStat>(); // 캐릭터 인스턴스의 뿌리
    public bool IsPlayerSide = true;
    public int PnlSize = 120;
    public int PnlGap = 40;
    public int TopPos = 420;

    public List<GameObject> CharPannels = new List<GameObject>();

   //private List<GameObject> ObjectPool = new List<GameObject>(10);

    LevelDatas levelDatas;

    GameObject Skillpnl;
    GameObject CurrentSkillUsePnl;

    // Use this for initialization
    void Start () {
        gm = GameObject.Find("GameManager");

        if (IsPlayerSide)
            Skillpnl = transform.FindChild("SkillPanel").gameObject;

    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void SkillTargetSelected ()
    {
        //스킬 선택상태를 해제한다.
        gm.GetComponent<GameManager>().isSkillSelecting = false;

        //적들 선택 상태를 되돌린다.
        //하이라이트를 위해 일시적으로 조정했던 뎁스를 되돌린다. (선택 버튼도 비활성화)
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("EnemyPanel"))
        {
            obj.transform.FindChild("body/button").gameObject.SetActive(false);

            foreach (UIWidget widget in obj.transform.GetComponentsInChildren<UIWidget>())
            {
                widget.depth += 4;
            }
        }

        //스킬 패널을 비활성화 시킨다.
        Skillpnl.SetActive(false);

        //하이라이트를 위해 일시적으로 캐릭터 패널들에 대해 뎁스를 조정한다.
        foreach (UIWidget widget in CurrentSkillUsePnl.transform.GetComponentsInChildren<UIWidget>())
        {
                widget.depth += 4;
        }
    }

    public void SkillTargetSelect ()
    {        
        //적들이 선택되도록 한다.
        //하이라이트를 위해 일시적으로 적 패널들에 대해 뎁스를 조정한다.
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("EnemyPanel"))
        {
            obj.transform.FindChild("body/button").gameObject.SetActive(true);

            foreach (UIWidget widget in obj.transform.GetComponentsInChildren<UIWidget>())
            {
                widget.depth -= 4;
            }
        }
    }

    public void SkillCancel ()
    {
        gm.GetComponent<GameManager>().isSkillSelecting = false;
        
        //skill button은 활성화 시킨다.
        CurrentSkillUsePnl.transform.FindChild("body/SkillUseButton").gameObject.SetActive(true);

        //스킬 패널을 비활성화 시킨다.
        Skillpnl.SetActive(false);

        //하이라이트를 위해 일시적으로 캐릭터 패널들에 대해 뎁스를 조정한다.
        foreach (UIWidget widget in CurrentSkillUsePnl.transform.GetComponentsInChildren<UIWidget>())
        {
                widget.depth += 4;
        }

        //적들이 선택되도록 한다.
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("EnemyPanel"))
        {
            if (obj.transform.FindChild("body/button").gameObject.activeSelf)
            {
                foreach (UIWidget widget in obj.transform.GetComponentsInChildren<UIWidget>())
                {
                    widget.depth += 4;
                }
            }
            else
                obj.transform.FindChild("body/button").gameObject.SetActive(true);
        }
    }

    public void ShowSkillPanel (GameObject localObj)
    {
        gm.GetComponent<GameManager>().isSkillSelecting = true;

        CurrentSkillUsePnl = localObj;

        //skill button은 비활성화 시킨다.
        localObj.transform.FindChild("body/SkillUseButton").gameObject.SetActive(false);

        //스킬 패널을 활성화 시키면서 위치를 캐릭터 패널 위치로 이동시킨다.
        Skillpnl.SetActive(true);
        Skillpnl.transform.localPosition = localObj.transform.localPosition;
        
        //하이라이트를 위해 일시적으로 캐릭터 패널들에 대해 뎁스를 조정한다.
        foreach (UIWidget widget in localObj.transform.GetComponentsInChildren<UIWidget>())
        {
            widget.depth -= 4;
        }

        //적들이 선택되지 않도록 한다.
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("EnemyPanel"))
        {
            obj.transform.FindChild("body/button").gameObject.SetActive(false);
        }
    }

    public void SetCharacters ()
    {
        levelDatas = GameObject.Find("LevelData").GetComponent<LevelDatas>();

        //만일 PC라면, 게임매니저로부터 PC 리스트 (Members)를 받는다.
        if (IsPlayerSide)
            Characters = levelDatas.GetMembers(GameObject.Find("GameManager").GetComponent<GameManager>().Members);            
        else
            Characters = levelDatas.GetEnemys();
    }

    public void DeployPanels ()
    {
        //패널들을 재사용이 가능하도록 수정한다.

        if (IsPlayerSide)
        {
            CharPannels.Clear();
            CharPannels.AddRange(GameObject.FindGameObjectsWithTag("CharPanel"));
        }
        else
        {
            CharPannels.Clear();
            CharPannels.AddRange(GameObject.FindGameObjectsWithTag("EnemyPanel"));
        }
        


        for (int i = 0; i < CharPannels.Count; i++)
        {
            CharPannels[i].GetComponent<DataHandler>().ResetData();

            //만일 이미 죽은 상태였다면 idle을 실행시키고 죽은 상태를 해제한다.
            if (CharPannels[i].GetComponent<Animator>().GetBool("Dead"))
            {
                CharPannels[i].GetComponent<Animator>().SetBool("Dead", false);
                CharPannels[i].GetComponent<Animator>().SetTrigger("ResetToIdle");
                CharPannels[i].transform.FindChild("body/thumb_cover").gameObject.SetActive(false);
                CharPannels[i].transform.FindChild("body/thumb_cover").GetComponent<TweenColor>().ResetToBeginning();
            }

            if (IsPlayerSide)
                CharPannels[i].transform.localPosition = new Vector3(40, TopPos - ((this.PnlSize + this.PnlGap) * i));
            else
                CharPannels[i].transform.localPosition = new Vector3(-40, TopPos - ((this.PnlSize + this.PnlGap) * i));


            if (i > Characters.Count-1)
                CharPannels[i].SetActive(false);
            else
            {
                CharPannels[i].SetActive(true);
                CharPannels[i].GetComponent<DataHandler>().EnterDefaultInfomation(Characters[i]);
            }
                
        }
    }
    
    public void EnemySelectBtnOn ()
    {
        foreach (GameObject pnl in CharPannels)
        {
            if (pnl.GetComponent<DataHandler>().isDead)
                continue;
            pnl.transform.FindChild("body/button").gameObject.SetActive(true);
            pnl.GetComponent<DataHandler>().EnemySelectionActivate(); //선택가능 상태를 알리기 위해 반짝이는 이펙트를 줌
        }
    }
    public void EnemySelectBtnOff ()
    {
        foreach (GameObject pnl in CharPannels)
        {
            pnl.transform.FindChild("body/button").gameObject.SetActive(false);
        }
    }

    //GameManager에서 참조
    //공격대상을 정해서 알려준다.
    public GameObject TargetRandom ()
    {
        bool isAttackable = false;
        GameObject tempObj = CharPannels[0];
        while (!isAttackable)
        {
            tempObj = CharPannels[Random.Range(0, (CharPannels.Count - 1))];
            if (tempObj.activeSelf && !tempObj.GetComponent<DataHandler>().isDead)
            {
                isAttackable = true;
                return tempObj;
            }
        }

        return tempObj;
    }
}

[System.Serializable]
public class CharacterStat
{
    public string name;
    public string thumb;
    public int id, level, pow, def, spd, hp, mp;
    public SkillData Skill_1;

    public CharacterStat Copy ()
    {
        CharacterStat temp = new CharacterStat();

        temp.name = this.name;
        temp.thumb = this.thumb;
        temp.id = this.id;
        temp.level = this.level;
        temp.pow = this.pow;
        temp.def = this.def;
        temp.spd = this.spd;
        temp.hp = this.hp;
        temp.mp = this.mp;
        temp.Skill_1 = this.Skill_1;

        return temp;
    }
}