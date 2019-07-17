using System;
using System.Collections.Generic;
using System.Windows.Forms;
using myOpenGL;
using System.Drawing;

namespace OpenGL
{
    class cOGL
    {
        Control p;
        int Width;
        int Height;
        public myOpenGL.Tank tank = new myOpenGL.Tank(0, 0, 0, 1);
        public float[] ScrollValue = new float[10];
        public float zShift = 0.0f;
        public float yShift = 0.0f;
        public float xShift = 0.0f;
        public float zAngle = 0.0f;
        public float yAngle = 0.0f;
        public float xAngle = 0.0f;
        public int intOptionC = 0;
        float zavit = 0;
        public static float worldsize = 25;
        double[] AccumulatedRotationsTraslations = new double[16];
        public bool[] keyDown = new bool[256];
        public bool leftMouseDown = false;
        public int lastMouseX = 970/2;
        public static GLUquadric obj;
        public bool fullscreen = false;
        public bool oldscreen = false;
        public bool stelthmode = false;
        public static Target target = new Target();
        public static int score = 0;

        public cOGL(Control pb)
        {
            p=pb;
            Width = p.Width;
            Height = p.Height; 
            InitializeGL();
            Bullet.bulletsize = 0.06f;
            obj = GLU.gluNewQuadric(); //!!!
        }

        ~cOGL()
        {
            GLU.gluDeleteQuadric(obj); //!!!
            WGL.wglDeleteContext(m_uint_RC);
        }

		uint m_uint_HWND = 0;

        public uint HWND
		{
			get{ return m_uint_HWND; }
		}
		
        uint m_uint_DC   = 0;

        public uint DC
		{
			get{ return m_uint_DC;}
		}
		uint m_uint_RC   = 0;

        public uint RC
		{
			get{ return m_uint_RC; }
		}

        void DrawBullets()
        {
            if (tank.bullets != null)
            {
                for (int i = 0; i < tank.bullets.Count; i++)
                {
                    if (!tank.bullets[i].boom)
                        tank.bullets[i].drawSelf();
                    else
                        tank.bullets[i].DrawBoom();
                }
            }
        }

        void DrawSky(float skysize)
        {
            float mid = skysize;
            GL.glEnable(GL.GL_TEXTURE_2D);
            GL.glBindTexture(GL.GL_TEXTURE_2D, Textures[6]);
            GL.glDisable(GL.GL_LIGHTING);

            GL.glTranslatef(0, 0, -mid);
            GL.glRotatef(90, 0,0 , 1);
            GL.glRotatef(zavit, 1, 0, 0);
            GLU.gluQuadricTexture(obj, 1);
            GLU.gluSphere(obj, skysize*1.5f, 50, 50);
            GL.glRotatef(-zavit, 1, 0, 0);
            GL.glRotatef(-90, 0, 0, 1);
            GL.glTranslatef(0, 0, mid);

            GL.glEnable(GL.GL_LIGHTING);
            GL.glDisable(GL.GL_TEXTURE_2D);
            zavit += 0.02f;
        }

        void DrawGrid(float gridsize)
        {
            //floor texture
            GL.glEnable(GL.GL_TEXTURE_2D);

            GL.glBindTexture(GL.GL_TEXTURE_2D, Textures[0]);
            GL.glBegin(GL.GL_QUADS);
            GL.glColor3f(1f,1f,1f);//white clear
            //left side

            GL.glNormal3f(0.0f, 1.0f, 0.0f);
            GL.glTexCoord2f(0, 0);
            GL.glVertex3f(0, 0, 0);
            GL.glTexCoord2f(0, 1);
            GL.glVertex3f(-gridsize, 0, 0);
            GL.glTexCoord2f(1f, 1f);
            GL.glVertex3f(-gridsize, 0, -gridsize * 2);
            GL.glTexCoord2f(1f, 0);
            GL.glVertex3f(0, 0, -gridsize * 2);
            //right side
            GL.glNormal3f(0.0f, 1.0f, 0.0f);
            GL.glTexCoord2f(0, 0);
            GL.glVertex3f(0, 0, 0);
            GL.glTexCoord2f(0, 1);
            GL.glVertex3f(gridsize, 0, 0);
            GL.glTexCoord2f(1f, 1f);
            GL.glVertex3f(gridsize, 0, -gridsize * 2);
            GL.glTexCoord2f(1f, 0);
            GL.glVertex3f(0, 0, -gridsize * 2);

            GL.glEnd();
            GL.glDisable(GL.GL_TEXTURE_2D);
            
        }
        public void DrawAxes()
        {
            GL.glPushMatrix();    
            GL.glDisable(GL.GL_LIGHTING);
            GL.glBegin(GL.GL_LINES);
            //x  RED
            GL.glColor3f(1.0f, 0.0f, 0.0f);
            GL.glVertex3f(0.0f, 0.0f, 0.0f);
            GL.glVertex3f(3.0f, 0.0f, 0.0f);
            //y  GREEN 
            GL.glColor3f(0.0f, 1.0f, 0.0f);
            GL.glVertex3f(0.0f, 0.0f, 0.0f);
            GL.glVertex3f(0.0f, 3.0f, 0.0f);
            //z  BLUE
            GL.glColor3f(0.0f, 0.0f, 1.0f);
            GL.glVertex3f(0.0f, 0.0f, 0.0f);
            GL.glVertex3f(0.0f, 0.0f, 3.0f);
            GL.glEnd();
            GL.glEnable(GL.GL_LIGHTING);
            GL.glPopMatrix();
        }

        void DrawAll()
        {
            DrawGrid(worldsize); //floor
            DrawSky(worldsize); //sky
        }
        public void Draw()
        {
            GL.glClear(GL.GL_COLOR_BUFFER_BIT | GL.GL_DEPTH_BUFFER_BIT | GL.GL_STENCIL_BUFFER_BIT);
            GL.glMatrixMode(GL.GL_MODELVIEW);
            GL.glLoadIdentity();

            GL.glTranslatef(0.0f, -1.5f, -6.0f);
            GL.glRotatef(10, 1.0f, 0.0f, 0.0f);
            GL.glRotatef(-tank.rotation, 0.0f, 1.0f, 0.0f);
            GL.glRotatef(-tank.turretRotation, 0.0f, 1.0f, 0.0f);
            GL.glTranslatef(-tank.posX, 0.0f, -tank.posZ);

            //DrawAxes();
            DrawAll();

            //target
            target.Drawself();

            //LIGHT - before transforms
            //  hence it is in const position
            GL.glPushMatrix();
            GL.glEnable(GL.GL_LIGHTING);
            GL.glEnable(GL.GL_LIGHT0);
                GL.glTranslatef(0, 11, -35);
                float[] ambient = { 0, 0, 0.3f, 1 };
                if (tank.TankType == 2)
                {
                    ambient[2] = 0.2f;
                }
                else
                {
                    ambient[0] = 0.2f;
                }
                float[] diffuse = { 1, 1, 1, 1 };
                float[] specular = { 0.5f, 0.5f, 0.5f, 1f };
                float[] pos = { 0, 1f, -0.5f, 0 };
                GL.glLightfv(GL.GL_LIGHT0, GL.GL_AMBIENT, ambient);
                GL.glLightfv(GL.GL_LIGHT0, GL.GL_DIFFUSE, diffuse);
                GL.glLightfv(GL.GL_LIGHT0, GL.GL_SPECULAR, specular);
                GL.glLightfv(GL.GL_LIGHT0, GL.GL_POSITION, pos);
                
                GL.glColor3f(1, 1, 0);
                GL.glDisable(GL.GL_LIGHTING);
                GLU.gluSphere(obj, 1, 12, 12);//SUN
                GL.glEnable(GL.GL_LIGHTING);
            GL.glPopMatrix();

            if (!stelthmode)
            {
                GL.glEnable(GL.GL_BLEND);
                GL.glBlendFunc(GL.GL_SRC_ALPHA, GL.GL_ONE_MINUS_SRC_ALPHA);
                GL.glEnable(GL.GL_STENCIL_TEST);
                GL.glStencilOp(GL.GL_REPLACE, GL.GL_REPLACE, GL.GL_REPLACE);
                GL.glStencilFunc(GL.GL_ALWAYS, 1, 0xFFFFFFFF); // draw floor always
                GL.glColorMask((byte)GL.GL_FALSE, (byte)GL.GL_FALSE, (byte)GL.GL_FALSE, (byte)GL.GL_FALSE);
                GL.glDisable(GL.GL_DEPTH_TEST);
                DrawMirror(false);
                // restore regular settings
                GL.glColorMask((byte)GL.GL_TRUE, (byte)GL.GL_TRUE, (byte)GL.GL_TRUE, (byte)GL.GL_TRUE);
                GL.glEnable(GL.GL_DEPTH_TEST);

                // reflection is drawn only where STENCIL buffer value equal to 1
                GL.glStencilFunc(GL.GL_EQUAL, 1, 0xFFFFFFFF);
                GL.glStencilOp(GL.GL_KEEP, GL.GL_KEEP, GL.GL_KEEP);

                GL.glEnable(GL.GL_STENCIL_TEST);
                // draw reflected scene
                GL.glPushMatrix();
                GL.glScalef(-1, 1, 1); //swap on Z axis
                GL.glEnable(GL.GL_CULL_FACE);
                GL.glCullFace(GL.GL_BACK);
                tank.drawSelf();
                DrawBullets();
                GL.glCullFace(GL.GL_FRONT);
                tank.drawSelf();
                DrawBullets();
                GL.glDisable(GL.GL_CULL_FACE);
                GL.glPopMatrix();

                GL.glDepthMask((byte)GL.GL_FALSE);
                DrawMirror(false);
                GL.glDepthMask((byte)GL.GL_TRUE);
                // Disable GL.GL_STENCIL_TEST to show All, else it will be cut on GL.GL_STENCIL
                GL.glDisable(GL.GL_STENCIL_TEST);
            }
            else
            {
                GL.glEnable(GL.GL_BLEND);
                GL.glBlendFunc(GL.GL_SRC_ALPHA, GL.GL_ONE_MINUS_SRC_ALPHA);
                GL.glEnable(GL.GL_STENCIL_TEST);
                GL.glStencilOp(GL.GL_REPLACE, GL.GL_REPLACE, GL.GL_REPLACE);
                GL.glStencilFunc(GL.GL_ALWAYS, 1, 0xFFFFFFFF); // draw floor always
                GL.glColorMask((byte)GL.GL_FALSE, (byte)GL.GL_FALSE, (byte)GL.GL_FALSE, (byte)GL.GL_FALSE);
                GL.glDisable(GL.GL_DEPTH_TEST);
                DrawMirror(false);
                // restore regular settings
                GL.glColorMask((byte)GL.GL_TRUE, (byte)GL.GL_TRUE, (byte)GL.GL_TRUE, (byte)GL.GL_TRUE);
                GL.glEnable(GL.GL_DEPTH_TEST);

                // reflection is drawn only where STENCIL buffer value equal to 1
                GL.glStencilFunc(GL.GL_EQUAL, 1, 0xFFFFFFFF);
                GL.glStencilOp(GL.GL_KEEP, GL.GL_KEEP, GL.GL_KEEP);

                GL.glEnable(GL.GL_STENCIL_TEST);
                // draw reflected scene
                GL.glPushMatrix();
                GL.glScalef(-1, 1, 1); //swap on Z axis
                GL.glEnable(GL.GL_CULL_FACE);
                GL.glCullFace(GL.GL_BACK);
                DrawBullets();
                GL.glCullFace(GL.GL_FRONT);
                DrawBullets();
                GL.glDisable(GL.GL_CULL_FACE);
                GL.glPopMatrix();

                GL.glDepthMask((byte)GL.GL_FALSE);
                DrawMirror(false);
                GL.glDepthMask((byte)GL.GL_TRUE);
                // Disable GL.GL_STENCIL_TEST to show All, else it will be cut on GL.GL_STENCIL
                GL.glDisable(GL.GL_STENCIL_TEST);
            }

            if (!stelthmode)
            {
                tank.drawSelf();
            }
            else
            {
                tank.drawstelth();
            }

            DrawMirror(true);
            DrawBullets();
            //REFLECTION e    
            
            update();
            WGL.wglSwapBuffers(m_uint_DC);
        }

        protected virtual void InitializeGL()
        {
            m_uint_HWND = (uint)p.Handle.ToInt32();
            m_uint_DC = WGL.GetDC(m_uint_HWND);

            // Not doing the following WGL.wglSwapBuffers() on the DC will
            // result in a failure to subsequently create the RC.
            WGL.wglSwapBuffers(m_uint_DC);

            WGL.PIXELFORMATDESCRIPTOR pfd = new WGL.PIXELFORMATDESCRIPTOR();
            WGL.ZeroPixelDescriptor(ref pfd);
            pfd.nVersion = 1;
            pfd.dwFlags = (WGL.PFD_DRAW_TO_WINDOW | WGL.PFD_SUPPORT_OPENGL | WGL.PFD_DOUBLEBUFFER);
            pfd.iPixelType = (byte)(WGL.PFD_TYPE_RGBA);
            pfd.cColorBits = 32;
            pfd.cDepthBits = 32;
            pfd.iLayerType = (byte)(WGL.PFD_MAIN_PLANE);

            int pixelFormatIndex = 0;
            pixelFormatIndex = WGL.ChoosePixelFormat(m_uint_DC, ref pfd);
            if (pixelFormatIndex == 0)
            {
                MessageBox.Show("Unable to retrieve pixel format");
                return;
            }

            if (WGL.SetPixelFormat(m_uint_DC, pixelFormatIndex, ref pfd) == 0)
            {
                MessageBox.Show("Unable to set pixel format");
                return;
            }
            //Create rendering context
            m_uint_RC = WGL.wglCreateContext(m_uint_DC);
            if (m_uint_RC == 0)
            {
                MessageBox.Show("Unable to get rendering context");
                return;
            }
            if (WGL.wglMakeCurrent(m_uint_DC, m_uint_RC) == 0)
            {
                MessageBox.Show("Unable to make rendering context current");
                return;
            }


            initRenderingGL();
        }

        public void OnResize()
        {
            Width = p.Width;
            Height = p.Height;
            GL.glViewport(0, 0, Width, Height);
            Draw();
        }

        protected virtual void initRenderingGL()
        {
            if (m_uint_DC == 0 || m_uint_RC == 0)
                return;
            if (this.Width == 0 || this.Height == 0)
                return;

            GL.glShadeModel(GL.GL_SMOOTH);
            GL.glClearColor(0.0f, 0.0f, 0.0f, 0.5f);
            GL.glClearDepth(1.0f);


            GL.glEnable(GL.GL_LIGHT0);
            GL.glEnable(GL.GL_COLOR_MATERIAL);
            GL.glColorMaterial(GL.GL_FRONT_AND_BACK, GL.GL_AMBIENT_AND_DIFFUSE);

            GL.glEnable(GL.GL_DEPTH_TEST);
            GL.glDepthFunc(GL.GL_LEQUAL);
            GL.glHint(GL.GL_PERSPECTIVE_CORRECTION_Hint, GL.GL_NICEST);


            GL.glViewport(0, 0, this.Width, this.Height);
            GL.glMatrixMode(GL.GL_PROJECTION);
            GL.glLoadIdentity();

            //nice 3D
            GLU.gluPerspective(45.0, 1.0, 0.4, 100.0);


            GL.glMatrixMode(GL.GL_MODELVIEW);
            GL.glLoadIdentity();

            //save the current MODELVIEW Matrix (now it is Identity)
            GL.glGetDoublev(GL.GL_MODELVIEW_MATRIX, AccumulatedRotationsTraslations);

            GenerateTextures();
        }

        public static uint[] Textures = new uint[9];       // texture

        void GenerateTextures()
        {
            GL.glGenTextures(9, Textures);
            string[] imagesName = { "world.bmp", "side.bmp", "top.bmp" ,"back.bmp","ontop.bmp", "front.bmp", "sky.bmp", "fire.bmp", "target.bmp"};
            for (int i = 0; i < 9; i++)
            {
                Bitmap image = new Bitmap(imagesName[i]);
                image.RotateFlip(RotateFlipType.RotateNoneFlipY); //Y axis in Windows is directed downwards, while in OpenGL-upwards
                System.Drawing.Imaging.BitmapData bitmapdata;
                Rectangle rect = new Rectangle(0, 0, image.Width, image.Height);

                bitmapdata = image.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

                GL.glBindTexture(GL.GL_TEXTURE_2D, Textures[i]);
                //2D for XYZ
                //The level-of-detail number. Level 0 is the base image level
                //The number of color components in the texture. 
                //Must be 1, 2, 3, or 4, or one of the following 
                //    symbolic constants: 
                //                GL_ALPHA, GL_ALPHA4, 
                //                GL_ALPHA8, GL_ALPHA12, GL_ALPHA16, GL_LUMINANCE, GL_LUMINANCE4, 
                //                GL_LUMINANCE8, GL_LUMINANCE12, GL_LUMINANCE16, GL_LUMINANCE_ALPHA, 
                //                GL_LUMINANCE4_ALPHA4, GL_LUMINANCE6_ALPHA2, GL_LUMINANCE8_ALPHA8, 
                //                GL_LUMINANCE12_ALPHA4, GL_LUMINANCE12_ALPHA12, GL_LUMINANCE16_ALPHA16, 
                //                GL_INTENSITY, GL_INTENSITY4, GL_INTENSITY8, GL_INTENSITY12, 
                //                GL_INTENSITY16, GL_R3_G3_B2, GL_RGB, GL_RGB4, GL_RGB5, GL_RGB8, 
                //                GL_RGB10, GL_RGB12, GL_RGB16, GL_RGBA, GL_RGBA2, GL_RGBA4, GL_RGB5_A1, 
                //                GL_RGBA8, GL_RGB10_A2, GL_RGBA12, or GL_RGBA16.


                GL.glTexImage2D(GL.GL_TEXTURE_2D, 0, (int)GL.GL_RGB8, image.Width, image.Height,
                                                              //The width of the border. Must be either 0 or 1.
                                                              //The format of the pixel data
                                                              //The data type of the pixel data
                                                              //A pointer to the image data in memory
                                                              0, GL.GL_BGR_EXT, GL.GL_UNSIGNED_byte, bitmapdata.Scan0);
                GL.glTexParameteri(GL.GL_TEXTURE_2D, GL.GL_TEXTURE_MIN_FILTER, (int)GL.GL_LINEAR);
                GL.glTexParameteri(GL.GL_TEXTURE_2D, GL.GL_TEXTURE_MAG_FILTER, (int)GL.GL_LINEAR);

                image.UnlockBits(bitmapdata);
                image.Dispose();
            }
        }

        void checkInput()
        {
            if (keyDown['W'])
            {
                tank.accelerate(true);
            }
            if (keyDown['S'])
            {
                tank.accelerate(false);
            }
            if (keyDown['A'])
            {
                tank.rotate(true);
            }
            if (keyDown['D'])
            {
                tank.rotate(false);
            }
            if (keyDown['F'])
            {
                fullscreen = !fullscreen;
            }
            if (keyDown['P'])
            {
                Bullet.bulletsize += 0.20f;
                if (Bullet.bulletsize > 0.66f)
                {
                    Bullet.bulletsize = 0.06f;
                }
                keyDown['P'] = false;
            }
            if (keyDown['U'])
            {
                stelthmode = !stelthmode;
                keyDown['U'] = false;
            }
            if (keyDown['T'])
            {
                tank.TankType +=1 ;
                if (tank.TankType > 2)
                {
                    tank.TankType = 1;
                }
                keyDown['T'] = false;
            }
        }

        void DrawMirror(bool screen)
        {
            GL.glTranslatef(0f, 0f, -30f);
            
            GL.glBegin(GL.GL_QUADS);
            //!!! for blended REFLECTION 
            float mirrorhight = 3;
            float mirrorwidth = 15;
            if (screen)
                GL.glColor4d(0.0, 0.0, 0.6, 0.4);
            else
                GL.glColor4d(1, 1, 1, 0);  
            GL.glVertex3f(0, 0, 0);
            GL.glVertex3f(0, 0, mirrorwidth);
            GL.glVertex3f(0, mirrorhight, mirrorwidth);
            GL.glVertex3f(0, mirrorhight, 0);
            GL.glEnd();
            
            GL.glTranslatef(0f, 0f, 30f);
        }

        void update()
        {
            checkInput();
            
            if (tank.bullets != null)
            {
                for (int i = 0; i < tank.bullets.Count; i++)
                {
                    tank.bullets[i].move();
                }
                for (int i = 0; i < tank.bullets.Count; i++)
                {
                    if (tank.bullets[i].isDead())
                    {
                        tank.bullets.Remove(tank.bullets[i]);
                    }
                }
            }
            tank.move();
        }
    }
}


