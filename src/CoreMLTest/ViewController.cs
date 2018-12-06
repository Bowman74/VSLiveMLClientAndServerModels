using System;
using CoreGraphics;
using CoreImage;
using CoreVideo;
using Foundation;
using UIKit;

namespace CoreMLTest
{
    public partial class ViewController : UIViewController
    {
        UIImagePickerController imagePicker;

        protected ViewController(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Perform any additional setup after loading the view, typically from a nib.

            imagePicker = new UIImagePickerController();

            imagePicker.SourceType = UIImagePickerControllerSourceType.PhotoLibrary;
            imagePicker.MediaTypes = UIImagePickerController.AvailableMediaTypes(UIImagePickerControllerSourceType.PhotoLibrary);

            imagePicker.FinishedPickingMedia += ImagePicker_FinishedPickingMedia;
            imagePicker.Canceled += ImagePicker_Canceled;
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }

        partial void BtnRun_TouchUpInside(UIButton sender)
        {
            PresentViewControllerAsync(imagePicker, true);
        }

        void ImagePicker_FinishedPickingMedia(object sender, UIImagePickerMediaPickedEventArgs e)
        {
            if (e.Info[UIImagePickerController.MediaType].ToString() == "public.image")
            {
                UIImage originalImage = e.Info[UIImagePickerController.OriginalImage] as UIImage;
                if (originalImage != null)
                {
                    var scaledImage = originalImage.Scale(new CGSize(300, 300));
                    var classifier = new ImageClassifier();
                    var coreImage = new CIImage(scaledImage);
                    CVPixelBuffer buffer = new CVPixelBuffer(300, 300, CVPixelFormatType.CV32ARGB);

                    UIGraphics.BeginImageContext(new CGSize(300, 300));
                    CIContext context = CIContext.FromContext(UIGraphics.GetCurrentContext(), null);
                    context.Render(coreImage, buffer);                           
                    UIGraphics.EndImageContext();


                    var output = classifier.GetPrediction(buffer, out NSError error);

                    imgSelected.Image = scaledImage;
                    lblResult.Text = $"This looks like: {output.ClassLabel}";
                }
            }

            imagePicker.DismissModalViewController(true);
        }

        void ImagePicker_Canceled(object sender, EventArgs e)
        {
            imagePicker.DismissModalViewController(true);
        }
    }
}