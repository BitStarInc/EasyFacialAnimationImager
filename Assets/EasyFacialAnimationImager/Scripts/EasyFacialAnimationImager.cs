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
    [SerializeField] private Vector3 _positionAdjust = new Vector3(0f, 0.075f, 0.5f);

    //デバッグ表示するディスプレイ
    [SerializeField] private Display _display;

    private GameObject _cameraObject = null;
    private bool _isRunningCreateImageCoroutine = false;

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
    private void GetAllFacialAnimationImage()
    {
        //再生中じゃないと表情変化できないので弾く
        if (!Application.isPlaying)
        {
            Debug.LogError("Plz Run!");
            return;
        }

        //コルーチン2重起動を防ぐ
        if (_isRunningCreateImageCoroutine)
        {
            Debug.LogError("Now Creating Facial Animation Image!");
            return;
        }

        //Animatorが無いとレイヤを漁れないため弾く
        if (_targetAnimator == null)
        {
            Debug.LogError("Plz Set Animator!");
            return;
        }

        //レイヤ名からインデックスを持ってきて見つからなかったら弾く
        var layerIndex = _targetAnimator.GetLayerIndex(_layerName);
        if (layerIndex < 0)
        {
            Debug.LogError("Layer is Missing!");
            return;
        }

        //AnimatorController取得に失敗したときに弾く
        var animatorController = _targetAnimator.runtimeAnimatorController as AnimatorController;
        if (animatorController == null)
        {
            Debug.LogError("AnimatorController is NULL!");
            return;
        }

        //AnimatorControllerLayerを持ってくる
        var faceLayer = animatorController.layers[layerIndex];

        //レイヤウェイトを1にしてAnimationが確実に適用されるようにする
        _targetAnimator.SetLayerWeight(layerIndex, 1f);

        //フォルダが存在しないなら作成
        var path = "Assets/Resources/" + _targetAnimator.name;
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        //表情変化はワンフレームで実行できないのでコルーチンで実行
        StartCoroutine(CreateFacialAnimationImageCoroutine(faceLayer, path));
    }

    //表情を変えてスクショを撮るコルーチン
    IEnumerator CreateFacialAnimationImageCoroutine(AnimatorControllerLayer facialLayer, string path)
    {
        _isRunningCreateImageCoroutine = true;

        //全ての表情に変化させながらスクショを撮る
        foreach (var animatorState in facialLayer.stateMachine.states)
        {
            _targetAnimator.Play(animatorState.state.name);
            yield return new WaitForEndOfFrame();
            ScreenCapture.CaptureScreenshot(path + "/" + animatorState.state + ".png");
        }

        _isRunningCreateImageCoroutine = false;
    }

    //カメラは残されても困るので消す
    private void OnApplicationQuit()
    {
        if (_cameraObject != null)
            Destroy(_cameraObject);
    }
}