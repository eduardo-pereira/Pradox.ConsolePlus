using Pradox.ConsolePlus.Core;
using Pradox.ConsolePlus.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Pradox.ConsolePlus
{
    public static class ConPlus
    {
        private static string _promptSymbol = "$ ";
        private static IEnumerable<Type> _modules;
        private static IEnumerable<CommandModuleInfo> _moduleInfos;

        public static void Initialize(string applicationTitle, string promptSymbol)
        {
            applicationTitle = applicationTitle.Trim();
            if (!string.IsNullOrEmpty(applicationTitle))
            {
                Console.Title = applicationTitle;
            }

            if (!string.IsNullOrEmpty(promptSymbol))
            {
                _promptSymbol = promptSymbol;
            }

            _modules = GetModules();
            _moduleInfos = GetModuleInfos(_modules);
        }

        public static void ExecuteCommand()
        {
            var textCommand = ReadTextCommand();

            ExecuteCommand(textCommand);
        }

        public static void ExecuteCommand(string textCommand)
        {
            textCommand = textCommand.Trim();

            if (string.IsNullOrEmpty(textCommand))
            {
                return;
            }

            var args = GetArgsFromText(textCommand);

            if (args[0].Trim() == "?")
            {
                ShowModules();

                return;
            }

            if (args[0].Trim() == "cls")
            {
                Console.Clear();

                return;
            }

            if (args[0].Trim() == "quit")
            {
                Environment.Exit(0);

                return;
            }

            var commandPipe = GetCommandPipe(args);

            object currentResult = null;
            for (int i = 0; i < commandPipe.Count; i++)
            {
                var command = commandPipe[i];
                var commandToExecute = GetCommandToExecute(command.Args[0]);
                if (commandToExecute == null)
                {
                    return;
                }

                currentResult = ExecuteMethod(currentResult, commandToExecute, command.Args);
            }

            if (currentResult != null)
            {
                if (typeof(IEnumerable<string>).IsAssignableFrom(currentResult.GetType()))
                {
                    var printCommand = GetCommandToExecute("print.write");

                    ExecuteMethod(currentResult, printCommand, new string[] { });

                    return;
                }

                Write("{0}", currentResult.ToString());
            }
        }

        private static void ShowModules()
        {
            ShowModules(_moduleInfos);
        }

        private static void ShowModules(IEnumerable<CommandModuleInfo> _moduleInfos)
        {
            Write("Modules:");
            foreach (var item in _moduleInfos)
            {
                Write("{0} - {1}", item.Alias, item.Description);

                //foreach (var method in item.Methods)
                //{
                //    Write("\t{0} - {1}", method.Alias, method.Description);
                //}
            }
        }

        private static object ExecuteMethod(object previousResult, MethodInfo method, string[] args)
        {
            object instance;

            if (previousResult != null)
            {
                var argsToUse = args.Skip(1).Select(x => (object)x).ToList();

                argsToUse.Insert(0, previousResult);

                instance = Activator.CreateInstance(method.DeclaringType);

                return method.Invoke(instance, argsToUse.ToArray());

                //return method.Invoke(instance, new object[] { previousResult });                
            }

            var parameters = method.GetParameters();

            var methodArgs = args.Skip(1).ToArray();

            if (parameters.Length != methodArgs.Length)
            {
                Write(MessageType.Warning, "Invalid arguments");

                ShowMethodHelp(parameters);

                return null;
            }

            instance = Activator.CreateInstance(method.DeclaringType);

            //return method.Invoke(instance, args.Skip(1).Select(x => (object)int.Parse(x)).ToArray());

            var convertedParams = new List<object>();
            for (int i = 0; i < parameters.Length; i++)
            {
                var param = parameters[i];
                convertedParams.Add(Convert.ChangeType(methodArgs[i], param.ParameterType));
            }

            return method.Invoke(instance, convertedParams.ToArray());
            //return method.Invoke(instance, args.Skip(1).Select(x => (object)x).ToArray());
        }

        private static void ShowMethodHelp(ParameterInfo[] parameters)
        {
            Write("Arguments:");

            foreach (var item in parameters)
            {
                var paramLine = GetParamLine(item);
                Write("\t{0}", paramLine);
            }
        }

        private static string GetParamLine(ParameterInfo item)
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.AppendFormat("{0} [{1}]", item.Name, item.ParameterType);

            if (item.IsOptional)
            {
                stringBuilder.AppendFormat(" (optional) Default: [{0}]", item.DefaultValue);
            }

            return stringBuilder.ToString();
        }

        private static MethodInfo GetCommandToExecute(string parameter)
        {
            var cleanParameter = GetCleanParameter(parameter);

            if (string.IsNullOrEmpty(cleanParameter))
            {
                return null;
            }

            var moduleAlias = string.Empty;
            var commandAlias = string.Empty;

            var parts = cleanParameter.Split('.');
            if (parts.Length == 1)
            {
                commandAlias = parts[0];
            }
            else
            {
                moduleAlias = parts[0];
                commandAlias = parts[1];
            }

            if (commandAlias == "?")
            {
                ShowModuleHelp(moduleAlias);

                return null;
            }

            var command = GetCommand(moduleAlias, commandAlias);

            return command;
        }

        private static string GetCleanParameter(string parameter)
        {
            var pos = 0;

            if (parameter.StartsWith("-") || parameter.StartsWith("/"))
            {
                pos = 1;
            }

            return parameter.Substring(pos).Trim();
        }

        private static MethodInfo GetCommand(string moduleAlias, string commandAlias)
        {
            var module = _moduleInfos.FirstOrDefault(x => string.Compare(x.Alias, moduleAlias, StringComparison.InvariantCultureIgnoreCase) == 0);

            if (module == null)
            {
                return null;
            }

            var command = module.Methods.FirstOrDefault(x => string.Compare(x.Alias, commandAlias, StringComparison.InvariantCultureIgnoreCase) == 0);

            return command?.Method;
        }

        private static void ShowModuleHelp(string moduleAlias)
        {
            var module = _moduleInfos.FirstOrDefault(x => string.Compare(x.Alias, moduleAlias, StringComparison.InvariantCultureIgnoreCase) == 0);

            if (module == null)
            {
                Write(MessageType.Warning, "Invalid module {0}", moduleAlias);
                return;
            }

            Write("Module: {0} - {1}", module.Alias, module.Description);
            Write("Commands:");

            foreach (var item in module.Methods)
            {
                Write("{0} - {1}", item.Alias, item.Description);
            }
        }

        private static IEnumerable<CommandModuleInfo> GetModuleInfos(IEnumerable<Type> _modules)
        {
            var result = ReflectModuleInfos(_modules);

            FillMethodsInModules(result);

            return result;
        }

        private static void FillMethodsInModules(IEnumerable<CommandModuleInfo> modules)
        {
            foreach (var item in modules)
            {
                FillMethods(item);
            }
        }

        private static void FillMethods(CommandModuleInfo item)
        {
            var methods = item.ClassType.GetMethods()
                .Select(x => new { Method = x, Attr = (CommandMethodAttribute)x.GetCustomAttribute(typeof(CommandMethodAttribute), false) })
                .Where(x => x.Attr != null);

            foreach (var method in methods)
            {
                var methodInfo = new CommandMethodInfo
                {
                    Alias = method.Attr.Alias,
                    Description = method.Attr.Description,
                    Method = method.Method
                };

                item.Methods.Add(methodInfo);
            }
        }

        private static IEnumerable<CommandModuleInfo> ReflectModuleInfos(IEnumerable<Type> _modules)
        {
            var result = new List<CommandModuleInfo>();

            foreach (var item in _modules)
            {
                var attribute = Helpers.AttributeHelper.GetAttribute(item);

                if (attribute == null)
                {
                    continue;
                }

                var info = new CommandModuleInfo
                {
                    ClassType = item,
                    Alias = attribute.Alias,
                    Description = attribute.Description
                };

                result.Add(info);
            }

            return result;
        }

        private static List<CommandLineInfo> GetCommandPipe(string[] args)
        {
            var result = new List<CommandLineInfo>();

            var currentList = new List<string>();

            foreach (var item in args)
            {
                if (item == "|")
                {
                    if (currentList.Count > 0)
                    {
                        result.Add(new CommandLineInfo(currentList.ToArray()));

                        currentList = new List<string>();
                    }

                    continue;
                }

                currentList.Add(item);
            }

            if (currentList.Count > 0)
            {
                result.Add(new CommandLineInfo(currentList.ToArray()));

                currentList = new List<string>();
            }

            return result;
        }

        private static IEnumerable<Type> GetModules()
        {
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => GetTypesWithCommandModuleAttribute(x, typeof(CommandModuleAttribute)));

            return types;
        }

        static IEnumerable<Type> GetTypesWithCommandModuleAttribute(Assembly assembly, Type attributeType)
        {
            foreach (Type type in assembly.GetTypes())
            {
                if (type.GetCustomAttributes(attributeType, true).Length > 0)
                {
                    yield return type;
                }
            }
        }

        private static string[] GetArgsFromText(string commandLine)
        {
            StringBuilder argsBuilder = new StringBuilder(commandLine);
            bool inQuote = false;

            // Convert the spaces to a newline sign so we can split at newline later on
            // Only convert spaces which are outside the boundries of quoted text
            for (int i = 0; i < argsBuilder.Length; i++)
            {
                if (argsBuilder[i].Equals('"'))
                {
                    inQuote = !inQuote;
                }

                if (argsBuilder[i].Equals(' ') && !inQuote)
                {
                    argsBuilder[i] = '\n';
                }
            }

            // Split to args array
            string[] args = argsBuilder.ToString().Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

            // Clean the '"' signs from the args as needed.
            for (int i = 0; i < args.Length; i++)
            {
                args[i] = ClearQuotes(args[i]);
            }

            return args;
        }

        private static string ClearQuotes(string stringWithQuotes)
        {
            var quote = '"';
            int quoteIndex;
            if ((quoteIndex = stringWithQuotes.IndexOf(quote)) == -1)
            {
                // String is without quotes..
                return stringWithQuotes;
            }

            if (stringWithQuotes.StartsWith(quote.ToString()) && stringWithQuotes.EndsWith(quote.ToString()))
            {
                stringWithQuotes = stringWithQuotes.Substring(1, stringWithQuotes.Length - 2);

                stringWithQuotes = stringWithQuotes.Replace("\\\"", "\"");
            }

            return stringWithQuotes;

            // Linear sb scan is faster than string assignemnt if quote count is 2 or more (=always)
            StringBuilder sb = new StringBuilder(stringWithQuotes);
            for (int i = quoteIndex; i < sb.Length; i++)
            {
                if (sb[i].Equals('"'))
                {
                    // If we are not at the last index and the next one is '"', we need to jump one to preserve one
                    if (i != sb.Length - 1 && sb[i + 1].Equals('"'))
                    {
                        i++;
                    }

                    // We remove and then set index one backwards.
                    // This is because the remove itself is going to shift everything left by 1.
                    sb.Remove(i--, 1);
                }
            }

            return sb.ToString();
        }

        public static string ReadTextCommand()
        {
            ShowPrompt();

            return Console.ReadLine();
        }

        public static void Write(MessageType type, string format, params object[] args)
        {
            ChangeColor(type);

            Console.WriteLine(string.Format(format, args));

            ResetColor();
        }

        private static void Write(string format, params object[] items)
        {
            Console.WriteLine(string.Format(format, items));
        }

        private static void ChangeColor(MessageType type)
        {
            switch (type)
            {
                case MessageType.Info:
                    break;
                case MessageType.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case MessageType.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                default:
                    break;
            }
        }

        private static void ResetColor()
        {
            Console.ResetColor();
        }

        private static void ShowPrompt()
        {
            Console.Write(_promptSymbol);
        }
    }
}
