# GoogleTangoを用いてロボットの移動経路を可視化
GoogleTangoを用いてロボットの上に乗せたARマーカーを認識し、そのARマーカーを頼りにロボットの移動経路を可視化します。ARマーカーを認識しているため、ロボットはなくても大丈夫です。

## 動かしたときの動画
[Youtube - Tango-ロボットの移動経路の可視化](https://youtu.be/C9WDgyOMOnE)

## 開発環境
* GoogleTangoを搭載した端末（私はZenFoneARを使っています）
* Unity（バージョン5.5.5f1で動作を確認しました。5.2.1以降なら動くらしいです）
* Windows 10
* ロボット : Raspberry Pi Mouse

## 参考にしたもの
* GoogleTango - Unity用のTangoSDKをダウンロード  
https://developers.google.com/tango/apis/unity/
* GoogleTango - Tangoアプリを作成するためのUnity初期セットアップ  
https://developers.google.com/tango/apis/unity/unity-setup
* Github - Tangoのサンプルコード  
https://github.com/googlesamples/tango-examples-unity
* Github - MarkerDitectionのページ  
https://github.com/googlesamples/tango-examples-unity/tree/master/UnityExamples/Assets/TangoSDK/Examples/MarkerDetection

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
2. HierarchyパネルのCreateボタンで3D Object > Cubeを選択
3. 作成したCubeをクリック、InspectorパネルでScaleをxyz全て0.02にする（Unityの単位はメートル）
4. HierarchyパネルのCubeをProjectパネルのAssetsフォルダにドラッグ＆ドロップ、これによりCubeはPrefab化される
5. HierarchyパネルのCubeは削除（Prefabがあるので必要ない）
6. ProjectパネルのCreateボタンからC#Scriptを選択。名前は「MarkerDetectionRouteVisualization」
7. 今作ったMarkerDetectionRouteVisualization.csを開いて、このGithubの同じディレクトリ内にあるMarkerDetectionRouteVisualization.csの中身をコピー＆ペースト
8. HierarchyパネルのMarkerDetectionをクリック、Inspectorパネルをみて以下の4つの作業を行う
* Add Componentボタンを押して、MarkerDetectionRouteVisualization.csを検索して選択
* MarkerDetectionUIController.csのクリックを外す
* MarkerDetectionRouteVisualization.csにあるMarkerPrefabの選択boxの右横にある丸印をクリック、「Marker」を選択
* 同じ場所のrouteObjectの選択boxの右横にある丸印をクリック、さっき作成した「Cube」を選択

## ビルド
1. Unityエディタ画面の File > BuildSetting でビルドの設定画面を開く
2. 「Add Open Scenes」ボタンを押すとMarkerDetectionSceneが表示される
3. Platform選択でAndroidを選択し、「SwitchPlatform」ボタンを押す
4. 「PlayerSetting」ボタンを押して設定を書き換える([参考にした公式サイト](https://developers.google.com/tango/apis/unity/unity-setup))
    * CompanyNameを会社の名前か好きな名前に（例：hoge）
    * ProductNameはアプリの名前（例：huga）
    * DefaultIconはアイコンがあるなら変更（デフォルトのままで大丈夫）
    * OtherSettingの変更
        * Bundle Identifierを「com.[CompanyName].[ProductName]」にする（例：com.hoge.huga）
        * MinimumAPILevelをAPILevel17以上にする（Android4.2'JellyBean'のこと）
5. Tango端末の開発者向けオプションでUSBデバッグを有効にする（PC側ではなくスマホ側での話）
6. UnityのBuildSettingにある「Build and run」ボタンを押す
7. apkファイルの名前は適当につける。apkファイルの場所はAssetsフォルダ直下にする（だめならAndroidSDKと同じフォルダ）

## ARマーカーの印刷
1. Projectパネルの Assets > TangoSDK > Examples > MarkerDetection > ar_markers.pdfを開く
2. 一つ目のマーカーだけをA4サイズの紙に印刷（全部印刷しないように注意。1枚しか使いません）
* タブレット端末などに表示してもできますが、その場合はスクリプト内のマーカーサイズを書き換える必要があります。

## 注意点
* Tango端末の画面内にマーカー全体が入っていないと認識できない
* マーカーが少しでも欠けると、認識できなくなる
