﻿using System;
using System.IO;
using System.Threading.Tasks;

using Plugin.Media;
using Plugin.Permissions;
using Plugin.Media.Abstractions;
using Plugin.Permissions.Abstractions;

using Xamarin.Forms;

namespace OnSight
{
    public static class MediaService
    {
        #region Events 
        public static event EventHandler NoCameraDetected;
        public static event EventHandler PermissionsDenied;
        #endregion

        #region Methods
        public static Stream GetPhotoStream(MediaFile mediaFile, bool disposeMediaFile)
        {
            var stream = mediaFile.GetStream();

            if (disposeMediaFile)
                mediaFile.Dispose();

            return stream;
        }

        public static async Task<MediaFile> GetMediaFileFromCamera(string directory, string photoName)
        {
			await CrossMedia.Current.Initialize().ConfigureAwait(false);

            var arePermissionsGranted = await ArePermissionsGranted().ConfigureAwait(false);
            if(!arePermissionsGranted)
            {
                OnPermissionsDenied();
                return null;
            }

            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                OnNoCameraDetected();
                return null;
            }

            var mediaFileTCS = new TaskCompletionSource<MediaFile>();

            Device.BeginInvokeOnMainThread(async()=>
            {
                var file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
                {
                    PhotoSize = PhotoSize.Small,
                    Directory = directory,
                    Name = photoName,
                    DefaultCamera = CameraDevice.Rear,
                }).ConfigureAwait(false);

                mediaFileTCS.SetResult(file);
			});

            return await mediaFileTCS.Task.ConfigureAwait(false);
        }

        static async Task<bool> ArePermissionsGranted()
        {
            var cameraStatus = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Camera);
            var storageStatus = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Storage);

            if (cameraStatus != PermissionStatus.Granted || storageStatus != PermissionStatus.Granted)
            {
                var results = await CrossPermissions.Current.RequestPermissionsAsync(new[] { Permission.Camera, Permission.Storage });
                cameraStatus = results[Permission.Camera];
                storageStatus = results[Permission.Storage];
            }

            if (cameraStatus == PermissionStatus.Granted && storageStatus == PermissionStatus.Granted)
                return true;
            
            return false;
        }

        static void OnNoCameraDetected() => NoCameraDetected?.Invoke(null, EventArgs.Empty);
        static void OnPermissionsDenied() => PermissionsDenied?.Invoke(null, EventArgs.Empty);
        #endregion
    }
}
