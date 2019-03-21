using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class GameManager : MonoBehaviour {
	public static int MAX_CHARACTER_NUMBER = 7;
	public static float Y_OF_PLAYER1= 2.5f;
	public static float Y_OF_PLAYER2= -2.5f;

	public Texture[] texture;
	public Texture[] texture_d;
    public Texture[] texture_name;

	public GameObject[] cardObject;
	public GameObject selectBoardObject;
	public GameObject bloodObject;
	public GameObject skillButtonsObject;
    public GameObject skillButton1;
    public GameObject skillButton2;
    public GameObject skillButton3;
	public Vector3 skillButtonsDefaultPos;
	public Text text;
	public GameObject fireObject;
	//默认位置坐标
	public Vector3[] defaultPos;
	public int curSelectCardIndex;

	public SIDE isWhoTurn;
	public GAME_STAGE gameStage;

	public int chooseEnd;

	public GameObject endTurnButtonObj;

	public bool isBlood;

    public bool isSkill6_3Used;

	//攻击者序号 1-7
	public int attackIndex;
	//目标序号 1-7
	public int targetIndex;
	//是否选择攻击者
	public bool isSelectAttacker;

    public int skillIndex;

    public bool isPress;
    public float pressTime;
    public bool isShowTime;
    public int showCardIndex;

    GameObject MainCamera;

	public enum GAME_STAGE{
		CHOOSE,
		FIGHT,
		END
	};

	public enum MP_MODE{
		NORMAL,
		INCREASE,
		DECREASE,
		EXTRACT
	};

	//游戏哪方的回合
	public enum SIDE
	{
		NONE_SIDE,
		P1_SIDE,
		P2_SIDE
	};
		
	public delegate void SkillHandler();

	//角色属性类
	public class characterStatus{
		public string name;
		public int HP;
		public int curHP;
		public int MP;
		public int curMP;
		public int ATK;
		public int curATK;
		public Vector3 curPos;
		public SIDE side;
		public int motion;
        public int index;

		public static SkillHandler[] MainSkillList;

		public characterStatus(){
			name="";
			HP=1;
			curHP=HP;
			MP=1;
			curMP=MP;
			ATK=1;
			curATK=ATK;
			side=SIDE.NONE_SIDE;
			motion = 1;
		}

		public characterStatus(string argName, int argHP, int argMP, int argATK, SIDE argSide, int argIndex){
			name=argName;
			HP=argHP;
			curHP=HP;
			MP=argMP;
			curMP=0;
			ATK=argATK;
			curATK=ATK;
			side=argSide;
			motion = 1;
            index = argIndex;
		}

		public void Attack(characterStatus attackTarget){
			print ("attack");
			attackTarget.curHP -= curATK;
			if (attackTarget.curHP < 0) {
				attackTarget.curHP = 0;
			}
		}
	}

    public delegate void skill(int attackIndex, int targetIndex);
    public static skill[] skillArray;

    public void skill1_1(int attackIndex, int targetIndex){
        int atk = characterList[attackIndex - 1].curATK;
        characterList[attackIndex - 1].curATK = characterList[targetIndex - 1].curATK;
        characterList[targetIndex - 1].curATK = atk;
        characterList[attackIndex - 1].curMP -= 6;
    }

    public void skill1_2(int attackIndex, int targetIndex)
    {
        int hp = characterList[attackIndex - 1].curHP;
        characterList[attackIndex - 1].curHP = characterList[targetIndex - 1].curHP;
        characterList[targetIndex - 1].curHP = hp;
        characterList[attackIndex - 1].curMP -= 6;
    }

    public void skill1_3(int attackIndex, int targetIndex)
    {
        for (int i = 0; i < 6;)
        {
            int index = UnityEngine.Random.Range(1, 7);
            if (characterList[index].side != isWhoTurn && characterList[index].curHP>0)
            {
                characterList[index].curHP--;
                i++;
            }
        }
        characterList[attackIndex - 1].curMP -= 6;
    }

    public void skill2_1(int attackIndex, int targetIndex)
    {
    }

    public void skill2_2(int attackIndex, int targetIndex)
    {
        int hit = UnityEngine.Random.Range(0, 10);
        if (hit > 5)
        {
            characterList[targetIndex - 1].motion=-1;
        }
    }

    public void skill2_3(int attackIndex, int targetIndex)
    {
        foreach (characterStatus c in characterList)
        {
            if (c.side != characterList[attackIndex - 1].side)
            {
                c.motion=-1;
            }
        }
        characterList[attackIndex - 1].curMP = 0;
    }

    public void skill3_1(int attackIndex, int targetIndex)
    {
        if (characterList[3 - 1].curMP < 9)
        {
            characterList[3 - 1].curMP++;
        }
        if (characterList[3 - 1].curATK < 9)
        {
            characterList[3 - 1].curATK =1 + characterList[3 - 1].curMP/3;
        }
    }

    public void skill3_2(int attackIndex, int targetIndex)
    {
        if (characterList[3 - 1].curMP >= 3)
        {
            int halfTotalHP = (int)Math.Ceiling(Convert.ToDouble(characterList[3 - 1].curHP + characterList[targetIndex - 1].curHP)/2);
            characterList[3 - 1].curHP=halfTotalHP;
            characterList[targetIndex - 1].curHP=halfTotalHP;
            characterList[3 - 1].curMP -= 3;
            characterList[attackIndex - 1].motion--;
        }
    }

    public void skill3_3(int attackIndex, int targetIndex)
    {
        characterList[targetIndex - 1].curHP -= characterList[attackIndex - 1].curMP / 2 + characterList[attackIndex -1].curATK;
        if (characterList[targetIndex - 1].curHP < 0)
        {
            characterList[targetIndex - 1].curHP = 0;
        }
        characterList[attackIndex - 1].curMP = 0;
        characterList[attackIndex - 1].curHP = characterList[attackIndex - 1].curHP / 2;
    }

    public void skill4_1(int attackIndex, int targetIndex)
    {
        if (characterList[attackIndex - 1].curMP == 4)
        {
            characterList[attackIndex - 1].curMP -= 4;
            characterList[targetIndex - 1].curATK = 1;
            if (targetIndex == 3)
            {
                characterList[targetIndex - 1].curMP = 3;
            }
        }
    }

    public void skill4_2(int attackIndex, int targetIndex)
    {
        characterList[targetIndex - 1].curHP -= characterList[attackIndex - 1].curMP;
        characterList[attackIndex - 1].curMP = 0;
        if (characterList[targetIndex - 1].curHP < 0)
        {
            characterList[targetIndex - 1].curHP = 0;
        }        
    }

    public void skill4_3(int attackIndex, int targetIndex)
    {
        if (characterList[attackIndex - 1].curMP == 4)
        {
            characterList[attackIndex - 1].curMP -= 4;
            characterList[attackIndex - 1].curHP += 4;
            if (characterList[attackIndex - 1].curHP > 9)
            {
                characterList[attackIndex - 1].curHP = 9;
            }
        }
    }

    public void skill5_1(int attackIndex, int targetIndex)
    {
        if (characterList[attackIndex - 1].curMP >= 3)
        {
            characterList[attackIndex - 1].curMP -=3;
            characterList[targetIndex - 1].curHP += 3;
            if (characterList[targetIndex - 1].curHP > 9)
            {
                characterList[targetIndex - 1].curHP = 9;
            }
            characterList[attackIndex - 1].motion--;
        }
    }

    public void skill5_2(int attackIndex, int targetIndex)
    {
        characterList[targetIndex - 1].curMP += characterList[attackIndex - 1].curMP;
        characterList[attackIndex - 1].curMP = 0;
            
        if (characterList[targetIndex - 1].curMP > 9)
        {
            characterList[targetIndex - 1].curMP = 9;
        }
        characterList[attackIndex - 1].motion--;
    }

    public void skill5_3(int attackIndex, int targetIndex)
    {
        characterList[attackIndex - 1].curMP += 2;
        if (characterList[attackIndex - 1].curMP > 9)
        {
            characterList[attackIndex - 1].curMP = 9;
        }
        characterList[attackIndex - 1].motion--;
    }

    public void skill6_1(int attackIndex, int targetIndex)
    {
    }

    public void skill6_2(int attackIndex, int targetIndex)
    {
        if (characterList[attackIndex - 1].curMP >= 6)
        {
            characterList[targetIndex - 1].curHP = characterList[targetIndex - 1].HP / 2;
            characterList[targetIndex - 1].curMP /= 2;
            characterList[targetIndex - 1].curATK = 1;
            characterList[attackIndex - 1].curMP -= 6;
        }
    }

    public void skill6_3(int attackIndex, int targetIndex)
    {
        if (characterList[attackIndex - 1].curMP == 9 && isSkill6_3Used==false)
        {
            foreach (characterStatus c in characterList)
            {
                if (c.side == characterList[attackIndex - 1].side)
                {
                    c.curHP = c.HP;
                    c.curMP = 0;
                    c.curATK = c.ATK;
                    GameObject.Find("card" + (c.index + 1)).GetComponent<Renderer>().material.mainTexture = texture[c.index];
                }
            }
            isSkill6_3Used = true;
        }
    }

    public void skill7_1(int attackIndex, int targetIndex)
    {
        characterList[attackIndex - 1].curHP += characterList[attackIndex - 1].curATK;
        if (characterList[attackIndex - 1].curHP > 9)
        {
            characterList[attackIndex - 1].curHP = 9;
        }
        characterList[targetIndex - 1].curMP -= characterList[attackIndex - 1].curATK;
        if (characterList[targetIndex - 1].curMP < 0)
        {
            characterList[targetIndex - 1].curMP = 0;
        }
    }

    public void skill7_2(int attackIndex, int targetIndex)
    {
        if (characterList[attackIndex - 1].curMP >= 6)
        {
            characterList[attackIndex - 1].curMP -= 6;
            characterList[targetIndex - 1].curHP -= characterList[targetIndex - 1].curMP / 2;
            if (characterList[targetIndex - 1].curHP < 0)
            {
                characterList[targetIndex - 1].curHP = 0;
            }
        }
    }

    public void skill7_3(int attackIndex, int targetIndex)
    {
        if (characterList[attackIndex - 1].curMP >= 9)
        {
            characterList[attackIndex - 1].curMP -= 9;
            if (characterList[targetIndex - 1].curHP <= 3)
            {
                characterList[attackIndex - 1].curMP += characterList[targetIndex - 1].curMP;
                characterList[attackIndex - 1].curHP += characterList[targetIndex - 1].curHP;
                characterList[attackIndex - 1].curATK += characterList[targetIndex - 1].curATK;
                if (characterList[attackIndex - 1].curMP > 9)
                {
                    characterList[attackIndex - 1].curMP = 9;
                }
                if (characterList[attackIndex - 1].curHP > 9)
                {
                    characterList[attackIndex - 1].curHP = 9;
                }
                if (characterList[attackIndex - 1].curATK > 9)
                {
                    characterList[attackIndex - 1].curATK = 9;
                }
                characterList[targetIndex - 1].curHP = 0;
            }

        }
    }

	public characterStatus[] characterList;

    public static string[,] skillName=new string[7,3]{
        {"化物之力",
            "不朽之躯",
            "灵界之主"},
        {"三尾·灵动",
            "六尾·魅惑",
            "九尾·倾城"},
        {"血怒",
            "血契",
            "炎杀黑龙波"},
        {"平和",
            "墨剑",
            "破卷"},
        {"愈合",
            "滋养",
            "激活"},
        {"复苏",
            "衰老",
            "时光倒流"},
        {"饮血",
            "反噬",
            "收割"},
    };

    public static string[,] skillDetail = new string[7, 3]{
        {"化物之力,消耗6MP,和目标交换攻击力",
            "不朽之躯,消耗6MP,和目标交换当前生命值",
            "灵界之主,消耗6MP,对随机目标总共造成6点伤害"},
        {"三尾·灵动,被动技能,3MP之后激活,被攻击时50%几率躲闪",
            "六尾·魅惑,被动技能,6MP之后激活,攻击时50%几率让目标下回合无法行动",
            "九尾·倾城,消耗9MP,下回合对方所有人员无法行动"},
        {"血怒,被动技能,角色每有3点怒气增加1点攻击力",
            "血契,消耗3怒气,和己方目标平分血量总和",
            "炎杀黑龙波,消耗所有怒气和一半当前生命值,伤害为攻击力加上每两点怒气多一点伤害"},
        {"平和,消耗4MP,强制目标攻击力为1",
            "墨剑,消耗所有MP,对目标造成等量伤害",
            "破卷,消耗4MP,回复自己等量生命值"},
        {"愈合,消耗3MP,回复己方目标3生命值",
            "滋养,消耗所有MP,增加给队友相等的MP",
            "激活,回复自身2MP"},
        {"复苏,被动技能,己方回合结束,自己增加一点生命值",
            "衰老,消耗6MP,强制目标攻击力为1,血量为开局生命值的一半,法力值减半",
            "时光倒流,消耗9MP,所有队友回复到开局状态,每局游戏限定只能使用一次"},
        {"饮血,被动技能,每次攻击对目标生命和法力造成同等伤害,自身增加对应的生命",
            "反噬,消耗6MP,目标受到自身法力一半的伤害",
            "收割,消耗9MP,立刻杀死生命值小于等于3的目标,自身增加被杀死目标的生命,法力,攻击力"},
    };

    public static string[] characterName = new string[7]{
        "桑不语","苏妍","白秋夜","冉墨","徐蕾","宁小月","夏夕雪"
    };

    public static Color[] colorName = new Color[]{
      new Color(220,0,0),
      new Color(250,125,125),
      new Color(255,215,0),
      new Color(0,0,0),
      new Color(0,220,0),
      new Color(0,191,255),
      new Color(255,0,255)
    };

    //技能类型，true为主动技能，false为被动技能
    public static bool[,] skillType = new bool[7, 3]{
        {true,true,true},
        {false,false,true},
        {false,true,true},
        {true,true,true},
        {true,true,true},
        {false,true,true},
        {false,true,true},
    };

	// Use this for initialization
	void Start () {
		isWhoTurn = SIDE.P1_SIDE;
		gameStage = GAME_STAGE.CHOOSE;
		chooseEnd = 0;
		isSelectAttacker = false;
		isBlood = false;
        isSkill6_3Used = false;
     
		texture = new Texture[MAX_CHARACTER_NUMBER];
		texture_d = new Texture[MAX_CHARACTER_NUMBER];
        texture_name = new Texture[MAX_CHARACTER_NUMBER];
		defaultPos = new Vector3[MAX_CHARACTER_NUMBER];
		characterList = new characterStatus[MAX_CHARACTER_NUMBER];
		curSelectCardIndex = 0;
		attackIndex = -1;
		targetIndex = -1;
        skillIndex = 0;
        isPress = false;
        pressTime = 0f;
        isShowTime = false;
        MainCamera = GameObject.Find("Main Camera");

		initAllCharacterStatus ();
        
        skillArray = new skill[]{
            skill1_1,skill1_2,skill1_3,
            skill2_1,skill2_2,skill2_3,
            skill3_1,skill3_2,skill3_3,
            skill4_1,skill4_2,skill4_3,
            skill5_1,skill5_2,skill5_3,
            skill6_1,skill6_2,skill6_3,
            skill7_1,skill7_2,skill7_3
        };


		//初始默认位置和图案
		for (int n = 0; n < MAX_CHARACTER_NUMBER; n++) {
			texture[n] = (Texture)Resources.Load("card"+(n+1));
			texture_d[n] = (Texture)Resources.Load("card"+(n+1)+"_d");
            texture_name[n] = (Texture)Resources.Load("name"+(n+1));
			defaultPos [n] = new Vector3 (-7.5f+n*2.5f,0f,0f);
			characterList [n].curPos = defaultPos[n];
		}
	

		cardObject = GameObject.FindGameObjectsWithTag("Card");	
		endTurnButtonObj = GameObject.Find ("EndTurnButton");
		selectBoardObject = GameObject.Find ("SelectBoard");
		bloodObject = GameObject.Find ("Blood");
		text = GameObject.Find ("TurnText").GetComponent<Text> ();
		text.text = "";
		skillButtonsObject = GameObject.Find ("SkillButtons");
		skillButtonsDefaultPos = skillButtonsObject.transform.position;
		hideSkillButtons ();
		fireObject = GameObject.Find ("fire");
        skillButton1 = GameObject.Find("Skill1");
        skillButton2 = GameObject.Find("Skill2");
        skillButton3 = GameObject.Find("Skill3");

		int i=0;
		foreach (GameObject c in cardObject) {
			i =int.Parse( c.name.Substring (4, 1));
			c.transform.position = defaultPos[i-1];
			c.GetComponent<Renderer>().material.mainTexture = texture[i-1];
            c.GetComponentInChildren<Transform>().Find("Name").GetComponent<Renderer>().material.mainTexture = texture_name[i - 1];
		}

		updateCardValue ();

		selectCard (curSelectCardIndex);
	}

	public void hideSkillButtons(){
		Vector3 pos = skillButtonsDefaultPos;
		pos.x = 10000f;
		skillButtonsObject.transform.position = pos;
        skillIndex = 0;
	}

	public void showSkillButtons(){
		skillButtonsObject.transform.position = skillButtonsDefaultPos;
        skillIndex = 0;
        skillButton1.GetComponentInChildren<Text>().text = skillName[attackIndex - 1, 0];
        skillButton2.GetComponentInChildren<Text>().text = skillName[attackIndex - 1, 1];
        skillButton3.GetComponentInChildren<Text>().text = skillName[attackIndex - 1, 2];
	}

	//更新卡的数值
	public void updateCardValue(){
		int i=0;

		foreach (GameObject c in cardObject) {
			//print (c.name);
			i =int.Parse( c.name.Substring (4, 1));
			//print (i);
			c.GetComponentInChildren<Transform> ().Find ("HP").GetComponent<Renderer> ().material.SetTextureOffset ("_MainTex", new Vector2(0.1f*characterList[i-1].curHP,0f));
			c.GetComponentInChildren<Transform> ().Find ("MP").GetComponent<Renderer> ().material.SetTextureOffset ("_MainTex", new Vector2(0.1f*characterList[i-1].curMP,0f));
			c.GetComponentInChildren<Transform> ().Find ("ATK").GetComponent<Renderer> ().material.SetTextureOffset ("_MainTex", new Vector2(0.1f*characterList[i-1].curATK,0f));
		}		
	}

	//新回合状态重置
	public void setNewTurnStatus(){
		foreach (GameObject c in cardObject) {
			c.transform.localScale = new Vector3 (2f , 3f , 0.1f );
		}
		foreach (characterStatus m in characterList) {
            if (m.side == isWhoTurn)
            {
                m.motion++;
            }
            if (m.motion > 1)
            {
                m.motion = 1;
            }
		}

		isSelectAttacker = true;
		curSelectCardIndex = 0;
		attackIndex = -1;
		targetIndex = -1;
		//隐藏选择框
		Vector3 pos1 = selectBoardObject.transform.position;
		pos1.z = 1;
		selectBoardObject.transform.position = pos1;

		//隐藏血滴
		Vector3 pos2 = bloodObject.transform.position;
		pos2.z = 3;
		bloodObject.transform.position = pos2;
	}

	// Update is called once per frame
	void Update () {
        if (isPress)
        {
            pressTime += Time.deltaTime;
            if (pressTime > 3.0f)
            {
                GameObject.Find("card" + showCardIndex).transform.localScale = new Vector3(2f * 3f, 3f * 3f, 0.1f * 3f);
                GameObject.Find("card" + showCardIndex).transform.position = new Vector3(-20f, 0f, 0f);
                pressTime = 0f;
                isPress = false;
                isShowTime = true;
                Vector3 cameraPos = MainCamera.transform.position;
                cameraPos.x -= 20;
                MainCamera.transform.position = cameraPos;
                hideSkillButtons();
                GameObject.Find("showtext").GetComponentInChildren<Text>().color = colorName[showCardIndex - 1];
                GameObject.Find("skilltext1").GetComponentInChildren<Text>().color = colorName[showCardIndex - 1];
                GameObject.Find("skilltext2").GetComponentInChildren<Text>().color = colorName[showCardIndex - 1];
                GameObject.Find("skilltext3").GetComponentInChildren<Text>().color = colorName[showCardIndex - 1];
                GameObject.Find("showtext").GetComponentInChildren<Text>().text=characterName[showCardIndex-1];
                GameObject.Find("skilltext1").GetComponentInChildren<Text>().text = skillDetail[showCardIndex - 1,0];
                GameObject.Find("skilltext2").GetComponentInChildren<Text>().text = skillDetail[showCardIndex - 1,1];
                GameObject.Find("skilltext3").GetComponentInChildren<Text>().text = skillDetail[showCardIndex - 1,2];
            }
        }
		if (Input.GetMouseButtonDown (0)) {
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit raycastHit = new RaycastHit ();
			if (Physics.Raycast (ray, out raycastHit)) {
				string objName = raycastHit.collider.gameObject.name;
				if (objName.Contains ("card")) {
                    showCardIndex = int.Parse(objName.Substring(4, 1));
                    if (!isShowTime)
                    {
                        isPress = true;
                    }

				}
                if (objName.Contains("EndTurnButton"))
                {
                    changeTurn();
                    setNewTurnStatus();

                }
			}
            if (isShowTime)
            {
                GameObject.Find("card" + showCardIndex).transform.position = characterList[showCardIndex-1].curPos;
                GameObject.Find("card" + showCardIndex).transform.localScale = new Vector3(2f, 3f, 0.1f);
                isShowTime = false;
                Vector3 cameraPos = MainCamera.transform.position;
                cameraPos.x = 0;
                MainCamera.transform.position = cameraPos;
                if (curSelectCardIndex != 0 && gameStage == GAME_STAGE.FIGHT)
                {
                    showSkillButtons();
                }
            }
		}
        //print(Input.GetMouseButtonUp(0)+" "+isShowTime + " " + isPress);
        if (Input.GetMouseButtonUp(0) && !isShowTime && isPress)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit raycastHit = new RaycastHit();
            if (Physics.Raycast(ray, out raycastHit))
            {
                string objName = raycastHit.collider.gameObject.name;
                if (objName.Contains("card"))
                {
                    unselectCard(curSelectCardIndex);
                    selectCard(int.Parse(objName.Substring(4, 1)));
                    isPress = false;

                }
            }
        }

		if (Input.GetKeyDown (KeyCode.Escape)) {
			Application.Quit ();
		}

		if (gameStage == GAME_STAGE.FIGHT) {
			endTurnButtonObj.transform.Rotate(30*Time.deltaTime,0,0);
			if (isBlood) {
				Vector3 pos = bloodObject.transform.position;
				pos.y += 0.5f * Time.deltaTime;
				pos.z += 1 * Time.deltaTime;
				bloodObject.transform.position = pos;
				if(pos.z > 0.5f){
					isBlood = false;
					pos.z = 3;
					bloodObject.transform.position = pos;
				}
			}
		}
	}

	//初始化所有角色属性
	public void initAllCharacterStatus(){
		characterList[0] = new characterStatus("", 8, 9, 1, SIDE.NONE_SIDE, 0);
		characterList[1] = new characterStatus("", 7, 9, 1, SIDE.NONE_SIDE, 1);
		characterList[2] = new characterStatus("", 9, 0, 1, SIDE.NONE_SIDE, 2);
        characterList[3] = new characterStatus("", 6, 4, 0, SIDE.NONE_SIDE, 3);
        characterList[4] = new characterStatus("", 8, 6, 1, SIDE.NONE_SIDE, 4);
        characterList[5] = new characterStatus("", 5, 9, 1, SIDE.NONE_SIDE, 5);
        characterList[6] = new characterStatus("", 7, 9, 1, SIDE.NONE_SIDE, 6);


	}

	//选中某张卡,index 1-7
	public void selectCard(int index){
		//print (index);
		if (index > 0 && index < 8) {
			foreach (GameObject c in cardObject) {
				if (int.Parse (c.name.Substring (4, 1)) == index) {
					curSelectCardIndex = index;
					//选卡阶段
                    if (gameStage == GAME_STAGE.CHOOSE && characterList[index - 1].side==SIDE.NONE_SIDE)
                    {
						chooseEnd++;
						characterList [index - 1].side = isWhoTurn;
						Vector3 pos = c.transform.position;
						if (isWhoTurn == SIDE.P1_SIDE) {
							pos.y = Y_OF_PLAYER1;
						} else {
							pos.y = Y_OF_PLAYER2;
							if (chooseEnd == 6) {
								foreach (GameObject a in cardObject) {
									Vector3 pos_a = a.transform.position;
                                    if (characterList[int.Parse(a.name.Substring(4, 1)) - 1].side == SIDE.P1_SIDE)
                                    {
                                        if (characterList[int.Parse(a.name.Substring(4, 1)) - 1].curHP < 9)
                                        {
                                            characterList[int.Parse(a.name.Substring(4, 1)) - 1].curHP++;
                                        }
                                        characterList[int.Parse(a.name.Substring(4, 1)) - 1].curMP=2;
                                        print(int.Parse(a.name.Substring(4, 1)));
                                    }
									if(pos_a.y==0f){
										pos_a.y = Y_OF_PLAYER2;
										a.transform.position = pos_a;
										characterList [int.Parse (a.name.Substring (4, 1))-1].side = SIDE.P2_SIDE;
										characterList [int.Parse (a.name.Substring (4, 1))-1].curPos = pos_a;
										gameStage = GAME_STAGE.FIGHT;
										text.text = "Player1's Turn";
										Vector3 posEndTurnButton = endTurnButtonObj.transform.position;
										posEndTurnButton.z = 0;
										endTurnButtonObj.transform.position = posEndTurnButton;
										fireObject.transform.position = posEndTurnButton;
										unselectCard (curSelectCardIndex);
										isSelectAttacker = true;
									}
								}
							}
						}
						c.transform.position = pos;
						characterList [index - 1].curPos = pos;
						changeTurn ();
					}
					//战斗阶段
					else if (gameStage == GAME_STAGE.FIGHT) {
						print ("GAME_STAGE.FIGHT");
                        print("motion ="+ characterList[curSelectCardIndex - 1].motion);
						if (isSelectAttacker == true) {
                            //card3使用技能2，目标是己方card
                            if (attackIndex == 3 && skillIndex == 2 && characterList[curSelectCardIndex - 1].side == isWhoTurn)
                            {
                                skill3_2(attackIndex, curSelectCardIndex);
                                updateCardValue();
                                unselectCard(index);
                                //隐藏选择框
                                Vector3 pos = selectBoardObject.transform.position;
                                pos.z = 1;
                                selectBoardObject.transform.position = pos;
                                attackIndex = -1;
                                hideSkillButtons();
                            }
                            //card5使用技能1，目标是己方card
                            else if (attackIndex == 5 && skillIndex == 1 && characterList[curSelectCardIndex - 1].side == isWhoTurn)
                            {
                                skill5_1(attackIndex, curSelectCardIndex);
                                updateCardValue();
                                unselectCard(index);
                                //隐藏选择框
                                Vector3 pos = selectBoardObject.transform.position;
                                pos.z = 1;
                                selectBoardObject.transform.position = pos;
                                attackIndex = -1;
                                hideSkillButtons();
                            }
                            //card5使用技能2，目标是己方card
                            else
                                if (attackIndex == 5 && skillIndex == 2 && characterList[curSelectCardIndex - 1].side == isWhoTurn)
                                {
                                    skill5_2(attackIndex, curSelectCardIndex);
                                    updateCardValue();
                                    unselectCard(index);
                                    //隐藏选择框
                                    Vector3 pos = selectBoardObject.transform.position;
                                    pos.z = 1;
                                    selectBoardObject.transform.position = pos;
                                    attackIndex = -1;
                                    hideSkillButtons();
                                }
                                //card5使用技能3，目标是己方card
                                else
                                    if (attackIndex == 5 && skillIndex == 3 && characterList[curSelectCardIndex - 1].side == isWhoTurn)
                                    {
                                        skill5_3(attackIndex, curSelectCardIndex);
                                        updateCardValue();
                                        unselectCard(index);
                                        //隐藏选择框
                                        Vector3 pos = selectBoardObject.transform.position;
                                        pos.z = 1;
                                        selectBoardObject.transform.position = pos;
                                        attackIndex = -1;
                                        hideSkillButtons();
                                    }
                                    //卡牌属于行动方，血量大于0，行动力大于0才能选中
                                    else if (characterList[curSelectCardIndex - 1].side == isWhoTurn
                                        && characterList[curSelectCardIndex - 1].curHP > 0
                                        && characterList[curSelectCardIndex - 1].motion > 0)
                                    {
                                        c.transform.localScale = new Vector3(2f * 1.2f, 3f * 1.2f, 0.1f * 1f);
                                        selectAttacker(c, curSelectCardIndex);
                                        showSkillButtons();
                                    }
                                    else
                                    {
                                        if (attackIndex > -1 && characterList[curSelectCardIndex - 1].side != isWhoTurn && characterList[curSelectCardIndex - 1].curHP > 0)
                                        {
                                            selectTarget(c, curSelectCardIndex);
                                            hideSkillButtons();
                                        }
                                    }
						}
                        winOrLose();
					}
					break;
				}
			}
		}
	}

	public void unselectCard(int index){
		//print (index);
		if (index > 0 && index < 8) {
			foreach (GameObject c in cardObject) {
				if (int.Parse (c.name.Substring (4, 1)) == index) {
					c.transform.localScale = new Vector3 (2f , 3f , 0.1f );
					curSelectCardIndex = 0;
					break;
				}
			}
		}
	}

	//更换回合
	public void changeTurn(){
		Vector3 posEndTurnButton = endTurnButtonObj.transform.position;
        if (gameStage == GAME_STAGE.FIGHT)
        {
            for (int i = 0; i < 7; i++)
            {
                if (i != 2 && characterList[i].curMP < characterList[i].MP && characterList[i].curHP>0)
                {
                    characterList[i].curMP++;
                }
            }
            if (characterList[5].curHP < 9 && characterList[5].side == isWhoTurn && characterList[5].curHP > 0)
            {
                characterList[5].curHP++;
            }
        }
        if (isWhoTurn == SIDE.P1_SIDE)
        {
            isWhoTurn = SIDE.P2_SIDE;
            posEndTurnButton.x = -8;
            endTurnButtonObj.transform.position = posEndTurnButton;
            fireObject.transform.position = posEndTurnButton;
            if (gameStage == GAME_STAGE.FIGHT)
            {
                text.text = "Player2's Turn";
            }
        }
        else
        {
            isWhoTurn = SIDE.P1_SIDE;
            posEndTurnButton.x = 8;
            endTurnButtonObj.transform.position = posEndTurnButton;
            fireObject.transform.position = posEndTurnButton;
            if (gameStage == GAME_STAGE.FIGHT)
            {
                text.text = "Player1's Turn";
            }
        }
        updateCardValue();

        //技能按键恢复大小
        Vector3 v2 = new Vector3(1f, 1f, 1f);
        skillButton1.transform.localScale = v2;
        skillButton2.transform.localScale = v2;
        skillButton3.transform.localScale = v2;
        hideSkillButtons();
	}

	public void selectAttacker(GameObject obj, int index){
		obj.transform.localScale = new Vector3 (2f * 1.2f, 3f * 1.2f, 0.1f * 1f );
		//显示选择框
		Vector3 pos = obj.transform.position;
		print (selectBoardObject.transform.position);
		print (pos);
		selectBoardObject.transform.position = pos;
		print (selectBoardObject.transform.position);
		attackIndex = index;
	}

	public void selectTarget(GameObject obj, int index){
        print(skillIndex + " " + attackIndex);
        if (skillIndex == 0)
        {
            if (index == 2 && characterList[index - 1].curMP >= 3)
            {
                
                int hit = UnityEngine.Random.Range(0, 10);
                if (hit > 5)
                {
                    characterList[attackIndex - 1].Attack(characterList[index - 1]);
                }
            }
            //若为card3则攻击增加，怒气增加
            else if (attackIndex == 3 || index == 3)
            {
                skill3_1(attackIndex, index);
                characterList[attackIndex - 1].Attack(characterList[index - 1]);
            }
            else if (attackIndex == 7)
            {
                skill7_1(attackIndex, index);
                characterList[attackIndex - 1].Attack(characterList[index - 1]);
            }
            else if (attackIndex == 2 && characterList[attackIndex - 1].curMP >= 6)
            {
                skill2_2(attackIndex, index);
                characterList[attackIndex - 1].Attack(characterList[index - 1]);
            }
            else
            {
                characterList[attackIndex - 1].Attack(characterList[index - 1]);
            }
        }

        //若为card1使用技能
        if (attackIndex == 1)
        {
            if (skillIndex == 1 && characterList[attackIndex - 1].curMP >= 6)
            {
                skill1_1(attackIndex, index);
            }
            if (skillIndex == 2 && characterList[attackIndex - 1].curMP >= 6)
            {
                skill1_2(attackIndex, index);
            }
            if (skillIndex == 3 && characterList[attackIndex - 1].curMP >= 6)
            {
                skill1_3(attackIndex, index);
            }
        }

        //若为card2使用技能3
        if (attackIndex == 2 && skillIndex == 3 && characterList[attackIndex - 1].curMP == 9)
        {
            print("skill2_3");
            skill2_3(attackIndex, index);
        }

        //若为card3使用技能3
        if (attackIndex == 3 && skillIndex == 3)
        {
            skill3_3(attackIndex, index);
        }

        //若为card4使用技能1
        if (attackIndex == 4 && skillIndex == 1)
        {
            skill4_1(attackIndex, index);
        }
        //若为card4使用技能1
        if (attackIndex == 4 && skillIndex == 2)
        {
            skill4_2(attackIndex, index);
        }
        //若为card4使用技能1
        if (attackIndex == 4 && skillIndex == 3)
        {
            skill4_3(attackIndex, index);
        }
        //若为card6
        if (attackIndex == 6)
        {
            if (skillIndex == 2)
            {
                skill6_2(attackIndex, index);
            }
            if (skillIndex == 3)
            {
                skill6_3(attackIndex, index);
            }
        }

        //若为card7
        if (attackIndex == 7)
        {
            if (skillIndex == 2)
            {
                skill7_2(attackIndex, index);
            }
            if (skillIndex == 3)
            {
                skill7_3(attackIndex, index);
            }
        }

        characterList[attackIndex - 1].motion--;
		updateCardValue ();
		//目标流血，显示血滴
		isBlood = true;
		Vector3 bloodPos = obj.transform.position;
		bloodPos.z = -3;
		bloodObject.transform.position = bloodPos;

		//血量为0，触发死亡事件
        foreach (characterStatus c in characterList) { 
		    if (c.curHP == 0) {
			    GameObject.Find("card"+(c.index+1)).GetComponent<Renderer>().material.mainTexture = texture_d[c.index];
		    }
        }

		unselectCard (index);
		//隐藏选择框
		Vector3 pos = selectBoardObject.transform.position;
		pos.z = 1;
		selectBoardObject.transform.position = pos;
		attackIndex = -1;

        //winOrLose();
	}

    public void winOrLose()
    {
        int sideP1HP = 0;
        int sideP2HP = 0;
        foreach (characterStatus c in characterList)
        {
            if (c.side == SIDE.P1_SIDE)
            {
                sideP1HP += c.curHP;
            }
            if (c.side == SIDE.P2_SIDE)
            {
                sideP2HP += c.curHP;
            }
        }

        if (sideP1HP == 0)
        {
            print("p1 fail");
            text.text = "Player2 WIN! 双界之门被永久封印！";
            GameObject.Find("Quad").GetComponentInChildren<Renderer>().material.mainTexture = (Texture)Resources.Load("bg1");
            Destroy(endTurnButtonObj);
        }
        if (sideP2HP == 0)
        {
            print("p2 fail");
            text.text = "Player1 WIN! 双界之门开启！";
            GameObject.Find("Quad").GetComponentInChildren<Renderer>().material.mainTexture = (Texture)Resources.Load("bg2");
            Destroy(endTurnButtonObj);
        }
    }

    public void click_skill1()
    {
        Vector3 v1 = new Vector3(1.2f, 1.2f, 1.2f);
        Vector3 v2 = new Vector3(1f, 1f, 1f);
        skillButton1.transform.localScale = v1;
        skillButton2.transform.localScale = v2;
        skillButton3.transform.localScale = v2;
        skillButton1.GetComponentInChildren<Text>().text = skillDetail[attackIndex - 1, 0];
        skillButton2.GetComponentInChildren<Text>().text = skillName[attackIndex - 1, 1];
        skillButton3.GetComponentInChildren<Text>().text = skillName[attackIndex - 1, 2];
        if (skillType[attackIndex - 1, 0] == true)
        {
            skillIndex = 1;
        }
        else
        {
            skillButton1.transform.localScale = v2;
            skillIndex = 0;
        }
    }

    public void click_skill2()
    {
        Vector3 v1 = new Vector3(1.2f, 1.2f, 1.2f);
        Vector3 v2 = new Vector3(1f, 1f, 1f);
        skillButton2.transform.localScale = v1;
        skillButton1.transform.localScale = v2;
        skillButton3.transform.localScale = v2;
        skillButton1.GetComponentInChildren<Text>().text = skillName[attackIndex - 1, 0];
        skillButton2.GetComponentInChildren<Text>().text = skillDetail[attackIndex - 1, 1];
        skillButton3.GetComponentInChildren<Text>().text = skillName[attackIndex - 1, 2];
        if (skillType[attackIndex - 1, 1] == true)
        {
            skillIndex = 2;
        }
        else
        {
            skillButton2.transform.localScale = v2;
            skillIndex = 0;
        }
    }

    public void click_skill3()
    {
        Vector3 v1 = new Vector3(1.2f, 1.2f, 1.2f);
        Vector3 v2 = new Vector3(1f, 1f, 1f);
        skillButton3.transform.localScale = v1;
        skillButton1.transform.localScale = v2;
        skillButton2.transform.localScale = v2;
        skillButton1.GetComponentInChildren<Text>().text = skillName[attackIndex - 1, 0];
        skillButton2.GetComponentInChildren<Text>().text = skillName[attackIndex - 1, 1];
        skillButton3.GetComponentInChildren<Text>().text = skillDetail[attackIndex - 1, 2];
        if (skillType[attackIndex - 1, 2] == true)
        {
            skillIndex = 3;
        }
        else
        {
            skillButton3.transform.localScale = v2;
            skillIndex = 0;
        }
    }
}
