using System;
using reien;

public class GameRunner : IGameRunner
{
	public static void Main(string[] args)
	{
		ReiEngine runner = new ReiEngine(new GameRunner());
		runner.Run();
    }
    public void Init(Engine engine)
	{
	}
	public void Update(Engine engine, float deltaTime)
	{
	}
	public void Draw(Engine engine)
	{
	}
}
