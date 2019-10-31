using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KBEngine;

public class gameDataControl : MonoBehaviour {


    public static Room room;
    public static gameDataControl Instance;
    public List<playerInfor> playerInforList = new List<playerInfor>();         //保存玩家数据
	void Start () {

        
        KBEngine.Event.registerOut("onEnterWorld", this, "onEnterWorld");
    }
	
	void Update () {
		
	}


    //保存对象，保护数据
    private void Awake()
    {
        if (Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
        else if (Instance != null)
        {
            Destroy(gameObject);
        }
    }

    //转换房间座号，保持自己在第一个位置
    public int transformRoomIndex(int index)
    {
        Account player = KBEngineApp.app.player() as Account;
        int playerIndex = player.roomIndex;
        if(playerIndex!=index)
        {
            if(playerIndex==0)
            {
                return index;
            }

            if(playerIndex==1)
            {
                if (index == 0)
                    return 2;
                if (index == 2)
                    return 1;
            }

            if(playerIndex==2)
            {
                if (index == 0)
                    return 1;
                if (index == 1)
                    return 2;
            }
        }

        return 0;
    }

    //监控玩家进入房间
    public void onEnterWorld(KBEngine.Entity entity)
    {
        
        if (entity.className == "Account")
        {
            Debug.LogFormat("有玩家进入房间：{0}", entity.id);
            string playerName=((Account)entity).playerData.playerName;
            Debug.Log(playerName);
            int playerBean=((Account)entity).playerData.playerBean;
            int roomIndex = transformRoomIndex(((Account)entity).playerData.roomIndex);
            ulong playerid = ((Account)entity).playerData.playerID;
            playerInforList.Add(new playerInfor(playerName, playerBean, roomIndex,playerid));

            if (roomControl.Instance!=null)
            {
                roomControl.Instance.initPlayerInfor(playerName, playerBean, roomIndex);
            }
            
        }
        else if (entity.className == "Room")
        {
            room = (Room)entity;
        }

    }


}
