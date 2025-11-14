using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.IO;

public class PostBuildCopyFile
{
    [PostProcessBuild(1)] // The number defines the order of execution if you have multiple scripts
    public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
    {
        // Only run this for Windows builds
        if (target == BuildTarget.StandaloneWindows || target == BuildTarget.StandaloneWindows64)
        {
            // Get the path to the built game's executable
            string buildRootPath = Path.GetDirectoryName(pathToBuiltProject);
            
            // The source file is inside the build's StreamingAssets folder
            string sourceFile = Path.Combine(buildRootPath, "StreamingAssets", "Credit.txt");
            
            // The destination is the same folder as the .exe file
            string destFile = Path.Combine(buildRootPath, "Credit.txt");

            // Check if the source file exists in the build before trying to copy it
            if (File.Exists(sourceFile))
            {
                File.Copy(sourceFile, destFile, true); // The 'true' allows it to overwrite an existing file
                Debug.Log($"Successfully copied Credit.txt to build root: {buildRootPath}");
            }
            else
            {
                Debug.LogWarning($"Could not find Credit.txt in StreamingAssets at path: {sourceFile}. Copy operation skipped.");
            }
        }
    }
}