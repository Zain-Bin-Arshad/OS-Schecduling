using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace SRTF
{
    public class Process
    {
        // atirbutes of "Process" typ object
        public int P_id,burst_time,priorty,arrival_time, Pwaiting_time, PturnAround_time,status,compeletion_time; 
        public static Random rand = new Random();
        public Color process_color = new Color();

        public Process(int id, int b_time, int a_time)
        {
            P_id = id;
            burst_time = b_time;
            arrival_time = a_time;
            status = 0;
            PturnAround_time = 0;
            Pwaiting_time = 0;
            compeletion_time = 0;
            process_color = Color.FromArgb(rand.Next(256), rand.Next(256), rand.Next(256));
        }
        public Process(int id, int b_time, int a_time, int pri)
        {
            P_id = id;
            burst_time = b_time;
            arrival_time = a_time;
            priorty = pri;
            status = 0;
            PturnAround_time = 0;
            Pwaiting_time = 0;
            compeletion_time = 0;
            process_color = Color.FromArgb(rand.Next(256), rand.Next(256), rand.Next(256));
        }
    }
}
