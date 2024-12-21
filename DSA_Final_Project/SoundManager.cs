using DSA_Final_Project;
using System.IO;
using System.Media;
using System.Reflection;

public class SoundManager
{
    public SoundPlayer click;
    public SoundPlayer error;
    public SoundPlayer move;
    public SoundPlayer found;

    public SoundManager()
    {
        // Load sound files from the specified pathh
        string basePath = @"C:\Users\Hassan Shahid\Downloads\DSA_Final_Project (1)\DSA_Final_Project\DSA_Final_Project\Resources";

        click = LoadSound(basePath, "click.wav");
        error = LoadSound(basePath, "error.wav");
        move = LoadSound(basePath, "move.wav");
        found = LoadSound(basePath, "found.wav");
 

    }

    private SoundPlayer LoadSound(string directory, string fileName)
    {
        string filePath = Path.Combine(directory, fileName);
        return new SoundPlayer(filePath);
    }
}
