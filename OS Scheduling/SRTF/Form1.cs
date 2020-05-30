using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

 
namespace SRTF
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            SRTF.CheckedChanged += new EventHandler(radioButtons_CheckedChanged);
            SJF.CheckedChanged += new EventHandler(radioButtons_CheckedChanged);
            FCFS.CheckedChanged += new EventHandler(radioButtons_CheckedChanged);
            RR.CheckedChanged += new EventHandler(radioButtons_CheckedChanged);
            Priority.CheckedChanged += new EventHandler(radioButtons_CheckedChanged);
            PriorityP.CheckedChanged += new EventHandler(radioButtons_CheckedChanged);
        }

        List<Process> Process_List = new List<Process>(), ganttChart = new List<Process>();
        List<int> Arrival_time = new List<int>(); List<int> burst_time = new List<int>();
        List<int> flag = new List<int>(); List<int> service_time = new List<int>();
        int Process_ID, Process_Priorty, Process_Arrival, Process_Quantum, pervAlgo;
        int Process_Burst, X = 16, t = 0, Y = 16, temp = 0, sum = 0;
        int complete = 0, shortest = 0, finish_time, minimum;
        float total_waiting_time = 0f, total_turnAround_time = 0f;
        string[] row = new string[20]; // for displaying the result
        bool checke = false; bool insert = false; // check for same Process IDs
        

        private void radioButtons_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb1 = groupBox4.Controls.OfType<RadioButton>().FirstOrDefault(n => n.Checked);
            RadioButton radioButton = sender as RadioButton;
            if (!(RR == rb1))
                numericUpDown5.Enabled = false;
            else
                numericUpDown5.Enabled = true;
            if (!(Priority == rb1 || PriorityP == rb1))
            {
                numericUpDown4.Enabled = false;
                groupBox5.Enabled = false;
            }
            else
            {
                numericUpDown4.Enabled = true;
                groupBox5.Enabled = true;
            }
        }

        private void label1_Click(object sender, EventArgs e) // when INSERT button is pressed 
        {
            dataGridView1.Visible = false;
            Process_ID = Convert.ToInt32(numericUpDown1.Value);
            Process_Burst = Convert.ToInt32(numericUpDown3.Text);
            Process_Arrival = Convert.ToInt32(numericUpDown2.Text);

            if (Priority.Checked || PriorityP.Checked)
                Process_Priorty = Convert.ToInt32(numericUpDown4.Text);

            for (int i = 0; i < Process_List.Count; i++)
            {
                if (Process_List[i].P_id == Process_ID)
                    checke = true;
            }
            if (checke == true)
            {
                MessageBox.Show("Two process can't have same Process ID = "
                        + Convert.ToString(Process_ID)
                        + ", at a same time \n Please Enter Process ID again", "Error");
                checke = false;
                return;
            }
            else if (!(Priority.Checked == true || PriorityP.Checked == true))
            {
                Process_List.Add(new Process(Process_ID, Process_Burst, Process_Arrival));
                row = new[]
                              {
                              Convert.ToString(Process_List[Process_List.Count - 1].P_id), 
                              Convert.ToString(Process_List[Process_List.Count - 1].arrival_time),
                              Convert.ToString(Process_List[Process_List.Count - 1].burst_time),
                              "   --",
                              "   --",
                              "   --" 
                            };
            }
            else
            {
                Process_List.Add(new Process(Process_ID, Process_Burst, Process_Arrival, Process_Priorty));
                row = new[] 
							{
                              Convert.ToString(Process_List[Process_List.Count - 1].P_id), 
                              Convert.ToString(Process_List[Process_List.Count - 1].arrival_time),
                              Convert.ToString(Process_List[Process_List.Count - 1].burst_time),
                              Convert.ToString(Process_List[Process_List.Count - 1].priorty),
                              "   --",
                              "   --"
                            };
            }
            dataGridView2.Rows.Add(row);
            checke = false;

            //Process_List.Add(new Process(1, 5, 4));  for debugging
            //Process_List.Add(new Process(2, 5, 0));
            //Process_List.Add(new Process(3, 10, 1));
            //Process_List.Add(new Process(4, 3, 2));
        }

        private void button2_Click(object sender, EventArgs e) //when RUN button is pressed 
        {
            if (Process_List.Count==0)
            {
                MessageBox.Show("Please! Insert data first.","Error");
                return;
            }
            Process_Quantum = Convert.ToInt32(numericUpDown5.Text);

            if (pervAlgo == 0)
            {
                if (SRTF.Checked)
                    SRTFfindavgTime(Process_List, Process_List.Count);
                else if (RR.Checked)
                {
                    if (Process_Quantum == 0)
                    {
                        MessageBox.Show(null, "QUANTUM = 0 !\nEnter QUANTUM and press RUN.", "Invalid Input");
                        return;
                    }
                    else
                        RoundRobin(Process_List, Process_Quantum);
                }
                else if (FCFS.Checked)
                    FCFSTime(Process_List, Process_List.Count);
                else if (SJF.Checked)
                    SJFtime(Process_List, Process_List.Count);
                else if (Priority.Checked)
                    PriorityNonPreemptive(Process_List, Process_List.Count);
                else if (PriorityP.Checked)
                    PriorityPreemptive(Process_List, Process_List.Count);
            }
            else
            {
                if ((PriorityP.Checked || Priority.Checked) && !(pervAlgo == 1 || pervAlgo == 2))
                {
                    MessageBox.Show(null, "Please Again Enter Data with PRIORITY !", "Incompelete Input");
                    return;
                }
                else if (pervAlgo != 6 && RR.Checked)
                {
                    if (Process_Quantum == 0)
                    {
                        MessageBox.Show(null, "QUANTUM = 0 !\nEnter QUANTUM and press RUN.", "Invalid Input");
                        return;
                    }
                    else 
                    {
                        total_turnAround_time = total_waiting_time = 0; X = Y = 16; temp = sum = t = Process_Burst = 0;
                        complete = 0; shortest = 0; finish_time = 0; minimum = 0; insert = checke = false;
                        ganttChart.Clear(); flag.Clear();
                        burst_time.Clear(); Arrival_time.Clear(); service_time.Clear();
                        dataGridView1.Rows.Clear();
                        dataGridView1.Refresh();
                        textBox4.Text = textBox3.Text = "";
                        groupBox3.Controls.Clear();
                        RoundRobin(Process_List,Process_Quantum);
                    }
                }
                else
                {
                    total_turnAround_time = total_waiting_time = 0; X = Y = 16; temp = sum = t = Process_Burst = 0;
                    complete = 0; shortest = 0; finish_time = 0; minimum = 0; insert = checke = false;
                    ganttChart.Clear(); flag.Clear();
                    burst_time.Clear(); Arrival_time.Clear(); service_time.Clear();
                    dataGridView1.Rows.Clear();
                    dataGridView1.Refresh();
                    textBox4.Text = textBox3.Text = "";
                    groupBox3.Controls.Clear();

                    if (SRTF.Checked)
                        SRTFfindavgTime(Process_List, Process_List.Count);
                    else if (FCFS.Checked)
                        FCFSTime(Process_List, Process_List.Count);
                    else if (SJF.Checked)
                        SJFtime(Process_List, Process_List.Count);
                    else if (Priority.Checked)
                        PriorityNonPreemptive(Process_List, Process_List.Count);
                    else if (PriorityP.Checked)
                        PriorityPreemptive(Process_List, Process_List.Count);
                }
            }

            dataGridView2.Visible = false;
            dataGridView1.Visible = true;
            Process_List = Process_List.OrderBy(i => i.P_id).ToList();
            for (int i = 0; i < Process_List.Count; i++)
            {
                if (!(Priority.Checked || PriorityP.Checked))
                row = new[] {
                              Convert.ToString(Process_List[i].P_id), 
                              Convert.ToString(Process_List[i].arrival_time),
                              Convert.ToString(Process_List[i].burst_time),
                              "   --",
                              Convert.ToString(Process_List[i].PturnAround_time),
                              Convert.ToString(Process_List[i].Pwaiting_time) 
                            };
                else
                    row = new[] 
							{
                              Convert.ToString(Process_List[i].P_id), 
                              Convert.ToString(Process_List[i].arrival_time),
                              Convert.ToString(Process_List[i].burst_time),
                              Convert.ToString(Process_List[i].priorty),
                              Convert.ToString(Process_List[i].PturnAround_time),
                              Convert.ToString(Process_List[i].Pwaiting_time) 
                            };

                dataGridView1.Rows.Add(row);
            }

            if ((total_turnAround_time / Process_List.Count) == 0)
                textBox3.Text = "0.00";
            else
                textBox3.Text = (total_turnAround_time / Process_List.Count).ToString("#.##");

            if ((total_waiting_time / Process_List.Count) == 0)
                textBox4.Text = "0.00";
            else
                textBox4.Text = (total_waiting_time / Process_List.Count).ToString("#.##");   
        }

        //REMOVE ALL PROCESSES
        private void button3_Click(object sender, EventArgs e)
        {
            DialogResult dialog = MessageBox.Show("Do you really want to Remove all Process?", "Exit", MessageBoxButtons.YesNo);
            if (dialog == DialogResult.Yes)
            {
                total_turnAround_time = total_waiting_time = 0; X = Y = 16; temp = sum = t = Process_Burst = 0;
                complete = 0; shortest = 0; finish_time = 0; minimum = 0; insert = checke = false;
                Process_List.Clear(); ganttChart.Clear(); flag.Clear(); pervAlgo = 0;
                burst_time.Clear(); Arrival_time.Clear(); service_time.Clear();
                dataGridView1.Rows.Clear();
                dataGridView1.Refresh();
                textBox4.Text = textBox3.Text = "";
                groupBox3.Controls.Clear();
            }
        }

        public void PriorityPreemptive(List<Process> Pro_Lt, int n)
        {
            pervAlgo = 1;
            Process_List = Process_List.OrderBy(i => i.arrival_time).ToList();
            for (int i = 0; i < n; i++)
            {
                Process_List[i].status = 0;
                burst_time.Add(Process_List[i].burst_time);
            }

            while (complete != n)
            {//
                for (int i = 0; i < n; i++)
                {
                    if (Process_List[i].arrival_time <= t && Process_List[i].status != 1)
                    {
                        flag.Add(Process_List[i].priorty);
                        checke = true;
                    }
                }
                if (checke)
                {
                    flag.Sort((a, b) => a.CompareTo(b));
                    for (int i = 0; i < n; i++)
                    {
                        if (radioButton2.Checked)
                        { if (Process_List[i].priorty == flag[flag.Count - 1]) { temp = i; } }
                        else if (radioButton3.Checked)
                        { if (Process_List[i].priorty == flag[0]) { temp = i; } }
                    }
                    ganttChart.Add(Process_List[temp]);
                    burst_time[temp]--;

                    TextBox textBox1 = new TextBox();
                    groupBox3.Controls.Add(textBox1);
                    textBox1.Size = new System.Drawing.Size(25, 25);
                    textBox1.Text = ("P" + Convert.ToString(Process_List[temp].P_id));
                    textBox1.Location = new System.Drawing.Point(X, 34);
                    textBox1.BackColor = Process_List[temp].process_color;
                    textBox1.Name = "textBox1";
                    textBox1.ReadOnly = true;
                    X += textBox1.Width;
                    flag.Clear();

                    if (burst_time[temp] == 0)
                    {
                        complete++;
                        Process_List[temp].status = 1;
                        finish_time = t + 1;
                        Process_List[temp].Pwaiting_time = (finish_time - Process_List[temp].burst_time - Process_List[temp].arrival_time);

                        if (Process_List[temp].Pwaiting_time < 0)
                            Process_List[temp].Pwaiting_time = 0;
                    }
                }
                t++;
                checke = false;
                flag.Clear();
            }
            for (int i = 0; i < n; i++)
                Process_List[i].PturnAround_time = Process_List[i].burst_time + Process_List[i].Pwaiting_time;
            for (int i = 0; i < n; i++)
            {
                total_waiting_time += Process_List[i].Pwaiting_time;
                total_turnAround_time += Process_List[i].PturnAround_time;
            }
            printGanttChart(ganttChart);
        }

        public void PriorityNonPreemptive(List<Process> Pro_Lt, int n)
        {
            pervAlgo = 2;
            Process_List = Process_List.OrderBy(i => i.arrival_time).ToList();
            for (int i = 0; i < n; i++)
            {
                Process_List[i].status = 0;
                Process_Burst += Process_List[i].burst_time;
            }
            for (t = Process_List[0].arrival_time; t < Process_Burst; )
            {
                for (int i = 0; i < n; i++)
                {
                    if (Process_List[i].arrival_time <= t && Process_List[i].status != 1 )
                    {
                        flag.Add(Process_List[i].priorty);
                        checke = true;
                    }
                }
                if (checke)
                {
                    flag.Sort((a, b) => a.CompareTo(b));
                    for (int i = 0; i < n; i++)
                    {
                        if (radioButton2.Checked)
                        {
                            if (Process_List[i].priorty == flag[flag.Count - 1])
                            {
                                temp = i;
                            }
                        }
                        else if (radioButton3.Checked)
                        {
                            if (Process_List[i].priorty == flag[0])
                            {
                                temp = i;
                            }
                        }
                    }

                    t += Process_List[temp].burst_time;
                    Process_List[temp].compeletion_time = t;
                    Process_List[temp].Pwaiting_time = (Process_List[temp].compeletion_time - Process_List[temp].arrival_time - Process_List[temp].burst_time);
                    Process_List[temp].PturnAround_time = (Process_List[temp].compeletion_time - Process_List[temp].arrival_time);
                    Process_List[temp].status = 1;
                    flag.Clear();

                    for (int z = 0; z < Process_List[temp].burst_time; z++)
                    {
                        ganttChart.Add(Process_List[temp]);
                        TextBox textBox1 = new TextBox();
                        groupBox3.Controls.Add(textBox1);
                        textBox1.Size = new System.Drawing.Size(25, 25);
                        textBox1.Text = ("P" + Convert.ToString(Process_List[temp].P_id));
                        textBox1.Location = new System.Drawing.Point(X, 34);
                        textBox1.BackColor = Process_List[temp].process_color;
                        textBox1.Name = "textBox1";
                        textBox1.ReadOnly = true;
                        X += textBox1.Width;
                    }
                }
                else
                    t++;
                    checke = false;
            }
            for (int i = 0; i < n; i++)
            {
                total_waiting_time += Process_List[i].Pwaiting_time;
                total_turnAround_time += Process_List[i].PturnAround_time;
            }
            printGanttChart(ganttChart);
        }
        
        public void SJFtime(List<Process> Process_List, int n)
        {
            pervAlgo = 3;
            for (int i = 0; i < n; i++)
            {
                service_time.Add(i);
                flag.Add(0);
            }
            while (true)
            {
                temp = n;
                minimum = 999;
                if (sum == n) // if all process are completed loop will be terminated
                    break;

                for (int i = 0; i < n; i++)
                {
                    if ((Process_List[i].arrival_time <= t) && (flag[i] == 0) && (Process_List[i].burst_time < minimum))
                    {
                        minimum = Process_List[i].burst_time;
                        temp = i;
                    }
                }

                // If temp=n means no procss can be so move forward in time 
                if (temp == n)
                    t++;
                else
                {
                    service_time[temp] = t + Process_List[temp].burst_time;
                    t += Process_List[temp].burst_time;
                    Process_List[temp].PturnAround_time = service_time[temp] - Process_List[temp].arrival_time;
                    Process_List[temp].Pwaiting_time = Process_List[temp].PturnAround_time - Process_List[temp].burst_time;
                    flag[temp] = 1;

                    for (int z = 0; z < Process_List[temp].burst_time; z++)
                    {
                        ganttChart.Add(Process_List[temp]);
                        TextBox textBox1 = new TextBox();
                        groupBox3.Controls.Add(textBox1);
                        textBox1.Size = new System.Drawing.Size(25, 25);
                        textBox1.Text = ("P" + Convert.ToString(Process_List[temp].P_id));
                        textBox1.Location = new System.Drawing.Point(X, 34);
                        textBox1.BackColor = Process_List[temp].process_color;
                        textBox1.Name = "textBox1";
                        textBox1.ReadOnly = true;
                        X += textBox1.Width;
                    }
                    sum++;
                }
            }

            for (int i = 0; i < n; i++)
            {
                total_waiting_time += Process_List[i].Pwaiting_time;
                total_turnAround_time += Process_List[i].PturnAround_time;
            }
            printGanttChart(ganttChart);
        }

        //method to calculate time by FCFS algorithm
        public void FCFSTime(List<Process> Process_List, int n)
        {
            pervAlgo = 4;
            Process_List =  Process_List.OrderBy(i => i.arrival_time).ToList();
            for (int i = 0; i < n; i++)
            {
                service_time.Add(i);
            }
            Process_List[0].Pwaiting_time = 0;
            Process_List[0].PturnAround_time = Process_List[0].burst_time;
            // calculating waiting time  

            for (int i = 1; i < n; i++)
            {
                // Add burst time of previous processes
                service_time[i] = service_time[i - 1] + Process_List[i - 1].burst_time;

                // Find waiting time for current process =  
                // sum - at[i]  
                Process_List[i].Pwaiting_time = service_time[i] - Process_List[i].arrival_time;
                Process_List[i].PturnAround_time = Process_List[i].burst_time + Process_List[i].Pwaiting_time;
                if (Process_List[i].Pwaiting_time < 0)
                {
                    Process_List[i].Pwaiting_time = 0;
                }
                //printing Gant chart
                for (int z = 0; z < Process_List[i-1].burst_time; z++)
                {
                    ganttChart.Add(Process_List[i-1]);
                    TextBox textBox1 = new TextBox();
                    groupBox3.Controls.Add(textBox1);
                    textBox1.Size = new System.Drawing.Size(25, 25);
                    textBox1.Text = ("P" + Convert.ToString(Process_List[i-1].P_id));
                    textBox1.Location = new System.Drawing.Point(X, 34);
                    textBox1.BackColor = Process_List[i-1].process_color;
                    textBox1.Name = "textBox1";
                    textBox1.ReadOnly = true;
                    X += textBox1.Width;
                }
            }
            for (int z = 0; z < Process_List[n-1].burst_time; z++)
            {
                ganttChart.Add(Process_List[n-1]);
                TextBox textBox1 = new TextBox();
                groupBox3.Controls.Add(textBox1);
                textBox1.Size = new System.Drawing.Size(25, 25);
                textBox1.Text = ("P" + Convert.ToString(Process_List[n-1].P_id));
                textBox1.Location = new System.Drawing.Point(X, 34);
                textBox1.BackColor = Process_List[n-1].process_color;
                textBox1.Name = "textBox1";
                textBox1.ReadOnly = true;
                X += textBox1.Width;
            }
          
            for (int i = 0; i < n; i++)
            {
                total_waiting_time += Process_List[i].Pwaiting_time;
                total_turnAround_time += Process_List[i].PturnAround_time;
            }
            printGanttChart(ganttChart);
        }



        // Method that calculates average time
        public void SRTFfindavgTime(List<Process> Process_List, int n)
        {
            pervAlgo = 5;
            minimum = int.MaxValue;
            // Copy the burst time into rt[] 
            for (int i = 0; i < n; i++)
            {
                burst_time.Add(Process_List[i].burst_time); // making new burst time list 
            }
            //do until all processes are done
            while (complete != n)
            {
                //find minimn remaing time process, from process arrived till now
                for (int j = 0; j < n; j++)
                {
                    if ((Process_List[j].arrival_time <= t) && (burst_time[j] < minimum) && burst_time[j] > 0)
                    {
                        minimum = burst_time[j];
                        shortest = j;
                        checke = true;
                    }
                }

                if (checke == false)
                {
                    t++;
                    continue;
                }
                ganttChart.Add(Process_List[shortest]);
                // reduce remaining time by one 
                burst_time[shortest]--;

                //printing Gant chart
                TextBox textBox1 = new TextBox();
                groupBox3.Controls.Add(textBox1);
                textBox1.Size = new System.Drawing.Size(25, 25);
                textBox1.Text = ("P" + Convert.ToString(Process_List[shortest].P_id));
                textBox1.Location = new System.Drawing.Point(X, 34);
                textBox1.BackColor = Process_List[shortest].process_color;
                textBox1.Name = "textBox1";
                textBox1.ReadOnly = true;

                X += textBox1.Width;

                // Update minimum 
                minimum = burst_time[shortest];

                //if prcess is done executing
                if (burst_time[shortest] == 0)
                {
                    minimum = int.MaxValue;
                    // Increment complete 
                    complete++;
                    checke = false;

                    //finish time in array
                    finish_time = t + 1;
                    // waiting time calculate
                    Process_List[shortest].Pwaiting_time = finish_time - Process_List[shortest].burst_time - Process_List[shortest].arrival_time;
                    Process_List[shortest].PturnAround_time = Process_List[shortest].burst_time + Process_List[shortest].Pwaiting_time;

                    if (Process_List[shortest].Pwaiting_time < 0)
                        Process_List[shortest].Pwaiting_time = 0;
                }
                // Move next in time 
                t++;
            }
            printGanttChart(ganttChart);
            for (int i = 0; i < n; i++)
            {
                total_waiting_time += Process_List[i].Pwaiting_time;
                total_turnAround_time += Process_List[i].PturnAround_time;
            }
        }

        public void RoundRobin(List<Process> Process_List, int quantum)
        {
            pervAlgo = 6;
            Process_List = Process_List.OrderBy(i => i.arrival_time).ToList();
            for (int i = 0; i < Process_List.Count(); i++)
            {
                Arrival_time.Add(Process_List[i].arrival_time);
                burst_time.Add(Process_List[i].burst_time); // making new burst time list 
            }
            while (true) 
            { 
                checke = true; 
                for (int i = 0; i < Process_List.Count; i++) 
                { 
                    // these condition for if arivaltime !=0 && check that if there come before qtime 
                    if (Arrival_time[i] <= t) 
                    { 
                        if (Arrival_time[i] <= quantum) 
                        { 
                            if (burst_time[i] > 0) 
                            { 
                                checke = false; 
                                if (burst_time[i] > quantum) 
                                { 
                                    // make decrease the b time 
                                    t = t + quantum;
                                    burst_time[i] = burst_time[i]-quantum;
                                    Arrival_time[i] = Arrival_time[i]+ quantum;
                                    for (int h = 0; h < quantum; h++)
                                    {
                                        ganttChart.Add(Process_List[i]);
                                        //printing Gant chart
                                        TextBox textBox1 = new TextBox();
                                        groupBox3.Controls.Add(textBox1);
                                        textBox1.Size = new System.Drawing.Size(25, 25);
                                        textBox1.Text = ("P" + Convert.ToString(Process_List[i].P_id));
                                        textBox1.Location = new System.Drawing.Point(X, 34);
                                        textBox1.BackColor = Process_List[i].process_color;
                                        textBox1.Name = "textBox1";
                                        textBox1.ReadOnly = true;
                                        X += textBox1.Width;
                                    }

                                } 
                                else 
                                { 
                                    // for last time 
                                    t = t + burst_time[i];
                                    for (int h = 0; h < burst_time[i]; h++)
                                    {
                                        ganttChart.Add(Process_List[i]);
                                        //printing Gant chart
                                        TextBox textBox1 = new TextBox();
                                        groupBox3.Controls.Add(textBox1);
                                        textBox1.Size = new System.Drawing.Size(25, 25);
                                        textBox1.Text = ("P" + Convert.ToString(Process_List[i].P_id));
                                        textBox1.Location = new System.Drawing.Point(X, 34);
                                        textBox1.BackColor = Process_List[i].process_color;
                                        textBox1.Name = "textBox1";
                                        textBox1.ReadOnly = true;
                                        X += textBox1.Width;
                                    }
                                    // store comp time 
                                    Process_List[i].PturnAround_time = t - Process_List[i].arrival_time; 
                                    // store wait time 
                                    Process_List[i].Pwaiting_time = t - Process_List[i].burst_time - Process_List[i].arrival_time;
                                    burst_time[i] = 0; 

                                } 
                            } 
                        } 
                        else if (Arrival_time[i] > quantum) 
                        { 
                            // is any have less arrival time 
                            // the coming process then execute them 
                            for (int j = 0; j < Process_List.Count; j++) 
                            { 
                                // compare 
                                if (Arrival_time[j] < Arrival_time[i]) 
                                { 
                                    if (burst_time[j] > 0) 
                                    { 
                                        checke = false; 
                                        if (burst_time[j] > quantum) 
                                        { 
                                            t = t + quantum;
                                            burst_time[j] = burst_time[j] - quantum;
                                            Arrival_time[j] = Arrival_time[j] + quantum;
                                            for (int h = 0; h < quantum; h++)
                                            {
                                                ganttChart.Add(Process_List[j]);
                                                //printing Gant chart
                                                TextBox textBox1 = new TextBox();
                                                groupBox3.Controls.Add(textBox1);
                                                textBox1.Size = new System.Drawing.Size(25, 25);
                                                textBox1.Text = ("P" + Convert.ToString(Process_List[j].P_id));
                                                textBox1.Location = new System.Drawing.Point(X, 34);
                                                textBox1.BackColor = Process_List[j].process_color;
                                                textBox1.Name = "textBox1";
                                                textBox1.ReadOnly = true;
                                                X += textBox1.Width;
                                            }
                                        } 
                                        else 
                                        { 
                                            t = t + burst_time[j];
                                            for (int h = 0; h < burst_time[j]; h++)
                                            {
                                                ganttChart.Add(Process_List[j]);
                                                //printing Gant chart
                                                TextBox textBox1 = new TextBox();
                                                groupBox3.Controls.Add(textBox1);
                                                textBox1.Size = new System.Drawing.Size(25, 25);
                                                textBox1.Text = ("P" + Convert.ToString(Process_List[j].P_id));
                                                textBox1.Location = new System.Drawing.Point(X, 34);
                                                textBox1.BackColor = Process_List[j].process_color;
                                                textBox1.Name = "textBox1";
                                                textBox1.ReadOnly = true;
                                                X += textBox1.Width;
                                            }
                                            Process_List[j].PturnAround_time = t - Process_List[j].arrival_time; 
                                            Process_List[j].Pwaiting_time = t - Process_List[j].burst_time - Process_List[j].arrival_time;
                                            burst_time[j] = 0;
                                        } 
                                    } 
                                } 
                            } 
  
                            // now the previous porcess according to 
                            // ith is process 
                            if (burst_time[i] > 0) 
                            { 
                                checke = false; 
  
                                // Check for greaters 
                                if (burst_time[i] > quantum) 
                                { 
                                    t = t + quantum;
                                    burst_time[i] = burst_time[i] - quantum;
                                    Arrival_time[i] = Arrival_time[i] + quantum;
                                    for (int h = 0; h < quantum; h++)
                                    {
                                        ganttChart.Add(Process_List[i]);
                                        //printing Gant chart
                                        TextBox textBox1 = new TextBox();
                                        groupBox3.Controls.Add(textBox1);
                                        textBox1.Size = new System.Drawing.Size(25, 25);
                                        textBox1.Text = ("P" + Convert.ToString(Process_List[i].P_id));
                                        textBox1.Location = new System.Drawing.Point(X, 34);
                                        textBox1.BackColor = Process_List[i].process_color;
                                        textBox1.Name = "textBox1";
                                        textBox1.ReadOnly = true;
                                        X += textBox1.Width;
                                    }
                                } 
                                else 
                                { 
                                    t = t + burst_time[i];
                                    for (int h = 0; h < burst_time[i]; h++)
                                    {
                                        ganttChart.Add(Process_List[i]);
                                        //printing Gant chart
                                        TextBox textBox1 = new TextBox();
                                        groupBox3.Controls.Add(textBox1);
                                        textBox1.Size = new System.Drawing.Size(25, 25);
                                        textBox1.Text = ("P" + Convert.ToString(Process_List[i].P_id));
                                        textBox1.Location = new System.Drawing.Point(X, 34);
                                        textBox1.BackColor = Process_List[i].process_color;
                                        textBox1.Name = "textBox1";
                                        textBox1.ReadOnly = true;
                                        X += textBox1.Width;
                                    }
                                    Process_List[i].PturnAround_time = t - Process_List[i].arrival_time; 
                                    Process_List[i].Pwaiting_time = t - Process_List[i].burst_time - Process_List[i].arrival_time;
                                    burst_time[i] = 0;
                                } 
                            } 
                        } 
                    } 
  
                    // if no process is come on thse critical 
                    else if (Arrival_time[i] > t) 
                    {    
                        t++;
                        i--; 
                    } 
                } 
                // for exit the while loop 
                if (checke) 
                { 
                    break; 
                } 
            } 
  
            for (int i = 0; i < Process_List.Count; i++)
            {
                total_waiting_time += Process_List[i].Pwaiting_time;
                total_turnAround_time += Process_List[i].PturnAround_time;
            }
            printGanttChart(ganttChart);
        }
        // print gant chart
        public void printGanttChart(List<Process> ganttChart)
        {
            int num = 0;
            TextBox textBox6 = new TextBox();
            groupBox3.Controls.Add(textBox6);
            textBox6.Name = "textBox6";
            textBox6.ReadOnly = true;
            textBox6.Size = new System.Drawing.Size(25, 25);
            textBox6.BackColor = ganttChart[0].process_color;
            textBox6.Location = new System.Drawing.Point(Y, 59);
            textBox6.Text = (0.ToString());

            for (; num < ganttChart.Count; num++)
            {
                if (num < (ganttChart.Count - 1))
                {
                    if (ganttChart[num].P_id != ganttChart[num + 1].P_id)
                    {
                        TextBox textBox2 = new TextBox();
                        groupBox3.Controls.Add(textBox2);
                        textBox2.Name = "textBox2";
                        textBox2.ReadOnly = true;
                        textBox2.Size = new System.Drawing.Size(25, 25);
                        textBox2.BackColor = ganttChart[num].process_color;
                        textBox2.Location = new System.Drawing.Point(Y, 59);
                        textBox2.Text = (num.ToString());

                        TextBox textBox5 = new TextBox();
                        groupBox3.Controls.Add(textBox5);
                        textBox5.Name = "textBox5";
                        textBox5.ReadOnly = true;
                        textBox5.Size = new System.Drawing.Size(25, 25);
                        textBox5.BackColor = ganttChart[num + 1].process_color;
                        textBox5.Location = new System.Drawing.Point((Y + textBox2.Width), 59);
                        textBox5.Text = ((num + 1).ToString());
                    }
                    Y += 25;
                }
                if (num == (ganttChart.Count - 1))
                {
                    TextBox textBox7 = new TextBox();
                    groupBox3.Controls.Add(textBox7);
                    textBox7.ReadOnly = true;
                    textBox7.Name = "textBox7";
                    textBox7.Size = new System.Drawing.Size(25, 25);
                    if (num != 0)
                        textBox7.BackColor = ganttChart[num - 1].process_color;
                    else
                        textBox7.BackColor = ganttChart[num].process_color;
                    textBox7.Location = new System.Drawing.Point(Y, 59);
                    textBox7.Text = (num.ToString());
                }
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Form2 form2 = new Form2();
            form2.ShowDialog();
        }
    }
}
