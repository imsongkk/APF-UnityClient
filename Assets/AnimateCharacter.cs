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

public class AnimateCharacter : MonoBehaviour
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
        SetHumanPosition();
        SetHumanRotation();
        //PrintKeypointStatus();
    }

    private void SetHumanPosition()
    {
        float scaleX = 8f;
        float scaleY = 7f;
        float x, y;
        float z = 0;

        /* Head: same position with nose
        x = GetPartX("nose");
        y = GetPartY("nose");
        Head.transform.localPosition = new Vector3(x * scaleX, y * scaleY, z);
        */

        // Body: average of shoulders and hips
        x = (GetPartX("left_shoulder") + GetPartX("right_shoulder") + GetPartX("left_hip") + GetPartX("right_hip")) / 4f;
        y = (GetPartY("left_shoulder") + GetPartY("right_shoulder") + GetPartY("left_hip") + GetPartY("right_hip")) / 4f;
        Body.transform.localPosition = new Vector3(x * scaleX, y * scaleY, z);
        Head.transform.localPosition = new Vector3(0, 0.9f, 0);

        // LeftArm_top: average of shoulder and elbow
        x = (GetPartX("left_shoulder") + GetPartX("left_elbow")) / 2f;
        y = (GetPartY("left_shoulder") + GetPartY("left_elbow")) / 2f;
        LeftArm_top.transform.localPosition = new Vector3(x * scaleX, y * scaleY, z);
        
        // LeftArm_bottom: average of elbow and wrist
        x = (GetPartX("left_elbow") + GetPartX("left_wrist")) / 2f;
        y = (GetPartY("left_elbow") + GetPartY("left_wrist")) / 2f;
        LeftArm_bottom.transform.localPosition = new Vector3(x * scaleX, y * scaleY, z);

        // RightArm_top: average of shoulder and elbow
        x = (GetPartX("right_shoulder") + GetPartX("right_elbow")) / 2f;
        y = (GetPartY("right_shoulder") + GetPartY("right_elbow")) / 2f;
        RightArm_top.transform.localPosition = new Vector3(x * scaleX, y * scaleY, z);

        // RightArm_bottom: average of elbow and wrist
        x = (GetPartX("right_elbow") + GetPartX("right_wrist")) / 2f;
        y = (GetPartY("right_elbow") + GetPartY("right_wrist")) / 2f;
        RightArm_bottom.transform.localPosition = new Vector3(x * scaleX, y * scaleY, z);

        // LeftLeg_top: average of hip and knee
        x = (GetPartX("left_hip") + GetPartX("left_knee")) / 2f;
        y = (GetPartY("left_hip") + GetPartY("left_knee")) / 2f;
        LeftLeg_top.transform.localPosition = new Vector3(x * scaleX, y * scaleY, z);

        // LeftLeg_bottom: average of knee and ankle
        x = (GetPartX("left_knee") + GetPartX("left_ankle")) / 2f;
        y = (GetPartY("left_knee") + GetPartY("left_ankle")) / 2f;
        LeftLeg_bottom.transform.localPosition = new Vector3(x * scaleX, y * scaleY, z);

        // RightLeg_top: average of hip and knee
        x = (GetPartX("right_hip") + GetPartX("right_knee")) / 2f;
        y = (GetPartY("right_hip") + GetPartY("right_knee")) / 2f;
        RightLeg_top.transform.localPosition = new Vector3(x * scaleX, y * scaleY, z);

        // RightLeg_bottom: average of knee and ankle
        x = (GetPartX("right_knee") + GetPartX("right_ankle")) / 2f;
        y = (GetPartY("right_knee") + GetPartY("right_ankle")) / 2f;
        RightLeg_bottom.transform.localPosition = new Vector3(x * scaleX, y * scaleY, z);

    }

    private void SetHumanRotation()
    {
        float x, y, angle;
        float bodyAngle;

        // Body: use vector connecting shoulder and shoulder
        x = GetPartX("right_shoulder") - GetPartX("left_shoulder");
        y = GetPartY("right_shoulder") - GetPartY("left_shoulder");
        bodyAngle = GetAngleFromXaxis(new Vector2(x, y));
        Body.transform.localRotation = Quaternion.Euler(0, 0, bodyAngle);

        // Head: use vector connecting eye and eye
        x = GetPartX("right_eye") - GetPartX("left_eye");
        y = GetPartY("right_eye") - GetPartY("left_eye");
        angle = GetAngleFromXaxis(new Vector2(x, y));
        Head.transform.localRotation = Quaternion.Euler(0, 0, angle - bodyAngle);

        // LeftArm_top: use vector connecting elbow and shoulder
        x = GetPartX("left_shoulder") - GetPartX("left_elbow");
        y = GetPartY("left_shoulder") - GetPartY("left_elbow");
        angle = GetAngleFromXaxis(new Vector2(x, y));
        LeftArm_top.transform.localRotation = Quaternion.Euler(0, 0, angle - 90);
        
        // LeftArm_bottom: use vector connecting elbow and wrist
        x = GetPartX("left_wrist") - GetPartX("left_elbow");
        y = GetPartY("left_wrist") - GetPartY("left_elbow");
        angle = GetAngleFromXaxis(new Vector2(x, y));
        LeftArm_bottom.transform.localRotation = Quaternion.Euler(0, 0, angle - 90);

        // RightArm_top: use vector connecting elbow and shoulder
        x = GetPartX("right_shoulder") - GetPartX("right_elbow");
        y = GetPartY("right_shoulder") - GetPartY("right_elbow");
        angle = GetAngleFromXaxis(new Vector2(x, y));
        RightArm_top.transform.localRotation = Quaternion.Euler(0, 0, angle + 90);

        // RightArm_bottom: use vector connecting elbow and wrist
        x = GetPartX("right_wrist") - GetPartX("right_elbow");
        y = GetPartY("right_wrist") - GetPartY("right_elbow");
        angle = GetAngleFromXaxis(new Vector2(x, y));
        RightArm_bottom.transform.localRotation = Quaternion.Euler(0, 0, angle + 90);

        // LeftLeg_top: use vector connecting hip and knee
        x = GetPartX("left_hip") - GetPartX("left_knee");
        y = GetPartY("left_hip") - GetPartY("left_knee");
        angle = GetAngleFromXaxis(new Vector2(x, y));
        LeftLeg_top.transform.localRotation = Quaternion.Euler(0, 0, angle - 90);

        // LeftLeg_bottom: use vector connecting knee and ankle
        x = GetPartX("left_ankle") - GetPartX("left_knee");
        y = GetPartY("left_ankle") - GetPartY("left_knee");
        angle = GetAngleFromXaxis(new Vector2(x, y));
        LeftLeg_bottom.transform.localRotation = Quaternion.Euler(0, 0, angle - 90);

        // RightLeg_top: use vector connecting hip and knee
        x = GetPartX("right_hip") - GetPartX("right_knee");
        y = GetPartY("right_hip") - GetPartY("right_knee");
        angle = GetAngleFromXaxis(new Vector2(x, y));
        RightLeg_top.transform.localRotation = Quaternion.Euler(0, 0, angle + 90);

        // RightLeg_bottom: use vector connecting knee and ankle
        x = GetPartX("right_ankle") - GetPartX("right_knee");
        y = GetPartY("right_ankle") - GetPartY("right_knee");
        angle = GetAngleFromXaxis(new Vector2(x, y));
        RightLeg_bottom.transform.localRotation = Quaternion.Euler(0, 0, angle + 90);
    }

    private void PrintKeypointStatus()
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
        } else
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
        Body = transform.Find("Body").gameObject;
        Head = Body.transform.Find("Head").gameObject;
        LeftArm_top = transform.Find("LeftArm_top").gameObject;
        LeftArm_bottom = transform.Find("LeftArm_bottom").gameObject;
        RightArm_top = transform.Find("RightArm_top").gameObject;
        RightArm_bottom = transform.Find("RightArm_bottom").gameObject;
        LeftLeg_top = transform.Find("LeftLeg_top").gameObject;
        LeftLeg_bottom = transform.Find("LeftLeg_bottom").gameObject;
        RightLeg_top = transform.Find("RightLeg_top").gameObject;
        RightLeg_bottom = transform.Find("RightLeg_bottom").gameObject;
    }

    private void InitKeypoints()
    {
        keypoints_with_scores = ParseData("0,0,0/0,0,0/0,0,0/0,0,0/0,0,0/0,0,0/0,0,0/0,0,0/0,0,0/0,0,0/0,0,0/0,0,0/0,0,0/0,0,0/0,0,0/0,0,0/0,0,0");
        keypoints_with_scores_valid = ParseData("0,0,0/0,0,0/0,0,0/0,0,0/0,0,0/0,0,0/0,0,0/0,0,0/0,0,0/0,0,0/0,0,0/0,0,0/0,0,0/0,0,0/0,0,0/0,0,0/0,0,0");
    }

    private void InitTCP()
    {
        receiveThread = new Thread(new ThreadStart(ReceiveDataFromLocalhost));
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

    private void ReceiveDataFromLocalhost()
    {
        try
        {
            int localport = 2119;
            print("Waiting for connection");
            listener = new TcpListener(IPAddress.Parse("127.0.0.1"), localport);
            listener.Start();

            // Byte[] array for the transmitted data
            Byte[] bytes = new Byte[1024];
            while (true)
            {
                using (client = listener.AcceptTcpClient())
                {
                    print("Client connected");
                    using (NetworkStream stream = client.GetStream())
                    {
                        int length;
                        while ((length = stream.Read(bytes, 0, bytes.Length)) != 0)
                        {
                            var incommingData = new byte[length];
                            Array.Copy(bytes, 0, incommingData, 0, length);
                            string clientMessage = Encoding.ASCII.GetString(incommingData);
                            if (clientMessage.IndexOf("/") != -1)
                            {
                                //print(keypoints_with_scores);
                                keypoints_with_scores = ParseData(clientMessage);
                                bool isEveryPointValid = true;
                                foreach (List<float> keypoint in keypoints_with_scores)
                                {
                                    if (keypoint[PROB] < THRESHOLD)
                                    {
                                        isEveryPointValid = false;
                                        break;
                                    }
                                }
                                if (isEveryPointValid) keypoints_with_scores_valid = keypoints_with_scores;
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
