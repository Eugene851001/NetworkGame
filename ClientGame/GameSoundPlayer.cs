using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Media;

namespace ClientGame
{
    class GameSoundPlayer
    {
        Dictionary<string, SoundPlayer> sounds = new Dictionary<string, SoundPlayer>();

        public bool LoadSound(string soundName, string fileName)
        {
            FileStream soundFile;
            try
            {
                soundFile = new FileStream(fileName, FileMode.Open);
            }
            catch
            {
                return false;
            }
            MemoryStream soundContainer = new MemoryStream();
            soundFile.CopyTo(soundContainer);
            soundFile.Close();
            soundContainer.Position = 0;
            sounds.Add(soundName, new SoundPlayer(fileName));
            return true;
        }

        public void PlaySound(string soundName, bool isLooping)
        {
            if (sounds.ContainsKey(soundName))
            {
                if (isLooping)
                    sounds[soundName].PlayLooping();
                else
                    sounds[soundName].Play();
            }
        }

        public void StopSound(string soundName)
        {
            if(sounds.ContainsKey(soundName))
            {
                sounds[soundName].Stop();
            }
        }

    }
}
