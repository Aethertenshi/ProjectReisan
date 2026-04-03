using Raylib_cs;
using reie;

namespace MainGame;
public class CoreGame {
    public void Init(Engine engine) 
    {
        engine.Window(1280, 720);
        engine.SetFPS(60);
        engine.UseTexture("Char1", "media/scene1.png");
        engine.UseFont("Metropolis", "media/metropolis.medium.otf", 200);
    }
    public void Draw(Engine engine) 
    {
        engine.ClearScreen(Color.Black);
        
        // Draw Variables
        Button clickme = new Button(new Rectangle(540, 310, 200, 100), "Click Me!", 20, "Metropolis");

        if (engine.DrawButton(clickme)) {
            Console.WriteLine("Button Clicked!");
        }
        //_engine?.DrawCover("Char1", new Rectangle(0, 0, 1280, 720));
    }
    public void Update(Engine engine, float dt) 
    {
        
    }
}