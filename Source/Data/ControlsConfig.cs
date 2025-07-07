using System.Text.Json;
using System.Text.Json.Serialization;

namespace Celeste64;

public class ControlsConfig
{
	public const string FILTER_NINTENDO = "NINTENDO";
	public const string FILTER_XBOX = "XBOX";
	public const string FILTER_MOUSE_CAMERA = "MOUSE_CAMERA";
	public const string FILTER_KEYBOARD_CAMERA = "KEYBOARD_CAMERA";

	public const string FileName = "controls.json";

	public ActionBindingSet Jump { get; private set; } = new ActionBindingSet()
		.Add(Keys.C, Keys.Space)
		.Add([FILTER_MOUSE_CAMERA], MouseButtons.Left)
		.Add(Buttons.South, Buttons.North);

	public ActionBindingSet Dash { get; private set; } = new ActionBindingSet()
		.Add(Keys.X)
		.Add([FILTER_MOUSE_CAMERA], MouseButtons.Right)
		.Add(Buttons.West, Buttons.East);

	public ActionBindingSet Climb { get; private set; } = new ActionBindingSet()
		.Add(Keys.Z,Keys.V,Keys.LeftShift, Keys.RightShift)
		.Add(Buttons.LeftShoulder,Buttons.RightShoulder)
		.Add(Axes.LeftTrigger, 1, 0.4f)
		.Add(Axes.RightTrigger, 1, 0.4f);

	public ActionBindingSet Confirm { get; private set; } = new ActionBindingSet()
		.Add(Keys.C)
		.Add([FILTER_MOUSE_CAMERA], MouseButtons.Left)
		.Add([FILTER_XBOX], Buttons.South)
		.Add([FILTER_NINTENDO], Buttons.East);

	public ActionBindingSet Cancel { get; private set; } = new ActionBindingSet()
		.Add(Keys.X)
		.Add([FILTER_MOUSE_CAMERA], MouseButtons.Right)
		.Add([FILTER_XBOX], Buttons.East)
		.Add([FILTER_NINTENDO], Buttons.South);

	public ActionBindingSet Pause { get; private set; } = new ActionBindingSet()
		.Add(Keys.Enter, Keys.Escape)
		.Add(Buttons.Start, Buttons.Guide, Buttons.Back);

	public StickBindingSet Move { get; private set; } = new StickBindingSet()
		.AddWasd([FILTER_MOUSE_CAMERA])
		.AddArrowKeys([FILTER_KEYBOARD_CAMERA])
		.AddLeftJoystick(0.35f);

	public StickBindingSet Camera { get; private set; } = new StickBindingSet()
		.AddMouseMotion([FILTER_MOUSE_CAMERA])
		.AddWasd([FILTER_KEYBOARD_CAMERA])
		.AddRightJoystick(0.35f);

	public ActionBindingSet MenuLeft { get; private set; } = new ActionBindingSet()
		.Add(Keys.Left, Keys.A)
		.Add(Buttons.Left)
		.AddLeftJoystickLeft();

	public ActionBindingSet MenuRight { get; private set; } = new ActionBindingSet()
		.Add(Keys.Right, Keys.D)
		.Add(Buttons.Right)
		.AddLeftJoystickRight();

	public ActionBindingSet MenuUp { get; private set; } = new ActionBindingSet()
		.Add(Keys.Up, Keys.W)
		.Add(Buttons.Up)
		.AddLeftJoystickUp();

	public ActionBindingSet MenuDown { get; private set; } = new ActionBindingSet()
		.Add(Keys.Down, Keys.S)
		.Add(Buttons.Down)
		.AddLeftJoystickDown();
}

[JsonSourceGenerationOptions(
	WriteIndented = true,
	UseStringEnumConverter = true,
	AllowTrailingCommas = true
)]
[JsonSerializable(typeof(ControlsConfig))]
internal partial class ControlsConfigContext : JsonSerializerContext {}
