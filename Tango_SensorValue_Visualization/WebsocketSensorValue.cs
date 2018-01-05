//--------------------------------------------------
//
//--------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;
using UnityEngine.UI;

public class WebsocketSensorValue : MonoBehaviour {

    [System.Serializable]
    public class RosData
    {
        public string op;
        public string topic;
    }

    [System.Serializable]
    public class SensorValue
    {
        public string topic;
        public Msg msg;
        public string op;
    }

    [System.Serializable]
    public class Msg
    {
        public float right_forward;
        public float left_side;
        public float right_side;
        public float left_forward;
    }

    public float[] sensorValue = new float[4];

    WebSocket ws;

    public Text wsConnectionText;
    bool wsConnection = false;

    private float span = 0.1f;
    private float delta = 0;

    public Text InputIPAdress;
    private string ipadress;
    public Text NowIPText;

    public GameObject SettingPanel;
    private bool flag_SetPanel = false;

	void Start () {
        ipadress = "192.168.22.11";
        NowIPText.text = "Now:" + ipadress;
        WsSetting();
        wsConnectionText.text = "Connect";
	}
	
	void Update () {
        delta += Time.deltaTime;
	}

    void WsSetting ()
    {
        ws = new WebSocket("ws://" + ipadress + ":9090/");

        ws.OnOpen += (sender, e) =>
        {
            Debug.Log("WebSocket Open!!");
            RosData data = new RosData();
            data.op = "subscribe";
            data.topic = "/lightsensors";
            string json = JsonUtility.ToJson(data);
            ws.Send(json);
        };

        ws.OnError += (sender, e) =>
        {
            Debug.Log("WebSocket Error Message : " + e.Message);
        };

        ws.OnClose += (sender, e) =>
        {
            Debug.Log("Websocket Close");
            RosData data = new RosData();
            data.op = "unsubscribe";
            data.topic = "/lightsensors";
            string json = JsonUtility.ToJson(data);
            ws.Send(json);
        };

        ws.OnMessage += (sender, e) =>
        {
            if (delta >= span)
            {
                delta = 0;

                string message = e.Data;
                Debug.Log(message);
                SensorValue sensor = JsonUtility.FromJson<SensorValue>(message);
                sensorValue[0] = sensor.msg.right_forward;
                sensorValue[1] = sensor.msg.left_side;
                sensorValue[2] = sensor.msg.right_side;
                sensorValue[3] = sensor.msg.left_forward;
                //Debug.Log("right_forward : " + sensorValue[0]);
                //Debug.Log("left_side     : " + sensor.left_side.ToString());
                //Debug.Log("right_side    : " + sensor.right_side.ToString());
                //Debug.Log("left_forward  : " + sensor.left_forward.ToString());
                Debug.Log(sensorValue[0]);
            }
        };
    }

    public void WsConnection()
    {
        if(wsConnection)
        {
            ws.Close();
            wsConnection = false;
            wsConnectionText.text = "Connect";
            Debug.Log("Websocket Close ......");
        }
        else if(!wsConnection)
        {
            WsSetting();
            ws.Connect();
            wsConnection = true;
            wsConnectionText.text = "Close";
            Debug.Log("Websocket Connect!!");
        }
    }

    public void InputOKButton()
    {
        ipadress = InputIPAdress.text;
        NowIPText.text = "Now:" + ipadress;
    }

    public void SetPanelButton()
    {
        if(flag_SetPanel)
        {
            SettingPanel.SetActive(false);
            flag_SetPanel = false;
        }
        else if(!flag_SetPanel)
        {
            SettingPanel.SetActive(true);
            flag_SetPanel = true;
        }
    }

    private void OnApplicationQuit()
    {
        ws.Close();
        Debug.Log("Websocket Close ......");
    }
}
