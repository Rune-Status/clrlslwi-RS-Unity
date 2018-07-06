
using UnityEngine;
using UnityEngine.UI;

namespace RS
{
    /// <summary>
    /// Performs the rendering of the loading screen.
    /// </summary>
    public class LoadingScreen : MonoBehaviour
    {
        private enum Direction
        {
            Forward,
            Backward,
        }

        private AsyncCacheLoader loader;
        private Camera tmpCamera;
        private bool attemptedLoad = false;

        private GameObject texObject;

        private GameObject textObject;
        private Canvas canvas;
        private Text text;

        private Direction fadeDirection = Direction.Backward;
        private byte alpha = 255;
       
        public void Start()
        {
            texObject = new GameObject();

            var texCanvas = texObject.AddComponent<Canvas>();
            texCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            texCanvas.pixelPerfect = true;

            var backTexture = new Texture2D(1, 1);
            backTexture.SetPixel(0, 0, new Color32(255, 255, 255, 255));
            var rawImage = texObject.AddComponent<RawImage>();
            rawImage.uvRect = new Rect(0, 0, 1, 1);
            rawImage.color = new Color32(255, 255, 255, 255);
            rawImage.transform.position = new Vector3(0, 0, 0);
            rawImage.texture = backTexture;

            textObject = new GameObject();
            canvas = textObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.pixelPerfect = true;

            text = textObject.AddComponent<Text>();
            text.color = new Color32(0x9B, 0x00, 0x08, 0xFF);
            text.text = "Loading...";
            text.alignment = TextAnchor.MiddleCenter;

            var font = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");
            text.font = font;
            text.material = font.material;
        }

        public void Awake()
        {
            loader = new AsyncCacheLoader(GameContext.Cache);
            loader.Run();
            tmpCamera = Camera.main;
            tmpCamera.enabled = false;
        }

        /// <summary>
        /// Switches to the login screen.
        /// </summary>
        private void SwitchToLogin()
        {
            Destroy(texObject);
            Destroy(textObject);

            ResourceCache.Init();

            GameContext.MaterialPool = new MaterialPool();
            GameContext.Init();

            enabled = false;
            tmpCamera.enabled = true;

            GameObject loginScreen = new GameObject("LoginScreen");
            loginScreen.AddComponent<LoginScreen>();
        }

        public /* override */ void Update()
        {
            if (fadeDirection == Direction.Backward)
            {
                alpha -= 3;
                if (alpha <= 0)
                {
                    alpha = 0;
                    fadeDirection = Direction.Forward;
                }
            }
            else
            {
                alpha += 3;
                if (alpha >= 255)
                {
                    alpha = 255;
                    fadeDirection = Direction.Backward;
                }
            }

            text.color = new Color32(0x9B, 0x00, 0x08, alpha);

            if (loader.Completed && !attemptedLoad)
            {
                attemptedLoad = true;
                SwitchToLogin();
            }
        }
    }

}
