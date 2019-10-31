using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class rankItemControl : MonoBehaviour {

    public Text playerRank, playerName, playerBean, playerStatus;

    public void initItemData(int rank, string name, int  bean, int status)
    {
        playerRank.text = rank.ToString();
        playerName.text = name;
        playerBean.text = bean.ToString();

        if (status == 1)
            playerStatus.text = "在线";
        else
            playerStatus.text = "离线";
    }
}
