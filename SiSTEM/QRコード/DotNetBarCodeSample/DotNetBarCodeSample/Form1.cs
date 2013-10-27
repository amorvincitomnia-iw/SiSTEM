using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Linq;
using System.Collections.Generic;
using System.Drawing.Imaging;

namespace Csharp
{
	/// <summary>
	/// Form1 の概要の説明です。
	/// </summary>
	public class Form1 : System.Windows.Forms.Form
	{
		/// <summary>
		/// 必要なデザイナ変数です。
		/// </summary>
		private System.ComponentModel.Container components = null;
		internal System.Windows.Forms.TextBox BarNumber;
		internal System.Windows.Forms.Label Label1;
		internal System.Windows.Forms.Button Button2;
		internal System.Windows.Forms.Panel Panel1;
		internal System.Windows.Forms.CheckBox CheckBox3;
		internal System.Windows.Forms.CheckBox CheckBox2;
		internal System.Windows.Forms.CheckBox CheckBox1;
		internal System.Windows.Forms.RadioButton RadioButton3;
		internal System.Windows.Forms.RadioButton RadioButton2;
		internal System.Windows.Forms.RadioButton RadioButton1;
		internal System.Windows.Forms.GroupBox GroupBox3;
		internal System.Windows.Forms.Button Button4;
		private System.Windows.Forms.ColorDialog colorDialog1;
		internal System.Windows.Forms.Button Button5;
		internal System.Windows.Forms.Button Button6;
		private System.Windows.Forms.FontDialog fontDialog1;
        internal RadioButton RadioButton4;


		static public DotNetBarcode bc1= new DotNetBarcode();

		public Form1()
		{
			//
			// Windows フォーム デザイナ サポートに必要です。
			//
			InitializeComponent();

			//
			// TODO: InitializeComponent 呼び出しの後に、コンストラクタ コードを追加してください。
			//
		}

		/// <summary>
		/// 使用されているリソースに後処理を実行します。
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows フォーム デザイナで生成されたコード 
		/// <summary>
		/// デザイナ サポートに必要なメソッドです。このメソッドの内容を
		/// コード エディタで変更しないでください。
		/// </summary>
		private void InitializeComponent()
		{
            this.BarNumber = new System.Windows.Forms.TextBox();
            this.Label1 = new System.Windows.Forms.Label();
            this.Button2 = new System.Windows.Forms.Button();
            this.Panel1 = new System.Windows.Forms.Panel();
            this.CheckBox3 = new System.Windows.Forms.CheckBox();
            this.CheckBox2 = new System.Windows.Forms.CheckBox();
            this.CheckBox1 = new System.Windows.Forms.CheckBox();
            this.RadioButton3 = new System.Windows.Forms.RadioButton();
            this.RadioButton2 = new System.Windows.Forms.RadioButton();
            this.RadioButton1 = new System.Windows.Forms.RadioButton();
            this.GroupBox3 = new System.Windows.Forms.GroupBox();
            this.RadioButton4 = new System.Windows.Forms.RadioButton();
            this.Button4 = new System.Windows.Forms.Button();
            this.colorDialog1 = new System.Windows.Forms.ColorDialog();
            this.Button5 = new System.Windows.Forms.Button();
            this.Button6 = new System.Windows.Forms.Button();
            this.fontDialog1 = new System.Windows.Forms.FontDialog();
            this.GroupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // BarNumber
            // 
            this.BarNumber.Location = new System.Drawing.Point(12, 40);
            this.BarNumber.Multiline = true;
            this.BarNumber.Name = "BarNumber";
            this.BarNumber.Size = new System.Drawing.Size(176, 104);
            this.BarNumber.TabIndex = 4;
            this.BarNumber.Text = "123456789";
            //this.BarNumber.TextChanged += new System.EventHandler(this.BarNumber_TextChanged);
            // 
            // Label1
            // 
            this.Label1.Location = new System.Drawing.Point(8, 16);
            this.Label1.Name = "Label1";
            this.Label1.Size = new System.Drawing.Size(96, 16);
            this.Label1.TabIndex = 5;
            this.Label1.Text = "バーコードを入力";
            // 
            // Button2
            // 
            this.Button2.Location = new System.Drawing.Point(520, 16);
            this.Button2.Name = "Button2";
            this.Button2.Size = new System.Drawing.Size(152, 24);
            this.Button2.TabIndex = 6;
            this.Button2.Text = "バーコードの表示";
            this.Button2.Click += new System.EventHandler(this.Button2_Click);
            // 
            // Panel1
            // 
            this.Panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.Panel1.BackColor = System.Drawing.Color.White;
            this.Panel1.Location = new System.Drawing.Point(24, 168);
            this.Panel1.Name = "Panel1";
            this.Panel1.Size = new System.Drawing.Size(632, 201);
            this.Panel1.TabIndex = 7;
            this.Panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.Panel1_Paint);
            // 
            // CheckBox3
            // 
            this.CheckBox3.Location = new System.Drawing.Point(354, 26);
            this.CheckBox3.Name = "CheckBox3";
            this.CheckBox3.Size = new System.Drawing.Size(160, 24);
            this.CheckBox3.TabIndex = 17;
            this.CheckBox3.Text = "チェックディジットを追加";
            //this.CheckBox3.CheckedChanged += new System.EventHandler(this.CheckBox3_CheckedChanged);
            // 
            // CheckBox2
            // 
            this.CheckBox2.Location = new System.Drawing.Point(354, 74);
            this.CheckBox2.Name = "CheckBox2";
            this.CheckBox2.Size = new System.Drawing.Size(160, 24);
            this.CheckBox2.TabIndex = 16;
            this.CheckBox2.Text = "チェックディジット文字を表示";
            //this.CheckBox2.CheckedChanged += new System.EventHandler(this.CheckBox2_CheckedChanged);
            // 
            // CheckBox1
            // 
            this.CheckBox1.Checked = true;
            this.CheckBox1.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CheckBox1.Location = new System.Drawing.Point(354, 50);
            this.CheckBox1.Name = "CheckBox1";
            this.CheckBox1.Size = new System.Drawing.Size(160, 24);
            this.CheckBox1.TabIndex = 15;
            this.CheckBox1.Text = "文字を表示";
            //this.CheckBox1.CheckedChanged += new System.EventHandler(this.CheckBox1_CheckedChanged);
            // 
            // RadioButton3
            // 
            this.RadioButton3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.RadioButton3.Location = new System.Drawing.Point(16, 54);
            this.RadioButton3.Name = "RadioButton3";
            this.RadioButton3.Size = new System.Drawing.Size(88, 16);
            this.RadioButton3.TabIndex = 20;
            this.RadioButton3.Text = "CODE39";
            this.RadioButton3.CheckedChanged += new System.EventHandler(this.RadioButton3_CheckedChanged);
            // 
            // RadioButton2
            // 
            this.RadioButton2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.RadioButton2.Location = new System.Drawing.Point(16, 34);
            this.RadioButton2.Name = "RadioButton2";
            this.RadioButton2.Size = new System.Drawing.Size(88, 16);
            this.RadioButton2.TabIndex = 19;
            this.RadioButton2.Text = "JAN8";
            this.RadioButton2.CheckedChanged += new System.EventHandler(this.RadioButton2_CheckedChanged);
            // 
            // RadioButton1
            // 
            this.RadioButton1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.RadioButton1.Checked = true;
            this.RadioButton1.Location = new System.Drawing.Point(16, 12);
            this.RadioButton1.Name = "RadioButton1";
            this.RadioButton1.Size = new System.Drawing.Size(88, 16);
            this.RadioButton1.TabIndex = 18;
            this.RadioButton1.TabStop = true;
            this.RadioButton1.Text = "JAN13";
            this.RadioButton1.CheckedChanged += new System.EventHandler(this.RadioButton1_CheckedChanged);
            // 
            // GroupBox3
            // 
            this.GroupBox3.Controls.Add(this.RadioButton4);
            this.GroupBox3.Controls.Add(this.RadioButton1);
            this.GroupBox3.Controls.Add(this.RadioButton2);
            this.GroupBox3.Controls.Add(this.RadioButton3);
            this.GroupBox3.Location = new System.Drawing.Point(528, 50);
            this.GroupBox3.Name = "GroupBox3";
            this.GroupBox3.Size = new System.Drawing.Size(128, 101);
            this.GroupBox3.TabIndex = 21;
            this.GroupBox3.TabStop = false;
            this.GroupBox3.Text = "タイプ";
            // 
            // RadioButton4
            // 
            this.RadioButton4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.RadioButton4.Location = new System.Drawing.Point(16, 76);
            this.RadioButton4.Name = "RadioButton4";
            this.RadioButton4.Size = new System.Drawing.Size(88, 16);
            this.RadioButton4.TabIndex = 25;
            this.RadioButton4.Text = "QRCode";
            // 
            // Button4
            // 
            this.Button4.Location = new System.Drawing.Point(220, 26);
            this.Button4.Name = "Button4";
            this.Button4.Size = new System.Drawing.Size(112, 24);
            this.Button4.TabIndex = 22;
            this.Button4.Text = "バーコードの色";
            //this.Button4.Click += new System.EventHandler(this.Button4_Click);
            // 
            // Button5
            // 
            this.Button5.Location = new System.Drawing.Point(220, 64);
            this.Button5.Name = "Button5";
            this.Button5.Size = new System.Drawing.Size(112, 24);
            this.Button5.TabIndex = 23;
            this.Button5.Text = "バーコードの背景色";
            //this.Button5.Click += new System.EventHandler(this.Button5_Click);
            // 
            // Button6
            // 
            this.Button6.Location = new System.Drawing.Point(220, 104);
            this.Button6.Name = "Button6";
            this.Button6.Size = new System.Drawing.Size(112, 24);
            this.Button6.TabIndex = 24;
            this.Button6.Text = "フォントの色";
            //this.Button6.Click += new System.EventHandler(this.Button6_Click);
            // 
            // Form1
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 12);
            this.ClientSize = new System.Drawing.Size(680, 378);
            this.Controls.Add(this.Button6);
            this.Controls.Add(this.Button5);
            this.Controls.Add(this.Button4);
            this.Controls.Add(this.GroupBox3);
            this.Controls.Add(this.CheckBox3);
            this.Controls.Add(this.CheckBox2);
            this.Controls.Add(this.CheckBox1);
            this.Controls.Add(this.Panel1);
            this.Controls.Add(this.Button2);
            this.Controls.Add(this.BarNumber);
            this.Controls.Add(this.Label1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.GroupBox3.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		/// <summary>
		/// アプリケーションのメイン エントリ ポイントです。
		/// </summary>
		[STAThread]




		static void Main() 
		{

			Application.Run(new Form1());
		}


		private void Button2_Click(object sender, System.EventArgs e)
		{
			Panel1.Refresh();
		}

		private void Panel1_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
            Single x1;
            Single y1;
            Single x2;
            Single y2;
            String BarcodeNumber;
            x1 = 0;
            y1 = 0;
            x2 = this.Panel1.Size.Width;
            y2 = this.Panel1.Size.Height;
            BarcodeNumber = this.BarNumber.Text;
            /*if (this.RadioButton1.Checked == true)
                bc1.Type = DotNetBarcode.Types.Jan13;
			if (this.RadioButton2.Checked == true)
				bc1.Type = DotNetBarcode.Types.Jan8;
			if (this.RadioButton3.Checked == true)
				bc1.Type = DotNetBarcode.Types.Code39;
            if (this.RadioButton4.Checked == true)*/
                //bc1.Type = DotNetBarcode.Types.QRCode;
            /*
                bc1.BackGroundColor = Color.Azure;
                bc1.QRBackColorBigMark1 = Color.PaleVioletRed;
                bc1.QRBackColorBigMark2 = Color.PapayaWhip;
                bc1.QRBackColorBigMark3 = Color.PeachPuff;
                bc1.QRBackColorFormatInformation = Color.Gray;
                bc1.QRBackColorSmallMark = Color.Ivory;
                bc1.QRBackColorTimingPattern = Color.Lavender;
                bc1.QRBackColorVersionInformation = Color.LightBlue;
                //bc1.QRBackGroundFileName = @"C:\Users\Public\Pictures\Sample Pictures\Tulips.jpg";
                bc1.QRColorBigMark = Color.Blue;
                bc1.QRColorBigMark2 = Color.Green;
                bc1.QRColorBigMark3 = Color.Red;
                bc1.QRColorBigMarkBorder1= Color.Purple;
                bc1.QRColorBigMarkBorder2 = Color.Black;
                bc1.QRColorBigMarkBorder3 = Color.Brown;
                bc1.QRColorFormatInformation = Color.DarkGray;
                bc1.QRColorQuitZone = Color.MintCream;
                bc1.QRColorSmallMark = Color.DarkCyan;
                bc1.QRColorTimingPattern = Color.DarkViolet;
                bc1.QRColorVersionInformation = Color.Firebrick;
            */
            bc1.QRSetECCRate = DotNetBarcode.QRECCRates.Low7Percent;
            byte[] buf=new byte[2500];
            (new Random()).NextBytes(buf);
            //buf = Enumerable.Repeat<byte>(65, 1000).ToArray();
            string str = Convert.ToBase64String(buf);
            using (Bitmap bmap=bc1.Get1pxBitmap(buf))
            {
                //var imgattr=new ImageAttributes();
                e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                e.Graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
                e.Graphics.SmoothingMode= System.Drawing.Drawing2D.SmoothingMode.None;
                e.Graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;
                e.Graphics.DrawImage(bmap,getrect(10,bmap.Width,Panel1.ClientSize));
            }

		}
        private static Rectangle getrect(int margin, int imagesize, Size windowsize)
        {
            int scaleX = (windowsize.Width - margin - margin) / imagesize;
            int scaleY = (windowsize.Height - margin - margin) / imagesize;
            int scale = Math.Min(scaleX, scaleY);
            return new Rectangle((windowsize.Width - imagesize * scale) / 2, (windowsize.Height - imagesize * scale) / 2,
                imagesize * scale, imagesize * scale);
        }
        
		private void Form1_Load(object sender, System.EventArgs e)
		{
		
		}

        //ウィンドウの大きさが変わったときに再表示をする。
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            Panel1.Refresh();
        }

        //private void CheckBox3_CheckedChanged(object sender, System.EventArgs e)
        //{
        //    //if (this.CheckBox3.Checked == true)
        //    //{
        //    //    bc1.AddChechDigit = true;
        //    //}
        //    //else
        //    //{
        //    //    bc1.AddChechDigit = false;
        //    //}
        //    //Panel1.Refresh();

        //}

        //private void CheckBox1_CheckedChanged(object sender, System.EventArgs e)
        //{
        //    //if (this.CheckBox1.Checked == true)
        //    //{
        //    //    bc1.PrintChar = true;
        //    //}
        //    //else
        //    //{
        //    //    bc1.PrintChar = false;
        //    //}
        //    //Panel1.Refresh();
        //}

        //private void CheckBox2_CheckedChanged(object sender, System.EventArgs e)
        //{
        //    //if (this.CheckBox2.Checked == true)
        //    //{
        //    //    bc1.PrintCheckDigitChar = true;
        //    //}
        //    //else
        //    //{
        //    //    bc1.PrintCheckDigitChar = false;
        //    //}
        //    //Panel1.Refresh();
        //}

		private void RadioButton1_CheckedChanged(object sender, System.EventArgs e)
		{
			   Panel1.Refresh();
		}

		private void RadioButton2_CheckedChanged(object sender, System.EventArgs e)
		{
			Panel1.Refresh();
		}

		private void RadioButton3_CheckedChanged(object sender, System.EventArgs e)
		{
			Panel1.Refresh();
		}

        //private void Button4_Click(object sender, System.EventArgs e)
        //{
        //    //colorDialog1.Color = bc1.BarColor;
        //    //if (colorDialog1.ShowDialog() == DialogResult.OK)
        //    //    bc1.BarColor = colorDialog1.Color;
        //    //Panel1.Refresh();
        //}

        //private void Button5_Click(object sender, System.EventArgs e)
        //{
        //    //colorDialog1.Color = bc1.BackGroundColor;
        //    //if (colorDialog1.ShowDialog() == DialogResult.OK)
        //    //    bc1.BackGroundColor = colorDialog1.Color;
        //    //Panel1.Refresh();
        //}

        //private void Button6_Click(object sender, System.EventArgs e)
        //{
        //    //fontDialog1.ShowColor = true;
        //    //if (fontDialog1.ShowDialog() == DialogResult.OK)
        //    //{
        //    //    bc1.FontColor = fontDialog1.Color;
        //    //    bc1.FontName = fontDialog1.Font.Name;
        //    //    bc1.FontSize = fontDialog1.Font.Size;
        //    //    bc1.FontBold = fontDialog1.Font.Bold;
        //    //    bc1.FontItalic = fontDialog1.Font.Italic;
        //    //}
        //    //Panel1.Refresh();
        //}

        //private void BarNumber_TextChanged(object sender, EventArgs e)
        //{
        //    //Panel1.Refresh();

        //}

	}
}
