using MelonLoader;
using Mono.Cecil;
using System;
using System.IO;
using System.Linq;

[assembly: MelonInfo(typeof(Astrum.AstralScrambler), "AstralScrambler", "0.1.0")]

namespace Astrum
{
    public class AstralScrambler : MelonPlugin
    {
        public const string dataset = "abcdefghijklmnopqrstuvwxyz  ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        private static readonly Random random = new();

        public override void OnApplicationEarlyStart()
        {
            // this has to be converted to an array otherwise it will read the new files while we create them
            string[] files = Directory.EnumerateFiles("Mods", "*.dll").Concat(Directory.EnumerateFiles("Plugins", "*.dll")).ToArray();

            foreach (string file in files)
                if (!file.ToLower().Contains("loader"))
                    RewriteAssembly(file, GenerateString(12, 24, dataset), GenerateString(8, 20, dataset));
        }

        public static void RewriteAssembly(string path, string name, string author)
        {
            AssemblyDefinition asm = AssemblyDefinition.ReadAssembly(path, new ReaderParameters(ReadingMode.Deferred));

            foreach (var attribute in asm.CustomAttributes)
            {
                if (attribute.AttributeType.Name != "MelonInfoAttribute") continue;

                attribute.ConstructorArguments[1] = new(asm.MainModule.ImportReference(typeof(string)), name);
                attribute.ConstructorArguments[3] = new(asm.MainModule.ImportReference(typeof(string)), author);
            }

            asm.Write(Path.GetDirectoryName(path) + "/" + name + ".dll");
            asm.Dispose();
            File.Delete(path);
        }

        public static string GenerateString(int minLength, int maxLength, string dataset)
        {
            string s = "";
            for (int i = 0; i < random.Next(minLength, maxLength); i++)
                s += dataset[random.Next(0, dataset.Length)];
            return s.Trim();
        }
    }
}
