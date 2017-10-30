# GoogleTangoを用いてロボットの移動経路を可視化
GoogleTangoでロボットの上に乗せたARマーカーを認識し、そのARマーカーを頼りにロボットの移動経路を可視化します。

## 動かしたときの動画
[Youtube - Tango-ロボットの移動経路の可視化](https://youtu.be/C9WDgyOMOnE)

## 開発環境
* GoogleTangoを搭載した端末（私はZenFoneARを使っています）
* Unity（バージョン5.5.5f1で動作を確認しました、5.2.1以降なら動くらしいです）
* Windows 10

## 公式の情報で参考にしたもの
* [GoogleTango - Unity用のTangoSDKをダウンロード](https://developers.google.com/tango/apis/unity/)
* [GoogleTango - Tangoアプリを作成するためのUnity初期セットアップ](https://developers.google.com/tango/apis/unity/unity-setup)
* [Github - Tangoのサンプルコード](https://github.com/googlesamples/tango-examples-unity)
* [Github - MarkerDitectionのページ](https://github.com/googlesamples/tango-examples-unity/tree/master/UnityExamples/Assets/TangoSDK/Examples/MarkerDetection)

# 開発方法

## Unityの新規プロジェクト作成
1. Unityを起動
2. Project選択画面でnewをクリック
    * 適当なProject名を入力
    * 保存場所は分かりやすい場所を選択
    * 3Dと2Dの選択では、3Dを選択
    * AnalyticsはONのまま
3. 「Create Project」ボタンを押す

## 「Tango SDK for Unity」をUnityへインポート
1. GoogleTango公式サイトからUnity用のSDKをダウンロード
https://developers.google.com/tango/downloads
2. ダウンロードしたSDKパッケージをUnityへインポート(２つの方法のどちらかを行う)
* SDKパッケージをUnityエディタ画面のAssetsフォルダへドラッグ＆ドロップ
* Unityエディタ画面のAssetsタブから ImportPackage > CustomPackage を選択してSDKパッケージを選ぶ

## Unityエディタ上での作業
1. Projectパネルの Assets > TangoSDK > Examples > Scenes の中にMarkerDetectionというSceneがあるのでダブルクリック
2. HierarchyパネルのCreateボタンを押して、3D Object > Cube を選択
3. 作成したCubeをクリックして、Inspectorパネル内のScaleをxyz全て0.02にする（Unityの単位はメートル）
4. HierarchyパネルのMarkerDetectionをクリックし、Inspectorパネル上のMarkerDetectionUIControllerのMarkerPrefab選択boxに作成したCubeをドラッグ＆ドロップ（MarkerPrefab選択boxの右横にある丸印をクリックしても選択できる）
5. スクリプト変更（まだ）

## ビルド
1. Unityエディタ画面の File > BuildSetting でビルドの設定画面を開く
2. 「Add Open Scenes」ボタンを押すとMarkerDetectionSceneが表示される
3. Platform選択でAndroidを選択し、「SwitchPlatform」ボタンを押す
4. 「PlayerSetting」ボタンを押して設定を書き換える[参考にした公式サイト](https://developers.google.com/tango/apis/unity/unity-setup)
    * CompanyNameを会社の名前か好きな名前に（例：hoge）
    * ProductNameはアプリの名前（例：huga）
    * DefaultIconはアイコンがあるなら変更（デフォルトのままで大丈夫）
    * OtherSettingの変更
        * Bundle Identifierを「com.[CompanyName].[ProductName]」にする（例：com.hoge.huga）
        * MinimumAPILevelをAPILevel17以上にする（Android4.2'JellyBean'のこと）
5. Tango端末の開発者向けオプションでUSBデバッグを有効にする
6. UnityのBuildSettingにある「Build and run」ボタンを押す
7. apkファイルの名前は適当につける。apkファイルの場所はAssetsフォルダ直下にする（だめならAndroidSDKと同じフォルダ）