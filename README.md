# EasyFacialAnimationImager
指定したAnimatorに登録してあるAnimationを再生してスクショを撮るスクリプト。

## 概要
登録してあるAnimationのスクショを撮って説明書や資料などに使いたい、ということは往々にして発生するかと思います。  
そんな時にスクショを手動で1枚1枚丁寧に撮っていくのもいいですが、面倒なので自動化したいというのを実現したUnityチョットベンリになるスクリプトです。  
タイトルにFacialとありますが、表情アニメーションに限ったスクリプトではなくモーションなどにも使えるかと思います。

## 使い方
1. [Releases](https://github.com/Bizcast/EasyFacialAnimationImager/releases)からUnityPackageをダウンロードしてプロジェクトにインポートする。
2. 空のGameObjectに `EasyFacialAnimationImager` コンポーネントをアタッチする、もしくは同名のプレハブをHierarchyに配置する。
3. スクショを撮りたい対象のAnimatorを `TargetAnimator` に登録する。
4. AnimatorLayerを分けている場合は `LayerName` にスクショを撮りたい対象のレイヤ名を入力する。
5. エディタを再生する。
6. 子にCameraが生成させるのでGameウィンドウを見ながらいい感じに調整する。
7. コンポーネントを右クリックしてコンテキストメニューから `Get All Facial Animation Image` を選択する。
8. `Resources/(Animator名)` 配下にスクショが生成される。

## 動作環境
Unity2018.2.13f1 のエディタ上で動作確認をしています。サンプルプロジェクトも同バージョンで作成しています。  
特別な処理は行っていないのでUnity2017でも動作するかと思います。

## FAQ
### スクショが生成されないんだけど
Gameウィンドウを確認しながらでないと動作しません。確認せずに実行した場合はGameウィンドウを見ることでスクショが撮影されます。

### それでもスクショ/フォルダが生成されないんだけど
スクショを撮影した直後はProjectウィンドウでは見えません（Unityが読み込んでくれないため）。エクスプローラやFinderで確認すると生成されていることが確認できるかと思います。  
もしくは一旦別ウィンドウにフォーカスを移し、再度Unityに戻すと読み込みが行われます。

### アスペクト比やサイズを指定したいんだけど
Gameウィンドウのアスペクト比を操作してください。要望が多ければインスペクタ上から選択できるように改良します。

## ライセンス
このソフトウェアは[MITライセンス](https://github.com/Bizcast/EasyFacialAnimationImager/blob/master/LICENSE)の下で公開しています。
