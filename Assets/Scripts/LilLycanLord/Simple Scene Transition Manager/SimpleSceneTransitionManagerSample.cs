using LilLycanLord_Official;
using UnityEngine;
using UnityEngine.Assertions;

namespace LilLycanLord_Official
{
    public class SimpleSceneTransitionManagerSample : MonoBehaviour
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

        //* ╔════════════╗
        //* ║ Attributes ║
        //* ╚════════════╝

        //* ╔═══════════════╗
        //* ║ Monobehaviour ║
        //* ╚═══════════════╝

        //* ╔═════════════════════╗
        //* ║ Non - Monobehaviour ║
        //* ╚═════════════════════╝
        public void GoToA()
        {
            SimpleSceneTransitionManager.Instance.LoadSceneWithTransition("Transition A", "Crossfade");
        }

        public void GoToB()
        {
            SimpleSceneTransitionManager.Instance.LoadSceneWithTransition("Transition B", "Crossfade");
        }

        public void GoToC()
        {
            SimpleSceneTransitionManager.Instance.LoadSceneWithTransition("Transition C", "Crossfade");
        }
    }
}
