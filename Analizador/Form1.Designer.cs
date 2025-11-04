namespace Analizador
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            txtRuta = new TextBox();
            btnAbrir = new Button();
            btnAnalizar = new Button();
            rtbSalida = new RichTextBox();
            openFileDialog1 = new OpenFileDialog();
            SuspendLayout();
            // 
            // txtRuta
            // 
            txtRuta.Location = new Point(64, 44);
            txtRuta.Name = "txtRuta";
            txtRuta.Size = new Size(250, 27);
            txtRuta.TabIndex = 0;
            // 
            // btnAbrir
            // 
            btnAbrir.Location = new Point(354, 44);
            btnAbrir.Name = "btnAbrir";
            btnAbrir.Size = new Size(94, 29);
            btnAbrir.TabIndex = 1;
            btnAbrir.Text = "Abrir";
            btnAbrir.UseVisualStyleBackColor = true;
            btnAbrir.Click += btnAbrir_Click;
            // 
            // btnAnalizar
            // 
            btnAnalizar.Location = new Point(478, 44);
            btnAnalizar.Name = "btnAnalizar";
            btnAnalizar.Size = new Size(94, 29);
            btnAnalizar.TabIndex = 2;
            btnAnalizar.Text = "Analizar";
            btnAnalizar.UseVisualStyleBackColor = true;
            btnAnalizar.Click += btnAnalizar_Click;
            // 
            // rtbSalida
            // 
            rtbSalida.Font = new Font("Consolas", 10.2F, FontStyle.Regular, GraphicsUnit.Point, 0);
            rtbSalida.Location = new Point(64, 115);
            rtbSalida.Name = "rtbSalida";
            rtbSalida.Size = new Size(679, 318);
            rtbSalida.TabIndex = 3;
            rtbSalida.Text = "";
            // 
            // openFileDialog1
            // 
            openFileDialog1.FileName = "openFileDialog1";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(824, 501);
            Controls.Add(rtbSalida);
            Controls.Add(btnAnalizar);
            Controls.Add(btnAbrir);
            Controls.Add(txtRuta);
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            Name = "Form1";
            Text = "Form1";
            ResumeLayout(false);
            PerformLayout();
        }


        #endregion

        private TextBox txtRuta;
        private Button btnAbrir;
        private Button btnAnalizar;
        private RichTextBox rtbSalida;
        private OpenFileDialog openFileDialog1;
    }
}
