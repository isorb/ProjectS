using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DataHandler : MonoBehaviour {
    GameObject gm;

    public CharacterStat Character;

    public string CharName = "";
    public string CharThumb = "";
    
    private int id, level, pow, def, spd, hp, mp;
    public int CurHp, CurMp;
    public float CurActGuage = 0.0f;
    public float CurSkillGuage = 0.0f;
    private float SkillCastTime = 0.0f;
    private float ActTime = 1000.0f;

    Transform HP;
    Transform SP;
    
    bool isActable = false;
    bool isSkillTime = false;
    public bool isDead = false;

    GameObject ActGuage;
    //GameObject SkillBtn;

    // Use this for initialization
    void Start ()
    {
        gm = GameObject.Find("GameManager");
        ActGuage = gameObject.transform.FindChild("body/ActGuage/Guage").gameObject;
        //SkillBtn = gameObject.transform.FindChild("body/SkillBtn").gameObject;

        ActTime = gm.GetComponent<GameManager>().ActTime; // 캐릭터 행동시간 (공통, 글로벌)을 게임매니저로 부터 받아온다.

        HP = gameObject.transform.FindChild("body/HP"); // 동적으로 UI 정보를 갱신해야 하므로, 미리 메모리 로드를 해 놓는다.
        SP = gameObject.transform.FindChild("body/SP");
        
    }
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        UpdateUI();

        if (isDead) return;

        if (gm.GetComponent<GameManager>().isSkillSelecting || gm.GetComponent<GameManager>().isSkillPlaying) return;
                
        if (isSkillTime)
        {
            CurSkillGuage += Time.fixedDeltaTime;

            if (CurSkillGuage != 0 && SkillCastTime !=0)
            {
                ActGuage.transform.localScale = new Vector3(1, CurSkillGuage / SkillCastTime, 1);
            }

            if (CurSkillGuage >= SkillCastTime)
            {
                isSkillTime = false;
                SkillReadyReport();
                CurSkillGuage = 0;
            }

            return;
        }

        if (!isActable) // 캐릭터가 행동상태가 아니라면 속도가 맞춰지길 기다린다.
        {
            CurActGuage += (Time.fixedDeltaTime * spd);
            
            if (CurActGuage != 0)
                ActGuage.transform.localScale = new Vector3(1, CurActGuage/ActTime, 1);

            if (CurActGuage > ActTime)
            {
                isActable = true;
                ReadyReport();                    
            }
        }
    }

    // 스킬 준비가 되었음을 리포트 하는 방법은 큐에 올려진 스킬의 캐스팅 시간을 만땅으로 채워주는 방식이다.
    // 그럼 게임매니저에서 알아서 스킬을 써준다.
    public void SkillReadyReport()
    {
        foreach (SkillQue que in gm.GetComponent<GameManager>().SkillQueStack)
        {
            if (que.Skill == Character.Skill_1)
            {
                que.CurCastTime = CurSkillGuage;
                return;
            }                
        }
    }

    public void SkillReadyStart ()
    {
        SkillData skill = Character.Skill_1;

        SkillCastTime = skill.SkillCastingTime;
        isSkillTime = true;
    }

    public void ResetData ()
    {
        Character = null;
        CharName = ""; CharThumb = "";
        id = 0; level = 0; pow = 0; def = 0; spd = 0; hp = 0; mp = 0; CurHp = 0; CurMp = 0; CurActGuage = 0;
        isActable = false;
        isDead = false;
    }

    private void ReadyReport ()
    {
        gm.GetComponent<GameManager>().EnterQue(gameObject); //행동 순서가 되면 que에 오브젝트를 올린다.
    }
    public void SetReady ()
    {
        if (gameObject.tag == "CharPanel")
        {
            gameObject.transform.FindChild("body/FocusedCursor").gameObject.SetActive(true);
            gameObject.transform.FindChild("body/SkillUseButton").gameObject.SetActive(true);
            gameObject.GetComponent<Animator>().SetTrigger("Ready");
        }
    }

    public void EnterDefaultInfomation (CharacterStat stat)
    {
        Character = stat;

        CharName = stat.name;
        CharThumb = stat.thumb;
        id = stat.id;
        level = stat.level;
        pow = stat.pow;
        def = stat.def;
        spd = stat.spd;
        hp = stat.hp;
        mp = stat.mp;
        CurHp = hp;
        CurMp = mp;
    }

    //아군 애니메이션 이벤트용
    public void HitReport ()
    {
        gm.GetComponent<GameManager>().TargetObj.GetComponent<DataHandler>().Hit(pow);
        gm.GetComponent<GameManager>().TargetObj.GetComponent<Animator>().SetTrigger("Damaged");
    }
    //적군 애니메이션 이벤트용
    public void HitReportByEnemy()
    {
        gm.GetComponent<GameManager>().TargetObjByEnemy.GetComponent<DataHandler>().Hit(pow);
        gm.GetComponent<GameManager>().TargetObjByEnemy.GetComponent<Animator>().SetTrigger("Damaged");
    }

    //외부참조용
    public void Hit (int damage)
    {
        CurHp -= (damage-def);

        if (CurHp <= 0)
        {
            CurHp = 0;
            dead();
        }

        UpdateUI();
    }
    private void dead ()
    {
        isDead = true;
        gameObject.GetComponent<Animator>().SetBool("Dead", true);
        gameObject.transform.FindChild("body/thumb_cover").gameObject.SetActive(true);

        //dead report
        if (gameObject.tag == "CharPanel")
            gm.GetComponent<GameManager>().AlivePcCount--;
        else
            gm.GetComponent<GameManager>().AliveEnemyCount--;
    }

    public void UpdateUI ()
    {
        gameObject.transform.FindChild("body/thumb").GetComponent<UISprite>().spriteName = CharThumb;
        HP.GetComponent<UILabel>().text = CurHp + "/" + hp;
        SP.GetComponent<UILabel>().text = CurMp + "/" + mp;
    }
    
    public void Acted ()
    {
        isActable = false;
        CurActGuage = 0;
    }

    public void EnemySelectionActivate ()
    {
        gameObject.transform.FindChild("body/FocusedCursor").gameObject.SetActive(true);
    }
    public void EnemySelectionEffectOff ()
    {
        gameObject.transform.FindChild("body/FocusedCursor").gameObject.SetActive(false);
        gameObject.transform.FindChild("body/FocusedCursor").GetComponent<TweenColor>().enabled = true;
        gameObject.transform.FindChild("body/FocusedCursor").GetComponent<TweenColor>().ResetToBeginning();
    }    
}
