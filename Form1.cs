using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenGL;
using System.Runtime.InteropServices; 

namespace myOpenGL
{
    public partial class Form1 : Form
    {
        cOGL cGL;
        bool Firstboot;

        public Form1()
        {
            InitializeComponent();
            cGL = new cOGL(panel1);
            Cursor.Position = new Point(cGL.lastMouseX, Cursor.Position.Y);
            panel1.Focus();
            timer1.Enabled = true;
            Firstboot = true;
            Cursor.Hide();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            cGL.Draw();
            if (Firstboot)
            {
                Firstboot = false;
                ShowInfo();
            }
        }

        private void ShowInfo()
        {
            MessageBox.Show("hi!\nMove with W,A,S,D\nShoot with Mouse1\nPress F for fullscreen toggle\nPress P to change bullet size\nPress U to go undercover!\nPress T to change tank type", "OpenGl-Tank", MessageBoxButtons.OK);
        }

        private void panel1_Resize(object sender, EventArgs e)
        {
            base.OnResize(e);
            cGL.OnResize();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (cGL.oldscreen != cGL.fullscreen)
            {
                if (cGL.fullscreen == true)
                {
                    FormBorderStyle = FormBorderStyle.None;
                    WindowState = FormWindowState.Maximized;
                }
                else
                {
                    FormBorderStyle = FormBorderStyle.Sizable;
                    WindowState = FormWindowState.Normal;
                }
                cGL.oldscreen = cGL.fullscreen;
            }
            label1.Text = cOGL.score.ToString();
            cGL.Draw();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Q)
                Application.Exit();
            cGL.keyDown[(int)e.KeyCode] = true;
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                cGL.leftMouseDown = true;
                cGL.tank.fire();
            }
        }

        private void panel1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                cGL.leftMouseDown = false;
            }
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
           cGL.keyDown[(int)e.KeyCode] = false;
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            panel1.Focus();
            Point point = panel1.PointToClient(Cursor.Position);
            cGL.tank.rotateTurret((float)(0.1 * (cGL.lastMouseX - point.X)));

            cGL.lastMouseX = point.X;
            if (point.X <= 2 || point.X >= this.Width-20)
            {
                Point abc = new Point(0, 0);
                abc.Offset((this.Width - 6)/2, this.Height/2);
                Cursor.Position = PointToScreen(abc);
                cGL.lastMouseX = (this.Width - 6)/2;
            }

        }
       
    }
}