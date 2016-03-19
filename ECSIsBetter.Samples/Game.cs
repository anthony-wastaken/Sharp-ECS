﻿using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using ECSIsBetter;
using ECSIsBetter.Samples.Components;
using ECSIsBetter.Samples.Systems;

namespace ECSIsBetter.Samples
{
    public class Game : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        KeyboardState keyboard;
        KeyboardState previousKeyboard;

        MouseState mouse;
        MouseState previousMouse;

        EntityPool entityPool;

        EntityGroup renderableGroup;
        EntityGroup controllableGroup;

        GraphicsSystem graphicsSystem;
        ControllerSystem controllerSystem;

        Entity playerEntity;
        Entity hostileEntity;

        public Game()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            entityPool = EntityPool.New("EntityPool");

            controllableGroup = EntityGroup.New("ControllableGroup", new ControllerComponent());
            renderableGroup = EntityGroup.New("RenderGroup", new GraphicsComponent());

            hostileEntity = entityPool.CreateEntity("HostileEntity");
            playerEntity = entityPool.CreateEntity("Player");

            playerEntity += new TransformComponent();
            hostileEntity += new TransformComponent();

            controllableGroup.AddWithDependency(playerEntity);
            renderableGroup.AddWithDependency(playerEntity);

            renderableGroup.AddWithDependency(hostileEntity);

            playerEntity.GetComponent<GraphicsComponent>().Texture = Content.Load<Texture2D>("Sprite");
            playerEntity.GetComponent<TransformComponent>().Position = new Vector2(10, 20);

            hostileEntity.GetComponent<GraphicsComponent>().Texture = Content.Load<Texture2D>("Sprite");
            hostileEntity.GetComponent<TransformComponent>().Position = new Vector2(50, 20);

            controllerSystem = new ControllerSystem(controllableGroup);
            graphicsSystem = new GraphicsSystem(renderableGroup);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void UnloadContent()
        {
            Dispose();
        }

        protected override void Update(GameTime gameTime)
        {
            keyboard = Keyboard.GetState();
            mouse = Mouse.GetState();

            if (keyboard.IsKeyDown(Keys.Escape)) Exit();

            if (mouse.LeftButton == ButtonState.Pressed && previousMouse.LeftButton == ButtonState.Released
                && hostileEntity.Tag != string.Empty)
            {
                hostileEntity.GetComponent<TransformComponent>().Position = new Vector2(mouse.X, mouse.Y);
            }

            controllerSystem.Update(gameTime);

            previousMouse = mouse;
            previousKeyboard = keyboard;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            graphicsSystem.Draw(spriteBatch);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}