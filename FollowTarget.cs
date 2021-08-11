using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarget : MonoBehaviour
{

    public Transform targetToFollow;
    public Quaternion targetRot;                      // 设备相机从Frame.Pose.rotation的旋转 
    public float distanceToTargetXZ = 10.0f;          //XZ平面到目标的距离
    public float heightOverTarget = 5.0f;
    public float heightSmoothingSpeed = 2.0f;
    public float rotationSmoothingSpeed = 2.0f;
   
    void LateUpdate() // 使用lateUpdate可以确保在更新目标之后更新相机。
    {
        if (!targetToFollow)
            return;
        Vector3 targetEulerAngles = targetRot.eulerAngles;  
        float rotationToApplyAroundY = targetEulerAngles.y + 180.0f;// 计算我们要应用于相机的绕Y轴的当前旋转角度。  //当设备相机指向Z轴负方向时，我们增加180度
        float heightToApply = targetToFollow.position.y + heightOverTarget;       
        float newCamRotAngleY = Mathf.LerpAngle(transform.eulerAngles.y, rotationToApplyAroundY, rotationSmoothingSpeed * Time.deltaTime);   //当角度> 360时，使用LerpAngle正确处理
        float newCamHeight = Mathf.Lerp(transform.position.y, heightToApply, heightSmoothingSpeed * Time.deltaTime);//线性插值函数从transform.position.y到heightToApply
        Quaternion newCamRotYQuat = Quaternion.Euler(0, newCamRotAngleY, 0);   //绕y轴旋转

        transform.position = targetToFollow.position;//将相机位置设置为与目标位置相同   
        transform.position -= newCamRotYQuat * Vector3.forward * distanceToTargetXZ; // 平滑地沿newCamRotYQuat所定义的方向和distanceToTargetXZ所定义的量移动相机  
 
        transform.position = new Vector3(transform.position.x, newCamHeight, transform.position.z);   // 最后设置相机高度   

        transform.LookAt(targetToFollow);  // 保持相机一直对准目标以绕X轴旋转
    }


}
