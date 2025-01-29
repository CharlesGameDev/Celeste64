
namespace Celeste64;

public abstract class Scene
{
	public readonly Game Game = Game.Instance;
	public readonly Input Input = Game.Instance.Input;
	public readonly GraphicsDevice GraphicsDevice = Game.Instance.GraphicsDevice;
	public Controls Controls => Game.Controls;
	public Time Time => Game.Time;

	public string Music = string.Empty;
	public string Ambience = string.Empty;

	public virtual void Entered() {}
	public virtual void Exited() {}
	public virtual void Disposed() {}
	public abstract void Update();
	public abstract void Render(Target target);
}