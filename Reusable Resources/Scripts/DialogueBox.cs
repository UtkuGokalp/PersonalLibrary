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
        #region Constructors
        /// <summary>
        /// Default parameterless public constructor.
        /// </summary>
        public DialogueBoxOptions() { }

        /// <summary>
        /// Creates a deep copy of the given options object.
        /// </summary>
        public DialogueBoxOptions(DialogueBoxOptions options)
        {
            canvasResolution = options.canvasResolution;
            backgroundAlpha = options.backgroundAlpha;
            backgroundScale = options.backgroundScale;
            backgroundSprite = DeepCopySprite(options.backgroundSprite);

            buttonScale = options.buttonScale;
            buttonTextScale = options.buttonTextScale;
            okButtonSprite = DeepCopySprite(options.okButtonSprite);
            cancelButtonSprite = DeepCopySprite(options.cancelButtonSprite);
            okButtonText = new string(options.okButtonText);
            cancelButtonText = new string(options.cancelButtonText);
            buttonColorBlock = options.buttonColorBlock;

            dialogueText = new string(options.dialogueText);
            dialogueTextScale = options.dialogueTextScale;

            textColor = options.textColor;
            minFontSize = options.minFontSize;
            maxFontSize = options.maxFontSize;

            #region DeepCopySprite
            Sprite? DeepCopySprite(Sprite? s)
            {
                if (s == null)
                {
                    return null;
                }
                //Could not get extrude, mesh type and fallback physics shape generation values from sprite. The values used are the ones Unity uses as default values.
                return Sprite.Create(s.texture, s.rect, s.pivot, s.pixelsPerUnit, 0, SpriteMeshType.Tight, s.border, false);
            }
            #endregion
        }
        #endregion

        #region Fields
        [Header("Canvas Options")]
        [SerializeField]
        private Vector2 canvasResolution = new Vector2(1920, 1080);

        [Header("Background Options")]
        [Range(0f, 1f)]
        [SerializeField]
        private float backgroundAlpha = 0.5f;
        [Vector2Range(0f, 1f, 0f, 1f)]
        [SerializeField]
        private Vector2 backgroundScale = Vector2.one * 0.8f;
        [SerializeField]
        private Sprite? backgroundSprite;

        [Header("Button Options")]
        [SerializeField, Vector2Range(0f, 1f, 0f, 1f)]
        private Vector2 okButtonMinAnchor;
        [SerializeField, Vector2Range(0f, 1f, 0f, 1f)]
        private Vector2 okButtonMaxAnchor;
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
        private Vector2 dialogueTextScale = Vector2.one;

        [Header("Common Text Options")]
        [SerializeField]
        private Color textColor = Color.black;
        [SerializeField]
        private float minFontSize = 20;
        [SerializeField]
        private float maxFontSize = 80;
        #endregion

        #region Properties
        public Vector2 CanvasResolution
        {
            get => canvasResolution;
            set => canvasResolution = value;
        }

        public float BackgroundAlpha
        {
            get => backgroundAlpha;
            set => backgroundAlpha = Mathf.Clamp01(value);
        }
        public Vector2 BackgroundScale
        {
            get => backgroundScale;
            set => backgroundScale = new Vector2(Mathf.Clamp01(value.x), Mathf.Clamp01(value.y));
        }
        public Sprite? BackgroundSprite
        {
            get => backgroundSprite;
            set => backgroundSprite = value;
        }

        public Vector2 OkButtonMinAnchor
        {
            get => okButtonMinAnchor;
            set => okButtonMinAnchor = new Vector2(Mathf.Clamp01(value.x), Mathf.Clamp01(value.y));
        }
        public Vector2 OkButtonMaxAnchor
        {
            get => okButtonMaxAnchor;
            set => okButtonMaxAnchor = new Vector2(Mathf.Clamp01(value.x), Mathf.Clamp01(value.y));
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
        #endregion
    }
    #endregion

    #region DialogueBox
    public class DialogueBox
    {
        #region Variables
        private Canvas canvas;
        private bool destroyed;
        private bool active;
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
            canvas = CreateCanvas();
            CreateEventSystem();
            GameObject background = CreateBackground();
            CreateText(options.DialogueText, options.DialogueTextScale);

            CreateButton("OK Button", options.OkButtonText, options.OkButtonMinAnchor, options.OkButtonMaxAnchor, options.OkButtonSprite, okAction);
            Vector2 cancelButtonMinAnchor = new Vector2(1f - options.OkButtonMaxAnchor.x, options.OkButtonMinAnchor.y);
            Vector2 cancelButtonMaxAnchor = new Vector2(1f - options.OkButtonMinAnchor.x, options.OkButtonMaxAnchor.y);
            CreateButton("Cancel Button", options.CancelButtonText, cancelButtonMinAnchor, cancelButtonMaxAnchor, options.OkButtonSprite, cancelAction);

            canvas.gameObject.SetActive(startActive);
            active = startActive;
            destroyed = false;

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
                rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, canvas.pixelRect.width * options.BackgroundScale.x); //Set background width
                rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, canvas.pixelRect.height * options.BackgroundScale.y); //Set background height

                Image imageComponent = background.GetComponent<Image>();
                imageComponent.type = Image.Type.Sliced;
                imageComponent.sprite = options.BackgroundSprite;
                imageComponent.color = imageComponent.color.With(null, null, null, options.BackgroundAlpha);

                return background;
            }
            #endregion

            #region CreateText
            TextMeshProUGUI CreateText(string text, Vector2 scale)
            {
                GameObject textObject = new GameObject("TextMeshPro Text");
                textObject.layer = GetUILayer();
                textObject.transform.SetParent(background.transform);
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

            #region CreateButton
            Button CreateButton(string buttonName, string buttonText, Vector2 anchorMin, Vector2 anchorMax, Sprite? sprite, System.Action? onClickEvent)
            {
                GameObject button = new GameObject(buttonName);
                button.layer = GetUILayer();
                button.transform.SetParent(background.transform);
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

                RectTransform buttonRt = buttonComponent.GetComponent<RectTransform>();
                buttonRt.anchorMin = anchorMin;
                buttonRt.anchorMax = anchorMax;
                buttonRt.pivot = Vector2.zero;
                buttonRt.offsetMin = Vector2.zero;
                buttonRt.offsetMax = Vector2.zero;

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

        #region Toggle
        /// <summary>
        /// Shows the dialogue box if hidden. Hides it if shown.
        /// </summary>
        public void Toggle()
        {
            if (!destroyed)
            {
                if (active)
                {
                    Hide();
                }
                else
                {
                    Show();
                }
                active = !active;
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
