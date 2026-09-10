using System.IO.Pipes;
using System.Text.Json;

namespace FrameCtrl
{
    internal static class Program
    {
        public static Config config = new Config();

        public static MultiFormContext context = null;
        public static Mutex mutex = null;
        
        public static ConfigService configService = new ConfigService();
        public static string defaultExtension = ".FrameCtrl";
        public static string progId = "FrameCtrl.App";
        public static string AppGuid = "Global\\FrameCtrl";

        public static SynchronizationContext UiContext { get; private set; }

        public static bool Debug() 
        {
#if DEBUG   
            return true;
#else
            return false;
#endif
        }

        public static void SendArgumentsToRunningInstance(string[] args)
        {
            using (var client = new NamedPipeClientStream(".", AppGuid, PipeDirection.Out))
            {
                try
                {
                    client.Connect(1000);
                    using (var writer = new StreamWriter(client))
                    {
                        string jsonPayload = JsonSerializer.Serialize(args);
                        writer.WriteLine(jsonPayload);
                        writer.Flush();
                    }
                }
                catch (TimeoutException)
                {

                }
            }
        }

        public static void StartInstanceArgumentListener()
        {
            while (true)
            {
                using (var server = new NamedPipeServerStream(AppGuid, PipeDirection.In))
                {
                    server.WaitForConnection();
                    using (var reader = new StreamReader(server))
                    {
                        var receivedData = reader.ReadLine();

                        try
                        {
                            string[] args = JsonSerializer.Deserialize<string[]>(receivedData);
                            try
                            {
                                Program.UiContext.Post(_ =>
                                {
                                    Program.context.CreateNewForm(args, Program.config);
                                }, null);
                            }
                            catch (JsonException)
                            {

                            }
                        }
                        catch (JsonException)
                        {
                        }
                    }
                }
            }
        }

        public static void AssociateAppExtensionInSystem()
        {
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
        }
        public static bool CheckDupliciteRun(string[] args)
        {
            Program.mutex = new Mutex(false, AppGuid);

            if (!Program.mutex.WaitOne(TimeSpan.Zero, true))
            {
                SendArgumentsToRunningInstance(args);
                Program.mutex.ReleaseMutex();
                return true;
            }

            return false;
        }

        public static string[] LoadDebugOptions(string[] args)
        {
            if (args.Length == 0)
            {
                args = new string[] {
                        // @"c:\Documents\Projects\github\apps\FrameCtrl\FrameCtrl\bin\x64\Debug\net9.0-windows7.0\a.mp4"
                    };
            }

            return args;
        }


        [STAThread]
        static void Main(string[] args)
        {

            ApplicationConfiguration.Initialize();

            if (Program.Debug()) {
                args = LoadDebugOptions(args);
            }

            config = configService.Load();

            if (CheckDupliciteRun(args)) {
                return;
            }

            AssociateAppExtensionInSystem();

            context = new MultiFormContext(args, config);

            UiContext = SynchronizationContext.Current ?? new WindowsFormsSynchronizationContext();

            Task.Run(() => StartInstanceArgumentListener());

            Application.Run(context);

            configService.Save(config);

            Program.mutex.ReleaseMutex();
        }
    }
}