using System.Xml;

namespace DownloadManagerInstaller
{
    public partial class UpdateForm : Form
    {
        public UpdateForm()
        {
            InitializeComponent();
        }

        //Disable close button
        private const int CP_DISABLE_CLOSE_BUTTON = 0x200;
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ClassStyle = cp.ClassStyle | CP_DISABLE_CLOSE_BUTTON;
                return cp;
            }
        }

        private void UpdateForm_Load(object sender, EventArgs e)
        {
            this.Show();
            DownloadProgress xmlProgress = new DownloadProgress("https://raw.githubusercontent.com/Soniczac7/app-update/main/DownloadManager.xml", System.IO.Path.GetTempPath(), "");
            xmlProgress.ShowDialog();
            XmlDocument xml = new XmlDocument();
            xml.Load(System.IO.Path.GetTempPath() + "DownloadManager.xml");
            foreach (XmlNode node in xml.DocumentElement.ChildNodes)
            {
                foreach (XmlNode locNode in node)
                {
                    if (locNode.Name == "url")
                    {
                        string loc = locNode.InnerText;
                        MessageBox.Show(loc);
                    }
                }
            }
        }
    }
}