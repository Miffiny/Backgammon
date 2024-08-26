using System.Reflection;

namespace Backgammon.UI;

public partial class Window : Form
{
    private PictureBox boardPictureBox;
    private PictureBox[] points = new PictureBox[24]; // Array to hold the 24 points

    public Window()
    {
        InitializeComponent();
        InitializeBoard();
        InitializePoints();
    }

    private void InitializeBoard()
    {
        var height = 800;
        var width = 1000;
        // Initialize the PictureBox for the board
        boardPictureBox = new PictureBox
        {
            Size = new Size(width, height),
            Location = new Point(50, 50),
            BorderStyle = BorderStyle.FixedSingle,
            SizeMode = PictureBoxSizeMode.StretchImage
        };

        // Load the board image
        boardPictureBox.Load(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"..\..\..\UI\images\board.png"));

        // Add the board PictureBox to the form's controls
        Controls.Add(boardPictureBox);
    }

    private void InitializePoints()
    {
        // Determine the size of each point PictureBox
        Size pointSize = new Size(74, 300); // Adjust the size based on your image and layout
        int gap = 10; // The gap between the points

        // Calculate the locations for the top row (12 points)
        for (int i = 0; i < 12; i++)
        {
            points[i] = new PictureBox
            {
                Size = pointSize,
                Location = new Point(0 + i * (pointSize.Width + gap), 0), // Adjust the initial location (60, 60) based on your board
                BorderStyle = BorderStyle.None,
                SizeMode = PictureBoxSizeMode.StretchImage,
                BackColor = Color.Transparent, // Set background color to transparent
                Parent = boardPictureBox // Set the parent to the boardPictureBox
            };

            // Load and flip the image vertically to mirror it
            var pointImage = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"..\..\..\UI\images\point.png"));
            pointImage.RotateFlip(RotateFlipType.RotateNoneFlipY); // Flip the image vertically
            points[i].Image = pointImage;

            // Add the point to the board (as a child of boardPictureBox)
            boardPictureBox.Controls.Add(points[i]);
            points[i].BringToFront(); // Ensure point is in front of the board
        }

        // Calculate the locations for the bottom row (12 points)
        for (int i = 0; i < 12; i++)
        {
            points[i + 12] = new PictureBox
            {
                Size = pointSize,
                Location = new Point(0 + i * (pointSize.Width + gap), 500), // Adjust the initial location (600, 600) based on your board
                BorderStyle = BorderStyle.None,
                SizeMode = PictureBoxSizeMode.StretchImage,
                BackColor = Color.Transparent, // Set background color to transparent
                Parent = boardPictureBox // Set the parent to the boardPictureBox
            };

            // Load the image without flipping
            points[i + 12].Load(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"..\..\..\UI\images\point.png"));

            // Add the point to the board (as a child of boardPictureBox)
            boardPictureBox.Controls.Add(points[i + 12]);
            points[i + 12].BringToFront(); // Ensure point is in front of the board
        }
    }
}