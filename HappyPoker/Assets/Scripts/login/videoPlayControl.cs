using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class videoPlayControl : MonoBehaviour {

    public GameObject loginPanel;       //loginPanel对象
	void Update () {

        var vp = GetComponent<UnityEngine.Video.VideoPlayer>();     //获取Video Player组件
        if(!vp.isPlaying)                                           //视频加载完，显示登陆界面
        {
            loginPanel.SetActive(true);
        }
    }
}
