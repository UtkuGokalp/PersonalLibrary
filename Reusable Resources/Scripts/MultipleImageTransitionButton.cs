#nullable enable

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
        private Image[]? images;
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
            Color targetColor = state switch
            {
                SelectionState.Disabled => colors.disabledColor,
                SelectionState.Highlighted => colors.highlightedColor,
                SelectionState.Normal => colors.normalColor,
                SelectionState.Pressed => colors.pressedColor,
                SelectionState.Selected => colors.selectedColor,
                _ => Color.white
            };

            if (images != null)
            {
                foreach (Graphic graphic in images)
                {
                    graphic.CrossFadeColor(targetColor, instant ? 0f : colors.fadeDuration, true, true);
                }
            }
        }
        #endregion
    }
}
