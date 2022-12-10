using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TCP stuff
using System;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Globalization;

public class AnimateHuman : MonoBehaviour
{
    // For processing keypoints
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

    // TCP
    Thread receiveThread;
    TcpClient client;
    TcpListener listener;

    string ip = "43.200.230.144";
    int port = 22222;
    List<List<float>> keypoints_with_scores;
    List<List<float>> keypoints_with_scores_valid;

    GameObject Head, Body;
    GameObject LeftArm_top, LeftArm_bottom;
    GameObject RightArm_top, RightArm_bottom;
    GameObject LeftLeg_top, LeftLeg_bottom;
    GameObject RightLeg_top, RightLeg_bottom;

    GameObject character;

    // Start is called before the first frame update
    void Start()
    {
        InitTCP();
        InitGameObjects();
        InitKeypoints();
    }

    // Update is called once per frame
    void Update()
    {
        ShowKeypointStatus();
        SetHumanRotation();
    }

    private void SetHumanRotation()
    {
        float x, y, angle, refAngle;
        Vector3 rotation;

        // Body: use vector connecting the midpoint of shoulders and midpoint of hips
        x = (GetPartX("left_shoulder") + GetPartX("right_shoulder")) / 2f - (GetPartX("left_hip") + GetPartX("right_hip")) / 2f;
        y = (GetPartY("left_shoulder") + GetPartY("right_shoulder")) / 2f - (GetPartY("left_hip") + GetPartY("right_hip")) / 2f;
        angle = GetAngleFromXaxis(new Vector2(x, y));
        refAngle = angle;
        rotation = Body.transform.localEulerAngles;
        Body.transform.localRotation = Quaternion.Euler(rotation.x, rotation.y, angle - 90);

        // Head: use vector connecting eye and eye
        x = GetPartX("right_eye") - GetPartX("left_eye");
        y = GetPartY("right_eye") - GetPartY("left_eye");
        angle = GetAngleFromXaxis(new Vector2(x, y));
        rotation = Head.transform.localEulerAngles;
        Head.transform.localRotation = Quaternion.Euler(rotation.x, rotation.y, 90 + angle - refAngle);

        // LeftArm_top: use vector connecting elbow and shoulder
        x = GetPartX("left_shoulder") - GetPartX("left_elbow");
        y = GetPartY("left_shoulder") - GetPartY("left_elbow");
        angle = GetAngleFromXaxis(new Vector2(x, y));
        refAngle = angle;
        rotation = LeftArm_top.transform.localEulerAngles;
        LeftArm_top.transform.localRotation = Quaternion.Euler(rotation.x, rotation.y, 90 + angle);

        // LeftArm_bottom: use vector connecting elbow and wrist
        x = GetPartX("left_wrist") - GetPartX("left_elbow");
        y = GetPartY("left_wrist") - GetPartY("left_elbow");
        angle = GetAngleFromXaxis(new Vector2(x, y));
        rotation = LeftArm_bottom.transform.localEulerAngles;
        LeftArm_bottom.transform.localRotation = Quaternion.Euler(rotation.x, rotation.y, 180 - angle + refAngle);

        // RightArm_top: use vector connecting elbow and shoulder
        x = GetPartX("right_shoulder") - GetPartX("right_elbow");
        y = GetPartY("right_shoulder") - GetPartY("right_elbow");
        angle = GetAngleFromXaxis(new Vector2(x, y));
        refAngle = angle;
        rotation = RightArm_top.transform.localEulerAngles;
        RightArm_top.transform.localRotation = Quaternion.Euler(rotation.x, rotation.y, 90 + angle);

        // RightArm_bottom: use vector connecting elbow and wrist
        x = GetPartX("right_wrist") - GetPartX("right_elbow");
        y = GetPartY("right_wrist") - GetPartY("right_elbow");
        angle = GetAngleFromXaxis(new Vector2(x, y));
        rotation = RightArm_bottom.transform.localEulerAngles;
        RightArm_bottom.transform.localRotation = Quaternion.Euler(rotation.x, rotation.y, 180 - angle + refAngle);

        // LeftLeg_top: use vector connecting hip and knee
        x = GetPartX("left_hip") - GetPartX("left_knee");
        y = GetPartY("left_hip") - GetPartY("left_knee");
        angle = GetAngleFromXaxis(new Vector2(x, y));
        refAngle = angle;
        rotation = LeftLeg_top.transform.localEulerAngles;
        LeftLeg_top.transform.localRotation = Quaternion.Euler(rotation.x, rotation.y, angle - 90);

        // LeftLeg_bottom: use vector connecting knee and ankle
        x = GetPartX("left_ankle") - GetPartX("left_knee");
        y = GetPartY("left_ankle") - GetPartY("left_knee");
        angle = GetAngleFromXaxis(new Vector2(x, y));
        rotation = LeftLeg_bottom.transform.localEulerAngles;
        LeftLeg_bottom.transform.localRotation = Quaternion.Euler(rotation.x, rotation.y, 180 - angle + refAngle);

        // RightLeg_top: use vector connecting hip and knee
        x = GetPartX("right_hip") - GetPartX("right_knee");
        y = GetPartY("right_hip") - GetPartY("right_knee");
        angle = GetAngleFromXaxis(new Vector2(x, y));
        rotation = RightLeg_top.transform.localEulerAngles;
        refAngle = angle;
        RightLeg_top.transform.localRotation = Quaternion.Euler(rotation.x, rotation.y, angle - 90);

        // RightLeg_bottom: use vector connecting knee and ankle
        x = GetPartX("right_ankle") - GetPartX("right_knee");
        y = GetPartY("right_ankle") - GetPartY("right_knee");
        angle = GetAngleFromXaxis(new Vector2(x, y));
        rotation = RightLeg_bottom.transform.localEulerAngles;
        RightLeg_bottom.transform.localRotation = Quaternion.Euler(rotation.x, rotation.y, 180 - angle + refAngle);

        x = (GetPartX("left_hip") + GetPartX("right_hip")) / 2f;
        print(x);
        character.transform.position = new Vector3(x, 0, 0);
    }

    private void ShowKeypointStatus()
    {
        int count = 0;
        foreach (List<float> keypoint in keypoints_with_scores)
        {
            if (keypoint[PROB] > THRESHOLD) count++;
        }
        print(count + "/" + 17 + " Keypoints detected");
    }

    // Returns angle between given vector and x-axis
    private float GetAngleFromXaxis(Vector2 vec)
    {
        double radianAngle = Math.Atan2(vec.y, vec.x);
        return (float)(radianAngle * 180 / Math.PI);
    }

    private float GetPartX(string part)
    {
        if (keypoints_with_scores[PART[part]][PROB] > THRESHOLD)
        {
            return keypoints_with_scores[PART[part]][X];
        }
        else
        {
            return keypoints_with_scores_valid[PART[part]][X];
        }
    }

    private float GetPartY(string part)
    {
        if (keypoints_with_scores[PART[part]][PROB] > THRESHOLD)
        {
            return keypoints_with_scores[PART[part]][Y];
        }
        else
        {
            return keypoints_with_scores_valid[PART[part]][Y];
        }
    }

    private void InitGameObjects()
    {
        GameObject Armature = transform.Find("Armature").gameObject;
        GameObject Spine = Armature.transform.Find("Spine").gameObject;
        GameObject ThighL = Armature.transform.Find("ThighL").gameObject;
        GameObject ThighR = Armature.transform.Find("ThighR").gameObject;
        character = gameObject;

        Body = Spine;
        Head = Spine.transform.Find("Chest").Find("Neck").gameObject;
        LeftArm_top = Spine.transform.Find("Chest").Find("ShoulderL").gameObject;
        LeftArm_bottom = Spine.transform.Find("Chest").Find("ShoulderL").Find("UpperArmL").Find("LoweArmL").gameObject;
        RightArm_top = Spine.transform.Find("Chest").Find("ShoulderR").gameObject;
        RightArm_bottom = Spine.transform.Find("Chest").Find("ShoulderR").Find("UpperArmR").Find("LoweArmR").gameObject;
        LeftLeg_top = ThighL;
        LeftLeg_bottom = ThighL.transform.Find("ShinL").gameObject;
        RightLeg_top = ThighR;
        RightLeg_bottom = ThighR.transform.Find("ShinR").gameObject;
    }

    private void InitKeypoints()
    {
        keypoints_with_scores = ParseData("0,0,0/0,0,0/0,0,0/0,0,0/0,0,0/0,0,0/0,0,0/0,0,0/0,0,0/0,0,0/0,0,0/0,0,0/0,0,0/0,0,0/0,0,0/0,0,0/0,0,0");
        keypoints_with_scores_valid = ParseData("0,0,0/0,0,0/0,0,0/0,0,0/0,0,0/0,0,0/0,0,0/0,0,0/0,0,0/0,0,0/0,0,0/0,0,0/0,0,0/0,0,0/0,0,0/0,0,0/0,0,0");
    }

    private void InitTCP()
    {
        receiveThread = new Thread(new ThreadStart(ReceiveData));
        receiveThread.IsBackground = true;
        receiveThread.Start();
    }

    private void ReceiveData()
    {
        try
        {
            // Byte[] array for the transmitted data
            Byte[] bytes = new Byte[1024];
            while (true)
            {
                using (TcpClient tcpClient = new TcpClient())
                {
                    tcpClient.Connect(IPAddress.Parse(ip), port);
                    print("Client connected");
                    using (NetworkStream stream = tcpClient.GetStream())
                    {
                        int length;
                        while ((length = stream.Read(bytes, 0, bytes.Length)) != 0)
                        {
                            var incommingData = new byte[length];
                            Array.Copy(bytes, 0, incommingData, 0, length);
                            string clientMessage_recv = Encoding.ASCII.GetString(incommingData);
                            string[] clientMessage_recv_split = clientMessage_recv.Split('!');

                            foreach (string clientMessage in clientMessage_recv_split)
                            {
                                if (clientMessage.IndexOf("/") != -1)
                                {
                                    bool isEveryPointValid = true;
                                    bool isDataValid = true;
                                    List<List<float>> keypoints_with_scores_received = null;

                                    try
                                    {
                                        keypoints_with_scores_received = ParseData(clientMessage);
                                        foreach (List<float> keypoint in keypoints_with_scores_received)
                                        {
                                            if (keypoint[PROB] < THRESHOLD)
                                            {
                                                isEveryPointValid = false;
                                                break;
                                            }
                                        }
                                    }
                                    catch (Exception e)
                                    {
                                        print(e);
                                        isEveryPointValid = false;
                                        isDataValid = false;
                                    }

                                    if (isDataValid)
                                    {
                                        keypoints_with_scores = keypoints_with_scores_received;
                                        if (isEveryPointValid) keypoints_with_scores_valid = keypoints_with_scores;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        catch (Exception e)
        {
            // If something bad happened and threw an exception, just print out the error
            print(e.ToString());
        }
    }

    private List<List<float>> ParseData(string data)
    {
        List<List<float>> parsed = new List<List<float>>();
        // data is in the form: 0.12,0.34,0.56/0.78,0.90,0.12/0.34,0.56,0.78
        string[] rows = data.Split('/');
        foreach (string row in rows)
        {
            List<float> parsedRow = new List<float>();
            string[] values = row.Split(',');
            foreach (string value in values)
            {
                float floatValue = float.Parse(value, CultureInfo.InvariantCulture.NumberFormat);
                parsedRow.Add(floatValue);
            }
            parsed.Add(parsedRow);
        }
        return parsed;
    }

    void OnApplicationQuit()
    {
        // Close the thread when the application quits
        receiveThread.Abort();
    }
}
