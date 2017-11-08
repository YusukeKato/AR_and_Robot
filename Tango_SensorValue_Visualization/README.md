# GoogleTangoを用いてロボットのセンサ値を可視化
ロボット（Raspberry Pi Mouse）が持つ距離センサの値を実際の距離に変換し、直観的に理解できるようにします。  
ROSで動かすロボットなら同じようにできると思います。

## 動かしたときの動画
[Youtube - ARによるロボットのセンサ値の可視化](https://www.youtube.com/watch?v=CPMrsBE1d30)

## 開発環境
* GoogleTangoを搭載した端末（私はZenFoneARを使っています）
* Unity  
    * Unity5.5で動作を確認  
    * Unity2017以降のバージョンではアプリが起動しませんでした（TangoSDKが競合してるかも）  
* Windows 10  
* ロボット : [Raspberry Pi Mouse](http://products.rt-net.jp/micromouse/raspberry-pi-mouse)  
* ROS kinetic

## 流れ
* Raspberry Pi Mouse : ラズパイマウス
1. ラズパイマウスを動かせる状態へ
2. rosbridge_serverを使って、Topicのセンサ値を送信
3. Unityアプリ側ではwebsocket_sharpを使ってセンサ値を受信
4. センサ値を距離に変換
5. ARマーカーをラズパイマウスの上に乗せる
6. 距離センサの光の道筋に見立てたオブジェクトを生成
7. そのオブジェクトの長さは、センサ値より求めた距離に応じる  
完成

## Raspberry Pi Mouse を動かす
* ロボットを動かせる状態にしてください

## Tangoアプリの作成
以下の手順に従ってTangoアプリを作成してください。
まだです。

## 参考にしたもの
* GoogleTango - Unity用のTangoSDKをダウンロード  
https://developers.google.com/tango/apis/unity/
* GoogleTango - Tangoアプリを作成するためのUnity初期セットアップ  
https://developers.google.com/tango/apis/unity/unity-setup
* Github - Tangoのサンプルコード  
https://github.com/googlesamples/tango-examples-unity
* Github - MarkerDitectionのページ  
https://github.com/googlesamples/tango-examples-unity/tree/master/UnityExamples/Assets/TangoSDK/Examples/MarkerDetection  

