//----------------------------------------------------------------------------------------------------
// Copyright 2016 Google Inc. All Rights Reserved. 
// Copyright 2018 Yusuke Kato All Rights Reserved.
//
// Released under the Apache License 2.0
// http://www.apache.org/licenses/LICENSE-2.0
//
// Changed MarkerDetectionUIController.cs
// https://github.com/googlesamples/tango-examples-unity/blob/master/UnityExamples/Assets/TangoSDK/Examples/MarkerDetection/Scripts/MarkerDetectionUIController.cs
//
// Change Point:
// Add function (Check sensor values)
//----------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Tango;
using UnityEngine;
using UnityEngine.UI;

public class SensorValueVisualization : MonoBehaviour, ITangoVideoOverlay
{
    //Tango
    public GameObject m_markerPrefab;
    private const double MARKER_SIZE = 0.1397;
    private Dictionary<String, GameObject> m_markerObjects;
    private List<TangoSupport.Marker> m_markerList;
    private TangoApplication m_tangoApplication;

    private const int SENSOR_NUM = 4;
    private const int MARKER_NUM = 2;

    //My
    public GameObject SensorCube;
    public GameObject DistanceCube;
    public GameObject MarkerDetection;
    private GameObject[] sensor_obj = new GameObject[SENSOR_NUM];
    private GameObject[] dist_obj = new GameObject[SENSOR_NUM];
    private float[] sensor_val = new float[SENSOR_NUM] { 0, 0, 0, 0 };

    //debug
    public GameObject MenuPanel;
    public Text[] text2d = new Text[SENSOR_NUM];
    public Text[] text_markers = new Text[MARKER_NUM];
    public Text[] text_pram = new Text[SENSOR_NUM];
    public Text debugText;
    public Text debugText2;

    public void Start()
    {
        m_tangoApplication = FindObjectOfType<TangoApplication>();
        if (m_tangoApplication != null)
        {
            m_tangoApplication.Register(this);
        }
        else
        {
            Debug.Log("No Tango Manager found in scene.");
        }

        m_markerList = new List<TangoSupport.Marker>();
        m_markerObjects = new Dictionary<String, GameObject>();

        //センサオブジェクト生成
        for(int i = 0; i < SENSOR_NUM; i++)
        {
            sensor_obj[i] = Instantiate(SensorCube);
            dist_obj[i] = Instantiate(DistanceCube);
        }
    }

    public void OnTangoImageAvailableEventHandler(TangoEnums.TangoCameraId cameraId,
        TangoUnityImageData imageBuffer)
    {
        TangoSupport.DetectMarkers(imageBuffer, cameraId,
            TangoSupport.MarkerType.ARTAG, MARKER_SIZE, m_markerList);

        for (int i = 0; i < m_markerList.Count; ++i)
        {
            TangoSupport.Marker marker = m_markerList[i];

            if (m_markerObjects.ContainsKey(marker.m_content))
            {
                GameObject markerObject = m_markerObjects[marker.m_content];
                markerObject.GetComponent<MarkerVisualizationObject>().SetMarker(marker);
            }
            else
            {
                GameObject markerObject = Instantiate<GameObject>(m_markerPrefab);
                m_markerObjects.Add(marker.m_content, markerObject);
                markerObject.GetComponent<MarkerVisualizationObject>().SetMarker(marker);
            }
        }

        if (m_markerList.Count == MARKER_NUM)
        {
            if (m_markerList[0].m_content == "1")
            {
                float[] dist_val = DistanceFromSensorToWall(m_markerList[0], m_markerList[1]);
                SensorObjFunc(m_markerList[0].m_translation, m_markerList[0].m_orientation, m_markerList[0], m_markerList[1], dist_val);
            }
            else if (m_markerList[1].m_content == "1")
            {
                float[] dist_val = DistanceFromSensorToWall(m_markerList[1], m_markerList[0]);
                SensorObjFunc(m_markerList[1].m_translation, m_markerList[1].m_orientation, m_markerList[1], m_markerList[0], dist_val);
            }
        }

        if (m_markerList.Count == MARKER_NUM && MenuPanel.activeSelf)
        {
            //debug
            for (int i = 0; i < MARKER_NUM; i++)
            {
                text_markers[i].text = "m" + i.ToString() + " : " +
                    " x:" + m_markerList[i].m_translation.x.ToString() +
                    " ry:" + m_markerList[i].m_orientation.eulerAngles.y.ToString() +
                    " z:" + m_markerList[i].m_translation.z.ToString();
            }
        }
    }

    void SensorObjFunc(Vector3 marker_posi, Quaternion marker_rota, TangoSupport.Marker marker1, TangoSupport.Marker marker2, float[] dist_val)
    {
        //センサ値取得
        sensor_val = MarkerDetection.GetComponent<WebsocketSensorValue>().sensorValue;

        //マーカの角度差
        float[] sen_deg = new float[4] { 0, 55, -55, 0 };
        float theta = marker2.m_orientation.eulerAngles.y - marker1.m_orientation.eulerAngles.y;

        debugText2.text = theta.ToString();

        //距離変換
        float[] sensor_len = new float[4];
        for(int i = 0; i < 4; i++)
        {
            //sensor_len[i] = Mathf.Pow((20000f / sensor_val[i]), (1f / 2.0f));
            sensor_len[i] = Mathf.Pow((29154f / sensor_val[i]), (1f / 1.854f));
            sensor_len[i] /= 100f;//cm -> m
            //減衰率の計算（壁とセンサの角度差による）
            //float per = -0.0102f * (Mathf.Abs(theta-sen_deg[i])) + 0.9772f;
            //sensor_len[i] /= per;//減衰率による
            if (sensor_len[i] >= 0.6f) sensor_len[i] = 0.5f;
            else if (sensor_len[i] <= 0.001f) sensor_len[i] = 0.001f;
        }

        //3D
        //DebugText.GetComponent<TextMesh>().text = sensor_len[0].ToString();
        //DebugText.GetComponent<TextMesh>().text = "";

        //センサオブジェクトの位置・姿勢を調整

        //センサ値から求めた距離のオブジェクト
        for (int i = 0; i < SENSOR_NUM; i++)
        {
            sensor_obj[i].transform.position = marker_posi;
            sensor_obj[i].transform.rotation = marker_rota;
            Vector3 s = sensor_obj[i].transform.localScale;
            sensor_obj[i].transform.localScale = new Vector3(s.x, sensor_len[i], s.z);
            sensor_obj[i].transform.Rotate(90f, 0, 0);
        }

        float c35 = Mathf.Cos(35 * Mathf.PI / 180f);
        float s35 = Mathf.Sin(35 * Mathf.PI / 180f);

        sensor_obj[0].transform.Translate(0.045f, 0.05f + (sensor_len[0] / 2f), 0);
        sensor_obj[3].transform.Translate(-0.045f, 0.05f + (sensor_len[3] / 2f), 0);

        sensor_obj[1].transform.Translate(-0.01f + (-sensor_len[1] / 2f * c35), 0.07f + (sensor_len[1] / 2f * s35), 0);
        sensor_obj[2].transform.Translate(0.01f + (sensor_len[2] / 2f * c35), 0.07f + (sensor_len[2] / 2f * s35), 0);
        sensor_obj[1].transform.Rotate(0, 0, 55f);
        sensor_obj[2].transform.Rotate(0, 0, -55f);


        //二つのマーカから求めたセンサと壁の距離のオブジェクト
        for(int i = 0; i < SENSOR_NUM; i++)
        {
            if(dist_val[i] < 0)
            {
                dist_val[i] = -1f * dist_val[i];
            }
        }

        for (int i = 0; i < SENSOR_NUM; i++)
        {
            dist_obj[i].transform.position = marker_posi;
            dist_obj[i].transform.rotation = marker_rota;
            Vector3 s = dist_obj[i].transform.localScale;
            dist_obj[i].transform.localScale = new Vector3(s.x, dist_val[i], s.z);
            dist_obj[i].transform.Rotate(90f, 0, 0);
        }

        dist_obj[0].transform.Translate(0.045f, 0.05f + (dist_val[0] / 2f), 0);
        dist_obj[3].transform.Translate(-0.045f, 0.05f + (dist_val[3] / 2f), 0);

        dist_obj[1].transform.Translate(-0.01f + (-dist_val[1] / 2f * c35), 0.07f + (dist_val[1] / 2f * s35), 0);
        dist_obj[2].transform.Translate(0.01f + (dist_val[2] / 2f * c35), 0.07f + (dist_val[2] / 2f * s35), 0);
        dist_obj[1].transform.Rotate(0, 0, 55f);
        dist_obj[2].transform.Rotate(0, 0, -55f);
    }

    // 二つのマーカからセンサと壁の距離を計算
    private float[] DistanceFromSensorToWall(TangoSupport.Marker marker1, TangoSupport.Marker marker2)
    {
        // sensor: rightForward, rightSide, leftSide, leftForward, {x, z, angle}
        //縦
        //float[,] sensor = new float[SENSOR_NUM, 3] { { 0.05f, 0.045f, 0 }, { 0.07f, 0.01f, 55 }, { 0.07f, -0.01f, -55 }, { 0.05f, -0.045f, 0 } };
        //横
        float[,] sensor = new float[SENSOR_NUM, 3] { { 0.05f, 0.045f, 0 }, { 0.07f, 0.01f, 55f }, { 0.07f, -0.01f, -55f }, { 0.05f, -0.045f, 0 } };
        float[] dist = new float[SENSOR_NUM] { 0, 0, 0, 0 };
        for (int i = 0; i < SENSOR_NUM; i++)
        {
            //縦
            //float x1 = marker2.m_translation.x - (marker1.m_translation.x + sensor[i, 0]);
            //float z1 = marker2.m_translation.z - (marker1.m_translation.z + sensor[i, 1]);
            //float theta1 = marker2.m_orientation.eulerAngles.y - marker1.m_orientation.eulerAngles.y;
            //float x2 = x1 - (float)MARKER_SIZE / 2f * Mathf.Cos(theta1 * Mathf.PI / 180.0f);
            //float z2 = z1 - (float)MARKER_SIZE / 2f * Mathf.Sin(theta1 * Mathf.PI / 180.0f);
            //float theta2 = Mathf.Atan2(x2, z2) * 180.0f / Mathf.PI;
            //横
            float x1 = marker1.m_translation.x - (marker2.m_translation.x + sensor[i, 0]);
            float z1 = marker1.m_translation.z - (marker2.m_translation.z + sensor[i, 1]);
            float theta1 = marker1.m_orientation.eulerAngles.y - marker2.m_orientation.eulerAngles.y;
            float x2 = x1 - (float)MARKER_SIZE / 2f * Mathf.Cos(theta1 * Mathf.PI / 180.0f);
            float z2 = z1 - (float)MARKER_SIZE / 2f * Mathf.Sin(theta1 * Mathf.PI / 180.0f);
            float theta2 = Mathf.Atan2(x2, z2) * 180.0f / Mathf.PI;
            if (theta2 >= 90f) theta2 = -(180f - theta2);
            //
            float s = Mathf.Sqrt(x2 * x2 + z2 * z2);
            float d = s * Mathf.Sin((theta1 + theta2) * Mathf.PI / 180.0f);
            dist[i] = d / Mathf.Sin((90 - theta1 + sensor[i, 2]) * Mathf.PI / 180.0f);
            //debug
            text_pram[i].text = "s" + i.ToString() + " : " +
                " x1:" + x1.ToString() +
                " z1:" + z1.ToString() +
                " t1:" + theta1.ToString() +
                " x2:" + x2.ToString() +
                " z2:" + z2.ToString() +
                " t2:" + theta2.ToString() +
                " s:" + s.ToString() +
                " d:" + d.ToString();
            text2d[i].text = "sensor" + i.ToString() + " : " + dist[i].ToString();
        }
        float p = dist[0];
        dist[0] = dist[3];
        dist[3] = p;
        return dist;
    }

    public void MenuButton()
    {
        if (MenuPanel.activeSelf)
        {
            MenuPanel.SetActive(false);
        }
        else
        {
            MenuPanel.SetActive(true);
        }
    }
}