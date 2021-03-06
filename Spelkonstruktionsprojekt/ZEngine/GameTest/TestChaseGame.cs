﻿using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Penumbra;
using Spelkonstruktionsprojekt.ZEngine.Components;
using Spelkonstruktionsprojekt.ZEngine.Components.RenderComponent;
using Spelkonstruktionsprojekt.ZEngine.Constants;
using Spelkonstruktionsprojekt.ZEngine.Managers;
using Spelkonstruktionsprojekt.ZEngine.Systems;
using Spelkonstruktionsprojekt.ZEngine.Systems.Collisions;
using Spelkonstruktionsprojekt.ZEngine.Systems.InputHandler;
using ZEngine.Components;
using ZEngine.Managers;
using ZEngine.Systems;
using ZEngine.Systems.Collisions;
using ZEngine.Systems.InputHandler;
using ZEngine.Wrappers;

namespace Spelkonstruktionsprojekt.ZEngine.GameTest
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class TestChaseGame : Game
    {
        private readonly GameDependencies _gameDependencies = new GameDependencies();
        private KeyboardState _oldKeyboardState = Keyboard.GetState();

        private RenderSystem RenderSystem;
        private LoadContentSystem LoadContentSystem;
        private InputHandler InputHandlerSystem;
        private MoveSystem MoveSystem;
        private TankMovementSystem TankMovementSystem;
        private TitlesafeRenderSystem TitlesafeRenderSystem;
        private CollisionSystem CollisionSystem;
        private CameraSceneSystem CameraFollowSystem;
        private FlashlightSystem LightSystems;
        private CollisionResolveSystem CollisionResolveSystem;
        private WallCollisionSystem WallCollisionSystem;
        private EnemyCollisionSystem EnemyCollisionSystem;
        private BulletCollisionSystem BulletCollisionSystem;
        private AISystem AISystem;
        private AnimationSystem AnimationSystem;
        private SoundSystem SoundSystem;
        private Video video;
        private VideoPlayer player;
        private WeaponSystem WeaponSystem;
        private HealthSystem HealthSystem;

        private Vector2 viewportDimensions = new Vector2(1600, 800);
        private PenumbraComponent penumbraComponent;
        private TempGameEnder TempGameEnder;

        public SpriteBatch spriteBatch;

        private Song musicTest;

        public TestChaseGame()
        {
            _gameDependencies.GraphicsDeviceManager = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = (int)viewportDimensions.X,
                PreferredBackBufferHeight = (int)viewportDimensions.Y
            };
            Content.RootDirectory = "Content";

            // Turn off the fixed time step
            // and the synchronization with the vertical retrace
            // so the game's FPS can be measured
            IsFixedTimeStep = false;
            _gameDependencies.GraphicsDeviceManager.SynchronizeWithVerticalRetrace = false;
        }

        protected override void Initialize()
        {
            this.IsMouseVisible = true;
            //Get Systems
            RenderSystem = SystemManager.Instance.GetSystem<RenderSystem>();
            LoadContentSystem = SystemManager.Instance.GetSystem<LoadContentSystem>();
            InputHandlerSystem = SystemManager.Instance.GetSystem<InputHandler>();
            TankMovementSystem = SystemManager.Instance.GetSystem<TankMovementSystem>();
            TitlesafeRenderSystem = SystemManager.Instance.GetSystem<TitlesafeRenderSystem>();
            CollisionSystem = SystemManager.Instance.GetSystem<CollisionSystem>();
            CameraFollowSystem = SystemManager.Instance.GetSystem<CameraSceneSystem>();
            LightSystems = SystemManager.Instance.GetSystem<FlashlightSystem>();
            MoveSystem = SystemManager.Instance.GetSystem<MoveSystem>();
            CollisionResolveSystem = SystemManager.Instance.GetSystem<CollisionResolveSystem>();
            WallCollisionSystem = SystemManager.Instance.GetSystem<WallCollisionSystem>();
            AISystem = SystemManager.Instance.GetSystem<AISystem>();
            EnemyCollisionSystem = SystemManager.Instance.GetSystem<EnemyCollisionSystem>();
            AnimationSystem = SystemManager.Instance.GetSystem<AnimationSystem>();
            SoundSystem = SystemManager.Instance.GetSystem<SoundSystem>();
            WeaponSystem = SystemManager.Instance.GetSystem<WeaponSystem>();
            BulletCollisionSystem = SystemManager.Instance.GetSystem<BulletCollisionSystem>();
            HealthSystem = SystemManager.Instance.GetSystem<HealthSystem>();

            TempGameEnder = new TempGameEnder();

            //Init systems that require initialization
            TankMovementSystem.Start();
            WallCollisionSystem.Start();
            SoundSystem.Start();
            WeaponSystem.Start();
            EnemyCollisionSystem.Start();
            BulletCollisionSystem.Start();

            _gameDependencies.GameContent = this.Content;
            _gameDependencies.SpriteBatch = new SpriteBatch(GraphicsDevice);
            // just quickly done for FPS testing
            spriteBatch = _gameDependencies.SpriteBatch;
            _gameDependencies.Game = this;

            CreateTestEntities();

            base.Initialize();
        }

        private void CreateTestEntities()
        {
            var cameraCageId = SetupCameraCage();
            InitPlayers(cameraCageId);
            //SetupBackground();
            SetupBackgroundTiles(5,5);
            SetupCamera();
            SetupEnemy();
            CreateGlobalBulletSpriteEntity();
            SetupTempPlayerDeadSpriteFlyweight();
        }

        private void SetupTempPlayerDeadSpriteFlyweight()
        {
            var tempEntity = EntityManager.GetEntityManager().NewEntity();
            var spriteComponent = new SpriteComponent()
            {
                SpriteName = "dot"
            };
            var tempDeadSpriteComponent = new TempPlayerDeadSpriteComponent();
            ComponentManager.Instance.AddComponentToEntity(tempDeadSpriteComponent, tempEntity);
            ComponentManager.Instance.AddComponentToEntity(spriteComponent, tempEntity);
        }

        private static void CreateGlobalBulletSpriteEntity()
        {
            var bulletSprite = EntityManager.GetEntityManager().NewEntity();
            var bulletSpriteSprite = new SpriteComponent()
            {
                SpriteName = "dot2"
            };
            var bulletSpriteComponent = new BulletFlyweightComponent();
            var soundComponent = new SoundComponent()
            {
                SoundEffectName = "bullet9mm"
            };
            ComponentManager.Instance.AddComponentToEntity(soundComponent, bulletSprite);
            ComponentManager.Instance.AddComponentToEntity(bulletSpriteSprite, bulletSprite);
            ComponentManager.Instance.AddComponentToEntity(bulletSpriteComponent, bulletSprite);
        }

        //The camera cage keeps players from reaching the edge of the screen
        public int SetupCameraCage()
        {
            var cameraCage = EntityManager.GetEntityManager().NewEntity();
//            var renderComponentCage = new RenderComponentBuilder()
////                .Position((int)((int)viewportDimensions.X * 0.5), (int)(viewportDimensions.Y * 0.5), 2)
//                .Position(0, 0, 2)
//                .Dimensions((int)(viewportDimensions.X * 0.8), (int)(viewportDimensions.Y * 0.8))
//                .Fixed(true).Build();
            var cageSprite = new SpriteComponent()
            {
                SpriteName = "dot"
            };
            var collisionComponentCage = new CollisionComponent()
            {
                IsCage = true,
            };
            var offsetComponent = new RenderOffsetComponent()
            {
                Offset = new Vector2((float) (viewportDimensions.X * 0.25), (float) (viewportDimensions.Y * 0.25))
            };
  //          ComponentManager.Instance.AddComponentToEntity(renderComponentCage, cameraCage);
//            ComponentManager.Instance.AddComponentToEntity(cageSprite, cameraCage);
            ComponentManager.Instance.AddComponentToEntity(collisionComponentCage, cameraCage);
            ComponentManager.Instance.AddComponentToEntity(offsetComponent, cameraCage);
            return cameraCage;
        }

        public void SetupBackground()
        {
            var entityId3 = EntityManager.GetEntityManager().NewEntity();
            //var renderComponent3 = new RenderComponentBuilder()
            //    .Position(0, 0, 1)
            //    .Dimensions(1000, 1000).Build();
            //ComponentManager.Instance.AddComponentToEntity(renderComponent3, entityId3);

            var renderComponent3 = new RenderComponent() {DimensionsComponent = new DimensionsComponent() {Height = 1000, Width = 1000} };
            var positionComponent3 = new PositionComponent() {Position = new Vector2(0,0), ZIndex = 1};

            var spriteComponent3 = new SpriteComponent()
            {
                SpriteName = "Grass"
            };
            ComponentManager.Instance.AddComponentToEntity(spriteComponent3, entityId3);
            ComponentManager.Instance.AddComponentToEntity(positionComponent3, entityId3);
            ComponentManager.Instance.AddComponentToEntity(renderComponent3, entityId3);
        }

        public void SetupBackgroundTiles(int width, int height)
        {
            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    var entityId3 = EntityManager.GetEntityManager().NewEntity();
                    //var renderComponent3 = new RenderComponentBuilder()
                    //    .Position(x * 1000, y * 1000, 1)
                    //    .Dimensions(1000, 1000).Build();
                    //  ComponentManager.Instance.AddComponentToEntity(renderComponent3, entityId3);
                    var renderComponent3 = new RenderComponent() { DimensionsComponent = new DimensionsComponent() { Height = 1000, Width = 1000 } };
                    var positionComponent3 = new PositionComponent() { Position = new Vector2(x*1000, y*1000), ZIndex = 1 };

                    var spriteComponent3 = new SpriteComponent()
                    {
                        SpriteName = "Grass"
                    };
                    ComponentManager.Instance.AddComponentToEntity(positionComponent3, entityId3);
                    ComponentManager.Instance.AddComponentToEntity(renderComponent3, entityId3);
                    ComponentManager.Instance.AddComponentToEntity(spriteComponent3, entityId3);
                }
            }
        }

        public void SetupCamera()
        {
            var cameraEntity = EntityManager.GetEntityManager().NewEntity();
            var cameraViewComponent = new CameraViewComponent()
            {
                View = new Rectangle(0, 0, (int)viewportDimensions.X, (int)viewportDimensions.Y)
            };
            ComponentManager.Instance.AddComponentToEntity(cameraViewComponent, cameraEntity);
            //var cameraRenderable = new RenderComponentBuilder()
            //    .Position(0, 0, 500)
            //    .Dimensions(10, 10).Build();

            var renderComponent3 = new RenderComponent() { DimensionsComponent = new DimensionsComponent() { Height = 10, Width = 10 } };
            var positionComponent3 = new PositionComponent() { Position = new Vector2(0, 0), ZIndex = 500 };


            var cameraSprite = new SpriteComponent()
            {
                SpriteName = "dot"
            };
            var light = new LightComponent()
            {
                Light = new PointLight()
                {
                    Position = new Vector2(150, 150),
                    Scale = new Vector2(500f),
                    ShadowType = ShadowType.Solid // Will not lit hulls themselves
                }
            };

            ComponentManager.Instance.AddComponentToEntity(positionComponent3, cameraEntity);
            ComponentManager.Instance.AddComponentToEntity(renderComponent3, cameraEntity);
            //ComponentManager.Instance.AddComponentToEntity(light, cameraEntity);
            //ComponentManager.Instance.AddComponentToEntity(cameraRenderable, cameraEntity);
            //ComponentManager.Instance.AddComponentToEntity(cameraSprite, cameraEntity);
        }

        public void SetupEnemy()
        {
            var x = new Random(DateTime.Now.Millisecond).Next(0, 5000);
            var y = new Random(DateTime.Now.Millisecond).Next(0, 5000);

            var entityId = EntityManager.GetEntityManager().NewEntity();
            //var renderComponent = new RenderComponentBuilder()
            //    .Position(x, y, 20)
            //    .Dimensions(300, 300)
            //    .Build();

            var renderComponent3 = new RenderComponent() { DimensionsComponent = new DimensionsComponent() { Height = 300, Width = 300 } };
            var positionComponent3 = new PositionComponent() { Position = new Vector2(x, y), ZIndex = 20 };
            ComponentManager.Instance.AddComponentToEntity(renderComponent3, entityId);
            ComponentManager.Instance.AddComponentToEntity(positionComponent3, entityId);



            var spriteComponent = new SpriteComponent()
            {
                SpriteName = "zombieSquare"
            };
            var light = new LightComponent()
            {
                Light = new Spotlight()
                {
                    Position = new Vector2(150, 150),
                    Scale = new Vector2(500f),
                    ShadowType = ShadowType.Solid // Will not lit hulls themselves
                }
            };

            var sound = new SoundComponent()
            {
                SoundEffectName = "zombiewalking",
                Volume = 1f

            };
            ComponentManager.Instance.AddComponentToEntity(sound, entityId);

            var moveComponent = new MoveComponent()
            {
                MaxVelocitySpeed = 205,
                AccelerationSpeed = 5,
                RotationSpeed = 4,
                Direction = new Random(DateTime.Now.Millisecond).Next(0, 40) / 10
            };
            var aiComponent = new AIComponent();
           //ComponentManager.Instance.AddComponentToEntity(renderComponent, entityId);
            ComponentManager.Instance.AddComponentToEntity(spriteComponent, entityId);
            //ComponentManager.Instance.AddComponentToEntity(light, entityId);
            ComponentManager.Instance.AddComponentToEntity(moveComponent, entityId);
            ComponentManager.Instance.AddComponentToEntity(aiComponent, entityId);
            var collisionComponent = new CollisionComponent()
            {
                //spriteBoundingRectangle = new Rectangle(50, 50, 200, 200)
            };
            ComponentManager.Instance.AddComponentToEntity(collisionComponent, entityId);
            //if (collision)
            //{
            //
            //}
        }

        public void InitPlayers(int cageId)
        {
            var player1 = EntityManager.GetEntityManager().NewEntity();
            var actionBindings1 = new ActionBindingsBuilder()
                .SetAction(Keys.W, EventConstants.WalkForward) //Use of the next gen constants :)
                .SetAction(Keys.S, EventConstants.WalkBackward)
                .SetAction(Keys.A, "entityTurnLeft")
                .SetAction(Keys.D, "entityTurnRight")
                .SetAction(Keys.Q, "entityTurnAround")
                .SetAction(Keys.E, "entityFireWeapon")
                .SetAction(Keys.LeftShift, "entityRun")
                .Build();

            var player2 = EntityManager.GetEntityManager().NewEntity();
            var actionBindings2 = new ActionBindingsBuilder()
                .SetAction(Keys.I, "entityWalkForwards")
                .SetAction(Keys.K, "entityWalkBackwards")
                .SetAction(Keys.J, "entityTurnLeft")
                .SetAction(Keys.L, "entityTurnRight")
                .SetAction(Keys.O, "entityFireWeapon")
                .SetAction(Keys.U, "entityTurnAround")
                .SetAction(Keys.H, "entityRun")
                .Build();

            var player3 = EntityManager.GetEntityManager().NewEntity();
            var actionBindings3 = new ActionBindingsBuilder()
                .SetAction(Keys.Up, "entityWalkForwards")
                .SetAction(Keys.Down, "entityWalkBackwards")
                .SetAction(Keys.Left, "entityTurnLeft")
                .SetAction(Keys.Right, "entityTurnRight")
                .SetAction(Keys.PageDown, "entityFireWeapon")
                .SetAction(Keys.PageUp, "entityTurnAround")
                .SetAction(Keys.RightControl, "entityRun")
                .Build();

            CreatePlayer(player1, actionBindings1, position: new Vector2(200, 200), cameraFollow: true, collision: true, isCaged: true, cageId: cageId);
            CreatePlayer(player2, actionBindings2, position: new Vector2(400, 400), cameraFollow: true, collision: true, disabled: true);
            CreatePlayer(player3, actionBindings3, position: new Vector2(300, 300), cameraFollow: true, collision: true, isCaged: true, disabled: true);
        }

        //The multitude of options here is for easy debug purposes
        public void CreatePlayer(int entityId, ActionBindings actionBindings, Vector2 position = default(Vector2), bool movable = true, bool useDefaultMoveComponent = true, MoveComponent customMoveComponent = null, bool cameraFollow = false, bool collision = false, bool disabled = false, bool isCaged = false, int cageId = 0)
        {
            if (disabled) return;
            if(position == default(Vector2)) position = new Vector2(150, 150);
            //Initializing first, movable, entity
            //var renderComponent = new RenderComponentBuilder()
            //    //.Position(150 + new Random(DateTime.Now.Millisecond).Next(0, 500), 150, 10)
            //    .Position(position.X, position.Y, 10)
            //    //.Radius(60)
            //    .Dimensions(100, 100)
            //    .Build();

            var renderComponent3 = new RenderComponent() { DimensionsComponent = new DimensionsComponent() { Height = 100, Width = 100 }, Radius = 60};
            var positionComponent3 = new PositionComponent() { Position = position, ZIndex = 10 };
            ComponentManager.Instance.AddComponentToEntity(renderComponent3, entityId);
            ComponentManager.Instance.AddComponentToEntity(positionComponent3, entityId);


            var spriteComponent = new SpriteComponent()
            {
                SpriteName = "topDownSoldier"
            };
            var light = new LightComponent()
            {
                Light = new Spotlight()
                {
                    Position = new Vector2(150, 150),
                    Scale = new Vector2(850f),
                    Radius = (float) 0.0001,
                    Intensity = (float) 0.6,
                    ShadowType = ShadowType.Solid // Will not lit hulls themselves
                }
            };

            var sound = new SoundComponent()
            {
                SoundEffectName = "walking"

            };

            ComponentManager.Instance.AddComponentToEntity(sound, entityId);
           // ComponentManager.Instance.AddComponentToEntity(renderComponent, entityId);
            ComponentManager.Instance.AddComponentToEntity(spriteComponent, entityId);
            ComponentManager.Instance.AddComponentToEntity(actionBindings, entityId);
            ComponentManager.Instance.AddComponentToEntity(light, entityId);

            if (movable && useDefaultMoveComponent)
            {
                var moveComponent = new MoveComponent()
                {
                    MaxVelocitySpeed = 200,
                    AccelerationSpeed = 380,
                    RotationSpeed = 4,
                    Direction = new Random(DateTime.Now.Millisecond).Next(0, 40) / 10
                };
                ComponentManager.Instance.AddComponentToEntity(moveComponent, entityId);
            }
            else if (movable && customMoveComponent != null)
            {
                ComponentManager.Instance.AddComponentToEntity(customMoveComponent, entityId);
            }

            if (collision)
            {
                var collisionComponent = new CollisionComponent()
                {
                    spriteBoundingRectangle = new Rectangle(30, 20, 70, 60)
                };
                ComponentManager.Instance.AddComponentToEntity(collisionComponent, entityId);
            }
            if (cameraFollow)
            {
                var cameraFollowComponent = new CameraFollowComponent();
                ComponentManager.Instance.AddComponentToEntity(cameraFollowComponent, entityId);
            }

            if (isCaged)
            {
                var cageComponent = new CageComponent()
                {
                    CageId = cageId
                };
                ComponentManager.Instance.AddComponentToEntity(cageComponent, entityId);
            }

            var playerComponent = new PlayerComponent()
            {
                Name = entityId.ToString()
            };
            ComponentManager.Instance.AddComponentToEntity(playerComponent, entityId);
            var healthComponent = new HealthComponent()
            {
                CurrentHealth = new Random().Next(0, 100)
            };
            ComponentManager.Instance.AddComponentToEntity(healthComponent, entityId);
            var weaponComponent = new WeaponComponent()
            {
                Damage = 10
            };
            ComponentManager.Instance.AddComponentToEntity(weaponComponent, entityId);
        }
        
        protected override void LoadContent()
        {
            video = Content.Load<Video>("ZEngine-intro");

            player = new VideoPlayer();
            // musicTest = Content.Load<Song>("assassins");
            LoadContentSystem.LoadContent(this.Content);
            penumbraComponent = LightSystems.Initialize(_gameDependencies);
            //MediaPlayer.Play(musicTest);

            if (player.State == MediaState.Stopped)
            {
                player.Play(video);
            }
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        public void ToUpdate(GameTime gameTime)
        {
                EnemyCollisionSystem.GameTime = gameTime;
                InputHandlerSystem.HandleInput(_oldKeyboardState, gameTime);
                _oldKeyboardState = Keyboard.GetState();

                AISystem.Update(gameTime);
                MoveSystem.Move(gameTime);
                AnimationSystem.RunAnimations(gameTime);

                CollisionSystem.DetectCollisions();
                CollisionResolveSystem.ResolveCollisions(ZEngineCollisionEventPresets.StandardCollisionEvents, gameTime);

                CameraFollowSystem.Update(gameTime);
                LightSystems.Update(gameTime, viewportDimensions);
                //HealthSystem.TempEndGameIfDead(TempGameEnder);
                if (TempGameEnder.Score > 0)
                {
                    Debug.WriteLine("YOUR SCORE WAS: " + TempGameEnder.Score);
                    while (true) ;       
                }
        }

        // Game states
        public enum GameState {
            Intro,
            Menu,
            InGame,
            GameOver
        };

        // Here we just say that the first state is the Intro
        public GameState currentGameState = GameState.Intro;

        protected override void Update(GameTime gameTime)
        {
            ToUpdate(gameTime);                        
            base.Update(gameTime);
        }

        public void MainMenu()
        {
            MainMenuDisplay();
            ContinueButton();
            BackToMenu();
            ExitButton();
        }

        public void ContinueButton()
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed
                || Keyboard.GetState().IsKeyDown(Keys.Enter)) currentGameState = GameState.InGame;
        }

        public void ExitButton()
        {
            if ((GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                       Keyboard.GetState().IsKeyDown(Keys.S)) && currentGameState == GameState.Menu) Exit();
        }

        public void BackToMenu()
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                       Keyboard.GetState().IsKeyDown(Keys.Escape)) currentGameState = GameState.Menu;
        }

        public void MainMenuDisplay()
        {
            SpriteFont font = Content.Load<SpriteFont>("Score");

            String textEscape = "ESCAPE: BACK TO THE MAIN MENU / PAUSE THE GAME";
            String textContinue = "ENTER: CONTINUE";
            String textExit = "S: EXIT THE GAME";

            spriteBatch.Begin();

            spriteBatch.DrawString(font, textContinue, new Vector2(400, 170), Color.SaddleBrown);
            spriteBatch.DrawString(font, textEscape, new Vector2(400, 200), Color.SaddleBrown);
            spriteBatch.DrawString(font, textExit, new Vector2(400, 230), Color.SaddleBrown);

            spriteBatch.End();
        }

        public void IntroVideo()
        {
            Texture2D videoTexture = null;

            if (player.State != MediaState.Stopped)
                videoTexture = player.GetTexture();

            if (videoTexture != null)
            {
                spriteBatch.Begin();
                spriteBatch.Draw(videoTexture, new Rectangle(0, 0, (int)viewportDimensions.X, (int)viewportDimensions.Y), Color.White);
                spriteBatch.End();

            } else currentGameState = GameState.Menu;
        }

        public void PlayGame(GameTime gameTime)
        {
                LightSystems.BeginDraw(penumbraComponent);
                RenderSystem.Render(_gameDependencies);
                LightSystems.EndDraw(penumbraComponent, gameTime);
                TitlesafeRenderSystem.Draw(_gameDependencies);

                BackToMenu();
                ContinueButton();
                ExitButton();
        }

        protected override void Draw(GameTime gameTime)
        {
            switch (currentGameState) {

                case GameState.Intro:

                    IntroVideo();

                    break;

                case GameState.Menu:

                    MainMenu();

                    break;

                case GameState.InGame:

                    PlayGame(gameTime);

                    break;

                case GameState.GameOver:

                    ExitButton();

                    break;
            }
            base.Draw(gameTime);
        }
    }
}