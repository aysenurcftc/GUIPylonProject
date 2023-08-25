using System;
using System.Drawing.Imaging;
using System.Drawing;
using Basler.Pylon;
using System.Windows.Media.Imaging;
using System.IO;
using System.Windows;
using myPylonProject.Helpers;
using System.Windows.Controls;
using myPylonProject.Models;


namespace myPylonProject
{
  

    public partial class MainWindow : Window
    {

        Basler.Pylon.Camera camera = new Basler.Pylon.Camera();
        
        private Bitmap latestFrame;
        private GrabHelper grabHelper = new GrabHelper();   
        private CameraHelper cameraHelper = new CameraHelper();
     


        public MainWindow()
        {
            InitializeComponent();

        }

        private void SaveGrabImage(CameraSetting cameraSetting)
        {
            try
            {
                using (Camera camera = new Camera())
                {

                    camera.CameraOpened += Configuration.AcquireContinuous;
                    camera.Open();

                    cameraHelper.SetCameraStartupSettings(camera, cameraSetting);

                   
                    // Start grabbing.
                    camera.StreamGrabber.Start();


                    IGrabResult grabResult = camera.StreamGrabber.RetrieveResult(5000, TimeoutHandling.ThrowException);
                    using (grabResult)
                    {
                        // Image grabbed successfully?
                        if (grabResult.GrabSucceeded)
                        {
                            byte[] buffer = grabResult.PixelData as byte[];

                            latestFrame = grabHelper.grabResultToBitmap(grabResult);
                            image.Dispatcher.Invoke(() => { image.Source = grabHelper.ConvertBitmapToImageSource(latestFrame); });
                            string currentDirectory = @"C:\Users\Lenovo\Desktop\udemy_c#\baslerPylonProject";

                            var fileName = "capturedImage" + grabHelper.GenerateRandomAlphanumericString() + ".png";
                            var fullPath = Path.Combine(currentDirectory, fileName);
                            latestFrame.Save(fullPath, ImageFormat.Png);
                        }
                        else
                        {
                            Console.WriteLine("Error: {0} {1}", grabResult.ErrorCode, grabResult.ErrorDescription);
                        }
                    }

                    camera.StreamGrabber.Stop();
                    camera.Close();
                }
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("Exception: {0}", e.Message);
            }
        }


        private void GrabImage(CameraSetting cameraSetting)
        {
            try
            {
                using (Camera camera = new Camera())
                {
                    camera.CameraOpened += Configuration.AcquireContinuous;
                    camera.Open();
                    cameraHelper.SetCameraStartupSettings(camera, cameraSetting);
                    camera.StreamGrabber.Start();
                    IGrabResult grabResult = camera.StreamGrabber.RetrieveResult(5000, TimeoutHandling.ThrowException);
                    using (grabResult)
                    {
                        if (grabResult.GrabSucceeded)
                        {
                            byte[] buffer = grabResult.PixelData as byte[];
                            latestFrame = grabHelper.grabResultToBitmap(grabResult);
                            image.Dispatcher.Invoke(() => { image.Source = grabHelper.ConvertBitmapToImageSource(latestFrame); });
                        }
                        else
                        {
                            Console.WriteLine("Error: {0} {1}", grabResult.ErrorCode, grabResult.ErrorDescription);
                        }
                    }
                    camera.StreamGrabber.Stop();
                    camera.Close();
                }
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("Exception: {0}", e.Message);
            }
        }






        private void OnImageGrabbed(Object sender, ImageGrabbedEventArgs e)
        {

            try
            {

                IGrabResult grabResult = e.GrabResult;


                Bitmap? bitmap = grabHelper.grabResultToBitmap(grabResult);

                if (grabResult.IsValid)
                {
                    BitmapImage? bitmapImage = null;
                    image.Dispatcher.Invoke(() =>
                    {
                        bitmapImage = grabHelper.ConvertBitmapToImageSource(bitmap!);
                        image.Source = bitmapImage;
                    });

                    if (bitmapImage is not null)
                    {
                        Dispatcher.Invoke(() =>
                        {
                            Bitmap? image = bitmap;
                            image!.Dispose();
                        });
                    }

                }

            }
            catch (Exception ex)
            {

            }
            finally
            {
                e.DisposeGrabResultIfClone();
            }
        }



        private void ContinuousShot()
        {
            camera.CameraOpened += Configuration.AcquireContinuous;
            camera.StreamGrabber.ImageGrabbed += OnImageGrabbed;

            try
            {
                if (camera is null)
                {
                    MessageBox.Show("Please select a device.");
                    return;
                }
                if (camera.StreamGrabber.IsGrabbing)
                {
                    MessageBox.Show("Camera is already grabbing");
                    return;
                }

                camera.Open();
                Configuration.AcquireContinuous(camera, null);
                camera?.StreamGrabber.Start(GrabStrategy.OneByOne, GrabLoop.ProvidedByStreamGrabber);
            }
            catch (Exception ex)
            {

            }
        }


        private CameraSetting GetChangeableCameraSettings() => new CameraSetting()
        {
            Gain = gain.Value,
            ExposureTime = exposure.Value,
            Gamma = gamma.Value,
            BlackLevel = blacklevel.Value,
            PixelFormat = ((ComboBoxItem)selectformat.SelectedItem).Content.ToString(),  
            
            
        };



        private void SetSlidersEnableAndDisableSettings()
        {

            if (gainAuto.Items.Count > 0)
            {
                ComboBoxItem? item = gainAuto.SelectedItem as ComboBoxItem;

                if (gain is not null)
                {
                    if (item?.Name != "Off")
                    {
                        gain.IsEnabled = false;
                    
                    }

                    else
                    {
                        gain.IsEnabled = true;
                       
                       

                    }
                }
            }

        }


        private void txtboxGain()
        {
            if (gain is not null)
            {
                gaintxt.Text = gain.Value.ToString("0.##");
            }
        }


        private void txtExposureTime()
        {
            if (exposure is not null)
            {
                double exposureValue = exposure.Value;
                double c = Math.Round(exposureValue, 1);
                exposuretxt.Text = c.ToString();
            }
        }


        private void txtGamma()
        {
            if (gamma is not null)
            {
                gammatxt.Text = gamma.Value.ToString("0.##");
            }
        }


        








        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void btnMaximize_Click(object sender, RoutedEventArgs e)
        {
            if(WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Normal;
            }
            else
            {
                WindowState = WindowState.Maximized;
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
            //Application.Current.Shutdown();
        }

       

        private void stop_Click(object sender, RoutedEventArgs e)
        {

            camera.StreamGrabber.Stop();
            camera.Close();
            image.Source = null;
        }

        private void continiousShot_Click(object sender, RoutedEventArgs e)
        {
            ContinuousShot();
        }

        private void singleShot_Click(object sender, RoutedEventArgs e)
        {
            SaveGrabImage(GetChangeableCameraSettings());
        }

        private void Window_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void saveGrab_Click(object sender, RoutedEventArgs e)
        {
            SaveGrabImage(GetChangeableCameraSettings());
        }


        private void gainAuto_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetSlidersEnableAndDisableSettings();
        }

        private void selectformat_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           
        }

        private void gain_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            txtboxGain();
        }

        private void exposure_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            txtExposureTime();
        }

        private void gamma_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            txtGamma();
        }

        




    }
}
