using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NewJob.Emergent.InitialCodingChallenge
{
    internal class Program
    {
        static void Main(string[] args)
        {
        }
    }

    public static class CustomCode
    {
        static Regex versionRegex = new Regex(@"(?<major>[0-9]+)((\.)(?<minor>[0-9]+))?((\.)(?<patch>[0-9]+))?((\.)(?<build>[0-9]+))?((\.)(?<compilation>[0-9]+))?");
        
        public static int VersionCompare(string version1, string version2)
        {
            if (string.IsNullOrEmpty(version1) || string.IsNullOrEmpty(version2))
            {
                return -1;
            }

            var matches1 = versionRegex.Match(version1);
            var matches2 = versionRegex.Match(version2);
            if (matches1.Groups.Count > 0 && matches2.Groups.Count > 0)
            {
                var groups1 = matches1.Groups;
                var groups2 = matches2.Groups;

                Version v1 = new Version
                {
                    Major = groups1["major"].Value,
                    Minor = groups1["minor"].Value,
                    Patch = groups1["patch"].Value,
                    Build = groups1["build"].Value,
                    Compilation = groups1["compilation"].Value,
                };
                Version v2 = new Version
                {
                    Major = groups2["major"].Value,
                    Minor = groups2["minor"].Value,
                    Patch = groups2["patch"].Value,
                    Build = groups2["build"].Value,
                    Compilation = groups2["compilation"].Value,
                };

                var levels1 = v1.ToValues();
                var levels2 = v2.ToValues();
                if (levels1.Count != levels2.Count)
                {
                    throw new Exception("Unequal score count");
                }
                for (int i = 0; i < levels1.Count; i++)
                {
                    if (levels1[i] > levels2[i])
                    {
                        return 1;
                    }
                    else if (levels1[i] < levels2[i])
                    {
                        return -1;
                    }
                }
            }
            return 0;
        }
    }

    public class Version
    {
        public string Major { get; set; }
        public string Minor { get; set; }
        public string Patch { get; set; }
        public string Build { get; set; }
        public string Compilation { get; set; }

        public List<int> ToValues()
        {
            var toReturn = new List<int>();
            int.TryParse(Major, out int major);
            int.TryParse(Minor, out int minor);
            int.TryParse(Patch, out int patch);
            int.TryParse(Build, out int build);
            int.TryParse(Compilation, out int comp);

            toReturn.Add(major);
            toReturn.Add(minor);
            toReturn.Add(patch);
            toReturn.Add(build);
            toReturn.Add(comp);
            return toReturn;
        }
    }
}
