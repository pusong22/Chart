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

        if (false)
        {
#if NET6_0_OR_GREATER
                // workaround #250115
                this._skglControl = new SKGLControl();
                this._skglControl.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
                this._skglControl.Size = new System.Drawing.Size(1000, 1000);
                this._skglControl.TabIndex = 1;
                this._skglControl.PaintSurface += new System.EventHandler<SKPaintGLSurfaceEventArgs>(this.SkglControl_PaintSurface);
                this.Controls.Add(this._skglControl);
#else
            throw new PlatformNotSupportedException(
                "GPU rendering is only supported in .NET 6.0 or greater, " +
                "because https://github.com/mono/SkiaSharp/issues/3111 needs to be fixed.");
#endif
        }
        else
        {
            this._skControl = new SKControl();
            this._skControl.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            this._skControl.Size = new System.Drawing.Size(1000, 1000);
            this._skControl.TabIndex = 1;
            this._skControl.PaintSurface += new System.EventHandler<SKPaintSurfaceEventArgs>(this.SkControl_PaintSurface);
            this.Controls.Add(this._skControl);

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
