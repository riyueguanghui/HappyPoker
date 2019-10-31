using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class messagePanelControl : MonoBehaviour {

    public Text message;
    public GameObject messagePanel;
    
    //显示错误信息
    public void showErrorMessage(int type)
    {
        Debug.Log("显示消息");
        if (type == 0)
            message.text = "房间号不存在，请重新输入！";
        else if (type == 1)
            message.text = "该房间已被创建，请重新输入！";
        else
            message.text = "房间已满人，请重新输入！";
    }

    //关闭面板
    public void closePanel()
    {
        messagePanel.SetActive(false);
    }
}
