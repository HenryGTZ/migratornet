using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace migratornet.runner
{
    public class CommandLineArgs : Dictionary<string, string>
    {
        private const string Pattern = @"\/(?<argname>\w+):(?<argvalue>.+)";
        private readonly Regex _regex = new Regex(Pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);

        /// <summary>
        /// Determine if the user pass at least one valid parameter
        /// </summary>
        /// <returns></returns>
        public bool ContainsValidArguments()
        {
            return (this.ContainsKey("cnn"));
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public CommandLineArgs()
        {
            var args = Environment.GetCommandLineArgs();
            foreach (var match in args.Select(arg => _regex.Match(arg)).Where(m => m.Success))
            {
                try
                {
                    this.Add(match.Groups["argname"].Value, match.Groups["argvalue"].Value);
                }
                catch (Exception) { }
            }
        }
    }
}
