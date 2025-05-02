using Core.Helper;
using SkiaSharp.Views.Desktop;

namespace Plot.WinForm;

partial class CanvasControl
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

    #region Component Designer generated code

    /// <summary> 
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {

        this.SuspendLayout();

        if (ChartConfig.USE_GPU)
        {
            _skGLControl = new SKGLControl();
            _skGLControl.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            _skGLControl.Size = new System.Drawing.Size(1000, 1000);
            _skGLControl.TabIndex = 1;
            _skGLControl.PaintSurface += new System.EventHandler<SKPaintGLSurfaceEventArgs>(this.SkglControl_PaintSurface);
            Controls.Add(_skGLControl);
        }
        else
        {
            _skControl = new SKControl();
            _skControl.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            _skControl.Size = new System.Drawing.Size(1000, 1000);
            _skControl.TabIndex = 1;
            _skControl.PaintSurface += new System.EventHandler<SKPaintSurfaceEventArgs>(this.SkControl_PaintSurface);
            Controls.Add(_skControl);

        }

        this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.Name = "Canvas";
        this.ResumeLayout(false);
    }


    #endregion

    private SKControl _skControl;
    private SKGLControl _skGLControl;
}
