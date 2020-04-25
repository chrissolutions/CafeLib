using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using AtmAgentChequeUpload.Controller;

namespace AtmAgent
{
    public partial class DebugForm<T> : Form where T : class, IServiceController
    {
        private T _serviceRunner;

        public DebugForm()
        {
            InitializeComponent();
            SetButtonState(true);
        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            Task.Run(async () =>
            {
                try
                {
                    _serviceRunner = ServiceBootstrap.Bootstrap<T>().GetResolver().Resolve<T>();
                    await _serviceRunner.Start();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.InnerException?.Message ?? ex.Message);
                }
            });

            SetButtonState(false);
        }

        private void StopButton_Click(object sender, EventArgs e)
        {
            Task.Run(async () => await _serviceRunner.Stop());
            SetButtonState(true);
        }

        private void SetButtonState(bool state)
        {
            StartButton.Enabled = state;
            StopButton.Enabled = !state;
        }
    }
}