namespace Cmd.net;

[AttributeUsage(AttributeTargets.Method)]
public class HelpAttribute : Attribute
{
	public string Description { get; }

	public HelpAttribute(string description)
	{
		Description = description;
	}
}