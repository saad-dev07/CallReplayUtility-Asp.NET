using FFMpegCore;
using SevenZipExtractor;
using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace CallBackUtility.Utility
{
    public static class AudioFormatter
    {
        public static bool DecodeAudio(string LogPath,string tarFileName, string AudioFileName, string serverEncodedWavPath, string UserEncodeLocation, string serverDecodeDestination,string ffmpegLocation)
        {
            bool isFormatted = false;
            try
            {
                string tarFilePath = ConfigurationManager.AppSettings["ProductionDataFilesPath"].ToString()  + tarFileName;
                if (!Convert.ToBoolean(ConfigurationManager.AppSettings["isProduction"].ToString()))
                {
                    tarFilePath = ConfigurationManager.AppSettings["LabDataFilesPath"].ToString() + tarFileName;
                }
                if (!File.Exists(UserEncodeLocation+ AudioFileName))
                {
                    using (ArchiveFile zip = new ArchiveFile(tarFilePath))
                    {
                        UserEncodeLocation = UserEncodeLocation.Replace("/", @"\") + AudioFileName;
                        zip.Entries.First(archivefileinfo => archivefileinfo.FileName.Equals(AudioFileName)).Extract(UserEncodeLocation, true);
                        zip.Dispose();
                    } }
                Decode(LogPath,serverEncodedWavPath, serverDecodeDestination + AudioFileName, ffmpegLocation);
                isFormatted = true;
            }
            catch (Exception ex)
            {
                LogsManager.Logs(LogPath,"Archive Error:" + ex.Message);
                LogsManager.Logs(LogPath,"tarFileName: " + tarFileName);
                LogsManager.Logs(LogPath,"UserEncodeLocation: " + UserEncodeLocation);

                isFormatted = false;
            }
            return isFormatted;
        }
    
        public static bool Decode(string LogPath,string fileNameWithPath, string fileToBeDecodedWithPath,string ffmpegLocation)
        {
            bool isConvertted;
            try
            {
                fileNameWithPath = fileNameWithPath.Replace('/','\\');
                fileToBeDecodedWithPath= fileToBeDecodedWithPath.Replace('/', '\\');
                string ffmpeg_Command = "/c \".\\ffmpeg -i "+ fileNameWithPath + " -acodec pcm_s16le -ac 1 -ar 16000 "+ fileToBeDecodedWithPath;
                /// ok path= c ".\ffmpeg -i D:\MyProjects\FBL\CallReplayUtility\Code\Web\CallBackUtility\CallReplayUtility\AudioFiles\e915c1bb-a63e-40aa-8242-ae7d216e5fea\800000000000598.wav -acodec pcm_s16le -ac 1 -ar 16000 D:\MyProjects\FBL\CallReplayUtility\Code\Web\CallBackUtility\CallReplayUtility\AudioFiles\e915c1bb-a63e-40aa-8242-ae7d216e5fea\DecodedAudios\800000000000598.wav
                Process process = new Process();
                process.StartInfo.FileName = "cmd.exe";
                process.StartInfo.WorkingDirectory = ffmpegLocation;
                process.StartInfo.Arguments = ffmpeg_Command;
                process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                process.Start();
                process.WaitForExit();
                process.Dispose();
               
                isConvertted = true;

                //foreach (var _process in Process.GetProcessesByName("ffmpeg"))
                //{
                //    _process.Kill();
                //}
            }
            catch (Exception ex)
            {
                LogsManager.Logs(LogPath,"FFMpegArguments:" + ex.Message);
                isConvertted = false;
            }
            return isConvertted;
        }       
    }
}