
namespace Celeste64;

public class Controls(Input input, ControlsConfig config, int controllerIndex)
{
	public readonly Input Input = input;
	public readonly int ControllerIndex = controllerIndex;

	public readonly VirtualStick Move = new(input, "Move", config.Move, controllerIndex);
	public readonly VirtualStick Camera = new(input, "Camera", config.Camera, controllerIndex);
	public readonly VirtualAction Jump = new(input, "Jump", config.Jump, controllerIndex);
	public readonly VirtualAction Dash = new(input, "Dash", config.Dash, controllerIndex);
	public readonly VirtualAction Climb = new(input, "Climb", config.Climb, controllerIndex);
	public readonly VirtualAction Pause = new(input, "Pause", config.Pause, controllerIndex);
	public readonly VirtualAction Confirm = new(input, "Confirm", config.Confirm, controllerIndex);
	public readonly VirtualAction Cancel = new(input, "Cancel", config.Cancel, controllerIndex);

	public readonly (VirtualAction Left, VirtualAction Right, VirtualAction Up, VirtualAction Down) Menu = (
		new(input, "MenuLeft", config.MenuLeft, controllerIndex),
		new(input, "MenuRight", config.MenuRight, controllerIndex),
		new(input, "MenuUp", config.MenuUp, controllerIndex),
		new(input, "MenuDown", config.MenuDown, controllerIndex)
	);

	public void Consume()
	{
		Jump.ConsumePress();
		Dash.ConsumePress();
		Climb.ConsumePress();
		Confirm.ConsumePress();
		Cancel.ConsumePress();
		Pause.ConsumePress();
	}

	private readonly Dictionary<string, Dictionary<string, string>> prompts = [];

	private string GetControllerName(GamepadProviders pad) => pad switch
	{
		GamepadProviders.PlayStation => "PlayStation 5",
		GamepadProviders.Nintendo => "Nintendo Switch",
		GamepadProviders.Xbox => "Xbox Series",
		_ => "Xbox Series",
	};

	private string GetPromptLocation(string name)
	{
		var gamepad = Input.Controllers[ControllerIndex];
		var deviceTypeName = 
			gamepad.Connected ? GetControllerName(gamepad.GamepadProvider) : "PC";

		if (!prompts.TryGetValue(deviceTypeName, out var list))
			prompts[deviceTypeName] = list = [];

		if (!list.TryGetValue(name, out var lookup))
			list[name] = lookup = $"Controls/{deviceTypeName}/{name}";
					
		return lookup;
	}

	public string GetPromptLocation(VirtualAction button)
	{
		// TODO: instead, query the button's actual bindings and look up a
		// texture based on that! no time tho
		if (button == Confirm)
			return GetPromptLocation("confirm");
		else
			return GetPromptLocation("cancel");
	}

	public Subtexture GetPrompt(VirtualAction button)
	{
		return Assets.Subtextures.GetValueOrDefault(GetPromptLocation(button));
	}

	public bool IsUsingNintendo => Input.Controllers[ControllerIndex].GamepadProvider == GamepadProviders.Nintendo;
}
