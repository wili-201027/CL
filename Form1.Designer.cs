using System.Drawing;
using System.Windows.Forms;

namespace Terminal
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

    protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
                components.Dispose();

            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            Output = new RichTextBox();
            inputTextBox = new TextBox();
            LastCommand = new RichTextBox();
            HeaderPanel.SuspendLayout();
            BodyPanel.SuspendLayout();
            MainContainer.SuspendLayout();
            SuspendLayout();
            // 
            // BtnMaximize
            // 
            BtnMaximize.FlatAppearance.BorderSize = 0;
            // 
            // BtnMinimize
            // 
            BtnMinimize.FlatAppearance.BorderSize = 0;
            // 
            // LblTitle
            // 
            LblTitle.Size = new Size(71, 16);
            LblTitle.Text = "TERMINAL";
            // 
            // MainContainer
            // 
            MainContainer.Controls.Add(Output);
            MainContainer.Controls.Add(inputTextBox);
            MainContainer.Controls.Add(LastCommand);
            // 
            // Output
            // 
            Output.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            Output.BackColor = Color.FromArgb(6, 32, 64);
            Output.BorderStyle = BorderStyle.None;
            Output.Font = new Font("Gunship", 8.5F);
            Output.ForeColor = Color.FromArgb(45, 180, 200);
            Output.Location = new Point(20, 200);
            Output.Name = "Output";
            Output.ReadOnly = true;
            Output.ScrollBars = RichTextBoxScrollBars.Vertical;
            Output.Size = new Size(510, 260);
            Output.TabIndex = 0;
            Output.Text = "";
            // 
            // inputTextBox
            // 
            inputTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            inputTextBox.BackColor = Color.FromArgb(6, 32, 64);
            inputTextBox.BorderStyle = BorderStyle.None;
            inputTextBox.Font = new Font("Gunship", 10F, FontStyle.Bold);
            inputTextBox.ForeColor = Color.FromArgb(80, 220, 255);
            inputTextBox.Location = new Point(205, 108);
            inputTextBox.Name = "inputTextBox";
            inputTextBox.Size = new Size(305, 14);
            inputTextBox.TabIndex = 1;
            inputTextBox.KeyDown += inputTextBox_KeyDown;
            inputTextBox.PreviewKeyDown += inputTextBox_PreviewKeyDown;
            // 
            // LastCommand
            // 
            LastCommand.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            LastCommand.BackColor = Color.FromArgb(6, 32, 64);
            LastCommand.BorderStyle = BorderStyle.None;
            LastCommand.Font = new Font("Gunship", 8.5F);
            LastCommand.ForeColor = Color.FromArgb(45, 180, 200);
            LastCommand.Location = new Point(205, 143);
            LastCommand.Name = "LastCommand";
            LastCommand.ReadOnly = true;
            LastCommand.ScrollBars = RichTextBoxScrollBars.None;
            LastCommand.Size = new Size(305, 40);
            LastCommand.TabIndex = 2;
            LastCommand.Text = "";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(900, 550);
            Name = "Form1";
            Text = "TERMINAL";
            HeaderPanel.ResumeLayout(false);
            HeaderPanel.PerformLayout();
            BodyPanel.ResumeLayout(false);
            MainContainer.ResumeLayout(false);
            MainContainer.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private RichTextBox Output;
    private TextBox inputTextBox;
    private RichTextBox LastCommand;
    }

}
