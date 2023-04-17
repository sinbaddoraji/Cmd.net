using System.Globalization;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Reflection;
using System.Net.NetworkInformation;
using Microsoft.Win32;

namespace Cmd.net
{
	public static class Commands
	{
		[Help("EXIT", "Quits the CMD.EXE program (command interpreter).", "exit")]
		public static void Exit(string[] tokens)
		{
			Environment.Exit(0);
		}

		[Help("DIR", "Displays a list of files and subdirectories in a directory.", "dir [path]")]
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

		[Help("CD", "Displays the name of or changes the current directory.", "cd [path]")]
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

		[Help("ECHO", "Displays messages, or turns command echoing on or off.", "echo [message]")]
		public static void Echo(string[] tokens)
		{
			tokens[0] = string.Empty;
			Console.WriteLine(string.Join(" ", tokens));
		}

		[Help("MKDIR", "Creates a directory.", "mkdir [path]")]
		public static void Mkdir(string[] tokens)
		{
			if (tokens.Length > 1)
			{
				string path = tokens[1];
				Directory.CreateDirectory(path);
			}
		}

		[Help("Rmdir", "Removes a directory. The directory must be empty before it can be removed.", "rmdir [directory]")]
		public static void Rmdir(string[] tokens)
		{
			if (tokens.Length > 1)
			{
				string path = tokens[1];
				Directory.Delete(path);
			}
		}

		[Help("Copy", "Copies one or more files to another location.", "copy [source] [destination]")]
		public static void Copy(string[] tokens)
		{
			if (tokens.Length > 2)
			{
				string source = tokens[1];
				string destination = tokens[2];
				File.Copy(source, destination);
			}
		}

		[Help("Move", "Moves one or more files from one directory to another directory.", "move [source] [destination]")]
		public static void Move(string[] tokens)
		{
			if (tokens.Length > 2)
			{
				string source = tokens[1];
				string destination = tokens[2];
				File.Move(source, destination);
			}
		}

		[Help("Del", "Deletes one or more files.", "del [file1] [file2] ...")]
		public static void Del(string[] tokens)
		{
			if (tokens.Length > 1)
			{
				string path = tokens[1];
				File.Delete(path);
			}
		}

		[Help("Del", "Deletes one or more files.", "del [file1] [file2] ...")]
		public static void Type(string[] tokens)
		{
			if (tokens.Length > 1)
			{
				string path = tokens[1];
				string contents = File.ReadAllText(path);
				Console.WriteLine(contents);
			}
		}

		[Help("Cls", "Clears the console screen.", "cls")]
		public static void Cls()
		{
			Console.Clear();
		}

		[Help("Help", "Displays help information about commands.", "help [command]")]
		public static void Help(string[] tokens)
		{
			if (tokens.Length > 1)
			{
				var commandName = tokens[1];
				var methods = typeof(Commands).GetMethods(BindingFlags.Static | BindingFlags.Public);

				foreach (var method in methods)
				{
					var helpAttribute = method.GetCustomAttribute<HelpAttribute>();

					if (helpAttribute != null && helpAttribute.CommandName == commandName)
					{
						Console.WriteLine(helpAttribute.Description);
						Console.WriteLine($"Syntax: {helpAttribute.Syntax}");
						return;
					}
				}

				Console.WriteLine($"Help topic not found: {commandName}");
			}
			else
			{
				Console.WriteLine("Displays help information for cmd.net commands.");
				Console.WriteLine();
				Console.WriteLine("HELP [command]");
				Console.WriteLine();
				Console.WriteLine("    command - Specifies the name of the command about which you want help.");
				Console.WriteLine();
				Console.WriteLine("Commands:");
				Console.WriteLine();

				var methods = typeof(Commands).GetMethods(BindingFlags.Static | BindingFlags.Public);

				foreach (var method in methods)
				{
					var helpAttribute = method.GetCustomAttribute<HelpAttribute>();

					if (helpAttribute != null)
					{
						Console.WriteLine($"{helpAttribute.CommandName}\t{helpAttribute.Description}");
					}
				}
			}
		}


		[Help("Color", "Sets the console foreground and background colors.", "color [background] [foreground]")]
		public static void Color(string[] tokens)
		{
			if (tokens.Length == 2)
			{
				if (int.TryParse(tokens[1], out int color))
				{
					if (color >= 0 && color <= 15)
					{
						Console.BackgroundColor = (ConsoleColor)color;
						Console.ForegroundColor = ConsoleColor.White;
						Console.Clear();
						return;
					}
				}
			}
			else if (tokens.Length == 1 || (tokens.Length == 2 && tokens[1] == "/?"))
			{
				Console.WriteLine("COLOR [color|/?]\n\n  color         Specifies the color of the console background and foreground. This\n                parameter can be one of the following values:\n\n                0 = Black       8 = Gray\n                1 = Blue        9 = Light Blue\n                2 = Green       A = Light Green\n                3 = Aqua        B = Light Aqua\n                4 = Red         C = Light Red\n                5 = Purple      D = Light Purple\n                6 = Yellow      E = Light Yellow\n                7 = White       F = Bright White\n\n  /?            Displays help at the command prompt.");
			}
			else
			{
				Console.WriteLine("Invalid syntax: COLOR [color|/?]");
			}
		}

		[Help("Comp", "Compares the contents of two files or sets of files.", "comp [file1] [file2]")]
		public static void Comp(string[] tokens)
		{
			if (tokens.Length >= 3 && tokens.Length <= 4)
			{
				string source1 = tokens[1];
				string source2 = tokens[2];
				bool displayDifferences = tokens.Length == 4 && tokens[3] == "/D";

				if (File.Exists(source1) && File.Exists(source2))
				{
					string arguments = $"\"{source1}\" \"{source2}\"{(displayDifferences ? " /d" : "")}";
					Process.Start(new ProcessStartInfo("fc.exe", arguments) { UseShellExecute = false }).WaitForExit();
				}
				else
				{
					Console.WriteLine("File not found.");
				}
			}
			else
			{
				Console.WriteLine("Invalid syntax: COMP [source1] [source2] [/D [text]]");
			}
		}

		[Help("Find", "Searches for a text string in a file or files. The output lists the files that contain the target string.", "find [/v] [/c] [/n] [/i] <string> <filename>")]
		public static void Find(string[] tokens)
		{
			if (tokens.Length < 3)
			{
				Console.WriteLine("Syntax: find \"search string\" file");
				return;
			}

			var searchString = tokens[1];
			var filePath = tokens[2];

			try
			{
				var fileContent = File.ReadAllText(filePath);

				if (fileContent.Contains(searchString))
				{
					Console.WriteLine("Match found in " + filePath);
				}
				else
				{
					Console.WriteLine("Match not found in " + filePath);
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
		}

		[Help("Format", "Formats a disk for use with Windows.", "format drive:")]
		public static void Format(string[] tokens)
		{
			if (tokens.Length != 2)
			{
				Console.WriteLine("Syntax: format drive:");
				return;
			}

			var drive = tokens[1];

			try
			{
				Process.Start(new ProcessStartInfo
				{
					FileName = "diskpart.exe",
					Arguments = $"/s \"{GetScriptPath(drive)}\"",
					UseShellExecute = false,
					RedirectStandardOutput = true,
					CreateNoWindow = true
				});

				Console.WriteLine("Format completed on " + drive);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
		}

		private static string GetScriptPath(string drive)
		{
			var scriptPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.txt");
			var scriptContents = $"select disk {drive[0] - 'A'}\n" +
			                     "clean\n" +
			                     "create partition primary\n" +
			                     "format fs=ntfs quick\n" +
			                     "assign\n" +
			                     "exit\n";

			File.WriteAllText(scriptPath, scriptContents);
			return scriptPath;
		}


		[Help("Label", "Creates, changes, or deletes the volume label of a disk.", "label [drive:] [label]")]
		public static void Label(string[] tokens)
		{
			if (tokens.Length != 3)
			{
				Console.WriteLine("Syntax: label drive: label");
				return;
			}

			var drive = tokens[1];
			var label = tokens[2];

			try
			{
				var d = DriveInfo.GetDrives().First(d => d.Name == drive);
				d.VolumeLabel = label;
				Console.WriteLine("Label set for " + drive);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
		}

		[Help("Ping", "Sends Internet Control Message Protocol (ICMP) echo request packets to the specified host to check if it is reachable. The result will be a list of statistics and the number of packets sent, received, and lost.", "ping [host]")]
		public static void Ping(string[] tokens)
		{
			if (tokens.Length != 2)
			{
				Console.WriteLine("Syntax: ping hostname");
				return;
			}

			var hostname = tokens[1];

			try
			{
				var ping = new Ping();
				var reply = ping.Send(hostname);

				if (reply != null && reply.Status == IPStatus.Success)
				{
					Console.WriteLine("Ping succeeded to " + hostname);
				}
				else
				{
					Console.WriteLine("Ping failed to " + hostname);
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
		}

		[Help("SystemInfo", "Displays detailed information about the operating system and hardware of the computer.", "systeminfo")]
		public static void SystemInfo(string[] tokens)
		{
			var process = new Process();
			process.StartInfo.FileName = "systeminfo";
			process.StartInfo.RedirectStandardOutput = true;
			process.StartInfo.UseShellExecute = false;
			process.Start();
			string output = process.StandardOutput.ReadToEnd();
			process.WaitForExit();

			Console.WriteLine(output);
		}


		[Help("Ver", "Displays the operating system version.", "ver")]
		public static void Ver(string[] tokens)
		{
			Console.WriteLine("Microsoft Windows [Version " + Environment.OSVersion.Version + "]");
		}

		[Help("Vol", "Displays the volume label and serial number of a specified disk.", "vol [drive letter]")]
		public static void Vol(string[] tokens)
		{
			if (tokens.Length != 2)
			{
				Console.WriteLine("Syntax: vol drive:");
				return;
			}

			var drive = tokens[1];

			try
			{
				var volumeLabel = DriveInfo.GetDrives().First(d => d.Name == drive)?.VolumeLabel;
				Console.WriteLine("Volume in drive " + drive + " is " + volumeLabel);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
		}




		[Help("Rename", "Renames a file or directory.", "rename [source] [destination]")]
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

		[Help("Attrib", "Displays or changes file attributes", "attrib [+/-]h +[/-]r +[/-]a +[/-]s [file/directory]")]
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

		[Help("Tree", "Displays the directory structure of a path and all subdirectories recursively.", "tree [path]")]
		public static void Tree(string[] tokens)
		{
			string path = tokens.Length > 1 ? tokens[1] : ".";

			try
			{
				// Display the directory structure recursively
				Console.WriteLine($"{path}");
				var directories = Directory.GetDirectories(path);
				foreach (var directory in directories)
				{
					Console.WriteLine($"|  +--{Path.GetFileName(directory)}");
					DisplayDirectoryStructure(directory, true);
				}
				var files = Directory.GetFiles(path);
				foreach (var file in files)
				{
					Console.WriteLine($"|     {Path.GetFileName(file)}");
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
		}

		private static void DisplayDirectoryStructure(string directoryPath, bool isSubdirectory)
		{
			var directories = Directory.GetDirectories(directoryPath);
			foreach (var directory in directories)
			{
				if (isSubdirectory)
				{
					Console.WriteLine($"|  |  +--{Path.GetFileName(directory)}");
				}
				else
				{
					Console.WriteLine($"|  +--{Path.GetFileName(directory)}");
				}
				DisplayDirectoryStructure(directory, true);
			}
			var files = Directory.GetFiles(directoryPath);
			foreach (var file in files)
			{
				Console.WriteLine($"|  |     {Path.GetFileName(file)}");
			}
		}


		[Help("Assoc", "Displays or modifies file extension associations.", "assoc [.extension[=[fileType]]]")]
		public static void Assoc(string[] tokens)
		{
			if (tokens.Length != 2)
			{
				Console.WriteLine("Syntax: assoc [file extension]");
				return;
			}

			var extension = tokens[1].TrimStart('.');
			var keyName = $@"HKEY_CLASSES_ROOT\.{extension}";
			var defaultValue = (string)Registry.GetValue(keyName, null, null);

			if (defaultValue == null)
			{
				Console.WriteLine($"No association found for '.{extension}'");
				return;
			}

			var commandKeyName = $@"{defaultValue}\shell\open\command";
			var command = (string)Registry.GetValue(commandKeyName, null, null);

			if (command == null)
			{
				Console.WriteLine($"No command found for association '{defaultValue}'");
				return;
			}

			Console.WriteLine($"Association for '.{extension}': {defaultValue}");
			Console.WriteLine($"Command: {command}");
		}


		[Help("Ftype", "Displays or modifies file types used in file extension associations.", "ftype [filetype] [command]")]
		public static void Ftype(string[] tokens)
		{
			if (tokens.Length == 1)
			{
				Console.WriteLine("Displays or modifies file type associations.");

				Console.WriteLine("\nSyntax:");
				Console.WriteLine("\tftype [fileType]\n\tftype fileType=[executablePath] [arguments]");

				Console.WriteLine("\nArguments:");
				Console.WriteLine("\tfileType:\tSpecifies the file type to examine or change association.");
				Console.WriteLine("\texecutablePath:\tSpecifies the executable to be used in opening files of the specified fileType.");
				Console.WriteLine("\targuments:\tSpecifies any arguments to be used when the specified executable is used.");
			}
			else if (tokens.Length == 2)
			{
				string fileType = tokens[1];

				// Get the current executable for the given file type
				string command = $"HKCR\\{fileType}\\shell\\open\\command";
				string defaultExecutable = Registry.GetValue($"HKEY_CLASSES_ROOT\\{fileType}\\shell\\open\\command", "", null)?.ToString();

				if (defaultExecutable == null)
				{
					Console.WriteLine($"File type \"{fileType}\" is not associated with any program.");
				}
				else
				{
					Console.WriteLine($"File type \"{fileType}\" is associated with:");
					Console.WriteLine($"\t{defaultExecutable}");
				}
			}
			else if (tokens.Length > 2 && tokens[1].Contains("="))
			{
				string[] parts = tokens[1].Split('=');
				string fileType = parts[0];
				string executable = parts[1];

				// Save the new file type association
				Registry.SetValue($"HKEY_CLASSES_ROOT\\{fileType}\\shell\\open\\command", "", executable);
				Console.WriteLine($"File type \"{fileType}\" association set to \"{executable}\"");
			}
			else
			{
				Console.WriteLine("Invalid syntax. Type \"ftype\" for help.");
			}
		}

		[Help("Md", "Creates a new directory.", "md [directory name]")]
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

		[Help("Rd", "Removes a directory. The directory must be empty before it can be removed.", "rd [directory]")]
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

		[Help("More", "Displays the contents of a text file one screen at a time.", "more [file]")]
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

		[Help("Call", "Calls another batch file and continues executing the current batch file after the called batch file finishes.", "call [batch_file]")]
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
		

		[Help("Date", "Displays or sets the system date.", "date [mm-dd-yy]")]
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

		[Help("Time", "Displays the current system time and allows you to set the system time.",
			"time /T\n" +
			"time /SET <hh:mm:ss>\n" +
			"time /HELP")]
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


