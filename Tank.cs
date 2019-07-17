using System;
using System.Collections.Generic;
using System.Text;
using OpenGL;


namespace myOpenGL
{
    public class Tank
    {
        public float speed = 0f;
        public float rotationSpeed = 0f;
        public float rotation;
        public float turretRotationSpeed = 0;
        public float turretRotation = 0;
        public float posX;
        public float posZ;
        public float width = 0.5f;
        public float height = 0.5f;
        public float depth = 0.7f;
        public float destinX;
        public float destinZ;
        public float reloadTime = 20;
        public float reloadCounter;
        public float speedX = 0;
        public float speedZ = 0;
        public int TankType;
        public List<Bullet> bullets = new List<Bullet>();

        public Tank(float positionX, float positionZ, float initialRotation, int type)
        {
            rotation = initialRotation;
            posX = positionX;
            posZ = positionZ;
            destinX = positionX;
            destinZ = positionZ;
            reloadCounter = reloadTime;
            TankType = type;
        }

        public void accelerate(bool directionIsForward)
        {
            if (directionIsForward)
            {
                speed += 0.02f;
            }
            else
            {
                speed -= 0.02f;
            }
        }
        public void rotate(bool directionIsPositive)
        {
            rotationSpeed *= 0.01f;
            if (directionIsPositive)
            {
                rotationSpeed += 3.5f;
            }
            else
            {
                rotationSpeed -= 3.5f;
            }
        }
        public void rotateTurret(float amount)
        {
            turretRotationSpeed += amount;
            if (turretRotation > 180)
            {
                turretRotation -= 360;
            }
            else if (turretRotation < -180)
            {
                turretRotation += 360;
            }
        }
        public bool fire()
        {
            if (reloadCounter <= 0)
            {
                float angle = rotation + turretRotation;
                bullets.Add(new Bullet((float)(posX - 1.0 * System.Math.Sin(angle * System.Math.PI / 180)),
                                            0.65f,
                                             (float)(posZ - 1.0 * System.Math.Cos(angle * System.Math.PI / 180)), speedX, speedZ,
                                             angle, TankType
                                             ));
                reloadCounter = reloadTime;
                return true;
            }
            return false;
        }
        public void move()
        {
            if (!ColitionDetection())
            {
                speedX = (float)((-speed) * System.Math.Sin(rotation * System.Math.PI / 180));
                speedZ = (float)((-speed) * System.Math.Cos(rotation * System.Math.PI / 180));

                posX += speedX;
                posZ += speedZ;

                rotation += rotationSpeed;
                turretRotation -= rotationSpeed;
                turretRotation += turretRotationSpeed;
                turretRotationSpeed *= 0.5f;
                rotationSpeed *= 0.5f;
                speed *= 0.8f;
                if (rotation > 360.0f)
                {
                    rotation -= 360.0f;
                }
                else if (rotation < -360.0f)
                {
                    rotation += 360.0f;
                }
                if (turretRotation > 360.0f)
                {
                    turretRotation -= 360.0f;
                }
                else if (turretRotation < -360.0f)
                {
                    turretRotation += 360.0f;
                }
                reloadCounter -= 1;
            }
            else
            {
                posX -= speedX;
                posZ -= speedZ;
            }
        }

        public bool ColitionDetection()
        {
            float boarder = cOGL.worldsize;
            if (posZ > 0) //z boarder start
                return true;
            if (posZ < -boarder*2) //z boarder finish
                return true;
            if (posX < -boarder) //x boarder start
                return true;
            if (posX > boarder) //x boarder finish
                return true;

            if (InRange(posZ,-30,-15)) //mirror Z
                if (InRange(posX, -0.8f, 0.8f)) //mirror X
                    return true;

            if (InRange(posZ, cOGL.target.targetZ - 0.8f, cOGL.target.targetZ + 0.8f)) //target
                if (InRange(posX, cOGL.target.targetX, cOGL.target.targetX + cOGL.target.targetWidth))
                    return true;

            return false;
        }

        bool InRange(float value, float min, float max)
        {
            return value >= Math.Min(min, max) && value <= Math.Max(min, max);
        }

        public void drawSelf()
        {
            GL.glPushMatrix();
            GL.glTranslatef(posX, 0.0f, posZ);
            GL.glRotatef(rotation, 0.0f, 1.0f, 0.0f);

            float w, h, d, d2;
            w = width; //0.5
            h = height; //0.5
            d = depth;    //0.7
            d2 = depth / 1.4f; //0.5

            if (TankType==2)
                h += 0.5f;

            GL.glColor3f(0.273f, 0.252f, 0.135f); //army-green light
            GL.glBegin(GL.GL_QUADS);
                //Bottom
                GL.glNormal3f(0.0f, -1.0f, 0.0f);
                GL.glVertex3f(-w, 0.0f, -d2);
                GL.glVertex3f(-w, 0.0f, d);
                GL.glVertex3f(w, 0.0f, d);
                GL.glVertex3f(w, 0.0f, -d2);
            GL.glEnd();

            GL.glEnable(GL.GL_TEXTURE_2D);
            GL.glBindTexture(GL.GL_TEXTURE_2D, cOGL.Textures[1]);
            if (TankType == 2)
            {
                GL.glColor3f(0.2f, 0.2f, 1); //blue
            }
            else
                GL.glColor3f(1, 1, 1); //white
            GL.glBegin(GL.GL_QUADS);
                //Left
                GL.glNormal3f(-1.0f, 0.0f, 0.0f);
                GL.glTexCoord2f(0, 0);
                GL.glVertex3f(-w, h, -d);
                GL.glTexCoord2f(0, 1);
                GL.glVertex3f(-w, 0.0f, -d2);
                GL.glTexCoord2f(1, 1);
                GL.glVertex3f(-w, 0.0f, d);
                GL.glTexCoord2f(1, 0);
                GL.glVertex3f(-w, h, d2);
                //Right              
                GL.glNormal3f(1.0f, 0.0f, 0.0f);
                GL.glTexCoord2f(0, 0);
                GL.glVertex3f(w, h, -d);
                GL.glTexCoord2f(0, 1);
                GL.glVertex3f(w, 0.0f, -d2);
                GL.glTexCoord2f(1, 1);
                GL.glVertex3f(w, 0.0f, d);
                GL.glTexCoord2f(1, 0);
                GL.glVertex3f(w, h, d2);
            GL.glEnd();
            GL.glBindTexture(GL.GL_TEXTURE_2D, cOGL.Textures[3]);
            GL.glBegin(GL.GL_QUADS);
                //Back
                GL.glNormal3f(0.0f, -0.5f, 0.7f);
                GL.glTexCoord2f(0, 0);
                GL.glVertex3f(-w, h, d2);
                GL.glTexCoord2f(0, 1);
                GL.glVertex3f(w, h, d2);
                GL.glTexCoord2f(1, 1);
                GL.glVertex3f(w, 0.0f, d);
                GL.glTexCoord2f(1, 0);
                GL.glVertex3f(-w, 0.0f, d);
            GL.glEnd();
            GL.glBindTexture(GL.GL_TEXTURE_2D, cOGL.Textures[4]);
            GL.glBegin(GL.GL_QUADS);
                //Top
                GL.glNormal3f(0.0f, 1.0f, 0.0f);
                GL.glTexCoord2f(0, 0);
                GL.glVertex3f(-w, h, -d);
                GL.glTexCoord2f(0, 1);
                GL.glVertex3f(-w, h, d2);
                GL.glTexCoord2f(1, 1);
                GL.glVertex3f(w, h, d2);
                GL.glTexCoord2f(1, 0);
                GL.glVertex3f(w, h, -d);
            GL.glEnd();
            GL.glBindTexture(GL.GL_TEXTURE_2D, cOGL.Textures[5]);
            GL.glBegin(GL.GL_QUADS);
                //Front
                GL.glNormal3f(0.0f, h, -d);
                GL.glTexCoord2f(0, 0);
                GL.glVertex3f(-w, h, -d);
                GL.glTexCoord2f(0, 1);
                GL.glVertex3f(w, h, -d);
                GL.glTexCoord2f(1, 1);
                GL.glVertex3f(w, 0.0f, -d2);
                GL.glTexCoord2f(1, 0);
                GL.glVertex3f(-w, 0.0f, -d2);
            GL.glEnd();

            GL.glDisable(GL.GL_TEXTURE_2D);//!!!
                GL.glPushMatrix();
                GL.glTranslatef(0.0f, h * 1.3f, 0.0f);
                GL.glRotatef(turretRotation, 0.0f, 1.0f, 0.0f);
                makeRectangularPrism(w * (3.0f / 5.0f), 0.0f, -d / 2, -w * (3.0f / 5.0f), h / 2, d / 2);
                GL.glTranslatef(0.0f, 0.025f, -0.6f);
                GL.glTranslatef(0.0f, 0.0f, -0.2f + 1 * 0.8f);

                GL.glPopMatrix();

            GL.glPopMatrix();
            
        }

        void makeRectangularPrism(float x1, float y1, float z1, float x2, float y2, float z2)
        {
            float width = (x2 - x1) / 2;
            if (width < 1)
            {
                width = -width;
            }
            float height = (y2 - y1) / 2;
            if (height < 1)
            {
                height = -height;
            }
            float depth = (z2 - z1) / 2;
            if (depth < 1)
            {
                depth = -depth;
            }
            if (TankType ==2)
            {
                GL.glColor3f(0.273f, 0.252f, 0.735f); //army-green light
                height = 0.25f * height;
                GL.glTranslatef(0.0f, -0.3f, 0);
            }
            else
                GL.glColor3f(0.273f, 0.252f, 0.135f); //army-green light
            
            GL.glBegin(GL.GL_QUADS);
                //Front
                GL.glNormal3f(0.0f, 0.0f, -1.0f);
                GL.glVertex3f(-width, -height, depth);
                GL.glVertex3f(width, -height, depth);
                GL.glVertex3f(width, height, depth);
                GL.glVertex3f(-width, height, depth);
                //Top
                GL.glNormal3f(0.0f, 1.0f, 0.0f);
                GL.glVertex3f(-width, height, -depth);
                GL.glVertex3f(-width, height, depth);
                GL.glVertex3f(width, height, depth);
                GL.glVertex3f(width, height, -depth);
                //Bottom
                GL.glNormal3f(0.0f, -1.0f, 0.0f);
                GL.glVertex3f(-width, -height, -depth);
                GL.glVertex3f(-width, -height, depth);
                GL.glVertex3f(width, -height, depth);
                GL.glVertex3f(width, -height, -depth);
                //Right
                GL.glNormal3f(1.0f, 0.0f, 0.0f);
                GL.glVertex3f(width, -height, -depth);
                GL.glVertex3f(width, height, -depth);
                GL.glVertex3f(width, height, depth);
                GL.glVertex3f(width, -height, depth);
                //Left
                GL.glNormal3f(-1.0f, 0.0f, 0.0f);
                GL.glVertex3f(-width, -height, -depth);
                GL.glVertex3f(-width, -height, depth);
                GL.glVertex3f(-width, height, depth);
                GL.glVertex3f(-width, height, -depth);
            GL.glEnd();

            GL.glEnable(GL.GL_TEXTURE_2D);
            GL.glBindTexture(GL.GL_TEXTURE_2D, cOGL.Textures[2]);
            if (TankType==2)
	        {
                GL.glColor3f(0, 0, 1); //blue
	        }
            else
                GL.glColor3f(1, 1, 1); //white
            GL.glBegin(GL.GL_QUADS);
                //Back
                GL.glNormal3f(0.0f, 0.0f, 1.0f);
                GL.glTexCoord2f(0, 0);
                GL.glVertex3f(-width, -height, -depth);
                GL.glTexCoord2f(0, 1);
                GL.glVertex3f(-width, height, -depth);
                GL.glTexCoord2f(1, 1);
                GL.glVertex3f(width, height, -depth);
                GL.glTexCoord2f(1, 0);
                GL.glVertex3f(width, -height, -depth);
            GL.glEnd();

            GLUquadric obj;
            obj = GLU.gluNewQuadric(); //!!!

            //barrel
            GL.glPushMatrix();
            GL.glTranslatef(0.0f, 0.025f, -1f);
            GLU.gluCylinder(obj, 0.05, 0.05, 1.2, 10, 2);
            GL.glPopMatrix();
            
            GL.glDisable(GL.GL_TEXTURE_2D);//!!!
        }

        public void drawstelth()
        {
            GL.glPushMatrix();
            GL.glTranslatef(posX, 0.0f, posZ);
            GL.glRotatef(rotation, 0.0f, 1.0f, 0.0f);

            float w, h, d, d2;
            w = width; //0.5
            h = height; //0.5
            d = depth;    //0.7
            d2 = depth / 1.4f; //0.5

            if (TankType == 2)
            {
                h += 0.5f;
            }

            GL.glColor4f(0.2f, 0.2f, 0.2f, 0.8f); //black stelth
            GL.glBegin(GL.GL_QUADS);
            //Bottom
            GL.glNormal3f(0.0f, -1.0f, 0.0f);
            GL.glVertex3f(-w, 0.0f, -d2);
            GL.glVertex3f(-w, 0.0f, d);
            GL.glVertex3f(w, 0.0f, d);
            GL.glVertex3f(w, 0.0f, -d2);
            GL.glEnd();

            GL.glColor4f(0.2f, 0.2f, 0.2f, 0.8f); //black stelth
            GL.glBegin(GL.GL_QUADS);
            //Left
            GL.glNormal3f(-1.0f, 0.0f, 0.0f);
            GL.glVertex3f(-w, h, -d);
            GL.glVertex3f(-w, 0.0f, -d2);
            GL.glVertex3f(-w, 0.0f, d);
            GL.glVertex3f(-w, h, d2);
            //Right              
            GL.glNormal3f(1.0f, 0.0f, 0.0f);
            GL.glVertex3f(w, h, -d);
            GL.glVertex3f(w, 0.0f, -d2);
            GL.glVertex3f(w, 0.0f, d);
            GL.glVertex3f(w, h, d2);
            GL.glEnd();

            GL.glBegin(GL.GL_QUADS);
            //Back
            GL.glNormal3f(0.0f, -0.5f, 0.7f);
            GL.glVertex3f(-w, h, d2);
            GL.glVertex3f(w, h, d2);
            GL.glVertex3f(w, 0.0f, d);
            GL.glVertex3f(-w, 0.0f, d);
            GL.glEnd();

            GL.glBegin(GL.GL_QUADS);
            //Top
            GL.glNormal3f(0.0f, 1.0f, 0.0f);
            GL.glVertex3f(-w, h, -d);
            GL.glVertex3f(-w, h, d2);
            GL.glVertex3f(w, h, d2);
            GL.glVertex3f(w, h, -d);
            GL.glEnd();

            GL.glBegin(GL.GL_QUADS);
            //Front
            GL.glNormal3f(0.0f, h, -d);
            GL.glVertex3f(-w, h, -d);
            GL.glVertex3f(w, h, -d);
            GL.glVertex3f(w, 0.0f, -d2);
            GL.glVertex3f(-w, 0.0f, -d2);
            GL.glEnd();

            GL.glPushMatrix();
            GL.glTranslatef(0.0f, h * 1.3f, 0.0f);
            GL.glRotatef(turretRotation, 0.0f, 1.0f, 0.0f);
            makeRectangularPrismstelth(w * (3.0f / 5.0f), 0.0f, -d / 2, -w * (3.0f / 5.0f), h / 2, d / 2);
            GL.glTranslatef(0.0f, 0.025f, -0.6f);
            GL.glTranslatef(0.0f, 0.0f, -0.2f + 1 * 0.8f);

            GL.glPopMatrix();
            GL.glPopMatrix();
        }

        void makeRectangularPrismstelth(float x1, float y1, float z1, float x2, float y2, float z2)
        {
            float width = (x2 - x1) / 2;
            if (width < 1)
            {
                width = -width;
            }
            float height = (y2 - y1) / 2;
            if (height < 1)
            {
                height = -height;
            }
            float depth = (z2 - z1) / 2;
            if (depth < 1)
            {
                depth = -depth;
            }
            if (TankType == 2)
            {
                height = 0.25f * height;
                GL.glTranslatef(0.0f, -0.3f, 0);
            }
            GL.glColor4f(0.2f, 0.2f, 0.2f, 1f); //black stelth
            GL.glBegin(GL.GL_QUADS);
            //Front
            GL.glNormal3f(0.0f, 0.0f, -1.0f);
            GL.glVertex3f(-width, -height, depth);
            GL.glVertex3f(width, -height, depth);
            GL.glVertex3f(width, height, depth);
            GL.glVertex3f(-width, height, depth);
            //Top
            GL.glNormal3f(0.0f, 1.0f, 0.0f);
            GL.glVertex3f(-width, height, -depth);
            GL.glVertex3f(-width, height, depth);
            GL.glVertex3f(width, height, depth);
            GL.glVertex3f(width, height, -depth);
            //Bottom
            GL.glNormal3f(0.0f, -1.0f, 0.0f);
            GL.glVertex3f(-width, -height, -depth);
            GL.glVertex3f(-width, -height, depth);
            GL.glVertex3f(width, -height, depth);
            GL.glVertex3f(width, -height, -depth);
            //Right
            GL.glNormal3f(1.0f, 0.0f, 0.0f);
            GL.glVertex3f(width, -height, -depth);
            GL.glVertex3f(width, height, -depth);
            GL.glVertex3f(width, height, depth);
            GL.glVertex3f(width, -height, depth);
            //Left
            GL.glNormal3f(-1.0f, 0.0f, 0.0f);
            GL.glVertex3f(-width, -height, -depth);
            GL.glVertex3f(-width, -height, depth);
            GL.glVertex3f(-width, height, depth);
            GL.glVertex3f(-width, height, -depth);
            GL.glEnd();

            GL.glColor4f(0.2f, 0.2f, 0.2f, 1f); //black stelth
            GL.glBegin(GL.GL_QUADS);
            //Back
            GL.glNormal3f(0.0f, 0.0f, 1.0f);
            GL.glVertex3f(-width, -height, -depth);
            GL.glVertex3f(-width, height, -depth);
            GL.glVertex3f(width, height, -depth);
            GL.glVertex3f(width, -height, -depth);
            GL.glEnd();

            GLUquadric obj;
            obj = GLU.gluNewQuadric(); //!!!

            //barrel
            GL.glPushMatrix();
            GL.glTranslatef(0.0f, 0.025f, -1f);
            GLU.gluCylinder(obj, 0.05, 0.05, 1.2, 10, 2);
            GL.glPopMatrix();
        }
    }
}
        
