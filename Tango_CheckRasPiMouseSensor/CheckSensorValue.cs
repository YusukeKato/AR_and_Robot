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
// Create function(DistanceFromSensorToWall)
//----------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tango;
using UnityEngine.UI;

public class CheckSensorValue : MonoBehaviour {

    public GameObject m_markerPrefab;
    //private const double MARKER_SIZE = 0.1397;//100%
    private const double MARKER_SIZE = 0.084;//60%
    //private const double MARKER_SIZE = 0.056;//40%
    private Dictionary<String, GameObject> m_markerObjects;
    private List<TangoSupport.Marker> m_markerList;
    private TangoApplication m_tangoApplication;

    public GameObject MenuPanel;
    public Text[] text2d = new Text[4];

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

        if(m_markerList.Count == 2 && MenuPanel.activeSelf)
        {
            float[] dist = DistanceFromSensorToWall(m_markerList[0], m_markerList[1]);
            for(int i = 0; i < 4; i++)
            {
                text2d[i].text = "sensor" + i.ToString() + " : " + dist[i].ToString();
            }
        }
    }

    // 二つのマーカからセンサと壁の距離を計算
    private float[] DistanceFromSensorToWall(TangoSupport.Marker marker1, TangoSupport.Marker marker2)
    {
        // sensor: rightForward, rightSide, leftSide, leftForward, {x, z, angle}
        float[,] sensor = new float[4, 3] { { 0.06f, 0.04f, 0 }, { 0.01f, 0.06f, 55 }, { -0.01f, 0.06f, 55 }, { -0.06f, 0.04f, 0 } };
        float[] dist = new float[4] { 0, 0, 0, 0 };
        for (int i = 0; i < 4; i++)
        {
            float x1 = marker2.m_translation.x - (marker1.m_translation.x - sensor[i, 0]);
            float z1 = marker2.m_translation.z - (marker1.m_translation.z - sensor[i, 1]);
            float theta1 = marker2.m_orientation.eulerAngles.y - marker1.m_orientation.eulerAngles.y;
            float x2 = x1 - (float)MARKER_SIZE / 2f * Mathf.Sin(theta1 * Mathf.PI / 180.0f);
            float z2 = z1 - (float)MARKER_SIZE / 2f * Mathf.Cos(theta1 * Mathf.PI / 180.0f);
            float theta2 = Mathf.Atan2(z2, x2) * 180.0f / Mathf.PI;
            float s = Mathf.Sqrt(x2 * x2 + z2 * z2);
            float d = s * Mathf.Sin((theta1 + theta2) * Mathf.PI / 180.0f);
            dist[i] = d / Mathf.Sin((90 + theta1 - sensor[i, 2]) * Mathf.PI / 180.0f);
        }
        return dist;
    }

    public void MenuButton()
    {
        if(MenuPanel.activeSelf)
        {
            MenuPanel.SetActive(false);
        }
        else
        {
            MenuPanel.SetActive(true);
        }
    }
}
