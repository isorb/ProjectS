using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

    public int CurrentLevel = 1;
    public List<int> Members = new List<int>();
    
    public int AlivePcCount;
    public int AliveEnemyCount;

    public float ActTime = 50.0f;
    public float ActWaitTime = 5.0f;
    private float actWaitTime = 0.0f;

    public float EndWaitTime;
    private float endWaitTime;

    public List<GameObject> PcQueStack = new List<GameObject>();
    public List<GameObject> EnemyQueStack = new List<GameObject>();
    public List<SkillQue> SkillQueStack = new List<SkillQue>();

    public GameObject PcPnls;
    public GameObject EnemyPnls;
    private GameObject ResultPnl;
    public GameObject SkillPlayer;

    private bool isEnemySelectable = false;
    public bool isBattlePlaying = false;
    public bool isSkillSelecting = false;
    public bool isSkillPlaying = false;
    private bool isEnding = false;
        
    public GameObject TargetObj;
    public GameObject TargetObjByEnemy;

    public GameObject BattleScene;

    private SkillQue skillQuePool;

    // Use this for initialization
    void Start ()
    {
        PcPnls = GameObject.Find("UserSide");
        EnemyPnls = GameObject.Find("EnemySide");
        ResultPnl = GameObject.Find("UI");
        SkillPlayer = GameObject.Find("SkillPlayer");
    }
	
    
	// Update is called once per frame
	void FixedUpdate () {
        if (isBattlePlaying)
        {
            if (!isSkillSelecting && !isSkillPlaying)
            {
                BattlePlay();
                CheckBattleContiuable();
            }
        }
        //게임이 끝나면 지정된 시간 뒤 게임이 끝나도록 진행한다.
        if (isEnding) EndBattle();

	}

    // 스킬 연출이 완료되었음을 알린다.
    public void SkillActionEnd ()
    {
        isSkillPlaying = false;
    }

    //상태를 리셋한다.
    //결과창에서 확인 버튼을 누를때 호출됨
    public void ResetGameManager ()
    {
        Members.Clear(); PcQueStack.Clear(); EnemyQueStack.Clear();
        AlivePcCount = 0; AliveEnemyCount = 0; actWaitTime = 0; endWaitTime = 0;
        ResultPnl.transform.FindChild("ResultPanel").gameObject.SetActive(false);
        isEnemySelectable = false; isBattlePlaying = false; isEnding = false;
        TargetObj = null; TargetObjByEnemy = null;
    }

    //전투가 속행가능한지 체크한다.
    public void CheckBattleContiuable()
    {
        if (AlivePcCount <= 0)
        {
            isBattlePlaying = false;
            isEnding = true;
        }
        else if (AliveEnemyCount <= 0)
        {
            isBattlePlaying = false;
            isEnding = true;
        }
    }

    private void EndBattle ()
    {
        endWaitTime += Time.fixedDeltaTime;

        if (endWaitTime >= EndWaitTime)
        {
            isEnding = false;
            ResultPnl.transform.FindChild("ResultPanel").gameObject.SetActive(true);
        }
    }


    //글로벌매니저에 의해 호출되는 거당 실제 전투를 시작시킨당
    //group은 LevelData 클래스의 id를 의미한당, Level은 group의 레벨이당, member는 유저 파티의 인원들 id를 모아놓은 배열이당
    public void SetBattleStart (int group, int level, int[] member)
    {
        BattleScene.SetActive(true);

        CurrentLevel = group;
        Members.Clear();
        Members.AddRange(member);
        
        //캐릭터 정보를 Deploy매니저에 전달한다.
        PcPnls.GetComponent<DeployManager>().SetCharacters();
        EnemyPnls.GetComponent<DeployManager>().SetCharacters();
        //Deploy매니저가 UI를 표현하도록 시킨다.
        PcPnls.GetComponent<DeployManager>().DeployPanels();
        EnemyPnls.GetComponent<DeployManager>().DeployPanels();
        //전투에 참여하는 적과 아군의 수를 기록한다.
        AlivePcCount = PcPnls.GetComponent<DeployManager>().Characters.Count;
        AliveEnemyCount = EnemyPnls.GetComponent<DeployManager>().Characters.Count;
        
        //스킬 애니메이션 용 이미지를 준비한다. (캐릭터 정보를 Deploy 매니저에 전달한 뒤에만 동작된다.)
        GameObject.Find("SkillPlayer").GetComponent<SkillPlayer>().ReadySprites();

        isBattlePlaying = true;
    }

    //캐릭터로 부터 호출되어 사용된다.
    public void PlaySkill ()
    {
        if (SkillQueStack.Count != 0)
        {
            foreach (SkillQue que in SkillQueStack)
            {
                // que.CurCastTime += Time.fixedDeltaTime;
                // que 타임 계산은 각 캐릭터별로 진행
                if (que.CurCastTime >= que.CastTime)
                {
                    SkillPlayer.GetComponent<SkillPlayer>().Play(que.Skill, que.Target); // 스킬 연출이 끝나면 시간 정지를 풀어야 함

                    if (skillQuePool == null)
                        skillQuePool = que;
                }
            }

            if (skillQuePool != null)
            {
                SkillQueStack.Remove(skillQuePool);
                skillQuePool = null;
            }
        }
    }

    private void BattlePlay ()
    {
        actWaitTime += Time.fixedDeltaTime;

        if (actWaitTime > ActWaitTime)
        {
            actWaitTime = 0.0f;
            

            if (EnemyQueStack.Count != 0)
            {
                PlayEnemyQue();
                return;
            }
            
            // 순서가 돌아온 캐릭터가 있고 아직 적 선택상태가 아니라면, 적 선택상태로 바꿔준다.
            // 순서가 돌아온 캐릭터를 준비시킨다.
               // 스킬 선택 버튼 활성화 시킬것
            // 적 그룹에 선택 버튼 활성화 명령을 내림
            if (PcQueStack.Count > 0 && !isEnemySelectable)
            {
                PcQueStack[0].GetComponent<DataHandler>().SetReady();
                EnemySelectButtonOn();
                isEnemySelectable = true;
            }
        }
    }

    public void EnterQue (GameObject obj)
    {
        if (obj.tag == "CharPanel")
        {
            PcQueStack.Add(obj);
        }            
        else
            EnemyQueStack.Add(obj);

    }

    //해당 적의 애니메이션 트리거를 On
    //해당 적이 행동을 마쳐서 다시 쿨타임을 기다릴 수 있도록 Acted 처리
    //해당 적의 공격 대상을 가져와서 현재 타겟으로 지정 (TargetObjByEnemy)
    //해당 적이 스스로 HitReport를 하도록 조치
    //해당 적은 행동했으므로 QueStack에서 제외
    public void PlayEnemyQue ()
    {
        //que에 올렸어도 실행전 캐릭터가 죽었으면 실행 안함
        if (!EnemyQueStack[0].GetComponent<DataHandler>().isDead)
        {
            EnemyQueStack[0].GetComponent<Animator>().SetTrigger("Attack");
            EnemyQueStack[0].GetComponent<DataHandler>().Acted();
            TargetObjByEnemy = PcPnls.GetComponent<DeployManager>().TargetRandom();
            EnemyQueStack[0].GetComponent<DataHandler>().HitReportByEnemy();
        }
        EnemyQueStack.RemoveAt(0);

    }

    // 그룹 전체에 선택 버튼을 활성화 하라고 시킨다.
    public void EnemySelectButtonOn ()
    {
        EnemyPnls.GetComponent<DeployManager>().EnemySelectBtnOn();
    }

    // ngui 버튼 이벤트를 받아서 타겟 오브젝트를 포인팅한다.
    public void TargetSelectReport (GameObject obj)
    {
        TargetObj = obj;
    }


    // Skill 캐스팅이 끝나면 스킬을 발동시킨다.

// 대상을 선택 버튼을 누르면 호출됨
    public void SelectTarget(GameObject target)
    {
        //만일 스킬 사용이 아니라면 공격 수행

        if (isSkillSelecting)
        {
            // 스킬 사용 유저는 캐스팅에 들어간다.
            PcQueStack[0].GetComponent<Animator>().SetTrigger("Ready");
            PcQueStack[0].GetComponent<DataHandler>().SkillReadyStart();
            // 스킬 스택에 추가한다.
            SkillQue skillQue = new SkillQue();
            skillQue.Skill = PcQueStack[0].GetComponent<DataHandler>().Character.Skill_1;
            skillQue.Target = target;
            skillQue.CastTime = skillQue.Skill.SkillCastingTime;

            SkillQueStack.Add(skillQue);
            
            // PC가 공격가능한 상태임을 알려주는 커서를 꺼준다.
            if (PcQueStack[0].tag == "CharPanel")
            {
                PcQueStack[0].transform.FindChild("body/FocusedCursor").gameObject.SetActive(false);
            }

            // 분배 관리자에 스킬 대상이 선택되었음을 알려준다. (주로 UI 처리를 시킨다.)
            PcPnls.GetComponent<DeployManager>().SkillTargetSelected();

            //스킬 캐스팅에 들어갔음을 알려준다.
            isSkillSelecting = false;

            PcQueStack[0].transform.FindChild("body/SkillUseButton").gameObject.SetActive(false);
        }
        else
        {
            // 공격한 캐릭터는 공격 애니메이션을 수행한다. 
            PcQueStack[0].GetComponent<Animator>().SetTrigger("Attack");

            // PC가 공격가능한 상태임을 알려주는 커서를 꺼준다.
            if (PcQueStack[0].tag == "CharPanel")
            {
                PcQueStack[0].transform.FindChild("body/FocusedCursor").gameObject.SetActive(false);
                PcQueStack[0].transform.FindChild("body/SkillUseButton").gameObject.SetActive(false);
            }
        }
    }

    // 공격대상을 선택 버튼을 누르면 호출됨
    //명령 수행이 완료됨
    //명령 수행한 캐릭터는 spd를 리셋시켜주고 pcQueStack에서 제함
    //만일 다음 캐릭터가 있다면, 준비시킴
    //또한 적 선택 가능 상태를 풀어준다.
    public void OrderComplete ()
    {
        PcQueStack[0].GetComponent<DataHandler>().Acted();
        PcQueStack.RemoveAt(0);
        isEnemySelectable = false;
        EnemyPnls.GetComponent<DeployManager>().EnemySelectBtnOff();
    }
}
