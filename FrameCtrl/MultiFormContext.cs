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

        public MultiFormContext(Config config, string playlistPath = null)
        {
            CreateNewForm(config, playlistPath);
        }

        public void CreateNewForm(Config config, string playlistPath = null)
        {
            FrameCtrl form = new FrameCtrl(config, playlistPath);
            forms.Add(form);

            form.FormClosed += (s, e) =>
            {

                if (forms.Count <= 0)
                {
                    ExitThread();
                }
            };

            form.Show();
        }
    }
}
