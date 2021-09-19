namespace monoGameTest

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics
open Microsoft.Xna.Framework.Input

open System

type Sprite = {
    position : Vector2
    speed : float32
    texture : Texture2D
    size : Point
    offset : Point
} with
    member this.Draw (spriteBatch : SpriteBatch) =
        let sourceRect = Rectangle(this.offset, this.size)
        spriteBatch.Draw(this.texture, this.position, Nullable.op_Implicit sourceRect, Color.White)


type Game1 () as this =
    inherit Game()
 
    let graphics = new GraphicsDeviceManager(this)
    let mutable spriteBatch = Unchecked.defaultof<_>
    let mutable playerSpriteSheet = Unchecked.defaultof<Texture2D>
    let mutable player = Unchecked.defaultof<Sprite>

    let (|Up|_|) (state : KeyboardState) = 
        let keyState = state.IsKeyDown Keys.W
        if keyState then Some () else None
    let (|Down|_|) (state : KeyboardState) = if state.IsKeyDown Keys.S then Some () else None
    let (|Left|_|) (state : KeyboardState) = if state.IsKeyDown Keys.A then Some () else None
    let (|Right|_|) (state : KeyboardState) = if state.IsKeyDown Keys.D then Some () else None
    
    let getMovementVector = function
        | Up & Left -> Vector2(-1.f, -1.f)
        | Up & Right -> Vector2(1.f, -1.f)
        | Down & Left -> Vector2(-1.f, 1.f)
        | Down & Right -> Vector2(1.f, 1.f)
        | Up -> Vector2(0.f, -1.f)
        | Down -> Vector2(0.f, 1.f)
        | Left -> Vector2(-1.f, 0.f)
        | Right -> Vector2(1.f, 0.f)
        | _ -> Vector2.Zero

    do
        this.Content.RootDirectory <- "Content"
        this.IsMouseVisible <- true

    override this.Initialize() =
        // TODO: Add your initialization logic here
        
        base.Initialize()

    override this.LoadContent() =
        spriteBatch <- new SpriteBatch(this.GraphicsDevice)
        playerSpriteSheet <- this.Content.Load<Texture2D>("skeleton")
        player <- {
            position = Vector2.Zero
            speed = 166.f
            texture = playerSpriteSheet
            size = Point(64,64)
            offset = Point(0,128)
        }

        // TODO: use this.Content to load your game content here
 
    override this.Update (gameTime) =
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back = ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
        then this.Exit();

        let movementVector =
            let motion = getMovementVector(Keyboard.GetState())
            if motion <> Vector2.Zero then motion.Normalize()
            motion
            

        let newPosition =
           let newPos = player.position + movementVector * player.speed * float32 gameTime.ElapsedGameTime.TotalSeconds
           let playerSize = player.size.ToVector2()
           let minClamp = Vector2.Zero - playerSize * 0.5f
           let maxClamp = Vector2(float32 this.GraphicsDevice.Viewport.Width, float32 this.GraphicsDevice.Viewport.Height) - playerSize * 0.5f

           Vector2.Clamp(newPos, minClamp, maxClamp)
    
        player <- { player with position = newPosition }
        base.Update(gameTime)
 
    override this.Draw (gameTime) =
        this.GraphicsDevice.Clear Color.Black
        spriteBatch.Begin()
        player.Draw(spriteBatch)
        spriteBatch.End()

        // TODO: Add your drawing code here

        base.Draw(gameTime)

