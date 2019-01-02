using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Diagnostics;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;

namespace CheckImage {
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// 需要注意的是，该项目只是一个Demo，所以很多地方并没有使用MVVM的思想。
    /// 更多的是使用“事件驱动”，也就是说，很多的UI更新需要手动来完成。
    /// </summary>
    public partial class MainWindow : Window {

        // ----- 字段定义 -----
        private List<DetectedImage> Images = new List<DetectedImage>(); // 储存图片的列表
        // UI相关的字段
        private int pictureNum = -1; //当前显示的图片序号

        // ----- 初始化 -----
        public MainWindow() {
            InitializeComponent();
        }

        // ----- 事件响应 -----
        private void CheckOneBtn_Click(object sender, RoutedEventArgs e) {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "图片|*.jpg;*.png;*.bmp";
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                updateOneImage(openFileDialog.FileName);
                pictureNum = 0;
                updateUI();
            }
        }

        private void CheckFolderBtn_Click(object sender, RoutedEventArgs e) {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                updateImagesFromPath(folderBrowserDialog.SelectedPath);
                if (Images.Count == 0) {
                    pictureNum = -1;
                }
                else {
                    pictureNum = 0;
                }
                updateUI();
            }
        }

        private void LastBtn_Click(object sender, RoutedEventArgs e) {
            decreasePictureNum();
            updateUI();
        }

        private void NextBtn_Click(object sender, RoutedEventArgs e) {
            increasePictureNum();
            updateUI();
        }

        // ----- 后台逻辑代码 -----
        // 根据图片路径添加图片到Images
        private void addImageByPath(string path) {
            BitmapImage bitmapImage = GetImage(path);
            Images.Add(new DetectedImage(bitmapImage));
        }

        // 清空Images的图片
        private void clearImages() {
            Images.Clear(); // 清除Images对图片的引用
            GC.Collect(); // 使用GC自动进行内存的回收
        }

        // 从一个文件名数组中添加图片
        //  -注意：遇到某个错误时会直接忽略
        private void addImages(string[] files) {
            foreach(string file in files) {
                try {
                    addImageByPath(file);
                }
                catch (System.Exception e) {
                    Debug.WriteLine($"从文件夹导入文件时遇到了错误，已忽略[{file}:{e.Message}]");
                }
            }
        }

        // 通过一张图片的地址更新图片列表
        private void updateOneImage(string path) {
            // 先删除之前的图片
            clearImages();
            // 再添加新的图片
            addImageByPath(path);
        }

        // 通过文件夹地址更新多个图片到图片列表
        private void updateImagesFromPath(string folderPath) {
            // 清除之前的图片
            clearImages();
            // 用比较冗杂的方法将图片一张一张更新到Images中
            string[] files;
            files = Directory.GetFiles(folderPath, "*.jpg");
            addImages(files);
            files = Directory.GetFiles(folderPath, "*.png");
            addImages(files);
            files = Directory.GetFiles(folderPath, "*.bmp");
            addImages(files);
        }

        // 加载图片到内存中而不是引用文件句柄，内存资源能更好地被回收
        public static BitmapImage GetImage(string path) {
            // Read byte[] from png file
            BinaryReader binReader = new BinaryReader(File.Open(path, FileMode.Open));
            FileInfo fileInfo = new FileInfo(path);
            byte[] bytes = binReader.ReadBytes((int)fileInfo.Length);
            binReader.Close();

            // Init bitmap
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.StreamSource = new MemoryStream(bytes);
            bitmap.EndInit();
            bitmap.Freeze();

            return bitmap;
        }
        // ----- UI相关的逻辑代码 -----
        private void updateUI() {
            updateImageControl();
            updateNowImageInfo();
            updateDetectedImagesInfo();
        }

        private void updateImageControl() {
            if (pictureNum == -1) {
                ImageControl.Source = null;
            }
            else {
                ImageControl.Source = null;
                ImageControl.Source = Images[pictureNum].bitmapImage;
            }
        }

        private void updateNowImageInfo() {
            PictureNow.Text = $"当前图片：{pictureNum+1}/{Images.Count}";
        }

        private void updateDetectedImagesInfo() {
            int normalNum = 0;
            int illegalNum = 0;

            foreach(DetectedImage DImage in Images) {
                if (DImage.IsNormal) {
                    normalNum++;
                }
                else {
                    illegalNum++;
                }
            }
            ImageInfoTxBlock.Text = $"正常图片：{normalNum}张 非法图片：{illegalNum}张";
        }

        // 增加当前显示的图片序号
        //  -具体实施：
        //      如果已经是最后一张，序号为Count-1，则不会变
        //      如果没有图片，序号为-1，也不改变
        private void increasePictureNum() {
            if ((pictureNum < Images.Count-1) && (pictureNum>=0)) {
                pictureNum++;
            }
        }

        // 减少当前显示的图片序号
        //  -具体实施：
        //      如果已经是第一张，序号为0，则不会变
        //      如果没有图片，序号为-1，也不改变
        private void decreasePictureNum() {
            if (pictureNum > 0) {
                pictureNum--;
            }
        }
    }
}
