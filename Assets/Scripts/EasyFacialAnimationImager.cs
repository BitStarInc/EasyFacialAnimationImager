using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor.Animations;
using UnityEngine;

public class EasyFacialAnimationImager : MonoBehaviour
{
    //デバッグ表示するディスプレイ
    private enum Display
    {
        Display1,
        Display2,
        Display3,
        Display4,
        Display5,
        Display6,
        Display7,
        Display8,
        Display9
    }

    //スクショを撮りたいAnimator
    [SerializeField] private Animator _targetAnimator;

    //表情アニメが入っているレイヤー名
    [SerializeField] private string _layerName = "Base Layer";

    //アジャストの初期値
    [SerializeField] private Vector3 _positionAdjust = new Vector3(0f, 0.075f, 0.25f);

    //デバッグ表示するディスプレイ
    [SerializeField] private Display _display;

    private GameObject _cameraObject = null;

    private void Start()
    {
        //カメラを生成したり位置を移動したり回転を設定したりする

        Vector3 headPosition = Vector3.zero;
        //Animatorが無いなら原点基準にする
        if (_targetAnimator != null)
        {
            headPosition = _targetAnimator.GetBoneTransform(HumanBodyBones.Head).position;
        }

        _cameraObject =
            UnityEditor.EditorUtility.CreateGameObjectWithHideFlags("EasyFacialAnimationCamera", HideFlags.None,
                typeof(Camera));
        _cameraObject.transform.SetParent(this.transform);
        _cameraObject.transform.position = headPosition + _positionAdjust;
        _cameraObject.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        var cameraComponent = _cameraObject.GetComponent<Camera>();
        cameraComponent.nearClipPlane = 0.01f;
        cameraComponent.targetDisplay = (int) _display;
    }

    //インスペクタから選択したらスクショを撮る
    [ContextMenu("Get All Facial Animation Image")]
    private void GetAllFacialAnimScreenshot()
    {
        //再生中じゃないと表情変化できないので弾く
        if (!Application.isPlaying)
        {
            Debug.LogError("Plz Run!");
            return;
        }

        //Animatorが無いとレイヤを漁れないため弾く
        if (_targetAnimator == null)
        {
            Debug.LogError("Plz Set Animator!");
            return;
        }

        //無いとは思うけどレイヤ取得に失敗したときに弾く
        var animatorController = _targetAnimator.runtimeAnimatorController as AnimatorController;
        if (animatorController == null)
        {
            Debug.LogError("Layer NULL!");
            return;
        }

        //表情変化はワンフレームで実行できないのでコルーチンで実行
        StartCoroutine(FacialAnimationCreateScreenshotCoroutine(animatorController));
    }

    //表情を変えてスクショを撮るコルーチン
    IEnumerator FacialAnimationCreateScreenshotCoroutine(AnimatorController animatorController)
    {
        //レイヤ名からインデックスを持ってくる
        var layerIndex = _targetAnimator.GetLayerIndex(_layerName);

        //指定されたレイヤが見つからなかったら弾く
        if (layerIndex < 0)
        {
            Debug.LogError("Layer is Missing!");
            yield break;
        }

        //フォルダが存在しないなら作成
        var path = "Assets/Resources/" + _targetAnimator.name;
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        //レイヤのステートマシンから表情を持ってくる
        var faceLayer = animatorController.layers[layerIndex];

        //全ての表情に変化させながらスクショを撮る
        foreach (var state in faceLayer.stateMachine.states)
        {
            _targetAnimator.CrossFadeInFixedTime("default", 0f, layerIndex);
            yield return new WaitForSecondsRealtime(0.1f);
            _targetAnimator.CrossFadeInFixedTime(state.state.name, 0f, layerIndex);
            yield return new WaitForSecondsRealtime(0.1f);


            ScreenCapture.CaptureScreenshot(path + "/" + state.state.name + ".png");
            yield return new WaitForSecondsRealtime(0.1f);
        }

        //最後に表情をデフォルトに戻す
        _targetAnimator.CrossFadeInFixedTime("default", 0f, layerIndex);
    }

    //カメラは残されても困るので消す
    private void OnApplicationQuit()
    {
        if (_cameraObject != null)
            Destroy(_cameraObject);
    }
}