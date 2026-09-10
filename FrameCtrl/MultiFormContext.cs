using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrameCtrl
{
    public class MultiFormContext : ApplicationContext
    {
        private List<FrameCtrl> forms = new List<FrameCtrl>();

        public MultiFormContext(string[] args, Config config)
        {
            CreateNewForm(args, config);
        }

        public void CreateNewForm(string[] args, Config config)
        {

            string filePath = null;

            if (args.Length > 0)
            {
                filePath = args[0];
            }

            if (forms.Count() == 1 && !forms[0].HasVideoAssigned())
            {
                forms[0].OpenFile(filePath);
                forms[0].BringFormTofront();
                return;
            }

            foreach (FrameCtrl existingForm in forms) {
                if (existingForm.video.VideoPath == filePath) {
                    existingForm.BringFormTofront();
                    return;
                }
            }

            if ((filePath == "" || filePath  == null) && config.videoFilesHistory.Count() > 0) {
                filePath = config.videoFilesHistory[config.videoFilesHistory.Count() - 1].VideoPath;
            }

            FrameCtrl form = new FrameCtrl(config, filePath);
            forms.Add(form);

            form.FormClosed += (s, e) =>
            {
                forms.Remove(form);
                if (forms.Count <= 0)
                {
                    ExitThread();
                }
            };

            form.Show();
        }
    }
}
