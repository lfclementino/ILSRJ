namespace Transito
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.btAtualizar = new System.Windows.Forms.Button();
            this.Lista = new System.Windows.Forms.ListBox();
            this.Http = new System.Windows.Forms.WebBrowser();
            this.Verifica = new System.Windows.Forms.Timer(this.components);
            this.cbLigar = new System.Windows.Forms.CheckBox();
            this.Controle = new System.Windows.Forms.Timer(this.components);
            this.cbPostar = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // btAtualizar
            // 
            this.btAtualizar.Location = new System.Drawing.Point(12, 12);
            this.btAtualizar.Name = "btAtualizar";
            this.btAtualizar.Size = new System.Drawing.Size(155, 39);
            this.btAtualizar.TabIndex = 0;
            this.btAtualizar.Text = "Atualizar";
            this.btAtualizar.UseVisualStyleBackColor = true;
            this.btAtualizar.Click += new System.EventHandler(this.button1_Click);
            // 
            // Lista
            // 
            this.Lista.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.Lista.FormattingEnabled = true;
            this.Lista.HorizontalScrollbar = true;
            this.Lista.Location = new System.Drawing.Point(12, 61);
            this.Lista.Name = "Lista";
            this.Lista.Size = new System.Drawing.Size(601, 121);
            this.Lista.TabIndex = 1;
            // 
            // Http
            // 
            this.Http.AllowWebBrowserDrop = false;
            this.Http.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Http.Location = new System.Drawing.Point(396, 3);
            this.Http.MinimumSize = new System.Drawing.Size(20, 20);
            this.Http.Name = "Http";
            this.Http.ScriptErrorsSuppressed = true;
            this.Http.Size = new System.Drawing.Size(217, 48);
            this.Http.TabIndex = 2;
            this.Http.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.Http_DocumentCompleted);
            // 
            // Verifica
            // 
            this.Verifica.Interval = 300000;
            this.Verifica.Tick += new System.EventHandler(this.Verifica_Tick);
            // 
            // cbLigar
            // 
            this.cbLigar.AutoSize = true;
            this.cbLigar.Location = new System.Drawing.Point(193, 24);
            this.cbLigar.Name = "cbLigar";
            this.cbLigar.Size = new System.Drawing.Size(49, 17);
            this.cbLigar.TabIndex = 5;
            this.cbLigar.Text = "Ligar";
            this.cbLigar.UseVisualStyleBackColor = true;
            this.cbLigar.CheckedChanged += new System.EventHandler(this.cbLigar_CheckedChanged);
            // 
            // Controle
            // 
            this.Controle.Interval = 25000;
            this.Controle.Tick += new System.EventHandler(this.Controle_Tick);
            // 
            // cbPostar
            // 
            this.cbPostar.AutoSize = true;
            this.cbPostar.Checked = true;
            this.cbPostar.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbPostar.Location = new System.Drawing.Point(248, 24);
            this.cbPostar.Name = "cbPostar";
            this.cbPostar.Size = new System.Drawing.Size(56, 17);
            this.cbPostar.TabIndex = 14;
            this.cbPostar.Text = "Postar";
            this.cbPostar.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(625, 195);
            this.Controls.Add(this.cbPostar);
            this.Controls.Add(this.cbLigar);
            this.Controls.Add(this.Http);
            this.Controls.Add(this.Lista);
            this.Controls.Add(this.btAtualizar);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "Transito ";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btAtualizar;
        private System.Windows.Forms.ListBox Lista;
        private System.Windows.Forms.WebBrowser Http;
        private System.Windows.Forms.Timer Verifica;
        private System.Windows.Forms.CheckBox cbLigar;
        private System.Windows.Forms.Timer Controle;
        private System.Windows.Forms.CheckBox cbPostar;
    }
}

