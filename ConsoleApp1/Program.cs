using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            var globalFolders = new List<string>();
            addFolders("d:\\", globalFolders); //This is the initial folder
            string destination = "d:\\sorted\\";  //Using a hardcoded destination folder for now, would have to check for a backslash at the end of the string too
            Console.WriteLine("Globals: ");
            foreach (string folder in globalFolders)
            {
                Console.WriteLine(folder);
            }
            string[] fileEntries;
            Shell32.Shell shl = new Shell32.Shell();
            Console.WriteLine("Begin");
            Console.WriteLine("\n");
            
            foreach (string folder in globalFolders)
            {
                //Console.WriteLine(">> Folder: " + folder);
                //Console.WriteLine("\n");
                fileEntries = Directory.GetFiles(folder, "*.mp3");   //GetFiles(folder, "*.mp3");             
                foreach (string file in fileEntries)
                {
                    //Console.WriteLine("File: " + file);
                    int lastslash = file.LastIndexOf("\\");
                    string foldername = file.Substring(0, lastslash + 1);
                    string filename = file.Substring(lastslash + 1);
                    var fldr = shl.NameSpace(Path.GetDirectoryName(file)); //need to ignore recycle bin or this gets an exception // Put some consistent names!
                    var itm = fldr.ParseName(Path.GetFileName(file));
                    var propValue = fldr.GetDetailsOf(itm, 27); //Gets the duration of any media
                    //Console.WriteLine("Folder: " + foldername);
                    //Console.WriteLine("Filename: " + filename); //This should print out the file name only
                    //Console.WriteLine("TEST: \"" + propValue + "\"");
                    //Console.WriteLine("\n");
                    Console.WriteLine("Destination: "+ destination+"\\"+ filename); //int firstslash = file.IndexOf("\\");
                                                                                    //File.Copy(file, folder+"\\new\\"+filename); //This is the old main operation

                    try
                    {
                        File.Copy(file, destination + "\\" + filename); //This copies the file already in the destination folder after the operation... 
                    }
                    catch (DirectoryNotFoundException e) // Programs shouldn't have to rely on exception handling
                    {
                        Console.WriteLine("Creating folder: " + destination);
                        System.IO.Directory.CreateDirectory(destination);
                        File.Copy(file, destination + "\\" + filename); 
                    }
                    catch (IOException e)
                    {
                        Console.WriteLine("File " + filename + " already exists."); //Find a way to exclude the destination
                    }
                    //Need to catch DirectoryNotFoundException and IOException
                    // Instead try directoryinfo structure object - More efficient than strings, currently a resource hog
                    // Or create directory if doesn't exist
         
                }

                //Console.WriteLine("\n");
            }
            Console.WriteLine("DONE");
            Console.ReadKey();
        }

        //Recursively builds a folder list. This is horribly inefficient because it counts every folder, regardless of the type of file it contains

        public static void addFolders(string folderitem, List<string> global){ 
            string[] folderEntries;
            try
            {
                folderEntries = Directory.GetDirectories(folderitem); //Need exceptions for file access
                Console.WriteLine("Adding folder to list: " + folderitem);
                if (!folderitem.Contains("$Recycle.Bin")&&!folderitem.Contains("AppData")&&!folderitem.Contains("\\Windows\\")) //Exceptions occur in another method, so no point including them in the global folder list
                    global.Add(folderitem);
                else
                    Console.WriteLine("IGNORING FOLDER");
                foreach (string folder in folderEntries)
                {
                    addFolders(folder, global);
                }
            }
            catch (UnauthorizedAccessException e)
            {
                Console.WriteLine("Access denied: " + folderitem);
            }
 

            //return returnGlobal;  <-- Might have to return the global if it's passing down copies like in C++
        }
    }
}
