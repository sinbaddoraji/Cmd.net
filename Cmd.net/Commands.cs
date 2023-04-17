using System.Globalization;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Reflection;

namespace Cmd.net
{
	public static class Commands
	{
		[Help("Quits the CMD.NET program.\nSyntax: exit")]
		public static void Exit(string[] tokens)
		{
			Environment.Exit(0);
		}

		[Help("Displays a list of files and subdirectories in a directory.\nSyntax: dir [directory]")]
		public static void Dir(string[] tokens)
		{
			string path = ".";
			if (tokens.Length > 1)
			{
				path = tokens[1];
			}

			string[] files = Directory.GetFiles(path);
			string[] dirs = Directory.GetDirectories(path);
			Console.WriteLine("Directory of " + Path.GetFullPath(path));
			Console.WriteLine();
			foreach (string file in files)
			{
				Console.WriteLine(Path.GetFileName(file));
			}

			foreach (string dir in dirs)
			{
				Console.WriteLine(Path.GetFileName(dir) + " [DIR]");
			}
		}

		[Help("Displays the name of or changes the current directory.\nSyntax: cd [path|..]")]
		public static void Cd(string[] tokens)
		{
			if (tokens[0] == "cd..")
			{
				// Move up one directory level
				var currentDirectory = Directory.GetCurrentDirectory();
				var parentDirectory = Directory.GetParent(currentDirectory);

				if (parentDirectory != null)
				{
					Directory.SetCurrentDirectory(parentDirectory.FullName);
				}
			}
			else if (tokens.Length == 1)
			{
				Console.WriteLine("Current directory: " + Directory.GetCurrentDirectory());
			}
			else if (tokens.Length == 2)
			{

				// Change to specified directory
				var path = tokens[1];
				try
				{
					Directory.SetCurrentDirectory(path);
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex.Message);
				}
			}
			else
			{
				Console.WriteLine("Invalid syntax: cd [path]");
			}
		}

		[Help("Displays messages, or turns command echoing on or off.\nSyntax: echo [on|off|message]")]
		public static void Echo(string[] tokens)
		{
			tokens[0] = string.Empty;
			Console.WriteLine(string.Join(" ", tokens));
		}

		[Help("Creates a new directory.\nSyntax: mkdir directory")]
		public static void Mkdir(string[] tokens)
		{
			if (tokens.Length > 1)
			{
				string path = tokens[1];
				Directory.CreateDirectory(path);
			}
		}

		[Help("Removes a directory.\nSyntax: rmdir directory")]
		public static void Rmdir(string[] tokens)
		{
			if (tokens.Length > 1)
			{
				string path = tokens[1];
				Directory.Delete(path);
			}
		}

		[Help("Copies one or more files to another location.\nSyntax: copy source destination")]
		public static void Copy(string[] tokens)
		{
			if (tokens.Length > 2)
			{
				string source = tokens[1];
				string destination = tokens[2];
				File.Copy(source, destination);
			}
		}

		[Help("Moves one or more files from one directory to another directory.\nSyntax: move source destination")]
		public static void Move(string[] tokens)
		{
			if (tokens.Length > 2)
			{
				string source = tokens[1];
				string destination = tokens[2];
				File.Move(source, destination);
			}
		}

		[Help("Deletes one or more files.\nSyntax: del file [file2 ...]")]
		public static void Del(string[] tokens)
		{
			if (tokens.Length > 1)
			{
				string path = tokens[1];
				File.Delete(path);
			}
		}

		[Help("Displays the contents of a text file.\nSyntax: type file")]
		public static void Type(string[] tokens)
		{
			if (tokens.Length > 1)
			{
				string path = tokens[1];
				string contents = File.ReadAllText(path);
				Console.WriteLine(contents);
			}
		}

		[Help("Clears the screen.\nSyntax: cls")]
		public static void Cls()
		{
			Console.Clear();
		}

		[Help("Displays help information for CMD.NET commands.\nSyntax: help [command]")]
		public static void Help(string[] tokens)
		{
			if (tokens.Length > 1)
			{
				var commandName = char.ToUpper(tokens[1][0]) + tokens[1].Substring(1);
				var method = typeof(Commands).GetMethod(commandName);

				if (method != null)
				{
					Console.WriteLine($"Help for {commandName}:");
					Console.WriteLine(method.GetCustomAttribute<HelpAttribute>()?.Description ?? "No documentation available.");
				}
				else
				{
					Console.WriteLine($"Command not found: {tokens[1]}");
				}
			}
			else
			{
				Console.WriteLine("Cmd.net - A simple CMD clone written in C#");
				Console.WriteLine("Commands:");
				Console.WriteLine("  ASSOC - Displays or modifies file type associations.");
				Console.WriteLine("  ATTRIB - Displays or changes file attributes.");
				Console.WriteLine("  CD - Displays the name of or changes the current directory.");
				Console.WriteLine("  CD.. - Changes the current directory to the parent directory.");
				Console.WriteLine("  CLS - Clears the screen.");
				Console.WriteLine("  COPY - Copies one or more files to another location.");
				Console.WriteLine("  DATE - Displays or sets the date.");
				Console.WriteLine("  DEL - Deletes one or more files.");
				Console.WriteLine("  DIR - Displays a list of files and subdirectories in a directory.");
				Console.WriteLine("  ECHO - Displays messages, or turns command echoing on or off.");
				Console.WriteLine("  EXIT - Quits the CMD.NET program.");
				Console.WriteLine("  FTYPE - Displays or modifies file type associations for specific file types.");
				Console.WriteLine("  HELP - Provides help information for CMD.NET commands.");
				Console.WriteLine("  MD - Creates a new directory.");
				Console.WriteLine("  MORE - Displays the contents of a file one screen at a time.");
				Console.WriteLine("  MOVE - Moves one or more files from one directory to another directory.");
				Console.WriteLine("  PING - Sends an ICMP echo request to a specified computer.");
				Console.WriteLine("  RD - Removes a directory.");
				Console.WriteLine("  RENAME - Renames a file or files.");
				Console.WriteLine("  TIME - Displays or sets the system time.");
				Console.WriteLine("  TREE - Displays the directory structure of a path and its subdirectories.");
				Console.WriteLine("  TYPE - Displays the contents of a file.");
			}
		}

		[Help("Renames a file.\nSyntax: rename oldname newname")]
		public static void Rename(string[] tokens)
		{
			if (tokens.Length > 2)
			{
				string source = Path.Combine(Environment.CurrentDirectory, tokens[1]);
				string destination = Path.Combine(Environment.CurrentDirectory, tokens[2]);
				if (!File.Exists(source) && !Directory.Exists(source))
				{
					Console.WriteLine($"File or directory not found: {source}");
				}
				else if (File.Exists(destination) || Directory.Exists(destination))
				{
					Console.WriteLine($"File or directory already exists: {destination}");
				}
				else
				{
					if (File.Exists(source))
					{
						File.Move(source, destination);
					}
					else if (Directory.Exists(source))
					{
						Directory.Move(source, destination);
					}
				}
			}
		}

		[Help("Displays or changes the attributes of files.\nSyntax: attrib [+R|-R] [+A|-A] [+S|-S] [+H|-H] [file]")]
		public static void Attrib(string[] tokens)
		{
			if (tokens.Length > 1)
			{
				string path = Path.Combine(Environment.CurrentDirectory, tokens[1]);
				if (!File.Exists(path) && !Directory.Exists(path))
				{
					Console.WriteLine($"File or directory not found: {path}");
				}
				else if (tokens.Length == 2)
				{
					// Display attributes
					FileAttributes attributes = File.GetAttributes(path);
					Console.WriteLine($"{attributes}\t{path}");
				}
				else
				{
					// Set attributes
					FileAttributes attributes = File.GetAttributes(path);
					for (int i = 2; i < tokens.Length; i++)
					{
						string token = tokens[i];
						switch (token)
						{
							case "+h":
								attributes |= FileAttributes.Hidden;
								break;

							case "-h":
								attributes &= ~FileAttributes.Hidden;
								break;

							case "+r":
								attributes |= FileAttributes.ReadOnly;
								break;

							case "-r":
								attributes &= ~FileAttributes.ReadOnly;
								break;

							case "+a":
								attributes |= FileAttributes.Archive;
								break;

							case "-a":
								attributes &= ~FileAttributes.Archive;
								break;

							case "+s":
								attributes |= FileAttributes.System;
								break;

							case "-s":
								attributes &= ~FileAttributes.System;
								break;

							default:
								Console.WriteLine($"Invalid attribute: {token}");
								return;
						}
					}

					File.SetAttributes(path, attributes);
				}
			}
		}

		[Help("Graphically displays the folder structure of a drive or path.\nSyntax: tree [drive:][path] [/F]")]
		public static void Tree(string[] tokens)
		{
			string path = ".";
			if (tokens.Length > 1)
			{
				path = tokens[1];
			}

			var directories = new Stack<string>();
			directories.Push(path);

			while (directories.Count > 0)
			{
				var currentDirectory = directories.Pop();
				var subdirectories = Directory.GetDirectories(currentDirectory);
				var indent = new string(' ', currentDirectory.Count(c => c == Path.DirectorySeparatorChar));

				Console.WriteLine(indent + Path.GetFileName(currentDirectory));
				foreach (var subdirectory in subdirectories)
				{
					directories.Push(subdirectory);
				}
			}
		}

		[Help("Displays or modifies file extension associations.\nSyntax: assoc [.extension=[fileType]]")]
		public static void Assoc(string[] tokens)
		{
			if (tokens.Length > 1)
			{
				var fileType = tokens[1];
				var proc = new ProcessStartInfo
				{
					FileName = "cmd",
					Arguments = $"/C assoc {fileType}",
					RedirectStandardOutput = true,
					UseShellExecute = false
				};

				using (var process = Process.Start(proc))
				{
					Console.WriteLine(process?.StandardOutput.ReadToEnd());
				}
			}
			else
			{
				Console.WriteLine("Displays or modifies file type associations.");
				Console.WriteLine("Syntax: ASSOC [.ext=[fileType]]");
			}
		}

		[Help("Displays or modifies file type associations.\nSyntax: ftype [fileType[=[openCommandString]]]]")]
		public static void Ftype(string[] tokens)
		{
			if (tokens.Length > 1)
			{
				var fileType = tokens[1];
				var proc = new ProcessStartInfo
				{
					FileName = "cmd",
					Arguments = $"/C ftype {fileType}",
					RedirectStandardOutput = true,
					UseShellExecute = false
				};

				using (var process = Process.Start(proc))
				{
					Console.WriteLine(process?.StandardOutput.ReadToEnd());
				}
			}
			else
			{
				Console.WriteLine("Displays or modifies file type associations for specific file types.");
				Console.WriteLine("Syntax: FTYPE [fileType=[commandString]]");
			}
		}

		[Help("Creates a new directory.\nSyntax: md directory")]
		public static void Md(string[] tokens)
		{
			if (tokens.Length > 1)
			{
				var path = tokens[1];
				Directory.CreateDirectory(path);
			}
			else
			{
				Console.WriteLine("Creates a new directory.");
				Console.WriteLine("Syntax: MD [drive:][path]dirname");
			}
		}

		[Help("Removes a directory.\nSyntax: rd directory")]
		public static void Rd(string[] tokens)
		{
			if (tokens.Length > 1)
			{
				var path = tokens[1];
				Directory.Delete(path, true);
			}
			else
			{
				Console.WriteLine("Removes a directory.");
				Console.WriteLine("Syntax: RD [/S] [/Q] [drive:]path");
			}
		}

		[Help("Displays output one screen at a time.\nSyntax: more [file]")]
		public static void More(string[] tokens)
		{
			if (tokens.Length > 1)
			{
				var path = tokens[1];
				var lines = File.ReadLines(path);
				var lineNumber = 0;
				var linesPerPage = Console.WindowHeight - 1;

				foreach (var line in lines)
				{
					Console.WriteLine(line);
					lineNumber++;

					if (lineNumber >= linesPerPage)
					{
						Console.Write("Press any key to continue...");
						Console.ReadKey();
						Console.WriteLine();
						lineNumber = 0;
					}
				}
			}
		}

		[Help("Executes a batch file.")]
		public static void Call(string[] tokens)
		{
			if (tokens.Length != 2)
			{
				Console.WriteLine("Invalid syntax: call [batch_file]");
				return;
			}

			var batchFilePath = tokens[1];

			if (!File.Exists(batchFilePath))
			{
				Console.WriteLine("Batch file not found: " + batchFilePath);
				return;
			}

			var batchFileLines = File.ReadAllLines(batchFilePath);
			foreach (var line in batchFileLines)
			{
				// Split each line into tokens and execute the corresponding command
				var lineTokens = line.Split();
				var commandName = lineTokens[0];
				var methodName = char.ToUpper(commandName[0]) + commandName.Substring(1);

				var methods = typeof(Commands).GetMethods(BindingFlags.Static | BindingFlags.Public);

				foreach (var method in methods)
				{
					if (method.Name == methodName)
					{
						var parameters = method.GetParameters();
						var argsToPass = new List<object>();

						for (var i = 0; i < lineTokens.Length; i++)
						{
							var token = lineTokens[i];
							var paramType = parameters.ElementAtOrDefault(i)?.ParameterType ?? typeof(string[]);

							if (paramType.IsAssignableFrom(typeof(string[])))
							{
								argsToPass.Add(lineTokens.Skip(i).ToArray());
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



		[Help("Displays or sets the date.\nSyntax: date [newdate]")]
		public static void Date(string[] tokens)
		{
			if (tokens.Length == 1)
			{
				// Display current date
				Console.WriteLine(DateTime.Now.ToShortDateString());
			}
			else if (tokens.Length == 2)
			{
				// Set date
				string dateString = tokens[1];
				if (DateTime.TryParse(dateString, out DateTime date))
				{
					DateTime currentDate = DateTime.Now;
					DateTime newDateTime = new DateTime(date.Year, date.Month, date.Day, currentDate.Hour, currentDate.Minute, currentDate.Second);
					SystemTime systemTime = new SystemTime
					{
						Year = (ushort)newDateTime.Year,
						Month = (ushort)newDateTime.Month,
						Day = (ushort)newDateTime.Day,
						Hour = (ushort)newDateTime.Hour,
						Minute = (ushort)newDateTime.Minute,
						Second = (ushort)newDateTime.Second
					};
					SetSystemTime(ref systemTime);
				}
				else
				{
					Console.WriteLine($"Invalid date: {dateString}");
				}
			}
		}

		[Help("Displays or sets the time.\nSyntax: time [newtime]")]
		public static void Time(string[] tokens)
		{
			if (tokens.Length == 1)
			{
				// Display current time
				Console.WriteLine(DateTime.Now.ToShortTimeString());
			}
			else if (tokens.Length == 2)
			{
				// Set time
				string timeString = tokens[1];
				if (DateTime.TryParseExact(timeString, "HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime time))
				{
					DateTime currentDate = DateTime.Now;
					DateTime newDateTime = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day, time.Hour, time.Minute, time.Second);
					SystemTime systemTime = new SystemTime
					{
						Year = (ushort)newDateTime.Year,
						Month = (ushort)newDateTime.Month,
						Day = (ushort)newDateTime.Day,
						Hour = (ushort)newDateTime.Hour,
						Minute = (ushort)newDateTime.Minute,
						Second = (ushort)newDateTime.Second
					};
					SetSystemTime(ref systemTime);
				}
				else
				{
					Console.WriteLine($"Invalid time: {timeString}");
				}
			}
		}

		// Helper method to set system time
		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern bool SetSystemTime(ref SystemTime st);

		// Struct representing system time
		[StructLayout(LayoutKind.Sequential)]
		public struct SystemTime
		{
			public ushort Year;
			public ushort Month;
			public ushort DayOfWeek;
			public ushort Day;
			public ushort Hour;
			public ushort Minute;
			public ushort Second;
			public ushort Milliseconds;
		}
	}
}


