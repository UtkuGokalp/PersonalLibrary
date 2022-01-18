#nullable enable

using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Object = UnityEngine.Object;

namespace Utility.Development
{
    #region DialogueBoxOptions
    [System.Serializable]
    public class DialogueBoxOptions
    {
        [Header("Canvas Options")]
        [SerializeField]
        private Vector2 canvasResolution;

        [Header("Background Options")]
        [Range(0f, 1f)]
        [SerializeField]
        private float backgroundAlpha = 0.5f;
        [Range(0f, 1f)]
        [SerializeField]
        private float backgroundScale = 0.8f;
        [SerializeField]
        private Sprite? backgroundSprite;

        [Header("Button Options")]
        [SerializeField]
        private Vector2 buttonScale = Vector2.one;
        [SerializeField]
        private Vector2 buttonTextScale = Vector2.one;
        [SerializeField]
        private Sprite? okButtonSprite;
        [SerializeField]
        private Sprite? cancelButtonSprite;
        [SerializeField]
        private string okButtonText = "OK";
        [SerializeField]
        private string cancelButtonText = "Cancel";
        [SerializeField]
        private ColorBlock buttonColorBlock = new ColorBlock()
        {
            colorMultiplier = 1f,
            fadeDuration = 0.1f,
            normalColor = Color.white,
            highlightedColor = Color.white.With(null, null, null, 180f / 255f),
            pressedColor = Color.white.With(null, null, null, 100f / 255f),
            selectedColor = Color.gray,
            disabledColor = Color.red
        };

        [Header("Dialogue Text Options")]
        [SerializeField]
        private string dialogueText = "Placeholder Text";
        [SerializeField]
        private Vector2 dialogueTextScale = Vector2.zero;

        [Header("Common Text Options")]
        [SerializeField]
        private Color textColor = Color.black;
        [SerializeField]
        private float minFontSize = 20;
        [SerializeField]
        private float maxFontSize = 80;
        
        public Vector2 CanvasResolution
        {
            get => canvasResolution;
            set => canvasResolution = value;
        }

        public float BackgroundAlpha
        {
            get => backgroundAlpha;
            set => backgroundAlpha = value;
        }
        public float BackgroundScale
        {
            get => backgroundScale;
            set => backgroundScale = value;
        }
        public Sprite? BackgroundSprite
        {
            get => backgroundSprite;
            set => backgroundSprite = value;
        }

        public Vector2 ButtonScale
        {
            get => buttonScale;
            set => buttonScale = value;
        }
        public Vector2 ButtonTextScale
        {
            get => buttonTextScale;
            set => buttonTextScale = value;
        }
        public Sprite? OkButtonSprite
        {
            get => okButtonSprite;
            set => okButtonSprite = value;
        }
        public Sprite? CancelButtonSprite
        {
            get => cancelButtonSprite;
            set => cancelButtonSprite = value;
        }
        public string OkButtonText
        {
            get => okButtonText;
            set => okButtonText = value;
        }
        public string CancelButtonText
        {
            get => cancelButtonText;
            set => cancelButtonText = value;
        }
        public ColorBlock ButtonColorBlock
        {
            get => buttonColorBlock;
            set => buttonColorBlock = value;
        }

        public string DialogueText
        {
            get => dialogueText;
            set => dialogueText = value;
        }
        public Vector2 DialogueTextScale
        {
            get => dialogueTextScale;
            set => dialogueTextScale = value;
        }

        public Color TextColor
        {
            get => textColor;
            set => textColor = value;
        }
        public float MinFontSize
        {
            get => minFontSize;
            set => minFontSize = value;
        }
        public float MaxFontSize
        {
            get => maxFontSize;
            set => maxFontSize = value;
        }
    }
    #endregion

    #region DialogueBox
    public class DialogueBox
    {
        #region Variables
        private Canvas canvas;
        private bool destroyed;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of a dialogue box.
        /// </summary>
        /// <param name="startActive">Determines whether the dialogue box is immediately activated after initialization.</param>
        /// <param name="options">Options for the dialogue box.</param>
        /// <param name="okAction">Code to execute when OK button is pressed.</param>
        /// <param name="cancelAction">Code to execute when CANCEL button is pressed.</param>
        public DialogueBox(bool startActive, DialogueBoxOptions options, System.Action? okAction, System.Action? cancelAction)
        {
            destroyed = false;
            canvas = CreateCanvas();
            CreateEventSystem();
            CreateBackground();
            CreateText(options.DialogueText, options.DialogueTextScale);
            CreateButton("OK Button", options.OkButtonText, options.OkButtonSprite, okAction);
            CreateButton("Cancel Button", options.CancelButtonText, options.OkButtonSprite, cancelAction);
            canvas.gameObject.SetActive(startActive);

            #region CreateCanvas
            Canvas CreateCanvas()
            {
                GameObject canvasObject = new GameObject("DialogueBox Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
                canvasObject.layer = GetUILayer();

                Canvas canvas = canvasObject.GetComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                const ushort MAX_16_BIT_UNSIGNED_VALUE = 32767; //Max canvas sort order value
                canvas.sortingOrder = MAX_16_BIT_UNSIGNED_VALUE;

                CanvasScaler canvasScaler = canvasObject.GetComponent<CanvasScaler>();
                canvasScaler.referenceResolution = options.CanvasResolution;
                canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                canvasScaler.matchWidthOrHeight = 0.5f;

                return canvas;
            }
            #endregion

            #region CreateEventSystem
            void CreateEventSystem()
            {
                if (Object.FindObjectOfType<EventSystem>() == null)
                {
                    GameObject eventSystem = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
                    eventSystem.transform.SetParent(canvas.transform);
                }
            }
            #endregion

            #region CreateBackground
            GameObject CreateBackground()
            {
                GameObject background = new GameObject("Dialogue Box Background", typeof(Image));
                background.layer = GetUILayer();
                background.transform.SetParent(canvas.transform);
                background.transform.localPosition = Vector3.zero;
                RectTransform rt = background.GetComponent<RectTransform>();
                rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, canvas.pixelRect.width * options.BackgroundScale); //Set background width
                rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, canvas.pixelRect.height * options.BackgroundScale); //Set background height

                Image imageComponent = background.GetComponent<Image>();
                imageComponent.type = Image.Type.Sliced;
                imageComponent.sprite = options.BackgroundSprite;
                imageComponent.color = imageComponent.color.With(null, null, null, options.BackgroundAlpha);

                return background;
            }
            #endregion

            //TODO: Implement positioning text
            #region CreateText
            TextMeshProUGUI CreateText(string text, Vector2 scale)
            {
                GameObject textObject = new GameObject("TextMeshPro Text");
                textObject.layer = GetUILayer();
                textObject.transform.SetParent(canvas.transform);
                textObject.transform.localPosition = Vector3.zero;
                textObject.transform.localScale = scale;

                TextMeshProUGUI textComponent = textObject.AddComponent<TextMeshProUGUI>();
                textComponent.enableWordWrapping = true;
                textComponent.color = options.TextColor;
                textComponent.text = text;
                textComponent.verticalAlignment = VerticalAlignmentOptions.Middle;
                textComponent.horizontalAlignment = HorizontalAlignmentOptions.Center;
                textComponent.enableAutoSizing = true;
                textComponent.fontSizeMin = options.MinFontSize;
                textComponent.fontSizeMax = options.MaxFontSize;

                return textComponent;
            }
            #endregion

            //TODO: Implement positioning buttons
            #region CreateButton
            Button CreateButton(string buttonName, string buttonText, Sprite? sprite, System.Action? onClickEvent)
            {
                GameObject button = new GameObject(buttonName);
                button.layer = GetUILayer();
                button.transform.SetParent(canvas.transform);
                button.transform.localPosition = Vector3.zero;
                button.transform.localScale = options.ButtonScale;

                Image imageComponent = button.AddComponent<Image>();
                imageComponent.type = Image.Type.Sliced;
                imageComponent.sprite = sprite;

                Button buttonComponent = button.AddComponent<Button>();
                buttonComponent.image = imageComponent;
                buttonComponent.colors = options.ButtonColorBlock;
                Navigation n = buttonComponent.navigation;
                n.mode = Navigation.Mode.None;
                buttonComponent.navigation = n;
                buttonComponent.onClick.AddListener(() => onClickEvent?.Invoke());

                TextMeshProUGUI textComponent = CreateText(buttonText, options.ButtonTextScale);
                textComponent.gameObject.transform.SetParent(button.transform);
                textComponent.gameObject.transform.localPosition = Vector3.zero;

                RectTransform rt = textComponent.GetComponent<RectTransform>();
                rt.anchorMin = Vector2.zero;
                rt.anchorMax = Vector2.one;
                rt.pivot = Vector2.one * 0.5f;
                rt.offsetMin = Vector2.zero;
                rt.offsetMax = Vector2.zero;

                return buttonComponent;
            }
            #endregion

            #region GetUILayer
            int GetUILayer() => LayerMask.NameToLayer("UI"); //UI is a standard layer name that is set by Unity and it is not changeable so we can hardcode it
            #endregion
        }
        #endregion

        #region Show
        /// <summary>
        /// Enables the dialogue box.
        /// </summary>
        public void Show()
        {
            if (!destroyed)
            {
                canvas.gameObject.SetActive(true);
            }
        }
        #endregion

        #region Hide
        /// <summary>
        /// Disables the dialogue box.
        /// </summary>
        public void Hide()
        {
            if (!destroyed)
            {
                canvas.gameObject.SetActive(false);
            }
        }
        #endregion

        #region Destroy
        /// <summary>
        /// Destroys the dialogue box.
        /// </summary>
        public void Destroy()
        {
            if (canvas != null)
            {
                Object.Destroy(canvas.gameObject);
                destroyed = true;
            }
        }
        #endregion
    }
    #endregion
}
