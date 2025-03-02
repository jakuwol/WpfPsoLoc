using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using static System.Formats.Asn1.AsnWriter;

namespace WpfPsoLoc
{

    public class Station
    { 
        public string Name { get; set; }
        public double X { get; set; }
        public double Y{ get; set; }
        public double Z { get; set; }
        public double Time { get; set; }
    }

    public class PsoScore
    {
        public double Score { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public double Velocity { get; set; }
        public double DeltaT { get; set; }
    }

    public class PsoConfig
    {
        public int NrOfParticles;
        public int MaxIteration;
        public double TimeError;
        public double W;
        public double C1;
        public double C2;
        public double Velocity;
        public double XExp;
        public double YExp;
        public double ZUpExp;
        public double ZLowExp;
        public double DeltaT;
    }


    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        bool CalcInProgress = false;

        public ObservableCollection<Station> StationsList { get; set; }
        public ObservableCollection<PsoScore> PsoScoreList { get; set; }

        private int _NrOfParticles;
        public int NrOfParticles
        {
            get { return _NrOfParticles; }
            set
            {
                _NrOfParticles = value;
                OnPropertyChanged("NrOfParticles");
            }
        }

        private int _MaxIteration;
        public int MaxIteration
        {
            get { return _MaxIteration; }
            set
            {
                _MaxIteration = value;
                OnPropertyChanged("MaxIteration");
            }
        }

        private double _Velocity;
        public double Velocity
        {
            get { return _Velocity; }
            set
            {
                _Velocity = value;
                OnPropertyChanged("Velocity");
            }
        }

        private double _TimeError;
        public double TimeError
        {
            get { return _TimeError; }
            set
            {
                _TimeError = value;
                OnPropertyChanged("TimeError");
            }
        }

        private double _WCoef;
        public double WCoef
        {
            get { return _WCoef; }
            set
            {
                _WCoef = value;
                OnPropertyChanged("WCoef");
            }
        }

        private double _C1Coef;
        public double C1Coef
        {
            get { return _C1Coef; }
            set
            {
                _C1Coef = value;
                OnPropertyChanged("C1Coef");
            }
        }

        private double _C2Coef;
        public double C2Coef
        {
            get { return _C2Coef; }
            set
            {
                _C2Coef = value;
                OnPropertyChanged("C2Coef");
            }
        }

        private double _XExpand;
        public double XExpand
        {
            get { return _XExpand; }
            set
            {
                _XExpand = value;
                OnPropertyChanged("XExpand");
            }
        }

        private double _YExpand;
        public double YExpand
        {
            get { return _YExpand; }
            set
            {
                _YExpand = value;
                OnPropertyChanged("YExpand");
            }
        }

        private double _ZUpperExpand;
        public double ZUpperExpand
        {
            get { return _ZUpperExpand; }
            set
            {
                _ZUpperExpand = value;
                OnPropertyChanged("ZUpperExpand");
            }
        }

        private double _ZLowerExpand;
        public double ZLowerExpand
        {
            get { return _ZLowerExpand; }
            set
            {
                _ZLowerExpand = value;
                OnPropertyChanged("ZLowerExpand");
            }
        }

        private double _Vmax;
        public double Vmax
        {
            get { return _Vmax; }
            set
            {
                _Vmax = value;
                OnPropertyChanged("Vmax");
            }
        }

        private double _Vmin;
        public double Vmin
        {
            get { return _Vmin; }
            set
            {
                _Vmin = value;
                OnPropertyChanged("Vmin");
            }
        }

        private double _Vinc;
        public double Vinc
        {
            get { return _Vinc; }
            set
            {
                _Vinc = value;
                OnPropertyChanged("Vinc");
            }
        }

        private double _DeltaTmin;
        public double DeltaTmin
        {
            get { return _DeltaTmin; }
            set
            {
                _DeltaTmin = value;
                OnPropertyChanged("DeltaTmin");
            }
        }

        private double _DeltaTmax;
        public double DeltaTmax
        {
            get { return _DeltaTmax; }
            set
            {
                _DeltaTmax = value;
                OnPropertyChanged("DeltaTmax");
            }
        }

        private double _DeltaTinc;
        public double DeltaTinc
        {
            get { return _DeltaTinc; }
            set
            {
                _DeltaTinc = value;
                OnPropertyChanged("DeltaTinc");
            }
        }


        private double _VValue;
        public double VValue
        {
            get { return _VValue; }
            set
            {
                _VValue = value;
                OnPropertyChanged("VValue");
            }
        }

        private double _VDeltaTValue;
        public double VDeltaTValue
        {
            get { return _VDeltaTValue; }
            set
            {
                _VDeltaTValue = value;
                OnPropertyChanged("VDeltaTValue");
            }
        }

        private string _TxtResult;
        public string TxtResult
        {
            get { return _TxtResult; }
            set
            {
                if (string.Equals(value, _TxtResult))
                    return;
                _TxtResult = value;
                OnPropertyChanged("TxtResult");
            }
        }

        private bool _XAxisFilp;
        public bool XAxisFilp
        {
            get { return _XAxisFilp; }
            set
            {
                _XAxisFilp = value;
                if (_XAxisFilp)
                    XAxisFilpV = -1;
                else
                    XAxisFilpV = 1;
                OnPropertyChanged("XAxisFilp");
            }
        }


        private int _XAxisFilpV;
        public int XAxisFilpV
        {
            get { return _XAxisFilpV; }
            set
            {
                _XAxisFilpV = value;
                OnPropertyChanged("XAxisFilpV");
            }
        }

        private bool _YAxisFilp;
        public bool YAxisFilp
        {
            get { return _YAxisFilp; }
            set
            {
                _YAxisFilp = value;
                if (_YAxisFilp)
                    YAxisFilpV = -1;
                else
                    YAxisFilpV = 1;
                OnPropertyChanged("YAxisFilp");
            }
        }

        private int _YAxisFilpV;
        public int YAxisFilpV
        {
            get { return _YAxisFilpV; }
            set
            {
                _YAxisFilpV = value;
                OnPropertyChanged("YAxisFilpV");
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            StationsList = new ObservableCollection<Station>();
            PsoScoreList = new ObservableCollection<PsoScore>();

            if(!Button_Load_Settings())
            {
                Velocity = 4200;
                TimeError = 0.05;
                MaxIteration = 200;
                NrOfParticles = 400;
                WCoef = 0.7;
                C1Coef = 1.5;
                C2Coef = 1.5;
                XExpand = 500;
                YExpand = 500;
                ZUpperExpand = 50;
                ZLowerExpand = 50;
                Vmin = 4000;
                Vmax = 5500;
                Vinc = 50;
                DeltaTmin = 0.005;
                DeltaTinc = 0.005;
                DeltaTmax = 0.09;
                XAxisFilp = false;
                YAxisFilp = false;
            }
        }

        private void Load_Click(object sender, RoutedEventArgs e)
        {
            // Configure open file dialog box
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.FileName = "Document"; // Default file name
            dialog.DefaultExt = ".txt"; // Default file extension
            dialog.Filter = "Text documents (.txt)|*.txt"; // Filter files by extension

            // Show open file dialog box
            bool? result = dialog.ShowDialog();
            // Process open file dialog box results
            if (result == true)
            {
                // Open document
                string filename = dialog.FileName;

                try
                {
                    // Open the text file using a stream reader.
                    using StreamReader reader = new(filename);
                    List<Station> tempList = new List<Station>();
                    StationsList.Clear();
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        if (line != null)
                        {
                            Station station = new Station();
                            string[] lines = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                            if (lines.Length > 0)
                            {
                                station.Name = lines[0];
                                station.X = double.Parse(lines[1], CultureInfo.InvariantCulture);
                                station.Y = double.Parse(lines[2], CultureInfo.InvariantCulture);
                                station.Z = double.Parse(lines[3], CultureInfo.InvariantCulture);
                                station.Time = double.Parse(lines[4], CultureInfo.InvariantCulture);

                                tempList.Add(station);
                            }
                        }
                    }
                    //sortowanie listy stacji od najmnjszego czasu
                    var temp = tempList.OrderBy(x => x.Time);
                    foreach (var item in temp)
                        StationsList.Add(item);

                    PsoScoreList.Clear();
                }
                catch
                {

                }
            }
        }

        private PsoScore PsoLocalization(PsoConfig Params, List<Station> stationsList)
        {
            PsoScore score = new PsoScore();

            double[] measured_times = new double[stationsList.Count];
            double[,] sensor_coords = new double[stationsList.Count, 3];
            double[] time_errors = new double[stationsList.Count];

            for (int i = 0; i < stationsList.Count; i++)
            {
                measured_times[i] = stationsList[i].Time - stationsList[0].Time + Params.DeltaT;
                sensor_coords[i, 0] = stationsList[i].X;
                sensor_coords[i, 1] = stationsList[i].Y;
                sensor_coords[i, 2] = stationsList[i].Z;
                time_errors[i] = TimeError;
            }

            double xmin = stationsList.Min(x => x.X) - Params.XExp;
            double xmax = stationsList.Max(x => x.X) + Params.XExp;
            double ymin = stationsList.Min(x => x.Y) - Params.YExp;
            double ymax = stationsList.Max(x => x.Y) + Params.YExp;
            double zmin = stationsList.Min(x => x.Z) - Params.ZLowExp;
            double zmax = stationsList.Max(x => x.Z) + Params.ZUpExp;

            // Inicjalizacja pozycji i prędkości cząstek
            var rand = new Random();
            double[,] particle_positions = new double[Params.NrOfParticles, 3];
            double[,] personal_best_positions = new double[Params.NrOfParticles, 3];
            for (int i = 0; i < Params.NrOfParticles; i++)
            {
                personal_best_positions[i, 0] = particle_positions[i, 0] = (rand.NextDouble() * (xmax - xmin)) + xmin;
                personal_best_positions[i, 1] = particle_positions[i, 1] = (rand.NextDouble() * (ymax - ymin)) + ymin;
                personal_best_positions[i, 2] = particle_positions[i, 2] = (rand.NextDouble() * (zmax - zmin)) + zmin;
            }
            double[,] particle_velocities = new double[Params.NrOfParticles, 3];
            for (int i = 0; i < Params.NrOfParticles; i++)
            {
                particle_velocities[i, 0] = 0;
                particle_velocities[i, 1] = 0;
                particle_velocities[i, 2] = 0;
            }

            //Ocena początkowych pozycji cząstek
            double[] personal_best_scores = new double[Params.NrOfParticles];
            int minidx = 0;
            double global_best_score = 0;
            for (int i = 0; i < Params.NrOfParticles; i++)
            {
                personal_best_scores[i] = objective_function(particle_positions[i, 0],
                                       particle_positions[i, 1],
                                       particle_positions[i, 2],
                                       Params.Velocity,
                                       measured_times,
                                       sensor_coords,
                                       time_errors);
                if (i == 0)
                    global_best_score = personal_best_scores[i];
                if ((i > 0) && (personal_best_scores[i] < personal_best_scores[i - 1]))
                {
                    minidx = i;
                    global_best_score = personal_best_scores[i];
                }

            }

            double[] global_best_position = new double[3];
            global_best_position[0] = particle_positions[minidx, 0];
            global_best_position[1] = particle_positions[minidx, 1];
            global_best_position[2] = particle_positions[minidx, 2];


            //PSO Main Loop
            for (int i = 0; i < Params.MaxIteration; i++)
            {
                for (int j = 0; j < Params.NrOfParticles; j++)
                {
                    //Aktualizacja prędkości
                    particle_velocities[j, 0] = Params.W * particle_velocities[j, 0] +
                        Params.C1 * rand.NextDouble() * (personal_best_positions[j, 0] - particle_positions[j, 0]) +
                        Params.C2 * rand.NextDouble() * (global_best_position[0] - particle_positions[j, 0]);

                    particle_velocities[j, 1] = Params.W * particle_velocities[j, 1] +
                        Params.C1 * rand.NextDouble() * (personal_best_positions[j, 1] - particle_positions[j, 1]) +
                        Params.C2 * rand.NextDouble() * (global_best_position[1] - particle_positions[j, 1]);

                    particle_velocities[j, 2] = Params.W * particle_velocities[j, 2] +
                        Params.C1 * rand.NextDouble() * (personal_best_positions[j, 2] - particle_positions[j, 2]) +
                        Params.C2 * rand.NextDouble() * (global_best_position[2] - particle_positions[j, 2]);

                    //Aktualizacja pozycji
                    particle_positions[j, 0] = particle_positions[j, 0] + particle_velocities[j, 0];
                    particle_positions[j, 1] = particle_positions[j, 1] + particle_velocities[j, 1];
                    particle_positions[j, 2] = particle_positions[j, 2] + particle_velocities[j, 2];

                    //Ograniczenie pozycji do zadanego zakresu
                    particle_positions[j, 0] = Math.Min(Math.Max(particle_positions[j, 0], xmin), xmax);
                    particle_positions[j, 1] = Math.Min(Math.Max(particle_positions[j, 1], ymin), ymax);
                    particle_positions[j, 2] = Math.Min(Math.Max(particle_positions[j, 2], zmin), zmax);

                    // Ocena nowej pozycji
                    double current_score = objective_function(particle_positions[j, 0], particle_positions[j, 1], particle_positions[j, 2], Params.Velocity,
                        measured_times,
                       sensor_coords,
                       time_errors);

                    //Aktualizacja najlepszego osobistego wyniku
                    if (current_score < personal_best_scores[j])
                    {
                        personal_best_scores[j] = current_score;
                        personal_best_positions[j, 0] = particle_positions[j, 0];
                        personal_best_positions[j, 1] = particle_positions[j, 1];
                        personal_best_positions[j, 2] = particle_positions[j, 2];
                    }
                    //Aktualizacja najlepszego globalnego wyniku
                    if (current_score < global_best_score)
                    {
                        global_best_score = current_score;
                        global_best_position[0] = particle_positions[j, 0];
                        global_best_position[1] = particle_positions[j, 1];
                        global_best_position[2] = particle_positions[j, 2];
                    }
                }
            }

            score.Score = global_best_score;
            score.X = global_best_position[0];
            score.Y = global_best_position[1];
            score.Z = global_best_position[2];
            score.Velocity = Params.Velocity;
            score.DeltaT = Params.DeltaT;
            return score;
        }

        private void DrowMap(double gx, double gy)
        {
            this.Dispatcher.Invoke(() =>
            {

                double xmin = StationsList.Min(x => x.X) - XExpand;
                double xmax = StationsList.Max(x => x.X) + XExpand;
                double ymin = StationsList.Min(x => x.Y) - YExpand;
                double ymax = StationsList.Max(x => x.Y) + YExpand;
                double zmin = StationsList.Min(x => x.Z) - ZLowerExpand;
                double zmax = StationsList.Max(x => x.Z) + ZUpperExpand;

                Canvas_Plot.Children.Clear();
                double width = Canvas_Plot.Width;
                double height = Canvas_Plot.Height;
                //draw grid
                for (double i = xmin; i <= xmax; i += 500)
                {
                    Line el = new Line();
                    el.Stroke = System.Windows.Media.Brushes.Gray;
                    el.StrokeDashArray = new DoubleCollection() { 8, 8 };
                    el.StrokeThickness = 1;
                    double xx = (width / (xmax - xmin)) * (i - xmin);
                    el.HorizontalAlignment = HorizontalAlignment.Left;
                    el.VerticalAlignment = VerticalAlignment.Bottom;

                    el.Y1 = 1;
                    el.Y2 = height;
                    el.X1 = xx;
                    el.X2 = xx;
                    Canvas_Plot.Children.Add(el);
                }
                for (double i = ymin; i <= ymax; i += 500)
                {
                    Line el = new Line();
                    el.Stroke = System.Windows.Media.Brushes.Gray;
                    el.StrokeDashArray = new DoubleCollection() { 8, 8 };
                    el.StrokeThickness = 1;
                    double xx = (height / (ymax - ymin)) * (i - ymin);
                    el.HorizontalAlignment = HorizontalAlignment.Left;
                    el.VerticalAlignment = VerticalAlignment.Bottom;
                    el.Y1 = xx;
                    el.Y2 = xx;
                    el.X1 = 1;
                    el.X2 = width;
                    Canvas_Plot.Children.Add(el);
                }

                //draw station
                foreach (var s in StationsList)
                {
                    Ellipse el = new Ellipse();
                    el.Height = 10;
                    el.Width = 10;
                    el.Stroke = Brushes.Blue;
                    double xx = (width / (xmax - xmin)) * (s.X - xmin);
                    double yy = (height / (ymax - ymin)) * (s.Y - ymin);
                    Canvas.SetLeft(el, xx);
                    Canvas.SetTop(el, yy);
                    Canvas_Plot.Children.Add(el);
                }
                Ellipse el2 = new Ellipse();
                el2.Height = 10;
                el2.Width = 10;
                el2.Stroke = Brushes.Black;
                el2.Fill = Brushes.Red;
                double xx2 = (width / (xmax - xmin)) * (gx - xmin);
                double yy2 = (height / (ymax - ymin)) * (gy - ymin);
                Canvas.SetLeft(el2, xx2);
                Canvas.SetTop(el2, yy2);
                Canvas_Plot.Children.Add(el2);
            });
        }

        private async void Run_Click(object sender, RoutedEventArgs e)
        {
            double[] velocities = new double[0];
            double[] timeoffsets = new double[0];

            PsoScoreList.Clear();

            for (double v = Vmin; v < Vmax; v += Vinc)
            {
                velocities = velocities.Concat(new double[] { v }).ToArray();
            }
            velocities = velocities.Concat(new double[] { Vmax }).ToArray();
            for (double dt = DeltaTmin; dt < DeltaTmax; dt += DeltaTinc)
            {
                timeoffsets = timeoffsets.Concat(new double[] { dt }).ToArray();
            }
            timeoffsets = timeoffsets.Concat(new double[] { DeltaTmax }).ToArray();

            ProgressWindow wprogress = new ProgressWindow();
            wprogress.Show();
            Task t = Task.Run(() =>
            {
                CalcInProgress = true;
                int totaliteration = velocities.Length * timeoffsets.Length;
                int iteration = 0;
                foreach (double v in velocities)
                {
                    foreach (double dtime in timeoffsets)
                    {
                        wprogress.ProgressText = String.Format("Velocity: {0:N0}/DeltaTime: {1:N3}", v, dtime);
                        wprogress.ProgressValue = ((double)iteration++ / (double)totaliteration) * 100;

                        PsoConfig config = new PsoConfig()
                        {
                            NrOfParticles = NrOfParticles,
                            MaxIteration = MaxIteration,
                            C1 = C1Coef,
                            C2 = C2Coef,
                            W = WCoef,
                            TimeError = TimeError,
                            XExp = XExpand,
                            YExp = YExpand,
                            ZUpExp = ZUpperExpand,
                            ZLowExp = ZLowerExpand,
                            Velocity = v,
                            DeltaT = dtime,
                        };

                        PsoScore score = PsoLocalization(config, StationsList.ToList());
                        PsoScoreList.Add(score);
                        DrowMap(score.X, score.Y);
                    }
                }
                this.Dispatcher.Invoke(() => { wprogress.Close(); });

                var BestScore = PsoScoreList.MinBy(x => x.Score);
                TxtResult = String.Format("X = {0:N} Y = {1:N} Z = {2:N} Score = {3:N5}", BestScore.X, BestScore.Y, BestScore.Z, BestScore.Score);
                DrowMap(BestScore.X, BestScore.Y);
                VValue = BestScore.Velocity;
                VDeltaTValue = BestScore.DeltaT;
                CalcInProgress = false;
            });

        }

        static double objective_function(double x, double y, double z, double v, double[] measured_times, double[,] sensor_coords, double[] time_errors)
        {
            double summ = 0;

            for (int i = 0; i < measured_times.Length; i++)
            {
                double temp1 = Math.Sqrt(Math.Pow(x - sensor_coords[i, 0], 2) + Math.Pow(y - sensor_coords[i, 1], 2) + Math.Pow(z - sensor_coords[i, 2], 2));
                double temp2 = Math.Pow(measured_times[i] - (temp1 / v), 2) / Math.Pow(time_errors[i], 2);
                summ += temp2;

            }

            return summ * 0.5;
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if ((PsoScoreList.Count > 0) && !CalcInProgress)
            {
                var Score = PsoScoreList.ToList().Find(x => (x.Velocity == VValue) && (x.DeltaT == VDeltaTValue));
                if (Score != null)
                {
                    TxtResult = String.Format("X = {0:N} Y = {1:N} Z = {2:N} Score = {3:N4}", Score.X, Score.Y, Score.Z, Score.Score);
                    DrowMap(Score.X, Score.Y);
                }
            }
        }

        private void Button_Save_Settings_Click(object sender, RoutedEventArgs e)
        {
            //SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            //saveFileDialog1.Filter = "xml files (*.xml)|*.xml";
            //saveFileDialog1.RestoreDirectory = true;
            //if (saveFileDialog1.ShowDialog() == true)
            {
                string filePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.xml");
                XmlTextWriter writer = new XmlTextWriter(filePath, null);

                writer.WriteStartElement("Settings");
                writer.WriteElementString(nameof(NrOfParticles), NrOfParticles.ToString());
                writer.WriteElementString(nameof(MaxIteration), MaxIteration.ToString());
                writer.WriteElementString(nameof(TimeError), TimeError.ToString());
                writer.WriteElementString(nameof(WCoef), WCoef.ToString());
                writer.WriteElementString(nameof(C1Coef), C1Coef.ToString());
                writer.WriteElementString(nameof(C2Coef), C2Coef.ToString());
                writer.WriteElementString(nameof(XExpand), XExpand.ToString());
                writer.WriteElementString(nameof(YExpand), YExpand.ToString());
                writer.WriteElementString(nameof(ZUpperExpand), ZUpperExpand.ToString());
                writer.WriteElementString(nameof(ZLowerExpand), ZLowerExpand.ToString());
                writer.WriteElementString(nameof(Vmin), Vmin.ToString());
                writer.WriteElementString(nameof(Vmax), Vmax.ToString());
                writer.WriteElementString(nameof(Vinc), Vinc.ToString());
                writer.WriteElementString(nameof(DeltaTmin), DeltaTmin.ToString());
                writer.WriteElementString(nameof(DeltaTmax), DeltaTmax.ToString());
                writer.WriteElementString(nameof(DeltaTinc), DeltaTinc.ToString());
                writer.WriteEndElement();

                writer.Close();
            }
        }

        private bool Button_Load_Settings()
        {
            try
            {
                string filePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.xml");

                XmlTextReader textReader = new XmlTextReader(filePath);

                while (textReader.Read())
                {
                    if (textReader.IsStartElement())
                    {
                        if (textReader.Name == nameof(NrOfParticles))
                        {
                            textReader.Read();
                            int res;
                            if (int.TryParse(textReader.Value, out res))
                                NrOfParticles = res;
                        }
                        if (textReader.Name == nameof(MaxIteration))
                        {
                            textReader.Read();
                            int res;
                            if (int.TryParse(textReader.Value, out res))
                                MaxIteration = res;
                        }
                        if (textReader.Name == nameof(TimeError))
                        {
                            textReader.Read();
                            double res;
                            if (double.TryParse(textReader.Value, out res))
                                TimeError = res;
                        }
                        if (textReader.Name == nameof(WCoef))
                        {
                            textReader.Read();
                            double res;
                            if (double.TryParse(textReader.Value, out res))
                                WCoef = res;
                        }
                        if (textReader.Name == nameof(C1Coef))
                        {
                            textReader.Read();
                            double res;
                            if (double.TryParse(textReader.Value, out res))
                                C1Coef = res;
                        }
                        if (textReader.Name == nameof(C2Coef))
                        {
                            textReader.Read();
                            double res;
                            if (double.TryParse(textReader.Value, out res))
                                C2Coef = res;
                        }
                        if (textReader.Name == nameof(XExpand))
                        {
                            textReader.Read();
                            double res;
                            if (double.TryParse(textReader.Value, out res))
                                XExpand = res;
                        }
                        if (textReader.Name == nameof(YExpand))
                        {
                            textReader.Read();
                            double res;
                            if (double.TryParse(textReader.Value, out res))
                                YExpand = res;
                        }
                        if (textReader.Name == nameof(ZUpperExpand))
                        {
                            textReader.Read();
                            double res;
                            if (double.TryParse(textReader.Value, out res))
                                ZUpperExpand = res;
                        }
                        if (textReader.Name == nameof(ZLowerExpand))
                        {
                            textReader.Read();
                            double res;
                            if (double.TryParse(textReader.Value, out res))
                                ZLowerExpand = res;
                        }
                        if (textReader.Name == nameof(Vmin))
                        {
                            textReader.Read();
                            double res;
                            if (double.TryParse(textReader.Value, out res))
                                Vmin = res;
                        }
                        if (textReader.Name == nameof(Vmax))
                        {
                            textReader.Read();
                            double res;
                            if (double.TryParse(textReader.Value, out res))
                                Vmax = res;
                        }
                        if (textReader.Name == nameof(Vinc))
                        {
                            textReader.Read();
                            double res;
                            if (double.TryParse(textReader.Value, out res))
                                Vinc = res;
                        }
                        if (textReader.Name == nameof(DeltaTmin))
                        {
                            textReader.Read();
                            double res;
                            if (double.TryParse(textReader.Value, out res))
                                DeltaTmin = res;
                        }
                        if (textReader.Name == nameof(DeltaTmax))
                        {
                            textReader.Read();
                            double res;
                            if (double.TryParse(textReader.Value, out res))
                                DeltaTmax = res;
                        }
                        if (textReader.Name == nameof(DeltaTinc))
                        {
                            textReader.Read();
                            double res;
                            if (double.TryParse(textReader.Value, out res))
                                DeltaTinc = res;
                        }
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }

        }

        private void Button_Load_Settings_Click(object sender, RoutedEventArgs e)
        {
            if (!Button_Load_Settings())
            {
                Velocity = 4200;
                TimeError = 0.05;
                MaxIteration = 200;
                NrOfParticles = 400;
                WCoef = 0.7;
                C1Coef = 1.5;
                C2Coef = 1.5;
                XExpand = 500;
                YExpand = 500;
                ZUpperExpand = 50;
                ZLowerExpand = 50;
                Vmin = 4000;
                Vmax = 5500;
                Vinc = 50;
                DeltaTmin = 0.005;
                DeltaTinc = 0.005;
                DeltaTmax = 0.09;
            }
        }

        private void CheckBox_Changed(object sender, RoutedEventArgs e)
        {

        }


        //private void Run_Click(object sender, RoutedEventArgs e)
        //{
        //    double[] measured_times = new double[StationsList.Count];
        //    double[,] sensor_coords = new double[StationsList.Count, 3];
        //    double[] time_errors = new double[StationsList.Count];
        //    for (int i = 0; i < StationsList.Count; i++)
        //    {
        //        measured_times[i] = StationsList[i].Time - StationsList[0].Time;
        //        time_errors[i] = TimeError;
        //    }
        //    for (int i = 0; i < StationsList.Count; i++)
        //    {
        //        sensor_coords[i, 0] = StationsList[i].X;
        //        sensor_coords[i, 1] = StationsList[i].Y;
        //        sensor_coords[i, 2] = StationsList[i].Z;
        //    }

        //    double seismic_velocity = Velocity;

        //    // Parametry PSO

        //    double w = WCoef; // Waga inercji
        //    double c1 = C1Coef; // Współczynnik poznawczy(do najlepszej pozycji cząstki)
        //    double c2 = C2Coef; // Współczynnik społeczny(do najlepszej pozycji globalnej)

        //    double xmin = StationsList.Min(x => x.X) - XExpand;
        //    double xmax = StationsList.Max(x => x.X) + XExpand;
        //    double ymin = StationsList.Min(x => x.Y) - YExpand;
        //    double ymax = StationsList.Max(x => x.Y) + YExpand;
        //    double zmin = StationsList.Min(x => x.Z) - ZExpand;
        //    double zmax = StationsList.Max(x => x.Z) + ZExpand;

        //    // Inicjalizacja pozycji i prędkości cząstek
        //    var rand = new Random();
        //    double[,] particle_positions = new double[NrOfParticles, 3];
        //    double[,] personal_best_positions = new double[NrOfParticles, 3];
        //    for (int i = 0; i < NrOfParticles; i++)
        //    {
        //        personal_best_positions[i, 0] = particle_positions[i, 0] = (rand.NextDouble() * (xmax - xmin)) + xmin;
        //        personal_best_positions[i, 1] = particle_positions[i, 1] = (rand.NextDouble() * (ymax - ymin)) + ymin;
        //        personal_best_positions[i, 2] = particle_positions[i, 2] = (rand.NextDouble() * (zmax - zmin)) + zmin;
        //    }
        //    double[,] particle_velocities = new double[NrOfParticles, 3];
        //    for (int i = 0; i < NrOfParticles; i++)
        //    {
        //        particle_velocities[i, 0] = 0;
        //        particle_velocities[i, 1] = 0;
        //        particle_velocities[i, 2] = 0;
        //    }

        //    //Ocena początkowych pozycji cząstek
        //    double[] personal_best_scores = new double[NrOfParticles];
        //    int minidx = 0;
        //    double global_best_score = 0;
        //    for (int i = 0; i < NrOfParticles; i++)
        //    {
        //        personal_best_scores[i] = objective_function(particle_positions[i, 0],
        //                               particle_positions[i, 1],
        //                               particle_positions[i, 2], 
        //                               seismic_velocity,
        //                               measured_times,
        //                               sensor_coords,
        //                               time_errors);
        //        if (i == 0)
        //            global_best_score = personal_best_scores[i];
        //        if ((i > 0) && (personal_best_scores[i] < personal_best_scores[i - 1]))
        //        {
        //            minidx = i;
        //            global_best_score = personal_best_scores[i];
        //        }

        //    }

        //    double[] global_best_position = new double[3];
        //    global_best_position[0] = particle_positions[minidx, 0];
        //    global_best_position[1] = particle_positions[minidx, 1];
        //    global_best_position[2] = particle_positions[minidx, 2];


        //    //PSO Main Loop
        //    double[,] global_best_positions_history = new double[MaxIteration, 3];
        //    for (int i = 0; i < MaxIteration; i++)
        //    {
        //        for (int j = 0; j < NrOfParticles; j++)
        //        {
        //            //Aktualizacja prędkości
        //            particle_velocities[j, 0] = w * particle_velocities[j, 0] +
        //                c1 * rand.NextDouble() * (personal_best_positions[j, 0] - particle_positions[j, 0]) +
        //                c2 * rand.NextDouble() * (global_best_position[0] - particle_positions[j, 0]);

        //            particle_velocities[j, 1] = w * particle_velocities[j, 1] +
        //                c1 * rand.NextDouble() * (personal_best_positions[j, 1] - particle_positions[j, 1]) +
        //                c2 * rand.NextDouble() * (global_best_position[1] - particle_positions[j, 1]);

        //            particle_velocities[j, 2] = w * particle_velocities[j, 1] +
        //                c1 * rand.NextDouble() * (personal_best_positions[j, 2] - particle_positions[j, 2]) +
        //                c2 * rand.NextDouble() * (global_best_position[2] - particle_positions[j, 2]);

        //            //Aktualizacja pozycji
        //            particle_positions[j, 0] = particle_positions[j, 0] + particle_velocities[j, 0];
        //            particle_positions[j, 1] = particle_positions[j, 1] + particle_velocities[j, 1];
        //            particle_positions[j, 2] = particle_positions[j, 2] + particle_velocities[j, 2];

        //            //Ograniczenie pozycji do zadanego zakresu
        //            particle_positions[j, 0] = Math.Min(Math.Max(particle_positions[j, 0], xmin), xmax);
        //            particle_positions[j, 1] = Math.Min(Math.Max(particle_positions[j, 1], ymin), ymax);
        //            particle_positions[j, 2] = Math.Min(Math.Max(particle_positions[j, 2], zmin), zmax);

        //            // Ocena nowej pozycji
        //            double current_score = objective_function(particle_positions[j, 0], particle_positions[j, 1], particle_positions[j, 2], seismic_velocity,
        //                measured_times,
        //               sensor_coords,
        //               time_errors);

        //            //Aktualizacja najlepszego osobistego wyniku
        //            if (current_score < personal_best_scores[j])
        //            {
        //                personal_best_scores[j] = current_score;
        //                personal_best_positions[j, 0] = particle_positions[j, 0];
        //                personal_best_positions[j, 1] = particle_positions[j, 1];
        //                personal_best_positions[j, 2] = particle_positions[j, 2];
        //            }
        //            //Aktualizacja najlepszego globalnego wyniku
        //            if (current_score < global_best_score)
        //            {
        //                global_best_score = current_score;
        //                global_best_position[0] = particle_positions[j, 0];
        //                global_best_position[1] = particle_positions[j, 1];
        //                global_best_position[2] = particle_positions[j, 2];
        //            }
        //        }

        //        //Obliczenie średniej prędkości cząstek


        //        //Zapisanie najlepszej pozycji globalnej w historii
        //        global_best_positions_history[i, 0] = global_best_position[0];
        //        global_best_positions_history[i, 1] = global_best_position[1];
        //        global_best_positions_history[i, 2] = global_best_position[2];
        //    }

        //    Canvas_Plot.Children.Clear();
        //    double width = Canvas_Plot.Width;
        //    double height = Canvas_Plot.Height;
        //    //draw grid
        //    for (double i = xmin; i <= xmax; i += 500)
        //    {
        //        Line el = new Line();
        //        el.Stroke = System.Windows.Media.Brushes.Gray;
        //        el.StrokeDashArray = new DoubleCollection() { 8, 8 };
        //        el.StrokeThickness = 1;
        //        double xx = (width / (xmax - xmin)) * (i - xmin);
        //        el.HorizontalAlignment = HorizontalAlignment.Left;
        //        el.VerticalAlignment = VerticalAlignment.Bottom;

        //        el.Y1 = 1;
        //        el.Y2 = height;
        //        el.X1 = xx;
        //        el.X2 = xx;
        //        Canvas_Plot.Children.Add(el);
        //    }
        //    for (double i = ymin; i <= ymax; i += 500)
        //    {
        //        Line el = new Line();
        //        el.Stroke = System.Windows.Media.Brushes.Gray;
        //        el.StrokeDashArray = new DoubleCollection() { 8, 8 };
        //        el.StrokeThickness = 1;
        //        double xx = (height / (ymax - ymin)) * (i - ymin);
        //        el.HorizontalAlignment = HorizontalAlignment.Left;
        //        el.VerticalAlignment = VerticalAlignment.Bottom;
        //        el.Y1 = xx;
        //        el.Y2 = xx;
        //        el.X1 = 1;
        //        el.X2 = width;
        //        Canvas_Plot.Children.Add(el);
        //    }

        //    //draw station
        //    foreach (var s in StationsList)
        //    {
        //        Ellipse el = new Ellipse();
        //        el.Height = 10;
        //        el.Width = 10;
        //        el.Stroke = Brushes.Blue;
        //        double xx = (width / (xmax - xmin)) * (s.X - xmin);
        //        double yy = (height / (ymax - ymin)) * (s.Y - ymin);
        //        Canvas.SetLeft(el, xx);
        //        Canvas.SetTop(el, yy);
        //        Canvas_Plot.Children.Add(el);
        //    }
        //    Ellipse el2 = new Ellipse();
        //    el2.Height = 10;
        //    el2.Width = 10;
        //    el2.Stroke = Brushes.Black;
        //    el2.Fill = Brushes.Red;
        //    double xx2 = (width / (xmax - xmin)) * (global_best_position[0] - xmin);
        //    double yy2 = (height / (ymax - ymin)) * (global_best_position[1] - ymin);
        //    Canvas.SetLeft(el2, xx2);
        //    Canvas.SetTop(el2, yy2);
        //    Canvas_Plot.Children.Add(el2);

        //    TxtResult = String.Format("X = {0:N} Y = {1:N} Z = {2:N}", global_best_position[0], global_best_position[1], global_best_position[2]);
        //}



    }
}