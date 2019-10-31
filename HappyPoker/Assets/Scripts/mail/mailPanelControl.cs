using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using KBEngine;

public class mailPanelControl : MonoBehaviour {

    public Transform Content;

	void Start () {
        KBEngine.Event.registerOut("receiveMail", this, "receiveMail");
	}
	
    public  void receiveMail(MAILDATA arg1)
    {
        GameObject prefab = Resources.Load<GameObject>("prefabs/mail");
        GameObject mail = GameObject.Instantiate(prefab, Content) as GameObject;
        mail.GetComponent<mailControl>().initMailData(arg1.senderName, arg1.senderID);
    }

    public void closePanel()
    {
        Debug.Log("CLOSE CLOSE");
        GameObject.Destroy(gameObject);
    }
}
