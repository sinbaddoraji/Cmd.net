using System.Reflection;

namespace Cmd.net;

class Program
{
	private static void Main(string[] args)
	{
		string[] tokens;

		if (args.Length > 0)
		{
			// Use command-line arguments if provided
			tokens = args;
		}
		else
		{
			// Read input from console
			while (true)
			{
				Console.Write(Environment.CurrentDirectory);
				Console.Write("> ");
				var input = Console.ReadLine();
				tokens = input?.Split();

				if (tokens is { Length: > 0 })
				{
					// If the command ends with .bat or .batnet, replace it with "call"
					var lastToken = tokens[tokens.Length - 1];
					if (lastToken.EndsWith(".batnet"))
					{
						tokens[tokens.Length - 1] = "call";
					}

					break;
				}
			}
		}

		var commandName = tokens[0].Trim('.', ' ');
		var methodName = char.ToUpper(commandName[0]) + commandName.Substring(1);

		var methods = typeof(Commands).GetMethods(BindingFlags.Static | BindingFlags.Public);

		foreach (var method in methods)
		{
			if (method.Name == methodName)
			{
				var parameters = method.GetParameters();
				var argsToPass = new List<object>();

				for (var i = 0; i < tokens.Length; i++)
				{
					var token = tokens[i];
					var paramType = parameters.ElementAtOrDefault(i)?.ParameterType ?? typeof(string[]);

					if (paramType.IsAssignableFrom(typeof(string[])))
					{
						argsToPass.Add(tokens.Skip(i).ToArray());
						break;
					}
					else if (paramType == typeof(string))
					{
						argsToPass.Add(token);
					}
					else
					{
						argsToPass.Add(Convert.ChangeType(token, paramType));
					}
				}

				method.Invoke(null, argsToPass.ToArray());
				break;
			}
		}
	}

}