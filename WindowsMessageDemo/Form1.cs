using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsMessageDemo
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Process[] processes= Process.GetProcessesByName("WindowsMessageDemo");
            foreach(var process in processes)
            {
                if(process.Id!=Process.GetCurrentProcess().Id)
                {
                    byte[] bytData = null;
                    bytData = Encoding.Unicode.GetBytes(textBox1.Text);

                    COPYDATASTRUCT cdsBuffer;
                    cdsBuffer.dwData = (IntPtr)100;
                    cdsBuffer.cbData = bytData.Length;
                    cdsBuffer.lpData = Marshal.AllocHGlobal(bytData.Length);
                    Marshal.Copy(bytData, 0, cdsBuffer.lpData, bytData.Length);
                    SendMessage(process.MainWindowHandle, WM_COPYDATA, 0, ref cdsBuffer);
                }
            }
        }
        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hwnd, int wMsg, int wParam, ref COPYDATASTRUCT lParam);
        private void Form1_Load(object sender, EventArgs e)
        {
            Process[] process = Process.GetProcessesByName("WindowsMessageDemo");
            this.Text = "Demo" + process.Length;
        }
        /// <summary>  
        /// System defined message  
        /// </summary>  
        private const int WM_COPYDATA = 0x004A;

        /// <summary>  
        /// CopyDataStruct  
        /// </summary>  
        public struct COPYDATASTRUCT
        {
            public IntPtr dwData;
            public int cbData;
            public IntPtr lpData;
        }
        /// <summary>
        /// DefWndProc
        /// </summary>
        /// <param name="m"></param>
        protected override void DefWndProc(ref System.Windows.Forms.Message m)
        {
            switch (m.Msg)
            {
               
                case WM_COPYDATA:
                    COPYDATASTRUCT cds = new COPYDATASTRUCT();
                    cds = (COPYDATASTRUCT)m.GetLParam(typeof(COPYDATASTRUCT));
                    byte[] bytData = new byte[cds.cbData];
                    Marshal.Copy(cds.lpData, bytData, 0, bytData.Length);
                    ProcessIncomingData(bytData);
                    break;



                default:
                    base.DefWndProc(ref m);
                    break;
            }
        }
        /// <summary>
        /// ProcessIncomingData
        /// </summary>
        /// <param name="bytesData"></param>
        private void ProcessIncomingData(byte[] bytesData)
        {
            string str = BitConverter.ToString(bytesData);
            string msg = Encoding.Unicode.GetString(bytesData);
            Console.WriteLine(msg);

            string strRevMsg = msg;
            textBox2.AppendText(strRevMsg + "\r\n");
            string[] args = msg.Split(':');
            int count = 0;
            foreach(var arg in args)
            {
                textBox2.AppendText("arg" + count + "=" + arg + "\r\n");
            }



        }
    }
}
