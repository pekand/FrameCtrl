namespace FrameCtrl
{
    internal static class Program
    {
        public static MultiFormContext context = null;
        public static Config config = new Config();
        public static ConfigService configService = new ConfigService();
        public static string defaultExtension = ".FrameCtrl";
        public static string progId = "FrameCtrl.App";

        [STAThread]
        static void Main(string[] args)
        {
            config = configService.Load();

            if (!FileAssociationHelper.IsAlreadyAssociated(defaultExtension, progId))
            {
                try
                {
                    FileAssociationHelper.AssociateExtension(defaultExtension, progId, "FrameCtrl Control Config");
                }
                catch (System.Security.SecurityException)
                {

                }
            }

            string playlistPath = null;

            if (args.Length > 0)
            {
                string filePath = "";

                if (File.Exists(filePath))
                {
                    string extension = Path.GetExtension(filePath);

                    if (extension == defaultExtension)
                    {
                        playlistPath = filePath;
                    }
                    else 
                    {
                        config.videoPath = filePath;
                    }
                    
                }
            }

            ApplicationConfiguration.Initialize();

            context = new MultiFormContext(config, playlistPath);           

            Application.Run(context);

            configService.Save(config);
        }
    }
}