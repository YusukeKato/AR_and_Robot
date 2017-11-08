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

# 開発方法

## Raspberry Pi Mouse を動かす
* ロボットを動かせる状態にしてください
* rosbridgeをインストール
* roslaunch
* 距離センサのTopic
* rosbridge_server

## Tangoアプリの作成
以下の手順に従ってTangoアプリを作成してください。  
1. 新規プロジェクト作成
2. TangoSDKをインポート
3. MarkerDetectionSceneを開く
4. MarkerDetectionのコンポーネントのcsファイルのチェックを外す
5. このGithubのディレクトリにある２つのcsファイルをUnityへ
6. その２つのcsファイルをMarkerDetectionのコンポーネントとして付ける

## 実機へビルド
1. Tango端末をパソコンへ接続
2. USBデバッグを許可
3. BuildSettingを開いてMarkerDetectionSceneを登録
4. Androidへスイッチ
5. PlayerSettingで会社の名前やアプリの名前を入力
6. Bundle Identifierを「com.CompanyName.ProductName」のように入力
7. ビルド開始

## 参考にしたもの
* GoogleTango - Unity用のTangoSDKをダウンロード  
https://developers.google.com/tango/apis/unity/
* GoogleTango - Tangoアプリを作成するためのUnity初期セットアップ  
https://developers.google.com/tango/apis/unity/unity-setup
* Github - Tangoのサンプルコード  
https://github.com/googlesamples/tango-examples-unity
* Github - MarkerDitectionのページ  
https://github.com/googlesamples/tango-examples-unity/tree/master/UnityExamples/Assets/TangoSDK/Examples/MarkerDetection  

