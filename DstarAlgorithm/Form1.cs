using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace DstarAlgorithm
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        DateTime starttime,endtime;
        node[,] state;
        Queue OpenList = new Queue();
        int correnti, correntj;
        int hp, wp;
        Color[,] pixel;
        Bitmap orb;
        int gx, gy, sx, sy,cx,cy;
        
        //SET TAG OF MY NODE
        static void settag(node[,] a,int hp,int wp)
        {
            for (int j = 0; j < hp; j++)
                for (int i = 0; i < wp; i++)
                    a[i, j].tag = "NEW";
        }//end of settag


        //SET DIMATION VALUE OF MY NODE
        static void setdimval(node[,] a,int hp,int wp)
        {
            for (int j = 0; j < hp; j++)
                for (int i = 0; i < wp; i++)
                {
                    a[i, j].ni = i;
                    a[i, j].nj = j;
                }
        }//end of setdimval



        //SET STATUSE OF MY NODE
        static void setstatus(node [,] a,int hp,int wp,Color[,] pixel,int gx,int gy,int sx,int sy)
        {
            for (int j = 0; j < hp; j++)
                for (int i = 0; i < wp; i++)
                {
                    Color pix = pixel[i, j];

                    if (i == gx && j == gy)
                        a[i, j].status  = "GOAL";
                    else if (i == sx && j == sy)
                        a[i, j].status  = "START";
                    else if (pix.G == 0 && pix.R == 0 && pix.B == 0)
                        a[i, j].status  = "OBSTACLE";
                    else
                        a[i, j].status  = "CLEAR";
                }
        }//end of set status


       //SORT ELEMENT OF QUEUE INCRESING
       static void sortqueue(Queue OpenList,node[,] state)
        {
            int ind = OpenList.Count;
 
           double[] kk = new double[ind];
           int[] ii = new int[ind];
           int[] jj = new int[ind];
            
           node d = new node();

            for (int i = 0; i < ind; i++)
            {
                d = (node)OpenList.Dequeue();
                kk[i] = d.k;
                ii[i] = d.ni;
                jj[i] = d.nj;
            }

            for(int i = 0; i< ind; i++)
                for (int j = 0; j < ind; j++)
                {
                    if (kk[i] < kk[j])
                    {
                        double tempk = kk[i];
                        kk[i] = kk[j];
                        kk[j] = tempk;
                       
                        int tempi = ii[i];
                        ii[i] = ii[j];
                        ii[j] = tempi;

                        int tempj = jj[i];                        
                        jj[i] = jj[j];
                        jj[j] = tempj;
                    }
                }

            for (int i = 0; i < ind; i++)
            {
                OpenList.Enqueue(state[ii[i], jj[i]]);
             }
        }//end of sortqueue
        

        //INSERT THE ELEMENT S INTO OPENLIST
        static void insert(Queue OpenList, node s)
        {
            s.tag = "OPEN";            
            OpenList.Enqueue(s);
        }//end of insert

        //FIND THE MINIMUM VALUE OF K IN NEIGHBOURS
        static void k_min(int ni,int nj,node s, double[] mink, int[] mi, int[] mj)
        {
           
            if (s.k < mink[0])
              {
                  mink[0] = s.k;
                  mi[0] = ni; mj[0] = nj;
                  
              }
         }//end of k_min


        //THE PROCESS STEPS IN "CROSS" MOVING TO NIEGHBOURS
        static void crossmove(int si, int sj, node s, node old, Queue OpenList)
        {
                if (s.tag == "NEW")
                {
                    if (s.status == "OBSTACLE")
                        s.h = 10000;
                    else s.h = old.h + 1.4;
                    s.k = s.h;
                    insert(OpenList, s);
                    s.b = old;
                }
            
        }//end of cossmove
        

        //THE PROCESS STEPS IN "HORIZONTALY" OR "VERTICALLY" MOVING TO NIEGHBOURS
        static void linemove(int si, int sj, node s,node old,Queue OpenList)
        {

                if (s.tag == "NEW")
                {
                    if (s.status == "OBSTACLE")
                        s.h = 10000;
                    else s.h = old.h + 1;
                    s.k = s.h;
                    insert(OpenList, s);
                    s.b = old;
                }

        }//end of linemove

        //FIND THE COST VALUE BETWEEN TWO NODE
        static double cost(int x1, int y1, int x2, int y2)
        {
            if ((x2 - x1 == 0) || (y2 - y1 == 0))
            {
                return (1.0);
            }
            return (1.4);
        }//end cost

        
        //GIVING VALUES TO SI,SJ (THE PROBEBLY VALUE OF NEIGHBOURS OF STATE IN [IOLD,JOLD] POSITION)
        static void vs(int[] si, int[] sj,int iold,int jold)
        {
            si[0] = iold-1;    sj[0] = jold+1;
            
            si[1] = iold;      sj[1] = jold+1;
            
            si[2] = iold+1;    sj[2] = jold+1;
            
            si[3] = iold-1;    sj[3] = jold;
            
            si[4] = iold+1;    sj[4] = jold;
            
            si[5] = iold-1;    sj[5] = jold-1;
            
            si[6] = iold;      sj[6] = jold-1;
            
            si[7] = iold+1;    sj[7] = jold-1;
        }//end of vs


        public void Analys(object sender, EventArgs e)
        {
            if (tgx.Text == "" || tgy.Text == "" || tsx.Text == "" || tsy.Text == "")
                MessageBox.Show("Erorr in Entries of Goal or Start Coordinates");
            else
            {
                Form1.ActiveForm.Refresh();
                starttime = DateTime.Now;
                gx = int.Parse(tgx.Text);
                gy = int.Parse(tgy.Text);

                sx = int.Parse(tsx.Text);
                sy = int.Parse(tsy.Text);

                for (int j = 0; j < hp; j++)
                    for (int i = 0; i < wp; i++)
                        state[i, j] = new node();

                setdimval(state, hp, wp);
                settag(state, hp, wp);
                setstatus(state, hp, wp, pixel, gx, gy, sx, sy);

                state[gx, gy].h = 0.0;
                state[gx, gy].k = 0.0;
                insert(OpenList, state[gx, gy]);

                double kold;
                int iold, jold, ind;
                node d = new node();
                int[] si = new int[8];
                int[] sj = new int[8];

                do
                {

                    d = (node)OpenList.Dequeue();
                    kold = d.k;
                    iold = d.ni;
                    jold = d.nj;

                    state[iold, jold].tag = "CLOSE";

                    vs(si, sj, iold, jold);

                    for (int i = 0; i < 8; i++)
                    {
                        int row = si[i], col = sj[i];

                        if ((row <= wp - 1 && row >= 0) && (col <= hp - 1 && col >= 0))
                        {
                            if (row != iold && col != jold)
                                crossmove(row, col, state[row, col], state[iold, jold], OpenList);

                            else
                                linemove(row, col, state[row, col], state[iold, jold], OpenList);
                        }
                    }

                    ind = OpenList.Count;
                    sortqueue(OpenList, state);
                    
                        text2.Text = text2.Text + "[" + iold.ToString() + "," + jold.ToString() + "] " + state[iold, jold].k.ToString() + " - ";

                } while (state[iold, jold].status != "START");


                correnti = iold;
                correntj = jold;

                textBox4.Text = "Finished";
                text2.Visible = true;


            }

            findpath(sender, e);
        }


        // // // // // // // // // // // // // // // // // // // // // // // // // // // // //


        private void findpath(object sender, EventArgs e)
        {
                   
                double[] mink = new double[1]; mink[0] = 100000;
                int[] mi = new int[1]; mi[0] = 0;
                int[] mj = new int[1]; mj[0] = 0;
                int[] neighbori = new int[8];
                int[] neighborj = new int[8];
                int[] Mi = new int[4];
                int[] Mj = new int[4];
                string[] y = new string[4];
                string[] z = new string[4];
                double pathcoast = 0.0;

                pathcoast += state[correnti, correntj].h;

                int goali = gx;
                int goalj = gy;
                bool ddd = true;
                textBox5.Text = textBox5.Text + "(" + correnti.ToString() + ", " + correntj.ToString() + ") - ";
                do
                {

                    vs(neighbori, neighborj, correnti, correntj);
                    mink[0] = 10000;
                    for (int i = 0; i < 8; i++)
                    {
                        int row = neighbori[i]; int col = neighborj[i];
                        if ((row <= wp - 1 && row >= 0) && (col <= hp - 1 && col >= 0) && (state[row, col].status != "OBSTACLE"))
                            k_min(row, col, state[row, col], mink, mi, mj);
                    }

                    int mii = mi[0]; int mjj = mj[0];


                    //To Here we find the minimum k value 


                    Mi[0] = correnti + 1; Mj[0] = correntj + 1;
                    Mi[1] = correnti + 1; Mj[1] = correntj - 1;
                    Mi[2] = correnti - 1; Mj[2] = correntj + 1;
                    Mi[3] = correnti - 1; Mj[3] = correntj - 1;


                    if (correntj + 1 > hp - 1)
                        z[0] = "NULL";
                    else
                        z[0] = state[correnti, correntj + 1].status;

                    if (correntj - 1 < 0)
                        z[1] = "NULL";
                    else
                        z[1] = state[correnti, correntj - 1].status;

                    if (correnti - 1 < 0)
                        z[2] = "NULL";
                    else
                        z[2] = state[correnti - 1, correntj].status;

                    z[3] = z[1];


                    if (correnti + 1 > wp - 1)
                        y[0] = "NULL";
                    else
                        y[0] = state[correnti + 1, correntj].status;

                    y[1] = y[0];

                    if (correntj + 1 > hp - 1)
                        y[2] = "NULL";
                    else
                        y[2] = state[correnti, correntj + 1].status;

                    if (correnti - 1 < 0)
                        y[3] = "NULL";
                    else
                        y[3] = state[correnti - 1, correntj].status;


                    for (int ii = 0; ii < 4; ii++)
                        if (mii == Mi[ii] && mjj == Mj[ii])
                            if (y[ii] == "OBSTACLE" && z[ii] == "OBSTACLE")
                            {

                                state[mii, mjj].h = 10000;
                                state[mii, mjj].status = "OBSTACLE";
                                insert(OpenList, state[mii, mjj]);

                                int[] si = new int[8];
                                int[] sj = new int[8];
                                vs(si, sj, mii, mjj);

                                for (int i = 0; i < 8; i++)
                                    if (state[si[i], sj[i]].tag == "CLOSE")
                                        insert(OpenList, state[si[i], sj[i]]);
                                int iold, jold;
                                double kold;
                                bool good;
                                good = true;
                                sortqueue(OpenList, state);

                                do
                                {
                                    node d = new node();
                                    d = (node)OpenList.Dequeue();
                                    kold = d.k;
                                    iold = d.ni;
                                    jold = d.nj;
                                    state[iold, jold].tag = "CLOSE";

                                    if (iold == correnti && jold == correntj)
                                        good = false;

                                    vs(si, sj, iold, jold);


                                    if (kold == state[iold, jold].h)
                                    {
                                        for (int i = 0; i < 8; i++)
                                        {
                                            int r = si[i];
                                            int c = sj[i];

                                            if ((r <= wp - 1 && r >= 0) && (c <= hp - 1 && c >= 0))
                                            {
                                                double co = cost(iold, jold, r, c);
                                                double tt = co + state[iold, jold].h;

                                                if (state[r, c].tag == "NEW")
                                                {
                                                    state[r, c].b = state[iold, jold];
                                                    state[r, c].h = tt;
                                                    state[r, c].k = tt;
                                                    insert(OpenList, state[r, c]);
                                                    sortqueue(OpenList, state);
                                                }
                                                else if (((state[r, c].b == state[iold, jold]) && (state[r, c].h != tt)) || ((state[r, c].b != state[iold, jold]) && ((float)state[r, c].h > (float)tt)))
                                                    if (state[r, c].status != "OBSTACLE")
                                                    {
                                                        state[r, c].b = state[iold, jold];
                                                        state[r, c].h = tt;

                                                        if (state[r, c].tag == "CLOSE")
                                                        {
                                                            insert(OpenList, state[r, c]);
                                                            sortqueue(OpenList, state);
                                                        }
                                                    }
                                            }
                                        }
                                    }


                                    if (kold < state[iold, jold].h)
                                    {
                                        for (int i = 0; i < 8; i++)
                                        {
                                            int r = si[i];
                                            int c = sj[i];

                                            if ((r <= wp - 1 && r >= 0) && (c <= hp - 1 && c >= 0))
                                            {
                                                double co = cost(iold, jold, r, c);
                                                double tt = co + state[iold, jold].h;

                                                if (state[r, c].tag == "NEW")
                                                {
                                                    state[r, c].b = state[iold, jold];
                                                    state[r, c].h = tt;
                                                    state[r, c].k = tt;
                                                    insert(OpenList, state[r, c]);
                                                }
                                                else
                                                {
                                                    if ((state[r, c].b == state[iold, jold]) && (state[r, c].h != tt))
                                                    {
                                                        state[r, c].b = state[iold, jold];
                                                        if (state[r, c].tag == "CLOSE")
                                                        {
                                                            if (state[iold, jold].h == 10000)
                                                                state[r, c].h = 10000;
                                                            insert(OpenList, state[r, c]);
                                                            sortqueue(OpenList, state);
                                                        }

                                                        else
                                                        {
                                                            state[r, c].h = tt;
                                                            if (state[r, c].h > 10000)
                                                                state[r, c].h = 10000;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if ((state[r, c].b != state[iold, jold]) && (state[r, c].h > tt))
                                                        {
                                                            state[iold, jold].k = tt;
                                                            insert(OpenList, state[iold, jold]);
                                                            sortqueue(OpenList, state);
                                                        }
                                                        else
                                                        {
                                                            if (state[r, c].b != state[iold, jold])
                                                            {
                                                                if (state[iold, jold].h > state[r, c].h + cost(iold, jold, r, c))
                                                                {
                                                                    if (state[r, c].tag == "CLOSE")
                                                                    {
                                                                        if (state[r, c].h > kold)
                                                                        {
                                                                            insert(OpenList, state[r, c]);
                                                                            sortqueue(OpenList, state);
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }

                                } while (good);



                                bool end = true;

                                while (end)
                                {
                                    node d = new node();
                                    d = (node)OpenList.Dequeue();
                                    kold = d.k;
                                    iold = d.ni;
                                    jold = d.nj;
                                    state[iold, jold].tag = "CLOSE";

                                    if (kold < state[correnti, correntj].h)
                                    {

                                        vs(si, sj, iold, jold);

                                        if (kold == state[iold, jold].h)
                                        {
                                            for (int i = 0; i < 8; i++)
                                            {
                                                int r = si[i];
                                                int c = sj[i];

                                                if ((r <= wp - 1 && r >= 0) && (c <= hp - 1 && c >= 0))
                                                {
                                                    double co = cost(iold, jold, r, c);
                                                    double tt = state[iold, jold].h + co;

                                                    if (state[r, c].tag == "NEW")
                                                    {
                                                        state[r, c].b = state[iold, jold];
                                                        state[r, c].h = state[iold, jold].h + co;
                                                        state[r, c].k = state[r, c].h;
                                                        insert(OpenList, state[r, c]);
                                                        sortqueue(OpenList, state);
                                                    }
                                                    else if (((state[r, c].b == state[iold, jold]) && ((float)state[r, c].h != (float)tt)) || ((state[r, c].b != state[iold, jold]) && ((float)state[r, c].h > (float)tt)))
                                                    {
                                                        if (state[r, c].status != "OBSTACLE")
                                                        {
                                                            state[r, c].b = state[iold, jold];
                                                            state[r, c].h = state[iold, jold].h + co;
                                                            state[r, c].k = state[r, c].h;
                                                            if (state[r, c].tag == "CLOSE")
                                                            {
                                                                insert(OpenList, state[r, c]);
                                                                sortqueue(OpenList, state);
                                                            }
                                                        }
                                                    }

                                                }
                                            }

                                        }


                                        else if (kold < state[iold, jold].h)
                                        {
                                            for (int i = 0; i < 8; i++)
                                            {
                                                int r = si[i];
                                                int c = sj[i];

                                                if ((r <= 4 && r >= 0) && (c <= 4 && c >= 0))
                                                {
                                                    double co = cost(iold, jold, r, c);
                                                    double tt = state[iold, jold].h + co;

                                                    if (state[r, c].tag == "NEW")
                                                    {
                                                        state[r, c].b = state[iold, jold];
                                                        state[r, c].h = tt;
                                                        state[r, c].k = tt;
                                                        insert(OpenList, state[r, c]);
                                                        sortqueue(OpenList, state);
                                                    }
                                                    else if ((state[r, c].b == state[iold, jold]) && ((float)state[r, c].h != (float)tt))
                                                    {
                                                        state[r, c].b = state[iold, jold];
                                                        if (state[r, c].tag == "CLOSE")
                                                        {
                                                            state[r, c].h = tt;
                                                            if (state[r, c].h > 10000)
                                                                state[r, c].h = 10000;
                                                            insert(OpenList, state[r, c]);
                                                            sortqueue(OpenList, state);
                                                        }

                                                    }
                                                    else if ((state[r, c].b != state[iold, jold]) && ((float)state[r, c].h > (float)tt))
                                                    {
                                                        if (state[iold, jold].tag == "CLOSE")
                                                        {
                                                            state[iold, jold].k = state[iold, jold].h;
                                                            insert(OpenList, state[iold, jold]);
                                                            sortqueue(OpenList, state);
                                                        }
                                                    }
                                                    else if ((state[r, c].b != state[iold, jold]) && ((float)state[iold, jold].h > (float)(state[r, c].h + co)) && (state[r, c].tag == "CLOSE") && (state[r, c].h > (float)kold))
                                                    {
                                                        insert(OpenList, state[r, c]);
                                                        sortqueue(OpenList, state);
                                                    }

                                                }
                                            }
                                        }


                                    }
                                    else
                                    {
                                        end = false;
                                        OpenList.Enqueue(state[iold, jold]);
                                        sortqueue(OpenList, state);
                                    }

                                }

                                mii = correnti;
                                mjj = correntj;

                            }
                    correnti = mii;
                    correntj = mjj;
                    if ((correnti == goali) && (correntj == goalj))
                        ddd = false;

                    textBox5.Text = textBox5.Text + "(" + correnti.ToString() + ", " + correntj.ToString() + ") - ";
                    orb.SetPixel(correnti, correntj, Color.Green);
                    Form1.ActiveForm.Refresh();
                    pathcoast += state[correnti, correntj].h;

                } while (ddd);
                orb.SetPixel(correnti, correntj, Color.Gold);
                textBox1.Text = pathcoast.ToString();
                textBox5.Visible = true;
                
                endtime = DateTime.Now;

                TimeSpan duration = endtime - starttime;
                int sec= duration.Seconds;
                int milli = duration.Milliseconds;

                textBox2.Text = sec.ToString()+" Sec , "+milli.ToString() + " MS";
                
                Form1.ActiveForm.Refresh();

            
        }

       private void button3_Click(object sender, EventArgs e)
       {
           
           op1.Filter = "Image Files(*.bmp)|*.bmp";

           if (op1.ShowDialog() == DialogResult.OK)
           {
               orb = new Bitmap(op1.FileName);
               pb1.Image = orb;
               hp = orb.Height;
               wp = orb.Width;

               state = new node[hp, wp];
               pixel = new Color[hp, wp];

               for (int y = 0; y < hp; y++)
               {
                   for (int x = 0; x < wp; x++)
                   {
                       pixel[x, y] = orb.GetPixel(x, y);
                   }
               }
           
           }
       
       }


       private void pb1_Click(object sender, EventArgs e)
       {
           Point ll;
           ll = pb1.PointToClient(Cursor.Position);
           cx = (ll.X) / 5;
           cy = (ll.Y) / 5;

           if (tsx.Text == "")
           {
               tsx.Text = cx.ToString();
               tsy.Text = cy.ToString();
               orb.SetPixel(cx, cy, Color.Red);
               Form1.ActiveForm.Refresh();
           }
           else
           {
               tgx.Text = cx.ToString();
               tgy.Text = cy.ToString();
               orb.SetPixel(cx, cy, Color.Gold);
               Form1.ActiveForm.Refresh();
               Analys(sender, e);
           }

           

       }

       private void button4_Click(object sender, EventArgs e)
       {
           Application.Restart();
       }



    }

   }
