using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// TODO - Try to refactor to something less heavy

namespace Aoc22
{
    enum Commands
    { 
        CD,
        LS
    }

    class TerminalDirectory
    {
        public string name;
        public int size;
        List<TerminalDirectory> Subdirectories;
        List<TerminalFile> files;
        public TerminalDirectory? parent;

        public TerminalDirectory(string name, TerminalDirectory? parent = null) 
        {
            this.name = name;
            size = -1;
            this.parent = parent;
            Subdirectories = new();
            files = new();
        }

        public void AddDir(string name, TerminalDirectory parent)
        {
            Subdirectories.Add(new TerminalDirectory(name, parent));
        }

        public void AddFile(string name, int size)
        {
            files.Add(new TerminalFile(name, size));
        }

        public TerminalDirectory? DirByName(string name)
        {
            return Subdirectories.FirstOrDefault(x => x.name == name);
        }


        public int GetSize()
        { 
            if(size == -1)
                size =  Subdirectories.Select(x=> x.GetSize()).Sum() + files.Select(x=>x.size).Sum();
            return size;
        }

        public List<TerminalDirectory> GetFlatList()
        {
            var result = new List<TerminalDirectory>(Subdirectories);
            var subResults = Subdirectories.Select(x => x.GetFlatList());
            foreach(var subResult in subResults)
                result.AddRange(subResult);

            return result;
        }
    }

    class TerminalFile
    {
        string name;
        public int size;

        public TerminalFile(string name, int size)
        {
            this.name = name;
            this.size = size;
        }
    }

    class TerminalCommand
    {
        public Commands command;
        public string arg = "";
    }

    class TerminalLsEntry
    {
        public string name = "";
        public int size;
        public bool isDir;
    }

    internal class TerminalParser
    {
        List<TerminalDirectory> fileSystem;
        public List<TerminalDirectory> flatListDir;
        int totalSpace = 70000000;
        public int availableSpace = -1;



        public TerminalParser()
        {
            flatListDir = new();
            fileSystem = new();
        }

        public void ParseCommands(List<string> terminalOutput)
        {
            TerminalDirectory? current = null;

            fileSystem = new();

            TerminalDirectory root = new("root");
            fileSystem.Add(root);
            bool lsMode = false;
            current = root;

            for (int i=1; i<terminalOutput.Count; i++) 
            {
                if (current == null)
                    throw new Exception("Something went wrong - current dir is null ");

                var entry = terminalOutput[i];
                if (LineIsCommand(entry))
                {
                    if (lsMode)
                        lsMode = false;

                    var cmd = ParseCommand(entry);
                    if (cmd.command == Commands.LS)
                        lsMode = true;
                    if (cmd.command == Commands.CD)
                    {
                        TerminalDirectory? terminalDirectory = null;
                        terminalDirectory = cmd.arg switch
                        {
                            "/" => root,
                            ".." => current.parent,
                            _ => current.DirByName(cmd.arg),
                        };
                        current = terminalDirectory;
                    }
                }
                else if(lsMode)
                {
                    var lsItem = ParseLsEntry(entry);
                    if (lsItem.isDir)
                        current.AddDir(lsItem.name, current);
                    else
                        current.AddFile(lsItem.name, lsItem.size);
                }
            }

            root.GetSize();
            flatListDir = root.GetFlatList();
            availableSpace = totalSpace - root.size;
        }

        bool LineIsCommand(string line)
        { return line.IndexOf("$") != -1; }

        TerminalCommand ParseCommand(string line)
        {
            TerminalCommand result = new();
            string cmd = line.Substring(2, 2).ToLower();

            result.command = cmd switch
            {
                "cd" => Commands.CD,
                "ls" => Commands.LS,
                _ => throw new ArgumentException("Invalid command in terminal :" + cmd),
            };

            if(result.command == Commands.CD) 
                result.arg = line.Substring(5, line.Length-5).ToLower();

            return result;
        }

        TerminalLsEntry ParseLsEntry(string line)
        {
            TerminalLsEntry entry = new();
            var elements = line.Split(" ", StringSplitOptions.RemoveEmptyEntries);
            
            entry.isDir = (line.StartsWith("dir")) ? true : false;
            entry.name = elements[1];
            entry.size = (entry.isDir) ? -1 : int.Parse(elements[0]);

            return entry;
        }

    }
}
