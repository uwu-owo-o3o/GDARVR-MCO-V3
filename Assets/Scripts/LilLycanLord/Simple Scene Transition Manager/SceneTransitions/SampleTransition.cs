using LilLycanLord_Official;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace LilLycanLord_Official
{
    public class SampleTransition : SceneTransition
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
        // [Space(10)]
        // [Header("Fields")]

        [Space(10)]
        [Header("Timing")]
        [SerializeField]
        float transitionDuration = 0.0f;

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
            //? Put Transition specific code below - - - - -
        }

        public override void SwitchScene()
        {
            //? Make sure to call this function at the last frame of the Scene Exiting Animation
            SimpleSceneTransitionManager.Instance.LoadToTargetScene();
            Invoke("CleanUpTransition", transitionDuration);
        }

        public override void CleanUpTransition()
        {
            //? Put Transition specific code above - - - - -
            SimpleSceneTransitionManager.Instance.EndTransition();
        }
    }
}
