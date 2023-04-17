namespace Cmd.net;

[AttributeUsage(AttributeTargets.Method)]
public class HelpAttribute : Attribute
{
	public string CommandName { get; set; }
	public string Description { get; set; }
	public string Syntax { get; set; }

	public HelpAttribute(string commandName, string description, string syntax)
	{
		CommandName = commandName;
		Description = description;
		Syntax = syntax;
	}
}
