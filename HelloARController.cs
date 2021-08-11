//-----------------------------------------------------------------------
// <copyright file="HelloARController.cs" company="Google">
//
// Copyright 2017 Google Inc. All Rights Reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// </copyright>
//-----------------------------------------------------------------------

namespace GoogleARCore.Examples.HelloAR
{
    using System.Collections.Generic;
    using GoogleARCore;
    using GoogleARCore.Examples.Common;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.Rendering;
    using UnityEngine.UI;

#if UNITY_EDITOR
    // Set up touch input propagation while using Instant Preview in the editor.
    using Input = InstantPreviewInput;

#endif

    /// <summary>
    /// Controls the HelloAR example.
    /// </summary>
    public class HelloARController : MonoBehaviour
    {
        public Text camPoseText;
        public GameObject m_firstPersonCamera;
        public GameObject cameraTarget;
        private Vector3 m_prevARPosePosition;
        private bool trackingStarted = false;



        /// <summary>
        /// </summary>
        private bool m_IsQuitting = false;
        public void Start()
        {
            m_prevARPosePosition = Vector3.zero;//初始化赋值，将坐标初始化在（0，0，0）
        }
        public void Update()
        {
            _UpdateApplicationLifecycle();//用来监听应用程序的生命周期变化
            if (Session.Status != SessionStatus.Tracking)
            {
                trackingStarted = false;                      // 如果跟踪丢失或者没有初始化
                camPoseText.text = "Lost tracking, wait ...";
                const int LOST_TRACKING_SLEEP_TIMEOUT = 15;
                Screen.sleepTimeout = LOST_TRACKING_SLEEP_TIMEOUT;//处于非跟踪状态，15秒后进入休眠
                return;
            }
            else
            {
                //清空面板
                camPoseText.text = "";
            }
            camPoseText.text = "";
            Screen.sleepTimeout = SleepTimeout.NeverSleep; //防止设备休眠

            //移动定位
            Vector3 currentARPosition = Frame.Pose.position;//初始状态会获得手机设备的姿势信息
            if (!trackingStarted)
            {
                trackingStarted = true;
                m_prevARPosePosition = Frame.Pose.position;//跟踪开始后会重新获得姿势信息
            }
            //记住以前的位置，就可以获得移动增量
            Vector3 deltaPosition = currentARPosition - m_prevARPosePosition;
            m_prevARPosePosition = currentARPosition;
            if (cameraTarget != null)
            {
                //我们仅在XZ平面上应用平移。
                cameraTarget.transform.Translate(deltaPosition.x, 0.0f, deltaPosition.z);
                // 设置要在CameraFollow脚本中使用的姿势旋转
                m_firstPersonCamera.GetComponent<FollowTarget>().targetRot = Frame.Pose.rotation;
            }



        }
        /// <summary>
        /// Check and update the application lifecycle.
        /// </summary>
        private void _UpdateApplicationLifecycle()
        {
            // 按下“后退”按钮时退出应用程序。
            if (Input.GetKey(KeyCode.Escape))
            {
                Application.Quit();
            }

            // 仅在不跟踪时让屏幕进入休眠状态.
            if (Session.Status != SessionStatus.Tracking)
            {
                const int lostTrackingSleepTimeout = 15;
                Screen.sleepTimeout = lostTrackingSleepTimeout;
            }
            else
            {
                Screen.sleepTimeout = SleepTimeout.NeverSleep;
            }

            if (m_IsQuitting)
            {
                return;
            }

            // 退出ARCore是否无法连接，并给Unity一些时间进行连接
            //出现
            if (Session.Status == SessionStatus.ErrorPermissionNotGranted)
            {
                _ShowAndroidToastMessage("Camera permission is needed to run this application.");
                m_IsQuitting = true;
                Invoke("_DoQuit", 0.5f);
            }
            else if (Session.Status.IsError())
            {
                _ShowAndroidToastMessage(
                    "ARCore encountered a problem connecting.  Please start the app again.");
                m_IsQuitting = true;
                Invoke("_DoQuit", 0.5f);
            }
        }

        /// <summary>
        /// Actually quit the application.
        /// </summary>
        private void _DoQuit()
        {
            Application.Quit();
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
}
