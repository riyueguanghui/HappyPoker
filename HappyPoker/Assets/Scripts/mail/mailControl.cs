using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using KBEngine;

public class mailControl : MonoBehaviour {


    public Text senderName;
    public ulong senderID;
	
    //加载邮件数据
    public void initMailData(string name,ulong id)
    {
        senderID = id;
        senderName.text = name;
    }

    //拒绝添加
    public void refuseToAddFriend()
    {
        Account player = KBEngineApp.app.player() as Account;
        player.baseEntityCall.playerAgreeFriend(senderID, 0);
        Destroy(gameObject);
    }

    //同意添加
    public void agreeToAddFriend()
    {
        Account player = KBEngineApp.app.player() as Account;
        player.baseEntityCall.playerAgreeFriend(senderID, 1);
        Destroy(gameObject);
    }


}
