using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelDatas : MonoBehaviour {

    public List<ALevel> Levels = new List<ALevel>();

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    //요청된 캐릭터 데이터를 List 형태로 돌려준다.
    public List<CharacterStat> GetMembers(List<int> list)
    {
        List<CharacterStat> tempLists = new List<CharacterStat>();

        for (int i = 0; i < list.Count; i++)
        {
            List<CharacterStat> Characters;
            //GameManager\CharacterDatas 에 있는 PC캐릭터 데이터로 부터 데이터를 가져온다.

            Characters = GameObject.Find("CharacterData").GetComponent<CharacterDatas>().Characters;


            foreach (CharacterStat c in Characters)
            {
                if (c.id == list[i])
                {
                    tempLists.Add(c.Copy()); // 인스턴스 데이터를 넣어줌
                }
            }

        }


        return tempLists;
    }

    //요청된 난이도 데이터에 따른 몬스터 List를 돌려준다.
    public List<CharacterStat> GetEnemys()
    {
        List<CharacterStat> tempLists = new List<CharacterStat>();

        List<int> EnemyLists = new List<int>();

        //1. 현재 난이도 정보를 가져온다.
        //2. 난이도 데이터에서 현재 난이도 정보와 일치하는 데이터를 가져온다.
        //3. EnemyData 에서 2.의 데이터에서 필요로 하는 값들을 가져와 List화 한다.
        //4. 3.에서 리스트화 한 데이터를 return 한다. (즉, 몬스터 리스트를 돌려준다.)

        //1
        int levelNo = GameObject.Find("GameManager").GetComponent<GameManager>().CurrentLevel;

        //2
        for (int i = 0; i < Levels.Count; i++)
        {
            if (Levels[i].Level == levelNo)
            {
                EnemyLists = Levels[i].CharacterIndexes;
            }
        }

        //3
        for (int i = 0; i < EnemyLists.Count; i++)
        {
            List<CharacterStat> Characters;

            Characters = GameObject.Find("CharacterData").GetComponent<CharacterDatas>().Enemys;

            foreach (CharacterStat c in Characters)
            {
                if (c.id == EnemyLists[i])
                {
                    tempLists.Add(c.Copy()); // 인스턴스 데이터를 넣어줌
                }
            }

        }

        return tempLists;
    }
}

[System.Serializable]
public class ALevel
{
    public int Level;
    public List<int> CharacterIndexes = new List<int>();
}
