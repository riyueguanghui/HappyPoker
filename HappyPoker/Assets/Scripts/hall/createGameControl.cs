using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using KBEngine;

public class createGameControl : MonoBehaviour {

    public GameObject thisPanel;        //当前面板
    public InputField createRoomID;     //要创建房间的ID

    //创建房间
    public void createGameRoom()
    {
        Account player = KBEngineApp.app.player() as Account;
        int roomId = int.Parse(createRoomID.text);
        player.baseEntityCall.playerCreateGame(roomId);
    }

    //关闭当前窗口
    public void closePanel()
    {
        thisPanel.SetActive(false);
    }
}
