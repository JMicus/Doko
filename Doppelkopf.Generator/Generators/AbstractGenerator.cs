using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Doppelkopf.Generator.Generators
{
    public abstract class AbstractGenerator
    {
        

        static string PROJECT_DIR = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;

        static string ROOT = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.Parent.FullName;

        protected static string getProjectFile(string relativePath)
        {
            return File.ReadAllText(Path.Combine(PROJECT_DIR, relativePath));
        }

        protected static void writeRootFile(string relativePath, string content)
        {
            File.WriteAllText(Path.Combine(ROOT, relativePath), content);
        }

        
    }
}
