using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;


/*
The below code was passed through https://cecilifier.me/ It was then modified to pull types from the .net 
framework 3.5 dlls that come with xenonauts and the code was added to program.cs and forms the base of 
the code that patches the xenonauts Assembly-CSharp.dll.

cecilifier.me isn't pefect and i had a number of issues with the code it generated (it didn't seem to like nested loops
for instance). The below code is the result of trial and error to get code that would transform into working mono.cecil code. 
Because of that the code isn't optimised and is written in a strange way. 
 */


interface IMod
{

}

class Foo
{
    void LoadMods()
    {
        //Debug.Log("Load MODS is called has been called");
        string[] directories = Directory.GetDirectories(Environment.ExpandEnvironmentVariables("%USERPROFILE%\\Documents\\Xenonauts 2\\Mods"));
        string[] dirsThatExist = new string[1000];

        for (int i = 0; i < dirsThatExist.Length; i++)
        {
            dirsThatExist[i] = "";
        }

        int count = 0;
        for (int i = 0; i < directories.Length; i++)
        {
            string fileName = Path.GetFileName(directories[i]);
            string path = directories[i] + "\\" + fileName + ".dll";
            if (File.Exists(path))
            {
                dirsThatExist[count] = path;
                count++;
            }
        }

        Type[] mods = new Type[1000];
        count = 0;
        for (int i = 0; i < dirsThatExist.Length; i++)
        {
            if (dirsThatExist[i] == null)
            {
                break;
            }

            Type[] exportedTypes = Assembly.LoadFile(dirsThatExist[i]).GetExportedTypes();

            for (int j = 0; j < exportedTypes.Length; j++)
            {
                if (exportedTypes[j].IsClass && typeof(IMod).IsAssignableFrom(exportedTypes[j]))
                {
                    mods[count] = exportedTypes[j];
                    count++;
                }
            }
        }



        for (int i = 0; i < mods.Length; i++)
        {
            if (mods[i] == null)
            {
                break;
            }

            IMod mod = (IMod)Activator.CreateInstance(mods[i]);
            object obj = Activator.CreateInstance(mods[i]);
            MethodInfo method = mods[i].GetMethod("Initialise");
            method.Invoke(obj, null);
            
        }
    }
}