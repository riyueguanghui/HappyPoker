using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class playerInfor {

    public string playerName;
    public int playerBean;
    public int roomIndex;
    public ulong playerID;

    public playerInfor(string name,int bean,int index,ulong id)
    {
        playerName=name;
        playerBean = bean;
        roomIndex = index;
        playerID = id;
    }
}
