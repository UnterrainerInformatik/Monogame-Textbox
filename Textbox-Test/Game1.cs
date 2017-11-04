// *************************************************************************** 
// This is free and unencumbered software released into the public domain.
// 
// Anyone is free to copy, modify, publish, use, compile, sell, or
// distribute this software, either in source code form or as a compiled
// binary, for any purpose, commercial or non-commercial, and by any
// means.
// 
// In jurisdictions that recognize copyright laws, the author or authors
// of this software dedicate any and all copyright interest in the
// software to the public domain. We make this dedication for the benefit
// of the public at large and to the detriment of our heirs and
// successors. We intend this dedication to be an overt act of
// relinquishment in perpetuity of all present and future rights to this
// software under copyright law.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS BE LIABLE FOR ANY CLAIM, DAMAGES OR
// OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
// ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
// 
// For more information, please refer to <http://unlicense.org>
// ***************************************************************************

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame_Textbox;

namespace Textbox_Test
{
    /// <summary>
    ///     This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        private TextBox textBox;
        private SpriteFont font;
        private Rectangle viewport;

        public Game1()
        {
            GraphicsDeviceManager graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            // Initialize the keyboard-event handler.
            KeyboardInput.Initialize(this, 500f, 20);
        }

        /// <summary>
        ///     LoadContent will be called once per game and is the place to load
        ///     all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            font = Content.Load<SpriteFont>("Arsenal");

            viewport = new Rectangle(50, 50, 400, 200);
            textBox = new TextBox(viewport, 200, "This is a test. Move the cursor, select, delete, write...",
                GraphicsDevice, font, Color.LightGray, Color.DarkGreen, 30);
        }

        /// <summary>
        ///     Allows the game to run logic such as updating the world,
        ///     checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            KeyboardInput.Update();

            // This code is in here to play with in debug mode.
            // You may as well initialize it in LoadContent() or change it in here to achieve lerp effects and so on...
            float margin = 3;
            textBox.Area = new Rectangle((int) (viewport.X + margin), viewport.Y, (int) (viewport.Width - margin),
                viewport.Height);
            textBox.Renderer.Color = Color.White;
            textBox.Cursor.Selection = new Color(Color.Purple, .4f);

            float lerpAmount = (float) (gameTime.TotalGameTime.TotalMilliseconds % 500f / 500f);
            textBox.Cursor.Color = Color.Lerp(Color.DarkGray, Color.LightGray, lerpAmount);

            textBox.Active = true;
            textBox.Update();

            base.Update(gameTime);
        }

        /// <summary>
        ///     This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.DarkSlateGray);

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);
            textBox.Draw(spriteBatch);
            spriteBatch.DrawRectangle(viewport, Color.Red, 1f, 1f);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}