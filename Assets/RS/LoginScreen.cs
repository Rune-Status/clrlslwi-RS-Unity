using System.Text;

using UnityEngine;

namespace RS
{
    /// <summary>
    /// Handles the rendering and input of the login screen.
    /// </summary>
    public class LoginScreen : MonoBehaviour
    {
        private enum SelectedElement
        {
            None,
            Username,
            Password,
        }
        
        /// <summary>
        /// The bounds that define where to render the left size of the screen.
        /// </summary>
        private Rect leftBounds;
        /// <summary>
        /// The bounds that define where to render the right size of the screen.
        /// </summary>
        private Rect rightBounds;
        /// <summary>
        /// The bounds that define where to place the login box.
        /// </summary>
        private Rect loginBoxBounds;
        /// <summary>
        /// The bounds to display the message in.
        /// </summary>
        private Rect msgBounds;
        /// <summary>
        /// The bounds to displaye the username in.
        /// </summary>
        private Rect usernameBounds;
        /// <summary>
        /// The bounds to display the password in.
        /// </summary>
        private Rect passwordBounds;
        /// <summary>
        /// The bounds to display the login button in.
        /// </summary>
        private Rect loginButtonBounds;

        /// <summary>
        /// The currently selected form element.
        /// </summary>
        private SelectedElement selected = SelectedElement.None;

        /// <summary>
        /// The entered username.
        /// </summary>
        private string username = "";

        /// <summary>
        /// The entered password.
        /// </summary>
        private string password = "";

        /// <summary>
        /// The left side of the background.
        /// </summary>
        private Texture2D backgroundLeftTex;

        /// <summary>
        /// The right side of the background.
        /// </summary>
        private Texture2D backgroundRightTex;

        /// <summary>
        /// The login box.
        /// </summary>
        private Texture2D loginBoxTexture;

        /// <summary>
        /// A button.
        /// </summary>
        private Texture2D buttonTexture;

        /// <summary>
        /// The message text baked into a texture.
        /// </summary>
        private Texture2D msgTex;

        /// <summary>
        /// The username text baked into a texture.
        /// </summary>
        private Texture2D usernameTex;

        /// <summary>
        /// The password text baked into a texture.
        /// </summary>
        private Texture2D passwordTex;

        private GameObject obj;
        private GameProcessor processor;
        
        private Camera tmpCamera;

        /// <summary>
        /// Called when the game disconnects from the remote server.
        /// </summary>
        private void HandleDisconnect()
        {
            enabled = true;
            processor.enabled = false;
            Camera.main.enabled = false;
            GameContext.ResetState();
            InputManager.Instance.Reset();
            GameContext.NetworkHandler.InBuffer.Position(0);
            GameContext.NetworkHandler.OutPackets.Clear();
        }

        /// <summary>
        /// Called when a successful login is performed.
        /// </summary>
        private void OnSuccessfulLogin()
        {
            enabled = false;
            GameContext.Self = GameContext.Players[2047] = new Player();
            GameContext.Chat.CreateNameTex(username);
            processor.enabled = true;
            tmpCamera.enabled = true;
        }

        public /* override */ void Awake()
        {
            var title = GameContext.Cache.GetArchive(1);
            backgroundLeftTex = new Texture2D(1, 1);
            backgroundLeftTex.LoadImage(title.GetFile("title.dat"));
            backgroundLeftTex.Apply();
            backgroundRightTex = TextureUtils.FlipHorizontal(backgroundLeftTex);

            loginBoxTexture = GameContext.Cache.GetImageAsTex(title, "titlebox", 0);
            buttonTexture = GameContext.Cache.GetImageAsTex(title, "titlebutton", 0);

            leftBounds = new Rect(0, 0, 765 / 2, 503);
            rightBounds = new Rect(765 / 2, 0, 765 / 2 + 1, 503);

            loginBoxBounds = new Rect(765 / 2 - loginBoxTexture.width / 2, 503 / 2 - loginBoxTexture.height / 2, loginBoxTexture.width, loginBoxTexture.height);

            msgBounds = new Rect(loginBoxBounds.x + 35, loginBoxBounds.y + 45, 218, 27);
            usernameBounds = new Rect(loginBoxBounds.x + 35, loginBoxBounds.y + 74, 218, 27);
            passwordBounds = new Rect(loginBoxBounds.x + 35, loginBoxBounds.y + 97, 218, 27);
            loginButtonBounds = new Rect(loginBoxBounds.x + 90, loginBoxBounds.y + 140, buttonTexture.width, buttonTexture.height);

            var handler = GameContext.NetworkHandler;
            handler.OnDisconnect = (() =>
            {
                HandleDisconnect();
            });

            obj = new GameObject("GameProcessor");
            processor = obj.AddComponent<GameProcessor>();
            processor.enabled = false;

            tmpCamera = Camera.main;
            tmpCamera.enabled = false;

            CreateMsgTex("Please enter your username & password");
            CreateUsernameTex();
            CreatePasswordTex();
        }

        /// <summary>
        /// Attempts a login.
        /// </summary>
        private void AttemptLogin()
        {
            var handler = GameContext.NetworkHandler;
            handler.Connect("127.0.0.1", 6666);
            if (handler.WriteAuthBlock(username, password) == LoginResponse.SuccessfulLogin)
            {
                OnSuccessfulLogin();
            }
        }

        /// <summary>
        /// Creates the message label texture.
        /// </summary>
        private void CreateMsgTex(string msg)
        {
            if (msg == null || msg == string.Empty)
                msgTex = null;
            else
                msgTex = GameContext.Cache.BoldFont.DrawString(msg, 0xFFFFFF00, true, true);
        }

        /// <summary>
        /// Creates the username label texture.
        /// </summary>
        private void CreateUsernameTex()
        {
            usernameTex = GameContext.Cache.FancyFont.DrawString("Username: " + username, 0xFFEBE0BC, false, true);
        }

        /// <summary>
        /// Creates the password label texture.
        /// </summary>
        private void CreatePasswordTex()
        {
            var pass = new StringBuilder();
            for (var i = 0; i < password.Length; i++)
            {
                pass.Append('*');
            }

            passwordTex = GameContext.Cache.FancyFont.DrawString("Password: " + pass.ToString(), 0xFFEBE0BC, false, true);
        }

        public /* override */ void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (InputUtils.MouseWithin(usernameBounds))
                {
                    selected = SelectedElement.Username;
                }
                else if (InputUtils.MouseWithin(passwordBounds))
                {
                    selected = SelectedElement.Password;
                }
                else if (InputUtils.MouseWithin(loginButtonBounds))
                {
                    AttemptLogin();
                }
                else
                {
                    selected = SelectedElement.None;
                }
            }

            while (InputManager.Instance.HasKeys())
            {
                var key = InputManager.Instance.GetNextKey();
                var text = InputUtils.ToText(key);
                if (InputManager.Instance.Pressed[(int) KeyCode.LeftShift] || 
                    InputManager.Instance.Pressed[(int) KeyCode.RightShift]) {

                    text = StringUtils.ToUpper(text);
                }

                switch (selected)
                {
                    case SelectedElement.Username:
                        if (key == KeyCode.Backspace)
                        {
                            if (username.Length > 0)
                            {
                                username = username.Substring(0, username.Length - 1);
                                CreateUsernameTex();
                            }
                        }
                        if (text.Length > 0)
                        {
                            username += text;
                            CreateUsernameTex();
                        }
                        break;

                    case SelectedElement.Password:
                        if (key == KeyCode.Backspace)
                        {
                            if (username.Length > 0)
                            {
                                password = password.Substring(0, password.Length - 1);
                                CreatePasswordTex();
                            }
                        }
                        if (text.Length > 0)
                        {
                            password += text;
                            CreatePasswordTex();
                        }
                        
                        break;
                }
            }
        }

        public /* override */ void OnGUI()
        {
            Graphics.DrawTexture(leftBounds, backgroundLeftTex);
            Graphics.DrawTexture(rightBounds, backgroundRightTex);
            Graphics.DrawTexture(loginBoxBounds, loginBoxTexture);
            Graphics.DrawTexture(loginButtonBounds, buttonTexture);

            if (msgTex != null)
            {
                GUI.DrawTexture(new Rect(msgBounds.x, msgBounds.y, msgTex.width, msgTex.height), msgTex);
            }

            if (usernameTex != null)
            {
                GUI.DrawTexture(new Rect(usernameBounds.x, usernameBounds.y, usernameTex.width, usernameTex.height), usernameTex);
            }

            if (passwordTex != null)
            {
                GUI.DrawTexture(new Rect(passwordBounds.x, passwordBounds.y, passwordTex.width, passwordTex.height), passwordTex);
            }
        }
    }
}
