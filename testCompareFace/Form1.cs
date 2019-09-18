using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using Amazon.Rekognition;
using Amazon.Rekognition.Model;
using Amazon.Runtime;
using Amazon.S3;

namespace testCompareFace
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            float similarityThreshold = 70F;
            String sourceImage = "source.jpg";
            String targetImage = "target.jpg";
            String accessKeyID="";
            String secretKey = "";

            AWSCredentials credentials;
            credentials = new BasicAWSCredentials(accessKeyID.Trim(), secretKey.Trim());

            AmazonRekognitionClient rekognitionClient = new AmazonRekognitionClient(credentials, Amazon.RegionEndpoint.USEast1);

            Amazon.Rekognition.Model.Image imageSource = new Amazon.Rekognition.Model.Image();
            try
            {
                using (FileStream fs = new FileStream(sourceImage, FileMode.Open, FileAccess.Read))
                {
                    byte[] data = new byte[fs.Length];
                    fs.Read(data, 0, (int)fs.Length);
                    imageSource.Bytes = new MemoryStream(data);
                }
            }
            catch (Exception)
            {
                //Console.WriteLine("Failed to load source image: " + sourceImage);
                listBox1.Items.Add("Failed to load source image: " + sourceImage);
                return;
            }

            Amazon.Rekognition.Model.Image imageTarget = new Amazon.Rekognition.Model.Image();
            try
            {
                using (FileStream fs = new FileStream(targetImage, FileMode.Open, FileAccess.Read))
                {
                    byte[] data = new byte[fs.Length];
                    data = new byte[fs.Length];
                    fs.Read(data, 0, (int)fs.Length);
                    imageTarget.Bytes = new MemoryStream(data);
                }
            }
            catch (Exception)
            {
                //Console.WriteLine("Failed to load target image: " + targetImage);
                listBox1.Items.Add("Failed to load target image: " + targetImage);
                return;
            }

            CompareFacesRequest compareFacesRequest = new CompareFacesRequest()
            {
                SourceImage = imageSource,
                TargetImage = imageTarget,
                SimilarityThreshold = similarityThreshold
            };

            // Call operation
            CompareFacesResponse compareFacesResponse = rekognitionClient.CompareFaces(compareFacesRequest);
            

            // Display results
            foreach (CompareFacesMatch match in compareFacesResponse.FaceMatches)
            {
                ComparedFace face = match.Face;
                BoundingBox position = face.BoundingBox;
                //Console.WriteLine("Face at " + position.Left
                //      + " " + position.Top
                //      + " matches with " + match.Similarity
                //      + "% confidence.");
                listBox1.Items.Add("Face at " + position.Left
                                             + " " + position.Top
                                             + " matches with " + match.Similarity
                                             + "% confidence.");
            }

            //Console.WriteLine("There was " + compareFacesResponse.UnmatchedFaces.Count + " face(s) that did not match");
            listBox1.Items.Add("There was " + compareFacesResponse.UnmatchedFaces.Count + " face(s) that did not match");
        }
    }
}
