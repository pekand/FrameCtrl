namespace FrameCtrl
{
    internal static class Program
    {
        public static FrameCtrl frameCtrl = null;
        public static Config config = new Config();
        public static ConfigService configService = new ConfigService();

        [STAThread]
        static void Main(string[] args)
        {
            config = configService.Load();

            string videoPath = null;
            string playlistPath = null;

            if (args.Length > 0)
            {
                string filePath = "";

                if (File.Exists(filePath))
                {
                    string extension = Path.GetExtension(filePath);

                    if (extension.ToLower() == "playlist")
                    {
                        playlistPath = filePath;
                    }
                    else 
                    {
                        videoPath = filePath;
                    }
                    
                }
            }

            ApplicationConfiguration.Initialize();
            frameCtrl = new FrameCtrl(videoPath, playlistPath);            
            Application.Run(frameCtrl);

            configService.Save(config);
        }
    }
}