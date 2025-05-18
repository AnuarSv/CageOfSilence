using UnityEngine;

namespace StarterAssets
{
    public class UICanvasControllerInput : MonoBehaviour
    {

        [Header("Output")]
        public StarterAssetsInputs starterAssetsInputs;

        public void VirtualMoveInput(Vector2 virtualMoveDirection)
        {
            starterAssetsInputs.MoveInput(virtualMoveDirection);
        }

        public void VirtualLookInput(Vector2 virtualLookDirection)
        {
            starterAssetsInputs.LookInput(virtualLookDirection);
        }

        public void VirtualUseInput(bool virtualUseState)
        {
            starterAssetsInputs.UseInput(virtualUseState);
        }

        public void VirtualSprintInput(bool virtualSprintState)
        {
            starterAssetsInputs.SprintInput(virtualSprintState);
        }
        public void VirtualFlashInput(bool virtualFlashState)
        {
            starterAssetsInputs.FlashInput(virtualFlashState);
        }
        public void VirtualReloadFlashInput(bool virtualReloadFlashState)
        {
            starterAssetsInputs.ReloadFlashInput(virtualReloadFlashState);
        }

    }

}
