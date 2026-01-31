
using System.Collections;
using QFramework;
using UnityEngine;

namespace GGJ2026
{
    public class Stage1ToStage2Controller : MonoBehaviour
    {
        public CanvasGroup cg;

        [Header("Timing")]
        public float showDuration = 1f;     // 显示总时长
        public float signalTime = 0.5f;     // 0.5秒时发送进入Stage2信号

        private bool _armed = true;         // 一局只触发一次（回到Stage1会重新武装）
        private bool _isPlaying = false;
        private Coroutine _co;

        private void Awake()
        {
            if (!cg) cg = GetComponent<CanvasGroup>();
            SetVisible(false);

            // ✅ 建议注册一次就够了，避免 OnEnable 多次注册
            var game = GameApp.Interface.GetModel<GameStateModel>();

            GamePhase last = game.Phase.Value;

            game.Phase.RegisterWithInitValue(phase =>
            {
                // 回到 Stage1：重新允许下一次触发
                if (phase == GamePhase.Stage1)
                    _armed = true;

                // 只在“从Stage1进入Win_stage1”时触发过渡（你也可以换成你想监听的Phase）
                if (!_armed || _isPlaying) { last = phase; return; }

                if (last == GamePhase.Stage1 && phase == GamePhase.Win_stage1)
                {
                    _armed = false;
                    _co = StartCoroutine(PlayTransition());
                }

                last = phase;
            }).UnRegisterWhenGameObjectDestroyed(gameObject);
        }

        private IEnumerator PlayTransition()
        {
            _isPlaying = true;
            SetVisible(true);

            // 0.5 秒发进入 Stage2 信号
            yield return new WaitForSeconds(signalTime);

            // ✅ 推荐：直接让系统切阶段（或发命令/事件都行）
            GameApp.Interface.SendCommand(new EnterStage2Command());

            // 保证总共显示 showDuration 秒
            float remain = Mathf.Max(0f, showDuration - signalTime);
            if (remain > 0f) yield return new WaitForSeconds(remain);

            // 关闭自己，保持静默（不再依赖 phase 去显示/隐藏）
            SetVisible(false);
            _isPlaying = false;
            _co = null;
        }

        private void SetVisible(bool on)
        {
            cg.alpha = on ? 1f : 0f;
            cg.blocksRaycasts = on;   // on: 吃掉所有点击
            cg.interactable = on;     // on: 自己可交互（如果你有按钮）
        }
    }
}
