using LilLycanLord_Official;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace LilLycanLord_Official
{
    public class Crossfade : SceneTransition
    {
        //* ╔════════════╗
        //* ║ Components ║
        //* ╚════════════╝

        //* ╔══════════╗
        //* ║ Displays ║
        //* ╚══════════╝
        // [Header("Displays")]

        //* ╔════════╗
        //* ║ Fields ║
        //* ╚════════╝
        [Space(10)]
        [Header("Fields")]
        [SerializeField]
        Color fadeInColor = Color.black;

        [SerializeField]
        Color fadeOutColor = Color.black;

        [Space(10)]
        [Header("Timing")]
        [SerializeField]
        float fadeInDuration = 1.0f;

        [SerializeField]
        float transitionDuration = 0.0f;

        [SerializeField]
        float fadeOutDuration = 1.0f;

        //* ╔════════════╗
        //* ║ Attributes ║
        //* ╚════════════╝

        //* ╔═══════════════╗
        //* ║ Monobehaviour ║
        //* ╚═══════════════╝
        void Start()
        {
            InitializeSceneTransition();
        }

        //* ╔═════════════════════╗
        //* ║ Non - Monobehaviour ║
        //* ╚═════════════════════╝
        public override void PrimeTransition()
        {
            UpdateDebugText();
            if (SimpleSceneTransitionManager.Instance.exitingDuration > 0)
                fadeInDuration = SimpleSceneTransitionManager.Instance.exitingDuration;

            if (fadeInDuration == 0.0f)
                fadeInDuration = 1.0f;
            animator.SetFloat("fadeInSpeed", 1.0f / fadeInDuration);

            if (background != null)
                background.color = fadeInColor;
        }

        public override void SwitchScene()
        {
            SimpleSceneTransitionManager.Instance.LoadToTargetScene();
            Invoke("CleanUpTransition", fadeInDuration + transitionDuration + fadeOutDuration);
        }

        public override void CleanUpTransition()
        {
            if (SimpleSceneTransitionManager.Instance.enteringDuration > 0)
                fadeOutDuration = SimpleSceneTransitionManager.Instance.enteringDuration;

            if (fadeOutDuration == 0.0f)
                fadeOutDuration = 1.0f;
            animator.SetFloat("fadeOutSpeed", 1.0f / fadeOutDuration);

            if (background != null)
                background.color = fadeOutColor;
            SimpleSceneTransitionManager.Instance.EndTransition();
        }
    }
}
