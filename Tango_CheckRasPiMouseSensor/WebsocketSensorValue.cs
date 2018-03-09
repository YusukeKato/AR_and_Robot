//--------------------------------------------------
// Copyright (C) 2018 Yusuke Kato, All rights reserved.
// ws_poweroff = new WebSocket("ws://" + ipadress + ":9090/");
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
    public class RosData2
    {
        public string op;
        public string topic;
        public string type;
    }

    [System.Serializable]
    public class RosData3
    {
        public string op;
        public string topic;
        public string msg;
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

    public float[] sensorValue = new float[4] { 0, 0, 0, 0 };

    WebSocket ws;
    WebSocket ws_poweroff;

    public Text wsConnectionText;
    bool wsConnection = false;

    private float span = 0.1f;
    private float delta = 0;

    public Text InputIPAdress;
    private string ipadress;
    public Text NowIPText;

    //public GameObject SettingPanel;
    //private bool flag_SetPanel = false;

	void Start () {
        ipadress = "192.168.43.234";
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

    void WsPoweroffSetting()
    {
        ws_poweroff = new WebSocket("ws://" + ipadress + ":9090/");

        ws_poweroff.OnOpen += (sender, e) =>
        {
            Debug.Log("WebSocket Open");
            RosData2 data = new RosData2();
            data.op = "advertise";
            data.topic = "/raspi_poweroff";
            data.type = "std_msgs/UInt16";
            string json = JsonUtility.ToJson(data);
            ws_poweroff.Send(json);
        };

        ws_poweroff.OnError += (sender, e) =>
        {
            Debug.Log("WebSocket Error Message : " + e.Message);
        };

        ws_poweroff.OnClose += (sender, e) =>
        {
            Debug.Log("Websocket Close");
        };

        ws_poweroff.OnMessage += (sender, e) =>
        {
           Debug.Log("OnMessage");
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
    /*
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
    */
    private void OnApplicationQuit()
    {
        ws.Close();
        Debug.Log("Websocket Close ......");
    }

    public void PoweroffButton()
    {
        //RosData3 data = new RosData3();
        //data.op = "publish";
        //data.topic = "/raspi_poweroff";
        //data.msg = "{1}";
        //string json = JsonUtility.ToJson(data);
        //ws_poweroff.Send(json);
        string msg2 = "{\"op\": \"publish\"," +
                       "\"topic\": \"/raspi_poweroff\"," +
                       "\"msg\": \"1\"" +
                       "}";
        ws_poweroff.Send(msg2);
    }
}
