using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using KBEngine;

public class loginControl : MonoBehaviour {

    public InputField userAccount;              //账号
    public InputField userPassword;             //密码

    //注册事件
	void Start ()
    {
        KBEngine.Event.registerOut("onLoginFailed", this, "onLoginFailed");     //监听服务端抛出的登陆失败事件
        KBEngine.Event.registerOut("onLoginSuccessfully", this, "onLoginSuccessfully");   //监听服务端抛出的登陆成功事件*/
	}
	
    //登陆
    public void login()
    {
        Debug.LogFormat("登陆事件：账号：{0}  密码：{1}", userAccount.text, userPassword.text);
        KBEngine.Event.fireIn("login", userAccount.text, userPassword.text, System.Text.Encoding.UTF8.GetBytes("PC"));  //发起登陆事件
    }

    //注册
    public void register()
    {

    }

    //登陆成功，进入游戏
    public void onLoginSuccessfully(UInt64 rndUUID, Int32 eid, KBEngine.Account accountEntity)
    {
        Debug.Log("登陆成功，进入游戏！");
        if (KBEngineApp.app.player().id != accountEntity.id)
            return;
        SceneManager.LoadScene(1);
    }

    //登陆失败，弹出信息窗口
    public void onLoginFailed(UInt16 failedcode)
    {
        Debug.Log("login is failed(登陆失败), err=" + KBEngineApp.app.serverErr(failedcode));
    }
}
