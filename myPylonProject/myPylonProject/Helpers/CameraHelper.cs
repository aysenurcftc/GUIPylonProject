using Basler.Pylon;
using myPylonProject.Models;
using System;


namespace myPylonProject.Helpers
{
    class CameraHelper
    {
       
        public void setCameraParameters(Camera camera, int exposure, int gain)
        {
            try
            {
                camera.Parameters[PLCamera.ExposureTime].TrySetValue(exposure);
                camera.Parameters[PLCamera.Gain].TrySetValue(gain);
            }
            catch(Exception ex)
            {
            }
        }


        public void SetCameraStartupSettings(Camera camera, CameraSetting cameraSetting)
        {
            camera?.Parameters[PLCamera.Gain].TrySetValue(cameraSetting.Gain);
            camera?.Parameters[PLCamera.ExposureTime].TrySetValue(cameraSetting.ExposureTime);
            camera?.Parameters[PLCamera.Gamma].TrySetValue(cameraSetting.Gamma);
            camera?.Parameters[PLCamera.PixelFormat].SetValue(cameraSetting.PixelFormat);

        }

        










    }
}
