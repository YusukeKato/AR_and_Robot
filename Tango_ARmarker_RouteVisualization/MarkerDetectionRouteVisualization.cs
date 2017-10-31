//----------------------------------------------------------------------------------------------------
// Copyright 2016 Google Inc. All Rights Reserved. 
// Released under the Apache License 2.0
// http://www.apache.org/licenses/LICENSE-2.0
// 
// I Changed MarkerDetectionUIController.cs
// https://github.com/googlesamples/tango-examples-unity/blob/master/UnityExamples/Assets/TangoSDK/Examples/MarkerDetection/Scripts/MarkerDetectionUIController.cs
// 
// Changes : 
// Visualize marker movement path
// Generate a cube every time the marker moves 30 mm
// Added explanation(English and Japanese)[/* */]
//----------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Tango;
using UnityEngine;

/// <summary>
/// Detect a single AR Tag marker and place a virtual reference object on the
/// physical marker position.
/// </summary>

/* Changed class name : MarkerDetectionUIContoroller -> MarkerDetectionRouteVisualization */
public class MarkerDetectionRouteVisualization : MonoBehaviour, ITangoVideoOverlay
{
    /// <summary>
    /// The prefabs of marker.
    /// </summary>
    public GameObject m_markerPrefab;

    /* Visualize marker movement path */
    /* マーカーの移動経路を可視化するため */
    public GameObject routeObject;
    List<Vector3> routeLog;

    /// <summary>
    /// Length of side of the physical AR Tag marker in meters.
    /// </summary>
    private const double MARKER_SIZE = 0.1397;

    /// <summary>
    /// The objects of all markers.
    /// </summary>
    private Dictionary<String, GameObject> m_markerObjects;

    /// <summary>
    /// The list of markers detected in each frame.
    /// </summary>
    private List<TangoSupport.Marker> m_markerList;

    /// <summary>
    /// A reference to TangoApplication in current scene.
    /// </summary>
    private TangoApplication m_tangoApplication;

    /// <summary>
    /// Unity Start function.
    /// </summary>
    public void Start()
    {
        /* Check if there is Tango Manager */
        /* Tango Manager があるか確認 */
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
        routeLog = new List<Vector3>();
    }

    /// <summary>
    /// Detect one or more markers in the input image.
    /// </summary>
    /// <param name="cameraId">
    /// Returned camera ID.
    /// </param>
    /// <param name="imageBuffer">
    /// Color camera image buffer.
    /// </param>
    /* Function for Tango Event */
    /* Tangoのイベント用関数 */
    public void OnTangoImageAvailableEventHandler(TangoEnums.TangoCameraId cameraId,
        TangoUnityImageData imageBuffer)
    {
        /* Marker detection by Tango */
        /* Tangoによるマーカー検出 */
        TangoSupport.DetectMarkers(imageBuffer, cameraId,
            TangoSupport.MarkerType.ARTAG, MARKER_SIZE, m_markerList);

        /* Turn around only the type of marker shown in the camera */
        /* カメラに写っているマーカーの種類だけ回る */
        for (int i = 0; i < m_markerList.Count; ++i)
        {
            TangoSupport.Marker marker = m_markerList[i];

            /* Check if there is a key (string in dictionary) */
            /* 一度登録された鍵かどうか確認 */
            if (m_markerObjects.ContainsKey(marker.m_content))
            {
                /* Correct the position / attitude of the marker object */
                /* マーカーオブジェクトの位置・姿勢を修正 */
                GameObject markerObject = m_markerObjects[marker.m_content];
                markerObject.GetComponent<MarkerVisualizationObject>().SetMarker(marker);
                /* Generate routeObject every time the marker moves 30 mm */
                /* マーカーが30mm移動したら、その場所にrouteObjectを生成 */
                if(Vector3.Distance(marker.m_translation, routeLog[routeLog.Count-1]) >= 0.03f)
                {
                    GameObject ro = Instantiate<GameObject>(routeObject);
                    ro.transform.position = marker.m_translation;
                    ro.transform.rotation = marker.m_orientation;
                    ro.transform.Translate(0, ro.transform.localScale.y / 2.0f, 0);
                }
            }
            else
            {
                /* Register new marker */
                /* 新しいマーカーは登録 */
                GameObject markerObject = Instantiate<GameObject>(m_markerPrefab);
                m_markerObjects.Add(marker.m_content, markerObject);
                markerObject.GetComponent<MarkerVisualizationObject>().SetMarker(marker);
                /* Get the coordinates of the marker */
                /* マーカーの座標を取得 */
                routeLog.Add(marker.m_translation);
            }
        }
    }
}