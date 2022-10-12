using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class IKScript : MonoBehaviour
{
    public float FrameRate;
    public string DataFile;
    public int FrameStart, FrameEnd;
    public List<Transform> Bones = new List<Transform>();
    public bool DebugLineFigures;

    GameObject FullbodyIK;
    float timeElapsedThisFrame;
    int currentFrame;

    int[,] JointMappingKP17Bones = new int[,] { { 0, 1 }, { 1, 2 }, { 2, 3 }, { 0, 4 }, { 4, 5 }, { 5, 6 }, { 0, 7 }, { 7, 8 }, { 8, 9 }, { 9, 10 }, { 8, 11 }, { 11, 12 }, { 12, 13 }, { 8, 14 }, { 14, 15 }, { 15, 16 } };
    int[,] JointMappingIKBones = new int[,] { { 0, 2 }, { 2, 3 }, { 0, 5 }, { 5, 6 }, { 0, 9 }, { 9, 10 }, { 9, 11 }, { 11, 12 }, { 12, 13 }, { 9, 14 }, { 14, 15 }, { 15, 16 } };
    int[,] BonePosMapping = new int[,] { { 0, 1 }, { 1, 2 }, { 0, 3 }, { 3, 4 }, { 0, 5 }, { 5, 6 }, { 5, 7 }, { 7, 8 }, { 8, 9 }, { 5, 10 }, { 10, 11 }, { 11, 12 } };

    List<List<Vector3>> allPoints;
    Vector3[] points = new Vector3[17];
    Vector3[] normalizedBoneVectors = new Vector3[12];
    float[] boneLength = new float[12];

    enum KP17Bones
    {
        Hips,

        RightKnee,
        RightFoot,
        LeftKnee,
        LeftFoot,
        Neck,
        Head,

        LeftArm,
        LeftElbow,
        LeftWrist,
        RightArm,
        RightElbow,
        RightWrist,
    }

    void Start()
    {
        FullbodyIK = GameObject.Find("FullBodyIK");
        if (FullbodyIK)
        {
            for (int i = 0; i < Enum.GetNames(typeof(KP17Bones)).Length; i++)
                Bones.Add(GameObject.Find(Enum.GetName(typeof(KP17Bones), i)).transform);
            for (int i = 0; i < 12; i++)
                boneLength[i] = Vector3.Distance(Bones[BonePosMapping[i, 0]].position, Bones[BonePosMapping[i, 1]].position);
            Debug.Log("IK initialized");
        }

        LoadData();
        timeElapsedThisFrame = 0;
    }

    void Update()
    {
        timeElapsedThisFrame += Time.deltaTime;
        if (timeElapsedThisFrame > (1 / FrameRate))
        {
            timeElapsedThisFrame = 0f;
            UpdateFrameData();
        }
        UpdateIK();
    }


    void LoadData()
    {
        allPoints = new List<List<Vector3>>(650);

        StreamReader reader = new StreamReader(Application.dataPath+DataFile);
        string line;
        int keypoint = 0;
        List<Vector3> list = new List<Vector3>(33);
        while ((line = reader.ReadLine()) != null)
        {
            if (keypoint == 33)
            {
                List<Vector3> mapped = new List<Vector3>(17);
                for (int i = 0; i < 17; i++)
                {
                    mapped.Add(map(i, list));
                }
                allPoints.Add(mapped);
                keypoint = 0;
                list = new List<Vector3>(33);
                continue;
            }

            float x = -float.Parse(line.Split(' ')[1]);
            float y = -float.Parse(reader.ReadLine().Split(' ')[1]);
            float z = float.Parse(reader.ReadLine().Split(' ')[1]);
            list.Add(new Vector3(x, y, z));
            keypoint++;
        }
        Debug.Log("Frames loaded: "+allPoints.Count);

        currentFrame = FrameStart;
        if (FrameEnd < 1 || FrameEnd > allPoints.Count)
            FrameEnd = allPoints.Count;

        UpdateFrameData();
    }

    Vector3 map(int i, List<Vector3> list)
    {
        /*
		openpose17, blazepose33
		0	mid(23,24)
		1	24
		2	26
		3	28
		4	23
		5	25
		6	27
		7	mid(12,23)
		8	mid(12,11)
		9	0
		10	mid(7,8) or mid(2,5)
		11	11
		12	13
		13	15
		14	12
		15	14
		16	16
		*/

        /*
         * this IK somehow mirrored keypoints of openpose17, so left and right are swapped
		IK17, blazepose33
		0	mid(23,24)
		
		1	23
		2	25
		3	27
        4	24
		5	26
		6	28

		7	mid(12,23)
		8	mid(12,11)
		9	0
		10	mid(7,8) or mid(2,5)

		11	12
		12	14
		13	16
        14	11
		15	13
		16	15
		*/
        switch (i)
        {
            case 0:
                return Vector3.Lerp(list[23], list[24], 0.5f);
            
            case 1:
                return list[23];
            case 2:
                return list[25];
            case 3:
                return list[27];
            case 4:
                return list[24];
            case 5:
                return list[26];
            case 6:
                return list[28];

            case 7:
                return Vector3.Lerp(list[23], list[12], 0.5f);
            case 8:
                return Vector3.Lerp(list[12], list[11], 0.5f);
            case 9:
                return list[0];
            case 10:
                return Vector3.Lerp(list[2], list[5], 0.5f);

            
            case 11:
                return list[12];
            case 12:
                return list[14];
            case 13:
                return list[16];
            case 14:
                return list[11];
            case 15:
                return list[13];
            case 16:
                return list[15];
        }
        return list[0];
    }

    void UpdateFrameData()
    {
        if (currentFrame >= FrameEnd)
            return;

        List<Vector3> curFramePoints = allPoints[currentFrame];
        for (int i = 0; i < 17; i++)
            points[i] = curFramePoints[i];
        for (int i = 0; i < 12; i++)
            normalizedBoneVectors[i] = (points[JointMappingIKBones[i, 1]] - points[JointMappingIKBones[i, 0]]).normalized;

        Debug.Log("frame " + currentFrame);
        currentFrame++;
    }

    void UpdateIK()
    {
        //move transform - temp solution, should use foot height to align
        Bones[0].position = Vector3.Lerp(Bones[0].position, points[0] + Vector3.up * 0.8f, 0.1f);
        FullbodyIK.transform.position = Vector3.Lerp(FullbodyIK.transform.position, points[0], 0.1f);
        //rotate model
        Vector3 normalizedHipVec = (normalizedBoneVectors[0] + normalizedBoneVectors[2] + normalizedBoneVectors[4]).normalized;
        FullbodyIK.transform.forward = Vector3.Lerp(FullbodyIK.transform.forward, new Vector3(normalizedHipVec.x, 0, normalizedHipVec.z), 0.1f);

        for (int i = 0; i < 12; i++)
        {
            Bones[BonePosMapping[i, 1]].position = Vector3.Lerp(
                Bones[BonePosMapping[i, 1]].position,
                Bones[BonePosMapping[i, 0]].position + normalizedBoneVectors[i] * boneLength[i], 0.05f
            );

            DrawDebugLine(Bones[BonePosMapping[i, 0]].position + Vector3.right, Bones[BonePosMapping[i, 1]].position + Vector3.right, Color.red);
        }

        for (int i = 0; i < JointMappingKP17Bones.Length / 2; i++)
        {
            DrawDebugLine(points[JointMappingKP17Bones[i, 0]] + Vector3.left + Vector3.up * 0.75f, points[JointMappingKP17Bones[i, 1]] + Vector3.left + Vector3.up * 0.75f, Color.blue);
        }
    }

    void DrawDebugLine(Vector3 start, Vector3 end, Color color)
    {
        if (DebugLineFigures)
        {
            Debug.DrawLine(start, end, color);
        }
    }
}