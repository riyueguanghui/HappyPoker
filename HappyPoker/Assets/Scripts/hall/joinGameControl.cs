using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using KBEngine;

public class joinGameControl : MonoBehaviour {

    public GameObject thisPanel;                //当前面板
    public InputField joinGameRoomID;           //要加入房间的ID

    //加入房间
    public void joinGameRoom()
    {
        Account player = KBEngineApp.app.player() as Account;

        int roomId = int.Parse(joinGameRoomID.text);
        player.baseEntityCall.playerJoinGame(roomId);
    }

    //关闭当前窗口
    public void closePanel()
    {
        thisPanel.SetActive(false);             //关闭当前面板
    }
}
