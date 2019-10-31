using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using KBEngine;

public class gaveOverControl : MonoBehaviour {

    public Text name0, name1, name2;
    //public Text score0, score1, score2;
    public Text mupe0, mupe1, mupe2;
    public Text bean0, bean1, bean2;

    public ulong playerID1, playerID2;

    // Use this for initialization
	void Start () {

        addPlayerName();
        calculateBean();
	}
	
    //加载玩家昵称
    void addPlayerName()
    {
        foreach (playerInfor player in gameDataControl.Instance.playerInforList)
        {
            if(player.roomIndex==0)
            {
                name0.text = player.playerName;
            }
            else if(player.roomIndex==1)
            {
                name1.text = player.playerName;
                playerID1 = player.playerID;
            }
            else
            {
                name2.text = player.playerName;
                playerID2 = player.playerID;
            }
        }
    }

    //计算结果
    void calculateBean()
    {
        int mupe = roomControl.Instance.curMupe;
        int landLord = roomControl.Instance.landlord;
        int index = roomControl.Instance.nextPlayer;

        mupe0.text = mupe.ToString();
        mupe1.text = mupe.ToString();
        mupe2.text = mupe.ToString();

        if(landLord==index)                     //地主胜利
        {
            if(landLord==0)
            {
                int bean = mupe * 40;
                bean0.text = bean.ToString();
                bean1.text = (bean / (-2)).ToString();
                bean2.text = (bean / (-2)).ToString();
                roomControl.Instance.updatePlayerInfor(0, bean);
                roomControl.Instance.updatePlayerInfor(1, (bean/(-2)));
                roomControl.Instance.updatePlayerInfor(2, (bean / (-2)));

            }
            else if(landLord==1)
            {
                int bean = mupe * 40;
                bean1.text = bean.ToString();
                bean0.text = (bean / (-2)).ToString();
                bean2.text = (bean / (-2)).ToString();
                roomControl.Instance.updatePlayerInfor(0, (bean / (-2)));
                roomControl.Instance.updatePlayerInfor(1, bean);
                roomControl.Instance.updatePlayerInfor(2, (bean / (-2)));
            }
            else
            {
                int bean = mupe * 40;
                bean2.text = bean.ToString();
                bean0.text = (bean / (-2)).ToString();
                bean1.text = (bean / (-2)).ToString();
                roomControl.Instance.updatePlayerInfor(0, (bean / (-2)));
                roomControl.Instance.updatePlayerInfor(1, (bean / (-2)));
                roomControl.Instance.updatePlayerInfor(2, bean);
            }
        }
        else                        //农民胜利
        {
            if (landLord == 0)
            {
                int bean = mupe * 20;
                bean0.text = (-2*bean).ToString();
                bean1.text = bean.ToString();
                bean2.text = bean.ToString();
                roomControl.Instance.updatePlayerInfor(0, -2*bean);
                roomControl.Instance.updatePlayerInfor(1,bean);
                roomControl.Instance.updatePlayerInfor(2,bean);
            }
            else if (landLord == 1)
            {
                int bean = mupe * 20;
                bean1.text = (-2 * bean).ToString();
                bean0.text = bean.ToString();
                bean2.text =bean.ToString();
                roomControl.Instance.updatePlayerInfor(0, bean);
                roomControl.Instance.updatePlayerInfor(1, -2*bean);
                roomControl.Instance.updatePlayerInfor(2, bean);
            }
            else
            {
                int bean = mupe * 20;
                bean2.text = (-2 * bean).ToString();
                bean0.text = bean.ToString();
                bean1.text = bean.ToString();
                roomControl.Instance.updatePlayerInfor(0, bean);
                roomControl.Instance.updatePlayerInfor(1, bean);
                roomControl.Instance.updatePlayerInfor(2, -2*bean);
            }
        }

    }




    //离开房间
	public void playerLeaveRoom()
    {
        roomControl.Instance.cleanRoomData();
        Account playerEntity = KBEngineApp.app.player() as Account;
        playerEntity.cellEntityCall.leaveRoom();
    }

    //继续游戏
    public void continuePlayGame()
    {

        roomControl.Instance.cleanRoomData();
        roomControl.Instance.closeGameOverPanel();
    }

    //添加好友1
    public void playerAddFriend1()
    {
        Account playerEntity = KBEngineApp.app.player() as Account;
        playerEntity.baseEntityCall.playerSendMail(playerID1);
    }

    //添加好友2
    public void playerAddFriend2()
    {
        Account playerEntity = KBEngineApp.app.player() as Account;
        playerEntity.baseEntityCall.playerSendMail(playerID2);
    }
}
