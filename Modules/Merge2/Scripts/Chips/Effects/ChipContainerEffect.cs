using UnityEngine;

namespace Merge2   // MonoBehaviour is the base class from which every Unity script derives.
{
    public class ChipContainerEffect : ChipEffect
    {
        [SerializeField]
        Animator animator;

        public override void Activate(Chip chip)
        {
            base.Activate(chip);

            if (animator == null)
            {
                Debug.LogError("ChipContainerEffect: animator is null");
                return;
            }
            animator.SetTrigger("Activate");
        }

        public override void Deactivate(Chip chip)
        {
            animator.SetTrigger("Deactivate");
            base.Deactivate(chip);
        }

    }
}