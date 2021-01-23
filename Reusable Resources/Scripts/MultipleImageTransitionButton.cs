using UnityEngine;
using UnityEngine.UI;

namespace Utility.Development
{
    /// <summary>
    /// Supports color tinting with multiple objects on a UI button.
    /// </summary>
    public class MultipleImageTransitionButton : Button
    {
        #region Variables
        private Image[] images;
        #endregion

        #region Awake
        protected override void Awake()
        {
            images = GetComponentsInChildren<Image>();
        }
        #endregion

        #region DoStateTransition
        protected override void DoStateTransition(SelectionState state, bool instant)
        {
            var targetColor =
                state == SelectionState.Disabled ? colors.disabledColor :
                state == SelectionState.Highlighted ? colors.highlightedColor :
                state == SelectionState.Normal ? colors.normalColor :
                state == SelectionState.Pressed ? colors.pressedColor :
                state == SelectionState.Selected ? colors.selectedColor : Color.white;
            
            foreach (Graphic graphic in images)
            {
                graphic.CrossFadeColor(targetColor, instant ? 0f : colors.fadeDuration, true, true);
            }
        }
        #endregion
    }
}
