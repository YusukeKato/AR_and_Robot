using System;
using System.Collections.Generic;
using Tango;
using UnityEngine;

public class SensorValueVisualization : MonoBehaviour, ITangoVideoOverlay
{
    //Tango
    public GameObject m_markerPrefab;
    private const double MARKER_SIZE = 0.1397;
    private Dictionary<String, GameObject> m_markerObjects;
    private List<TangoSupport.Marker> m_markerList;
    private TangoApplication m_tangoApplication;

    //My
    public GameObject SensorCube;
    public GameObject MarkerDetection;
    private GameObject[] sensor_obj = new GameObject[4];
    private float[] sensor_val = new float[4];

    public GameObject DebugText;

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
        for(int i = 0; i < 4; i++)
        {
            sensor_obj[i] = Instantiate(SensorCube);
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

                //関数呼び出し
                SensorObjFunc(marker.m_translation, marker.m_orientation);
            }
            else
            {
                GameObject markerObject = Instantiate<GameObject>(m_markerPrefab);
                m_markerObjects.Add(marker.m_content, markerObject);
                markerObject.GetComponent<MarkerVisualizationObject>().SetMarker(marker);
            }
        }
    }

    void SensorObjFunc(Vector3 marker_posi, Quaternion marker_rota)
    {
        //センサ値取得
        sensor_val = MarkerDetection.GetComponent<WebsocketSensorValue>().sensorValue;

        //距離変換
        float[] sensor_len = new float[4];
        for(int i = 0; i < 4; i++)
        {
            sensor_len[i] = Mathf.Pow((20000f / sensor_val[i]), (1f / 2.0f));
            sensor_len[i] /= 100f;//cm -> m
            if (sensor_len[i] >= 0.6f) sensor_len[i] = 0.6f;
            else if (sensor_len[i] <= 0.001f) sensor_len[i] = 0.001f;
        }

        //DebugText.GetComponent<TextMesh>().text = sensor_len[0].ToString();
        DebugText.GetComponent<TextMesh>().text = "";

        //センサオブジェクトの位置・姿勢を調整
        for(int i = 0; i < 4; i++)
        {
            sensor_obj[i].transform.position = marker_posi;
            sensor_obj[i].transform.rotation = marker_rota;
            Vector3 s = sensor_obj[i].transform.localScale;
            sensor_obj[i].transform.localScale = new Vector3(s.x, sensor_len[i], s.z);
            sensor_obj[i].transform.Rotate(90f, 0, 0);
        }

        sensor_obj[0].transform.Translate(0.05f, 0.05f + (sensor_len[0] / 2f), 0);
        sensor_obj[3].transform.Translate(-0.05f, 0.05f + (sensor_len[3] / 2f), 0);

        float c35 = Mathf.Cos(35 * Mathf.PI / 180f);
        float s35 = Mathf.Sin(35 * Mathf.PI / 180f);
        sensor_obj[1].transform.Translate(-0.01f + (-sensor_len[1] / 2f * c35), 0.07f + (sensor_len[1] / 2f * s35), 0);
        sensor_obj[2].transform.Translate(0.01f + (sensor_len[2] / 2f * c35), 0.07f + (sensor_len[2] / 2f * s35), 0);
        sensor_obj[1].transform.Rotate(0, 0, 55f);
        sensor_obj[2].transform.Rotate(0, 0, -55f);
    }
}