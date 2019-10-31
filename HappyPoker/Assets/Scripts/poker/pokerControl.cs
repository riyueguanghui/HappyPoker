using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class pokerControl : MonoBehaviour {


    public bool select = false;     //是否被选中
    public bool isMine;     //是否我的牌
    public int pokerNum;

    //初始化卡牌
    public void initPokerData(int index,bool isMine)
    {
        this.isMine = isMine;
        pokerNum = index;
    }

    //点击向上或者向下
    public void OnMouseDown()
    {
       
        if (isMine == false)
            return;
        this.select = !select;
        if(select==true)
        {
            Debug.Log(pokerNum);
            transform.localPosition += new Vector3(0, 10.0f, 0);
        }
        else
        {
            transform.localPosition -= new Vector3(0, 10.0f, 0);
        }
    }

    //提示出牌，向上移动卡牌
    public void movePokerUp()
    {
        if (select == true)
            return;
        this.select = !select;
        transform.localPosition += new Vector3(0, 10.0f, 0);
    }
}
