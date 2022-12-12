using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MoveColliders : MonoBehaviour
{
    Rigidbody Head_Collider, Body_Collider;
    Rigidbody LeftArm_top, LeftArm_bottom;
    Rigidbody RightArm_top, RightArm_bottom;
    Rigidbody LeftLeg_top, LeftLeg_bottom;
    Rigidbody RightLeg_top, RightLeg_bottom;

    HumanController myCharacter;
    GameObject Chest, Head_end;
    GameObject UpperArmL, LoweArmL, HandL;
    GameObject UpperArmR, LoweArmR, HandR;
    GameObject ThighL, ShinL, FootL;
    GameObject ThighR, ShinR, FootR;

    const int X = 0;
    const int Y = 1;
    const int PROB = 2;
    const float THRESHOLD = 0.15f;

    Dictionary<string, int> PART = new Dictionary<string, int>(){
        { "nose", 0 },
        { "left_eye", 1 },
        { "right_eye", 2 },
        { "left_ear", 3 },
        { "right_ear", 4 },
        { "left_shoulder", 5 },
        { "right_shoulder", 6 },
        { "left_elbow", 7 },
        { "right_elbow", 8 },
        { "left_wrist", 9 },
        { "right_wrist", 10 },
        { "left_hip", 11 },
        { "right_hip", 12 },
        { "left_knee", 13 },
        { "right_knee", 14 },
        { "left_ankle", 15 },
        { "right_ankle", 16 }
    };

    // Start is called before the first frame update
    void Start()
    {
        InitGameObjects();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        SetHumanPosition();
        SetHumanRotation();
    }

    private void SetHumanPosition()
    {
        LeftArm_top.MovePosition((UpperArmL.transform.position + LoweArmL.transform.position) / 2f);
        LeftArm_bottom.MovePosition(HandL.transform.position);
        RightArm_top.MovePosition((UpperArmR.transform.position + LoweArmR.transform.position) / 2f);
        RightArm_bottom.MovePosition(HandR.transform.position);
        Body_Collider.MovePosition(Chest.transform.position);
        Head_Collider.MovePosition(Head_end.transform.position);
        LeftLeg_top.MovePosition((ThighL.transform.position + ShinL.transform.position) / 2f);
        LeftLeg_bottom.MovePosition((ShinL.transform.position + FootL.transform.position) / 2f);
        RightLeg_top.MovePosition((ThighR.transform.position + ShinR.transform.position) / 2f);
        RightLeg_bottom.MovePosition((ShinR.transform.position + FootR.transform.position) / 2f);
    }

    private float GetAngleFromXaxis(Vector2 vec)
    {
        double radianAngle = Math.Atan2(vec.y, vec.x);
        return (float)(radianAngle * 180 / Math.PI);
    }

    private float GetPartX(string part)
    {

        if (myCharacter.keypoints_with_scores[PART[part]][PROB] > THRESHOLD)
        {
            return myCharacter.keypoints_with_scores[PART[part]][X];
        }
        else
        {
            return myCharacter.keypoints_with_scores_valid[PART[part]][X];
        }
    }

    private float GetPartY(string part)
    {
        if (myCharacter.keypoints_with_scores[PART[part]][PROB] > THRESHOLD)
        {
            return myCharacter.keypoints_with_scores[PART[part]][Y];
        }
        else
        {
            return myCharacter.keypoints_with_scores_valid[PART[part]][Y];
        }
    }

    private void InitGameObjects()
    {
        myCharacter = GameObject.Find("Character").GetComponent<HumanController>();
        Chest = GameObject.Find("Chest");
        Head_end = GameObject.Find("Head_end");
        UpperArmL = GameObject.Find("UpperArmL");
        LoweArmL = GameObject.Find("LoweArmL");
        HandL = GameObject.Find("HandL");
        UpperArmR = GameObject.Find("UpperArmR");
        LoweArmR = GameObject.Find("LoweArmR");
        HandR = GameObject.Find("HandR");
        ThighL = GameObject.Find("ThighL");
        ShinL = GameObject.Find("ShinL");
        FootL = GameObject.Find("FootL");
        ThighR = GameObject.Find("ThighR");
        ShinR = GameObject.Find("ShinR");
        FootR = GameObject.Find("FootR");

        Body_Collider = transform.Find("Body_Collider").gameObject.GetComponent<Rigidbody>();
        Head_Collider = transform.Find("Head_Collider").gameObject.GetComponent<Rigidbody>();
        LeftArm_top = transform.Find("LeftArm_top").gameObject.GetComponent<Rigidbody>();
        LeftArm_bottom = transform.Find("LeftArm_bottom").gameObject.GetComponent<Rigidbody>();
        RightArm_top = transform.Find("RightArm_top").gameObject.GetComponent<Rigidbody>();
        RightArm_bottom = transform.Find("RightArm_bottom").gameObject.GetComponent<Rigidbody>();
        LeftLeg_top = transform.Find("LeftLeg_top").gameObject.GetComponent<Rigidbody>();
        LeftLeg_bottom = transform.Find("LeftLeg_bottom").gameObject.GetComponent<Rigidbody>();
        RightLeg_top = transform.Find("RightLeg_top").gameObject.GetComponent<Rigidbody>();
        RightLeg_bottom = transform.Find("RightLeg_bottom").gameObject.GetComponent<Rigidbody>();
    }

    private void SetHumanRotation()
    {
        float x, y, angle;
        float bodyAngle;

        // Body: use vector connecting shoulder and shoulder
        x = GetPartX("right_shoulder") - GetPartX("left_shoulder");
        y = GetPartY("right_shoulder") - GetPartY("left_shoulder");
        bodyAngle = GetAngleFromXaxis(new Vector2(x, y));
        Body_Collider.MoveRotation(Quaternion.Euler(0, 0, bodyAngle));

        // Head: use vector connecting eye and eye
        x = GetPartX("right_eye") - GetPartX("left_eye");
        y = GetPartY("right_eye") - GetPartY("left_eye");
        angle = GetAngleFromXaxis(new Vector2(x, y));
        Head_Collider.MoveRotation(Quaternion.Euler(0, 0, angle - bodyAngle));

        // LeftArm_top: use vector connecting elbow and shoulder
        x = GetPartX("left_shoulder") - GetPartX("left_elbow");
        y = GetPartY("left_shoulder") - GetPartY("left_elbow");
        angle = GetAngleFromXaxis(new Vector2(x, y));
        LeftArm_top.MoveRotation(Quaternion.Euler(0, 0, angle - 90));

        // LeftArm_bottom: use vector connecting elbow and wrist
        x = GetPartX("left_wrist") - GetPartX("left_elbow");
        y = GetPartY("left_wrist") - GetPartY("left_elbow");
        angle = GetAngleFromXaxis(new Vector2(x, y));
        LeftArm_bottom.MoveRotation(Quaternion.Euler(0, 0, angle - 90));

        // RightArm_top: use vector connecting elbow and shoulder
        x = GetPartX("right_shoulder") - GetPartX("right_elbow");
        y = GetPartY("right_shoulder") - GetPartY("right_elbow");
        angle = GetAngleFromXaxis(new Vector2(x, y));
        RightArm_top.MoveRotation(Quaternion.Euler(0, 0, angle + 90));

        // RightArm_bottom: use vector connecting elbow and wrist
        x = GetPartX("right_wrist") - GetPartX("right_elbow");
        y = GetPartY("right_wrist") - GetPartY("right_elbow");
        angle = GetAngleFromXaxis(new Vector2(x, y));
        RightArm_bottom.MoveRotation(Quaternion.Euler(0, 0, angle + 90));

        // LeftLeg_top: use vector connecting hip and knee
        x = GetPartX("left_hip") - GetPartX("left_knee");
        y = GetPartY("left_hip") - GetPartY("left_knee");
        angle = GetAngleFromXaxis(new Vector2(x, y));
        LeftLeg_top.MoveRotation(Quaternion.Euler(0, 0, angle - 90));

        // LeftLeg_bottom: use vector connecting knee and ankle
        x = GetPartX("left_ankle") - GetPartX("left_knee");
        y = GetPartY("left_ankle") - GetPartY("left_knee");
        angle = GetAngleFromXaxis(new Vector2(x, y));
        LeftLeg_bottom.MoveRotation(Quaternion.Euler(0, 0, angle - 90));

        // RightLeg_top: use vector connecting hip and knee
        x = GetPartX("right_hip") - GetPartX("right_knee");
        y = GetPartY("right_hip") - GetPartY("right_knee");
        angle = GetAngleFromXaxis(new Vector2(x, y));
        RightLeg_top.MoveRotation(Quaternion.Euler(0, 0, angle + 90));

        // RightLeg_bottom: use vector connecting knee and ankle
        x = GetPartX("right_ankle") - GetPartX("right_knee");
        y = GetPartY("right_ankle") - GetPartY("right_knee");
        angle = GetAngleFromXaxis(new Vector2(x, y));
        RightLeg_bottom.MoveRotation(Quaternion.Euler(0, 0, angle + 90));
    }

}
