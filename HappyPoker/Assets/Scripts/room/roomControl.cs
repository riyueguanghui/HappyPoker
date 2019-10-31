using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Sprites;
using UnityEngine.SceneManagement;
using KBEngine;
using System;

public class roomControl : MonoBehaviour {


    public bool callLandlord = false;
    public int playerNum = 0;       //玩家数量
    public int landlord = -1;       //地主
    public int nextPlayer = -1;     //轮到谁出牌
    public int curMupe = 15;
    public int poker1=17,poker2=17;    //玩家一和玩家三得牌数

    public Room roomEntity;             //房间实体
    public Account playerEntity;        //玩家实体
    

    List<sbyte> playerPokerList;
    List<GameObject> pokerGameObjectList;
    List<GameObject> removeGameObjectList;

    public static roomControl Instance;
    public Text pokerCount1, pokerCount2;
    public Text playerName0,playerName1, playerName2;
    public Text playerBean0, playerBean1, playerBean2, gameMupe;

    public GameObject pokerNum1, pokerNum2;
    public GameObject pokerErrorMsg, gameOverPanel;
    public GameObject electBtn, notElectBtn, callBtn, notCallBtn;                   //是否抢地主
    public GameObject readyBtn, unreadyBtn,playBtn,passBtn;                     //准备游戏按钮

    public Transform pokerRoot,pokerRoot0, pokerRoot1, pokerRoot2;
    public Transform landlordRoot, landlordIcon0, landlordIcon1, landlordIcon2;


    void Start() {


        Instance = this;
        playerPokerList = new List<sbyte>();
        pokerGameObjectList=new List<GameObject>();
        removeGameObjectList = new List<GameObject>();
        foreach (playerInfor player in gameDataControl.Instance.playerInforList)
        {
            initPlayerInfor(player.playerName, player.playerBean, player.roomIndex);
        }

        registerOutEvent();
        playerEntity = KBEngineApp.app.player() as Account;

    }

    //注册监听事件
    public void registerOutEvent()
    {
        KBEngine.Event.registerOut("playerReadyGame", this, "playerReadyGame");
        KBEngine.Event.registerOut("playerToLandlord", this, "playerToLandlord");
        KBEngine.Event.registerOut("initGamePoker", this, "initGamePoker");
        KBEngine.Event.registerOut("notifyToCallLandlord", this, "notifyToCallLandlord");
        KBEngine.Event.registerOut("loadLandlordPoker", this, "loadLandlordPoker");
        KBEngine.Event.registerOut("notifyToPlayPoker", this, "notifyToPlayPoker");
        KBEngine.Event.registerOut("playPokerFailed", this, "playPokerFailed");
        KBEngine.Event.registerOut("autoPlayOnePoker", this, "autoPlayOnePoker");
        KBEngine.Event.registerOut("autoPlayZeroPokerr", this, "autoPlayZeroPoker");
        KBEngine.Event.registerOut("displayPoker", this, "displayPoker");
        KBEngine.Event.registerOut("passPoker", this, "passPoker");
        KBEngine.Event.registerOut("pokerGameOver", this, "pokerGameOver");
        KBEngine.Event.registerOut("onLeaveWorld", this, "onLeaveWorld");
        KBEngine.Event.registerOut("stayInRoom", this, "stayInRoom");
        KBEngine.Event.registerOut("updateGameMupe", this, "updateGameMupe");

    }

    // Update is called once per frame
    void Update() {

    }

    //初始所有玩家数据，显示
    public void initPlayerInfor(string name, int bean, int index)
    {
        if (index == 0)
        {
            playerName0.text = name;
            playerBean0.text = bean.ToString();
        }
        else if (index == 1)
        {
            playerName1.text = name;
            playerBean1.text = bean.ToString();
        }
        else
        {
            playerName2.text = name;
            playerBean2.text = bean.ToString();
        }
        playerNum += 1;
        if (playerNum == 3)
            readyBtn.SetActive(true);
    }


    //玩家退出，删除玩家数据
    public void removePlayerInfor(int index)
    {

        if(index==1)
        {
            playerName1.text = "";
            playerBean1.text = "";
            cleanPokerRoot1();
        }
        else if(index==2)
        {
            playerName2.text = "";
            playerBean2.text = "";
            cleanPokerRoot2();
        }
        playerNum -= 1;
    }



    //玩家准备游戏
    public void setReadyGame()
    {
        readyBtn.SetActive(false);
        unreadyBtn.SetActive(true);
        playerEntity.cellEntityCall.setGameReady(1);
    }

    //取消准备游戏
    public void resetReadyGame()
    {
        unreadyBtn.SetActive(false);
        readyBtn.SetActive(true);
        playerEntity.cellEntityCall.setGameReady(-1);
    }

    //显示玩家准备游戏
    public void playerReadyGame(int index, int state)
    {
        int roomIndex = gameDataControl.Instance.transformRoomIndex(index);
        if (state == 1)
        {
            if (roomIndex == 0)
            {
                //cleanPokerRoot0();
                GameObject prefab = Resources.Load<GameObject>("prefabs/ready");
                GameObject poker = GameObject.Instantiate(prefab, pokerRoot0) as GameObject;
                poker.transform.localPosition = new Vector3(0, 0, 0);
                Debug.Log("0 ready");
            }
            else if (roomIndex == 1)
            {
                //cleanPokerRoot1();
                GameObject prefab = Resources.Load<GameObject>("prefabs/ready");
                GameObject poker = GameObject.Instantiate(prefab, pokerRoot1) as GameObject;
                poker.transform.localPosition = new Vector3(0, 0, 0);
                Debug.Log("1 ready");
            }
            else
            {
                //cleanPokerRoot2();
                GameObject prefab = Resources.Load<GameObject>("prefabs/ready");
                GameObject poker = GameObject.Instantiate(prefab, pokerRoot2) as GameObject;
                poker.transform.localPosition = new Vector3(0, 0, 0);
                Debug.Log("2 ready");
            }
                
        }
        else
        {
            if (roomIndex == 0)
            {
                cleanPokerRoot0();
            }
            else if (roomIndex == 1)
            {
                cleanPokerRoot1();
            }
            else
            {
                cleanPokerRoot2();
            }
        }

    }

    //显示玩家抢地主过程
    public void playerToLandlord(int index, int state)
    {
        int i = gameDataControl.Instance.transformRoomIndex(index);
        if(callLandlord==false)
        {
            if(state==0)
            {
                if(i==0)
                {
                    cleanPokerRoot0();
                    GameObject prefab = Resources.Load<GameObject>("prefabs/bujiao");
                    GameObject poker = GameObject.Instantiate(prefab, pokerRoot0) as GameObject;
                    poker.transform.localPosition = new Vector3(0, 0, 0);
                }
                else if(i==1)
                {
                    cleanPokerRoot1();
                    GameObject prefab = Resources.Load<GameObject>("prefabs/bujiao");
                    GameObject poker = GameObject.Instantiate(prefab, pokerRoot1) as GameObject;
                    poker.transform.localPosition = new Vector3(0, 0, 0);
                }
                else
                {
                    cleanPokerRoot2();
                    GameObject prefab = Resources.Load<GameObject>("prefabs/bujiao");
                    GameObject poker = GameObject.Instantiate(prefab, pokerRoot2) as GameObject;
                    poker.transform.localPosition = new Vector3(0, 0, 0);
                }
            }
            else
            {
                if(i==0)
                {
                    cleanPokerRoot0();
                    GameObject prefab = Resources.Load<GameObject>("prefabs/jiaodizhu");
                    GameObject poker = GameObject.Instantiate(prefab, pokerRoot0) as GameObject;
                    poker.transform.localPosition = new Vector3(0, 0, 0);
                }
                else if(i==1)
                {
                    cleanPokerRoot1();
                    GameObject prefab = Resources.Load<GameObject>("prefabs/jiaodizhu");
                    GameObject poker = GameObject.Instantiate(prefab, pokerRoot1) as GameObject;
                    poker.transform.localPosition = new Vector3(0, 0, 0);
                }
                else
                {
                    cleanPokerRoot2();
                    GameObject prefab = Resources.Load<GameObject>("prefabs/jiaodizhu");
                    GameObject poker = GameObject.Instantiate(prefab, pokerRoot2) as GameObject;
                    poker.transform.localPosition = new Vector3(0, 0, 0);
                }
                callLandlord = true;
            }
        }
        else
        {
            if(state==0)
            {
                if(i==0)
                {
                    cleanPokerRoot0();
                    GameObject prefab = Resources.Load<GameObject>("prefabs/buqiang");
                    GameObject poker = GameObject.Instantiate(prefab, pokerRoot0) as GameObject;
                    poker.transform.localPosition = new Vector3(0, 0, 0);
                }
                else if(i==1)
                {
                    cleanPokerRoot1();
                    GameObject prefab = Resources.Load<GameObject>("prefabs/buqiang");
                    GameObject poker = GameObject.Instantiate(prefab, pokerRoot1) as GameObject;
                    poker.transform.localPosition = new Vector3(0, 0, 0);
                }
                else
                {
                    cleanPokerRoot2();
                    GameObject prefab = Resources.Load<GameObject>("prefabs/buqiang");
                    GameObject poker = GameObject.Instantiate(prefab, pokerRoot2) as GameObject;
                    poker.transform.localPosition = new Vector3(0, 0, 0);
                }
            }
            else
            {
                if(i==0)
                {
                    cleanPokerRoot0();
                    GameObject prefab = Resources.Load<GameObject>("prefabs/qiangdizhu");
                    GameObject poker = GameObject.Instantiate(prefab, pokerRoot0) as GameObject;
                    poker.transform.localPosition = new Vector3(0, 0, 0);
                }
                else if(i==1)
                {
                    cleanPokerRoot1();
                    GameObject prefab = Resources.Load<GameObject>("prefabs/qiangdizhu");
                    GameObject poker = GameObject.Instantiate(prefab, pokerRoot1) as GameObject;
                    poker.transform.localPosition = new Vector3(0, 0, 0);
                }
                else
                {
                    cleanPokerRoot2();
                    GameObject prefab = Resources.Load<GameObject>("prefabs/qiangdizhu");
                    GameObject poker = GameObject.Instantiate(prefab, pokerRoot2) as GameObject;
                    poker.transform.localPosition = new Vector3(0, 0, 0);
                }
            }
        }

    }


    //显示玩家扑克牌
    public void initGamePoker()
    {
        cleanPokerRoot0();
        cleanPokerRoot1();
        cleanPokerRoot2();
        unreadyBtn.SetActive(false);
        initPoker();
        pokerNum1.SetActive(true);
        pokerNum2.SetActive(true);
        
    }

    //告诉玩家进行叫地主
    public void notifyToCallLandlord()
    {
        cleanPokerRoot0();
        if(callLandlord==false)
        {
            if(callBtn!=null)
                callBtn.SetActive(true);
            if(notCallBtn!=null)
                notCallBtn.SetActive(true);
        }
        else
        {
            if(electBtn!=null)
                electBtn.SetActive(true);
            if(notElectBtn!=null)
                notElectBtn.SetActive(true);
        }
    }


    //叫地主或者抢地主
    public void playerCallLandlord()
    {
        if (electBtn && notElectBtn && callBtn && notCallBtn)
        {
            electBtn.SetActive(false);
            notElectBtn.SetActive(false);
            callBtn.SetActive(false);
            notCallBtn.SetActive(false);
        }


        int index = playerEntity.roomIndex;
        playerEntity.cellEntityCall.playerMakeLandlord((sbyte)index, 1);
    }

    //不抢地主
    public void playerNotCallLandlord()
    {
        if (electBtn && notElectBtn && callBtn && notCallBtn)
        {
            electBtn.SetActive(false);
            notElectBtn.SetActive(false);
            callBtn.SetActive(false);
            notCallBtn.SetActive(false);
        }

        int index = playerEntity.roomIndex;
        playerEntity.cellEntityCall.playerMakeLandlord((sbyte)index, 0);
    }

    //初始化手牌
    public void initPoker()
    {
        Debug.Log(playerEntity.playerPoker.Count);
        playerPokerList = new List<sbyte>(playerEntity.playerPoker);

        playerPokerList.Sort((x,y)=>-x.CompareTo(y));

        unLoadLnadlordPoker();
        for (int i = 0; i < pokerRoot.childCount; i++)
        {
            GameObject.Destroy(pokerRoot.GetChild(i).gameObject);
        }
        pokerGameObjectList.Clear();
        for (int i=0;i<playerPokerList.Count;i++)
         {
            int index = playerPokerList[i];
            Debug.Log(index);
            GameObject prefab = Resources.Load<GameObject>("prefabs/" + index.ToString());
            GameObject poker = GameObject.Instantiate(prefab, pokerRoot) as GameObject;
            poker.GetComponent<pokerControl>().initPokerData(index, true);
            poker.transform.localPosition = new Vector3(i * 20.0f, 0, 0);
            pokerGameObjectList.Add(poker);
         }
    }


    //反面地主3张牌
    public void unLoadLnadlordPoker()
    {
        for (int i = 0; i < landlordRoot.childCount; i++)
        {
            GameObject.Destroy(pokerRoot.GetChild(i).gameObject);
        }

        for (int i=0;i<3;i++)
        {
            GameObject prefab = Resources.Load<GameObject>("prefabs/landlord");
            GameObject poker = GameObject.Instantiate(prefab, landlordRoot) as GameObject;
            poker.transform.localScale *= 0.8f;
            poker.transform.localPosition = new Vector3(i * 40.0f, 0, 0);
        }
    }

    //加载地主3张牌
    public void loadLandlordPoker(int ld,List<sbyte> list)
    {

        roomEntity = gameDataControl.room;
        landlord = gameDataControl.Instance.transformRoomIndex(ld);
        nextPlayer = landlord;
        cleanPokerRoot0();
        cleanPokerRoot1();
        cleanPokerRoot2();
        if (electBtn && notElectBtn && callBtn && notCallBtn)
        {
            electBtn.SetActive(false);
            notElectBtn.SetActive(false);
            callBtn.SetActive(false);
            notCallBtn.SetActive(false);
        }



        //显示三张地主牌
        if (landlordRoot != null)
        {
            for (int i = 0; i < landlordRoot.childCount; i++)
            {
                GameObject.Destroy(landlordRoot.GetChild(i).gameObject);
            }
        }

        List<sbyte> pokerList = new List<sbyte>(list);
        pokerList.Sort((x, y) => -x.CompareTo(y));

        for (int i = 0; i < 3; i++)
        {
            int index = pokerList[i];
            GameObject prefab = Resources.Load<GameObject>("prefabs/"+index.ToString());
            GameObject poker = GameObject.Instantiate(prefab, landlordRoot) as GameObject;
            poker.transform.localScale *= 0.8f;
            poker.transform.localPosition = new Vector3(i * 40.0f, 0, 0);
        }

        //显示地主图标+初始化玩家1和玩家卡牌数
        if(landlord==0)
        {
            GameObject prefab = Resources.Load<GameObject>("prefabs/landlordIcon");
            GameObject icon = GameObject.Instantiate(prefab, landlordIcon0) as GameObject;
            icon.transform.localPosition = new Vector3(0, 0, 0);

        }
        else if(landlord==1)
        {
            GameObject prefab = Resources.Load<GameObject>("prefabs/landlordIcon");
            GameObject icon = GameObject.Instantiate(prefab, landlordIcon1) as GameObject;
            icon.transform.localPosition = new Vector3(0, 0, 0);

            poker1 = 20;
            pokerCount1.text = poker1.ToString();

        }
        else
        {
            GameObject prefab = Resources.Load<GameObject>("prefabs/landlordIcon");
            GameObject icon = GameObject.Instantiate(prefab, landlordIcon2) as GameObject;
            icon.transform.localPosition = new Vector3(0, 0, 0);

            poker2 = 20;
            pokerCount2.text = poker2.ToString();

        }

    }



    //移除出的手牌
   /* public void removePoker()
    {
        List<int> removeList = new List<int>();
        foreach (GameObject poker in pokerGameObjectList)
        {
            if (poker.GetComponent<pokerControl>().select == true)
            {
                int num = poker.GetComponent<pokerControl>().pokerNum;
                removeList.Add(num);
            }
        }

        //发送给服务端pokerList;
    }


    //更新卡牌
    public void updatePoker()
    {
        int orignP = 0, newP = 0;
        List<int> delListPoker = new List<int>();
        List<GameObject> delListObject = new List<GameObject>();
        for (int i = 0; i < pokerGameObjectList.Count; i++)
        {
            GameObject poker = pokerGameObjectList[i];
            if (poker.GetComponent<pokerControl>().select == true)
            {

                int index = poker.GetComponent<pokerControl>().pokerNum;
                playerPokerList.Remove((sbyte)index);
                delListPoker.Add(index);
                delListObject.Add(poker);
                Destroy(poker);

                Debug.Log(index);

                GameObject prefab = Resources.Load<GameObject>("prefabs/" + index.ToString());
                 GameObject newpoker = GameObject.Instantiate(prefab, pokerRoot0) as GameObject;
                 newpoker.GetComponent<pokerControl>().initPokerData(index, false);
                 newpoker.transform.localPosition = new Vector3(newP* 20.0f, 0, 0);
                 newP += 1;

            }
            else
            {
                poker.transform.localPosition = new Vector3(orignP * 20.0f, 0, 0);
                orignP += 1;
            }
        }

        for(int i=0;i<delListObject.Count;i++)
        {
            GameObject poker = delListObject[i];
            pokerGameObjectList.Remove(poker);
        }
        /*for (int i = 0; i < delListObject.Count; i++)
        {
            Destroy(delListObject[i]);
        }

        foreach (int index in delListPoker)
        {
            GameObject prefab = Resources.Load<GameObject>("prefabs/" + index.ToString());
            GameObject newpoker = GameObject.Instantiate(prefab, pokerRoot0) as GameObject;
            newpoker.GetComponent<pokerControl>().initPokerData(index, false);
            newpoker.transform.localPosition = new Vector3(newP * 20.0f, 0, 0);
            newP += 1;
        }
    }*/

    //通知出牌：
    //type=0:必须出牌
    //type=1:可以出牌和过牌
    public void notifyToPlayPoker(int type)
    {
        if(type==0)
        {
            playBtn.SetActive(true);
        }
        else
        {
            playBtn.SetActive(true);    
            passBtn.SetActive(true);
        }
    }

    //协程：出的牌错误
    public void playPokerFailed()
    {
        pokerErrorMsg.SetActive(true);
        StartCoroutine(cleanErrorMsg());
    }

    //携程执行程序
    IEnumerator cleanErrorMsg()
    {
        yield return new WaitForSeconds(1.0f);
        pokerErrorMsg.SetActive(false);
    }


    //定时器触发：自动打一张牌
    public void autoPlayOnePoker()
    {
        int index = playerPokerList.Count - 1;
        sbyte num = playerPokerList[index];

        List<sbyte> listPoker = new List<sbyte>();
        listPoker.Add(num);

        for (int i = pokerGameObjectList.Count; i>=0; i++)
        {
            GameObject poker = pokerGameObjectList[i];
            if (poker.GetComponent<pokerControl>().pokerNum==(int)num)
            {
                removeGameObjectList.Add(poker);
            }

        }
        roomEntity.cellEntityCall.playPoker((KBEngine.POKERLIST)listPoker);


    }

    //定时器触发：自动过牌
    public void autoPlayZeroPoker()
    {
        Debug.Log("system auto play poker!");
        if (nextPlayer == 0)
        {
            GameObject prefab = Resources.Load<GameObject>("prefabs/100");
            GameObject newpoker = GameObject.Instantiate(prefab, pokerRoot0) as GameObject;
            newpoker.GetComponent<pokerControl>().initPokerData(100, false);
        }
        else if (nextPlayer == 1)
        {
            GameObject prefab = Resources.Load<GameObject>("prefabs/100");
            GameObject newpoker = GameObject.Instantiate(prefab, pokerRoot1) as GameObject;
            newpoker.GetComponent<pokerControl>().initPokerData(100, false);
        }
        else
        {
            GameObject prefab = Resources.Load<GameObject>("prefabs/100");
            GameObject newpoker = GameObject.Instantiate(prefab, pokerRoot2) as GameObject;
            newpoker.GetComponent<pokerControl>().initPokerData(100, false);
        }

        cleanNextPlayerPoker();
    }

    //打牌成功或者其他玩家出牌：显示所打的牌
    public void displayPoker(List<sbyte> list)
    {
        Debug.Log("display poker");
        
        if (nextPlayer==0)                                                    //自己打的牌
        {
            disPlayPoker0(list);
        }
        else if(nextPlayer==1)                                              //1号玩家打的牌
        {
            disPlayPoker1(list);
        }
        else                                                                //2号玩家打的牌
        {
            disPlayPoker2(list);
        }

    }


    //过牌:显示不出
    public void passPoker()
    {
        Debug.Log(nextPlayer);
        if(nextPlayer==0)
        {
            playBtn.SetActive(false);
            passBtn.SetActive(false);
            GameObject prefab = Resources.Load<GameObject>("prefabs/buchu");
            GameObject poker = GameObject.Instantiate(prefab, pokerRoot0) as GameObject;
            poker.transform.localPosition = new Vector3(0, 0, 0);
        }
        else if(nextPlayer==1)
        {
            GameObject prefab = Resources.Load<GameObject>("prefabs/buchu");
            GameObject poker = GameObject.Instantiate(prefab, pokerRoot1) as GameObject;
            poker.transform.localPosition = new Vector3(0, 0, 0);
        }
        else
        {
            GameObject prefab = Resources.Load<GameObject>("prefabs/buchu");
            GameObject poker = GameObject.Instantiate(prefab, pokerRoot2) as GameObject;
            poker.transform.localPosition = new Vector3(0, 0, 0);
        }

        cleanNextPlayerPoker();
    }

    //出牌
    public void playPoker()
    {
        KBEngine.POKERLIST listPoker = new POKERLIST();
        for (int i=0;i < pokerGameObjectList.Count;i++)
        {
            GameObject poker = pokerGameObjectList[i];
            if(poker.GetComponent<pokerControl>().select == true)
            {
                int num=poker.GetComponent<pokerControl>().pokerNum;
                listPoker.Add((sbyte)num);
                removeGameObjectList.Add(poker);
            }

        }

        
        roomEntity.cellEntityCall.playPoker(listPoker);
    }


    //玩家不出牌
    public void unPoker()
    {
        roomEntity.cellEntityCall.unPlayPoker();
    }

    //显示玩家0出的牌
    public void disPlayPoker0(List<sbyte> list)
    {
        if (playBtn && passBtn)
        {
            playBtn.SetActive(false);
            passBtn.SetActive(false);
        }

        int index0 = 0;
        for(int i=0;i<pokerGameObjectList.Count;i++)                                                //销毁打出去的牌，同时重新排列手中的牌
        {
            GameObject poker = pokerGameObjectList[i];
            if(removeGameObjectList.Contains(poker))
            {
                Destroy(poker);
            }
            else
            {
                poker.transform.localPosition = new Vector3(index0 * 20.0f, 0, 0);
                index0 += 1;
            }
        }

        foreach(GameObject poker in removeGameObjectList)                                         //更新数据
        {
            pokerGameObjectList.Remove(poker);
        }
        removeGameObjectList.Clear();


       int index1 = 0;
        foreach(sbyte i in list)                                                                 //更新数据、显示打出去的牌
        {
            Debug.Log(i);
            playerPokerList.Remove(i);
            int num = i;
            GameObject prefab = Resources.Load<GameObject>("prefabs/" + num.ToString());
            GameObject newpoker = GameObject.Instantiate(prefab,pokerRoot0) as GameObject;
            newpoker.GetComponent<pokerControl>().initPokerData(num, false);
            newpoker.transform.localPosition = new Vector3(index1 * 20.0f, 0, 0);
            index1 += 1;
        }


        cleanNextPlayerPoker();
    }

    //显示玩家1出的牌
    //更新对应得卡牌数量
    public void disPlayPoker1(List<sbyte> list)
    {
        int index = list.Count;
        foreach (sbyte i in list)                                                                 //更新数据、显示打出去的牌
        {
            //playerPokerList.Remove(i);
            int num = i;
            GameObject prefab = Resources.Load<GameObject>("prefabs/" + num.ToString());
            GameObject newpoker = GameObject.Instantiate(prefab, pokerRoot1) as GameObject;
            newpoker.GetComponent<pokerControl>().initPokerData(num, false);
            newpoker.transform.localPosition = new Vector3(-index * 20.0f, 0, 0);
            index -= 1;
        }

        poker1 -= list.Count;                       //减去所出的牌数
        pokerCount1.text = poker1.ToString();

        cleanNextPlayerPoker();
    }

    //显示玩家2出的牌
    //更新对应得卡牌数量
    public void disPlayPoker2(List<sbyte> list)
    {
        int index = 0;
        foreach (sbyte i in list)                                                                 //更新数据、显示打出去的牌
        {
            //playerPokerList.Remove(i);
            int num = i;
            GameObject prefab = Resources.Load<GameObject>("prefabs/" + num.ToString());
            GameObject newpoker = GameObject.Instantiate(prefab, pokerRoot2) as GameObject;
            newpoker.GetComponent<pokerControl>().initPokerData(num, false);
            newpoker.transform.localPosition = new Vector3(index * 20.0f, 0, 0);
            index += 1;
        }

        poker2 -= list.Count;                               //减去所出的卡牌数
        pokerCount2.text = poker2.ToString();

        cleanNextPlayerPoker();
    }

    //跳转到下一个玩家，初始化显示
    public void cleanNextPlayerPoker()
    {
        nextPlayer = (nextPlayer + 1) % 3;
        if(nextPlayer==0)
        {
            cleanPokerRoot0();
            //添加定时器闹钟
        }
        else if(nextPlayer==1)
        {
            cleanPokerRoot1();
            //添加定时器闹钟
        }
        else
        {
            cleanPokerRoot2();
            //添加定时器闹钟
        }
    }

    //清除pokerRoot0的子部件
    public void cleanPokerRoot0()
    {
        if (pokerRoot0 != null)
        {
            for (int i = 0; i < pokerRoot0.childCount; i++)
            {
                Destroy(pokerRoot0.GetChild(i).gameObject);
            }
        }
    }


    //清除pokerRoo1的子部件
    public void cleanPokerRoot1()
    {
        Debug.Log("111");
        if (pokerRoot1 != null)
        {
            for (int i = pokerRoot1.childCount - 1; i >= 0; i--)
            {
                Debug.Log("222");
                Destroy(pokerRoot1.GetChild(i).gameObject);
                Debug.Log("333");
            }
        }
    }

    //清除pokerRoot2的子部件
    public void cleanPokerRoot2()
    {
        if (pokerRoot2 != null)
        {
            for (int i = 0; i < pokerRoot2.childCount; i++)
            {
                Destroy(pokerRoot2.GetChild(i).gameObject);
            }
        }
    }

    //清除房间数据
    public void cleanRoomData()
    {

        callLandlord = false;
        playerNum = 0;       //玩家数量
        nextPlayer = -1;     //轮到谁出牌
        poker1 = 17;
        poker2 = 17;    //玩家一和玩家三得牌数

        playerPokerList.Clear();
        pokerGameObjectList.Clear() ;
        removeGameObjectList.Clear();

        if(landlord==0)
        {
            if (landlordIcon0!= null)
            {
                for (int i = 0; i < landlordIcon0.childCount; i++)
                {
                    Destroy(landlordIcon0.GetChild(i).gameObject);
                }
            }
            landlord = -1;
        }
        else if(landlord==1)
        {
            if (landlordIcon1 != null)
            {
                for (int i = 0; i < landlordIcon1.childCount; i++)
                {
                    Destroy(landlordIcon1.GetChild(i).gameObject);
                }
            }
            landlord = -1;
        }
        else
        {
            if (landlordIcon2 != null)
            {
                for (int i = 0; i < landlordIcon2.childCount; i++)
                {
                    Destroy(landlordIcon2.GetChild(i).gameObject);
                }
            }
            landlord = -1;
        }

        if(landlordRoot!=null)
        {
            for(int i=0;i<landlordRoot.childCount;i++)
            {
                Destroy(landlordRoot.GetChild(i).gameObject);
            }
        }
}

    //游戏结束
    public void pokerGameOver(int roomIndex)
    {
        Debug.Log("game over");

        nextPlayer = gameDataControl.Instance.transformRoomIndex(roomIndex);
        Transform parentPanel = GameObject.Find("Canvas").transform;
        gameOverPanel = (GameObject)Instantiate(Resources.Load("prefabs/gameOverPanel"));
        gameOverPanel.transform.SetParent(parentPanel,false);

    }

    //继续游戏关闭结算按钮
    public void closeGameOverPanel()
    {
        Destroy(gameOverPanel);
    }


    //玩家离开房间
    public void onLeaveWorld(KBEngine.Entity entity)
    {
        int roomIndex = gameDataControl.Instance.transformRoomIndex(((Account)entity).playerData.roomIndex);
        Debug.Log(roomIndex);
        if(roomIndex==0)
        {
            gameDataControl.Instance.playerInforList.Clear();
            KBEngine.Event.deregisterOut("onLeaveWorld", this, "onLeaveWorld");
            SceneManager.LoadSceneAsync(1);
            return;
        }

        foreach (playerInfor player in gameDataControl.Instance.playerInforList)
        {
            if (player.roomIndex == roomIndex)
            {
                gameDataControl.Instance.playerInforList.Remove(player);
                break;
            }
        }
        removePlayerInfor(roomIndex);

    }

    
    //继续游戏：更新相关信息
    public void updatePlayerInfor(int i,int beans)
    {
        foreach (playerInfor player in gameDataControl.Instance.playerInforList)
        {
            if(player.roomIndex==i)
            {
                player.playerBean += beans;
                initPlayerInfor(player.playerName, player.playerBean, player.roomIndex);
            }  
        }
    }

    //继续游戏
    public void stayInRoom(int roomIndex)
    {
        int index = gameDataControl.Instance.transformRoomIndex(roomIndex);
        if (index == 0)
            cleanPokerRoot0();
        else if (index == 1)
            cleanPokerRoot1();
        else
            cleanPokerRoot2();
    }

    //离开房间
    public void playerLeaveRoom()
    {
        playerEntity = KBEngineApp.app.player() as Account;
        playerEntity.cellEntityCall.leaveRoom();
    }

    //更新倍数
    public void updateGameMupe(int num)
    {
        if(gameMupe!=null)
         gameMupe.text = num.ToString();
        curMupe = num;
    }


}
