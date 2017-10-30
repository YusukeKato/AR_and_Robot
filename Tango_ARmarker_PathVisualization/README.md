# GoogleTangoを用いてロボットの移動経路を可視化
GoogleTangoでロボットの上に乗せたARマーカーを認識し、そのARマーカーを頼りにロボットの移動経路を可視化します。

## 動かしたときの動画
https://youtu.be/C9WDgyOMOnE

## 開発環境
* GoogleTangoを搭載した端末（私はZenFoneARを使っています）
* Unity（バージョン5.5.5f1で動作を確認しました、5.2.1以降なら動くらしいです）
* Windows 10

## 公式の情報で参考にしたもの
* Unity用のTangoSDKをダウンロード
https://developers.google.com/tango/apis/unity/
* Tangoアプリを作成するためのUnity初期セットアップ
https://developers.google.com/tango/apis/unity/unity-setup
* Tangoのサンプルコード（SDKの中身）
https://github.com/googlesamples/tango-examples-unity

## Unityの新規プロジェクト作成
1. Projectの名前は適当につける
2. 3Dと2Dの選択では、3Dを選択
3. 保存場所も適当な場所を選ぶ

## 「Tango SDK for Unity」をUnityへインポート
1. GoogleTango公式サイトからUnity用のSDKをダウンロード
https://developers.google.com/tango/downloads
2. ダウンロードしたSDKパッケージをUnityへインポート(方法は２つのどちらかを行う)
* SDKパッケージをUnityエディタ画面のAssetsフォルダへドラッグ＆ドロップ
* Unityエディタ画面のAssetsタブからImportPackage > CustomPackageを選択してSDKパッケージを選ぶ