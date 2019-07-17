using System;
using System.Collections.Generic;
using System.Text;
using OpenGL;

namespace myOpenGL
{
    class Target
    {
        bool targetDead;
        bool rising;
        bool falling;
        public float targetX;
        public float targetZ;
        public float targetWidth = 1.5f;
        float targetHight = 3;
        float riseY;
        float riseZ;
        Random random;

        public Target()
        {
        targetDead = true;
        rising = false;
        falling = false;
        random = new Random();
        }

        public void Kill()
        {
            if (!targetDead && !rising && !falling)
            {
                falling = true;
                riseY = targetHight;
                riseZ = 0;
                cOGL.score++;
            }
        }

        public void Drawself()
        {
            int boarder = (int)cOGL.worldsize-2;
            GL.glPushMatrix();
            GL.glEnable(GL.GL_TEXTURE_2D);
            GL.glBindTexture(GL.GL_TEXTURE_2D, cOGL.Textures[8]);
            GL.glColor3f(0.7f, 0.7f, 0.7f);
            if (targetDead)
            {
                targetX = random.Next(-boarder, boarder);
                targetZ = random.Next(10, boarder*2);
                targetZ = -targetZ;
                //targetX = -6;
                //targetZ = -8;
                if (targetX > 0)
                {
                    targetWidth = 2;
                }
                else
                    targetWidth = -2;
                riseY = 0;
                riseZ = targetHight;
                targetDead = false;
                rising = true;
                Drawarise();
            }
            else if (rising)
            {
                Drawarise();
            }
            else if (falling)
            {
                Drawfall();
            }
            else
            {
                GL.glTranslatef(targetX, 0, targetZ);
                GL.glBegin(GL.GL_QUADS);
                GL.glNormal3f(0.0f, 1.0f, 1.0f);
                GL.glTexCoord2f(0, 0);
                GL.glVertex3f(0, 0, 0);
                GL.glTexCoord2f(0, 1);
                GL.glVertex3f(0, targetHight, 0);
                GL.glTexCoord2f(1, 1);
                GL.glVertex3f(targetWidth, targetHight, 0);
                GL.glTexCoord2f(1, 0);
                GL.glVertex3f(targetWidth, 0, 0);
            }
            GL.glEnd();
            GL.glPopMatrix();
            GL.glDisable(GL.GL_TEXTURE_2D);//!!!
        }

        void Drawarise()
        {
            GL.glTranslatef(targetX, 0, targetZ);
            GL.glBegin(GL.GL_QUADS);
            GL.glNormal3f(0.0f, 1.0f, 0.0f);
            GL.glTexCoord2f(0, 0);
            GL.glVertex3f(0, 0, 0);
            GL.glTexCoord2f(0, 1);
            GL.glVertex3f(0, riseY, 0-riseZ);
            GL.glTexCoord2f(1, 1);
            GL.glVertex3f(targetWidth, riseY, 0-riseZ);
            GL.glTexCoord2f(1, 0);
            GL.glVertex3f(targetWidth, 0, 0);

            if (riseY < targetHight)
            {
                riseY += 0.1f;
                riseZ -= 0.1f;
            }
            else
            {
                rising = false;
            }
        }

        void Drawfall()
        {
            GL.glTranslatef(targetX, 0, targetZ);
            GL.glBegin(GL.GL_QUADS);
            GL.glNormal3f(0.0f, 1.0f, 0.0f);
            GL.glTexCoord2f(0, 0);
            GL.glVertex3f(0, 0, 0);
            GL.glTexCoord2f(0, 1);
            GL.glVertex3f(0, riseY, 0-riseZ);
            GL.glTexCoord2f(1, 1);
            GL.glVertex3f(targetWidth, riseY, 0-riseZ);
            GL.glTexCoord2f(1, 0);
            GL.glVertex3f(targetWidth, 0, 0);

            if (riseY > 0)
            {
                riseY -= 0.1f;
                riseZ += 0.1f;
            }
            else
            {
                falling = false;
                targetDead = true;
            }
        }
    }
}
