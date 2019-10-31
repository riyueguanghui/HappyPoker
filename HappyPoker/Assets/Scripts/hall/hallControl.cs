using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using KBEngine;

public class hallControl : MonoBehaviour {

    Account player;
    public Text playerName;     //玩家昵称
    public Text playerBean;     //玩家欢乐豆数量
    public GameObject joinGamePanel;        //加入游戏窗口
    public GameObject createGamePanel;      //创建游戏窗口
    public GameObject errorMessagePanel;    //错误消息窗口

    public Transform Canvas, Content;

	void Start () {
        
        player = KBEngineApp.app.player() as Account;
        initPlayerData();       //初始化加载玩家数据
        Debug.Log("监听中。。。。。。。");
        KBEngine.Event.registerOut("returnErrorMessage", this, "returnErrorMessage");
        KBEngine.Event.registerOut("addSpaceGeometryMapping", this, "addSpaceGeometryMapping");     //监听进入目标空间
        KBEngine.Event.registerOut("friendListData", this, "friendListData");

        initFriendList();

    }

    //初始加载玩家数据
    void initPlayerData()
    {
        
        if (player != null)
        {
            playerName.text = player.playerName.ToString();
            playerBean.text = player.playerBean.ToString();
        }
    }
    //玩家匹配游戏
    public void matchGame()
    {
        player.baseEntityCall.playerMatchGame();
    }

    //玩家加入房间
    public void joinGame()
    {
        joinGamePanel.SetActive(true);
    }

    //玩家创建房间
    public void createGame()
    {
        createGamePanel.SetActive(true);
    }

    //客户端响应错误信息窗口
    public void returnErrorMessage(int type)
    {
        Debug.Log("成功响应：returnErrorMessage!");
        errorMessagePanel.SetActive(true);
        //调用messagePanelControl类的showErrorMessage函数
        GameObject.Find("errorMessagePanel").GetComponent<messagePanelControl>().showErrorMessage(type);
        

    }

    //玩家匹配成功进入目标房间
    public void addSpaceGeometryMapping(string path)
    {
        Debug.Log("进入房间");
        if (path == "spaces/gameMap")
            SceneManager.LoadScene(2);
    }

    //邮件系统
    public void openMailPanel()
    {
        GameObject go = Resources.Load<GameObject>("prefabs/mailPanel");
        go = Instantiate(go);
        go.transform.SetParent(Canvas, false);

        player.baseEntityCall.reqPlayerMail();
    }


    //好友列表
    public void initFriendList()
    {
        player.baseEntityCall.getFriendListData();
    }

    //好友数据
    public void friendListData(FRIENDDATALIST arg1)
    {

        int i = 1;
        foreach(var item in arg1)
        {
            GameObject prefab = Resources.Load<GameObject>("prefabs/rankItem");
            GameObject friend= GameObject.Instantiate(prefab, Content) as GameObject;
            friend.GetComponent<rankItemControl>().initItemData(i,item.playerName,item.playerBean,item.status);
            i++;
        }

    }


}
