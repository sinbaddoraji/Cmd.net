using System.Reflection;

namespace Cmd.net;

class Program
{
	private static void Main(string[] args)
	{
		while (true)
		{
			Console.Write(Environment.CurrentDirectory);
			Console.Write("> ");
			var input = Console.ReadLine().Trim();
			var tokens = input.Split();

			if (tokens.Length > 0)
			{
				var commandName = tokens[0].Trim('.', ' ');
				var methodName = char.ToUpper(commandName[0]) + commandName.Substring(1);

				var methods = typeof(Commands).GetMethods(BindingFlags.Static | BindingFlags.Public);

				bool commandFound = false;

				foreach (var method in methods)
				{
					if (method.Name == methodName)
					{
						commandFound = true;

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

				if (!commandFound)
				{
					// Check if input is a file and if it has a .batnet extension
					if (File.Exists(input) && Path.GetExtension(input).ToLower() == ".batnet")
					{
						var fileContents = File.ReadAllLines(input);
						var fileCommands = string.Join(" & ", fileContents);

						var batchTokens = fileCommands.Split();
						var batchMethodName = char.ToUpper(batchTokens[0][0]) + batchTokens[0].Substring(1);

						var batchMethods = typeof(Commands).GetMethods(BindingFlags.Static | BindingFlags.Public);

						bool batchCommandFound = false;

						foreach (var method in batchMethods)
						{
							if (method.Name == batchMethodName)
							{
								batchCommandFound = true;

								var parameters = method.GetParameters();
								var argsToPass = new List<object>();

								for (var i = 0; i < batchTokens.Length; i++)
								{
									var token = batchTokens[i];
									var paramType = parameters.ElementAtOrDefault(i)?.ParameterType ?? typeof(string[]);

									if (paramType.IsAssignableFrom(typeof(string[])))
									{
										argsToPass.Add(batchTokens.Skip(i).ToArray());
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

						if (!batchCommandFound)
						{
							Console.WriteLine($"Command or file '{input}' not found.");
						}
					}
					else
					{
						Console.WriteLine($"Command or file '{input}' not found.");
					}
				}
			}
		}
	}

}