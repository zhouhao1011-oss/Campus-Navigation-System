using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class collosionmanger : MonoBehaviour {

    public Text camPoseText;
    public Text camPoseText1;
    public GameObject cstoshuli;
    public GameObject cstolibrary;
    public GameObject guidepanel;
    public GameObject shulitubiao;
    public GameObject jisuanjitubiao;


    void OnTriggerEnter(Collider other)
    {
        camPoseText.text = other.gameObject.name;
        // 碰撞对象的音频附加到红色小球

       if (other.gameObject.tag == "shuli")
       {
           if (other.GetComponent<AudioSource>() != null)
           {
               other.GetComponent<AudioSource>().Play();
               cstoshuli.SetActive(false);
           }
           shulitubiao.SetActive(false);
           jisuanjitubiao.SetActive(false);
           _ShowAndroidToastMessage("The destination has arrived ！");
           camPoseText1.text = "  The school of mathematics and physics consists of mathematics department, physics department and solar energy research institute.\n  The college has a high-quality teaching staff with young and middle-aged backbone teachers as the main body.";
       }

       else  if (other.gameObject.tag == "library")
       {
           if (other.GetComponent<AudioSource>() != null)
           {
               other.GetComponent<AudioSource>().Play();
               cstolibrary.SetActive(false);
           }
           _ShowAndroidToastMessage("图书馆已到达！");
           Application.LoadLevel("library map");
          // camPoseText1.text = "上海电力大学图书馆始建于1951年，经过60多\n年的历史沧桑和文化积淀，形成了以工为主,兼有管、理、经、\n文等学科，动力、电力特色明显，传统藏书与数字信息兼具的资源体系。\n截止2018年12月31日，历年累计纸质藏书132.3万册\n";
       }
        

    }

    void OnTriggerExit(Collider other)
    {
        // 碰撞结束之后通知音频播放
        if (other.GetComponent<AudioSource>() != null)
        {
            other.GetComponent<AudioSource>().Stop();
        }
        //清理camposetext
        camPoseText.text = "";
        camPoseText1.text = "";
        GetComponent<LineRenderer>().enabled = false;
    }


    private void _ShowAndroidToastMessage(string message)
    {
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject unityActivity =
            unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

        if (unityActivity != null)
        {
            AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");
            unityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
            {
                AndroidJavaObject toastObject =
                    toastClass.CallStatic<AndroidJavaObject>(
                        "makeText", unityActivity, message, 0);
                toastObject.Call("show");
            }));
        }
    }



   
}

