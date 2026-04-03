using Raylib_cs;
using reie;

namespace MainGame;
public class CoreGame {

    Button clickme = new Button(new Rectangle(540, 310, 200, 100), "Click Me!", 20, "Metropolis");
    
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
        engine.DrawButton(ref clickme);

        if (clickme.isClicked)
        {
            Console.WriteLine("Button Clicked!");
        }
        if (clickme.isHovering)
        {
            Console.WriteLine("Button Hovered!");
        }
        //_engine?.DrawCover("Char1", new Rectangle(0, 0, 1280, 720));
    }
    public void Update(Engine engine, float dt) 
    {
        
    }
}