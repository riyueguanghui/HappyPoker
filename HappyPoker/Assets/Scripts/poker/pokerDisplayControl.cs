using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using KBEngine;

public class pokerDisplayControl : MonoBehaviour {

    /* Use this for initialization
    public Transform pokerRoot, pokerRoot0, pokerRoot1, pokerRoot2;
    List<sbyte> playerPokerList;
    List<GameObject> pokerGameObjectList = new List<GameObject>();
	void Start () {

       /* KBEngine.Event.registerOut("initGamePoker", this, "initPoker");

        for(int i=0;i<17;i++)
        {
            GameObject prefab = Resources.Load<GameObject>("prefabs/0");
            if (prefab == null)
                Debug.Log("ERROR!!!");
            GameObject poker = GameObject.Instantiate(prefab, pokerRoot) as GameObject;
            poker.GetComponent<pokerControl>().initPokerData(i, true);
            poker.transform.localPosition = new Vector3(i * 20.0f, 0, 0);
        }
        
	}*/


     /*
    //初始化卡牌
    public void initPoker()
    {
        playerPokerList.Reverse();
        foreach(int i in playerPokerList)
        {
            GameObject prefab= Resources.Load<GameObject>("prefabs/poker/" + i.ToString());
            GameObject poker=GameObject.Instantiate(prefab, pokerRoot) as GameObject;
            poker.GetComponent<pokerControl>().initPokerData(i, true);
            poker.transform.localPosition = new Vector3(i * 10.0f, 0, 0);
            pokerGameObjectList.Add(poker);
        }
    }*/

    /*
    //移除出的手牌
    public void removePoker()
    {
        List<int> removeList = new List<int>();
        foreach(GameObject poker in pokerGameObjectList)
        {
            if(poker.GetComponent<pokerControl>().select==true)
            {
                int num = poker.GetComponent<pokerControl>().pokerNum;
                removeList.Add(num);
            }
        }

        //发送给服务端pokerList;
    }*/

    /*
    //更新卡牌
    public void updatePoker()
    {
        for(int i=0;i<pokerGameObjectList.Count;i++)
        {
            if(pokerGameObjectList[i].GetComponent<pokerControl>().select==true)
            {
                int index = pokerGameObjectList[i].GetComponent<pokerControl>().pokerNum;
                playerPokerList.Remove(index);
                GameObject poker = pokerGameObjectList[i];
                pokerGameObjectList.Remove(poker);
                Destroy(poker);
            }
            else
            {
                pokerGameObjectList[i].transform.localPosition = new Vector3(i * 10.0f, 0, 0);
            }
        }
    }*/

    /*
    public void displayPoker(Transform pokerParent,List<int> pokerList)
    {

    }*/

}
