using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CollisionManager : MonoBehaviour
{

    public Text camPoseText;
    public Text camPoseText1;
    public GameObject baozhitozongjiao;
    public InputField inputorigin;
    public InputField inputdestination;
    public GameObject bookguide;

    void OnTriggerEnter(Collider other)
    {
        camPoseText.text = other.gameObject.name;
        // To play an audio attached to the collision object the red pointer
        // is colliding with

    

             if (other.gameObject.tag == "shuli")
            {
                if (other.GetComponent<AudioSource>() != null)
                {
                   // other.GetComponent<AudioSource>().Play();
                    baozhitozongjiao.SetActive(false);
                }
                _ShowAndroidToastMessage("The destination has arrived！");
                bookguide.SetActive(true);

                // camPoseText1.text = "  Confucius Research, founded in 1986, is a bimonthly International Chinese academic journal, which is in charge of the Publicity Department of Shandong provincial Party committee and sponsored by China Confucius Foundation\n Position:3-south01  number 11";
            }

       // 《孔子研究》创刊于1986年，双月刊，是由山东省委宣传部主管、中国孔子基金会主办的国际性中文学术期刊
     
    }

    void OnTriggerExit(Collider other)
    {
        // To stop playing the audio when the collision stops happening
        if (other.GetComponent<AudioSource>() != null)
        {
            other.GetComponent<AudioSource>().Stop();
        }
        // Clean the message
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

