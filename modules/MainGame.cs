using Raylib_cs;
using reie;

namespace MainGame;
public class CoreGame {
    DrawBatch batch = new DrawBatch();

    public void Init(Engine engine) 
    {
        engine.Window(1280, 720);
        engine.SetFPS(60);
        engine.UseTexture("Char1", "media/scene1.png");
        engine.UseFont("Metropolis", "media/metropolis.medium.otf", 200);

        new Button(batch,
            (btn) => {
                if (btn.isClicked)
                {
                    Console.WriteLine("Button Clicked!");
                }
                if (btn.isHovered)
                {
                    Console.WriteLine("Button Hovered!");
                }
            },
            new Rectangle(540, 310, 200, 100), "Click Me!", 20, "Metropolis");
    }
    public void Draw(Engine engine) 
    {
        engine.ClearScreen(Color.Black);
        batch.DrawAll(engine);
    }
    public void Update(Engine engine, float dt) 
    {
        
    }
}