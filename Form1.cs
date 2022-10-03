/* Author: Andrew Bach
 * Date: 12/6/2020
 * Program Version: 0.22
 * Program Description: A program that is used to create websites. The program takes user input to build an array that is then exported
 *                      to an html file. The program builds a directory that can be simply dropped onto a web server. Projects can be saved and loaded
 *                      for future editing.
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Website_Developer
{
    public partial class FormWebBuilder : Form
    {
        //Declare Constants
        const int MAX_PAGES = 5; //Max pages the editor can make at this time
        const double VERSION = 0.22; //The current version of the program

        //Declare Variables
        int currentPage = 0; //What page is being edited - 0, 1, 2, 3, 4
        int saveBoxAction; //1- Close Project, 2 - Open Project, 3 - Reset Page, 4 - Exit Page
        int themeSelected = 0; //Theme selection for new project panel: 1 - singleBars
        bool recentlySaved = false; //A check to determine if the site has been saved.
        bool fromMainMenu; //Variable to determine if the new project panel was accessed from the main menu

        //Create New Page Variables
        int createNewPageLayout = 1; //Variable to select a layout for the new page
        int newPage = 1; //Variable to determine what page number we are creating.


        //Rename Page Variables
        int pageToRename;


        //Panel Open Variables
        bool renamePanelOpen = false;
        bool createNewPagePanelOpen = false;
        bool colorPickerPanelOpen = false;

        //Side bar menu Open variables
        bool themeSettingsOpen = false;
        bool fontSettingsOpen = false;
        bool pagesOpen = false;

        //Theme Setting Variables
        Image[,] arraySingleBarTheme = new Image[2, 8]; //{0 , } - Colorbar Images {1 , } - Header Images
        Color[] themeColor = new Color[8];


        ////////////////////////////////////////////
        //Declare array to store details on project
        string[] arrayProject = new string[5];

        //Constants to target array variables by name
        const int PROJECT_NAME = 0;
        const int PROJECT_LOCATION = 1;
        const int PROJECT_TOTAL_PAGES = 2;
        const int PROJECT_THEME = 3;
        const int PROJECT_AUTHOR = 4;

        /*
        [0] - Project Name
        [1] - Project Location
        [2] - Project Total Pages
        [3] - Project Theme
        [4] - Project Author
         */

        /////////////////////////////////////////////////////////////
        //Declare array to store details on page for the page builder.
        string[,] arrayWebsite = new string[MAX_PAGES, 19];

        //Constants to target array variables by name
        const int PAGE_NAME = 0;
        const int PAGE_LAYOUT = 1;
        const int PAGE_COLOR = 2;
        const int TITLE_FONT = 3;
        const int TITLE_TEXT_COLOR = 4;
        const int TITLE_TEXT = 5;
        const int MENU_FONT = 6;
        const int MENU_TEXT_COLOR = 7;
        const int MENU_TEXT_COLOR_HOVER = 8;
        const int CONTENT_FONT = 9;
        const int CONTENT_TEXT_COLOR = 10;
        const int CONTENT_1_TEXT = 11;
        const int CONTENT_2_TEXT = 12;
        const int IMAGE_LOCATION = 13;
        const int IMAGE_ALT = 14;
        const int FOOTER_FONT = 15;
        const int FOOTER_TEXT = 16;
        const int FOOTER_TEXT_COLOR = 17;
        const int IMAGE_LOCATION_TYPE = 18;

        /*
        1st subscript
        [# , ] - Page Number

        2nd subscript
        [ , 0] - Page Name //First page will always be stored as index. Rest are user defined
        [ , 1] - Page Layout //1 - Single Layout, 2 - Multiple Layout
        [ , 2] - Page Color //0 - Navy, 1 - Maroon, 2 - Green, 3 - Light Blue, 4 - Orange, 5 - Yellow, 6 - Purple, 7 - Red
        [ , 3] - Title Font
        [ , 4] - Title Text Color
        [ , 5] - Title Text 
        [ , 6] - Menu Font
        [ , 7] - Menu Text Color
        [ , 8] - Menu Text Color On Hover
        [ , 9] - Content Font
        [ , 10] - Content Text Color
        [ , 11] - Content 1 Text
        [ , 12] - Content 2 Text
        [ , 13] - Image Location
        [ , 14] - Image Alt Text
        [ , 15] - Footer Font
        [ , 16] - Footer Text
        [ , 17] - Footer Text Color
        [ , 18] - Image Location Type //0 = none, 1 = File, 2 = URL
        */


        /////////////////////////////////////////////////
        //Declare an Array to store applications settings
        string[] arraySettings = new string[11];

        //Constants to target array variables by name
        const int USER_NAME = 0;
        const int RECENT_FILE_NAME_0 = 1;
        const int RECENT_FILE_PATH_0 = 2;
        const int RECENT_FILE_NAME_1 = 3;
        const int RECENT_FILE_PATH_1 = 4;
        const int RECENT_FILE_NAME_2 = 5;
        const int RECENT_FILE_PATH_2 = 6;
        const int RECENT_FILE_NAME_3 = 7;
        const int RECENT_FILE_PATH_3 = 8;
        const int RECENT_FILE_NAME_4 = 9;
        const int RECENT_FILE_PATH_4 = 10;

        /*
        [0] - User Name
        [1] - Recent file name 0
        [2] - Recent file name 1
        [3] - Recent file name 2
        [4] - Recent file name 3
        [5] - Recent file name 4
        [6] - Recent file path 0
        [7] - Recent file path 1
        [8] - Recent file path 2
        [9] - Recent file path 3
        [10] - Recent file path 4
        */


        public FormWebBuilder()
        {
            InitializeComponent();

            //Set form size and center. Did this so I could build a bunch of stuff off screen since I have so many layers.
            this.Size = new Size(1200, 770);
            this.CenterToScreen();

            //Position panels and menus
            panelThemeSettings.Location = new Point(239, 151);
            panelFontSettings.Location = new Point(239, 202);
            panelPages.Location = new Point(239, 255);
            panelImageSettings.Location = new Point(709, 383);
            panelCreateNewPage.Location = new Point(500, 280);
            panelRenamePage.Location = new Point(500, 320);
            panelCloseWithoutSaving.Location = new Point(500, 280);
            panelMainMenu.Location = new Point(0, 0);
            panelFirstUse.Location = new Point(400, 250);
            panelNewProject.Location = new Point(148, 100);
            panelMessageBox.Location = new Point(500, 350);
            panelGuide.Location = new Point(0, 0);

            //Load the settings
            try
            {
                StreamReader inputFile;

                inputFile = File.OpenText("settings.wdsettings");

                for (int i = 0; i < arraySettings.GetLength(0); i++)
                {
                    arraySettings[i] = inputFile.ReadLine();
                }

                inputFile.Close();
            }
            catch
            {
                //If there is no settings file, create 1
                StreamWriter outputFile;

                outputFile = File.CreateText("settings.wdsettings");

                for (int i = 0; i < arraySettings.GetLength(0); i++)
                {
                    outputFile.WriteLine("");
                }

                outputFile.Close();

                //Read new settings file
                StreamReader inputFile;
                inputFile = File.OpenText("settings.wdsettings");

                for (int i = 0; i < arraySettings.GetLength(0); i++)
                {
                    arraySettings[i] = inputFile.ReadLine();
                }

                inputFile.Close();
            }

            //Build the folder directories if they are not present
            //Project Directory
            var projectDirectory = Path.Combine(Environment.CurrentDirectory, @"Projects");

            if (!Directory.Exists(projectDirectory))
            {
                Directory.CreateDirectory(projectDirectory);
            }

            //User Images Directory
            var imageDirectory = Path.Combine(Environment.CurrentDirectory, @"User Images");

            if (!Directory.Exists(imageDirectory))
            {
                Directory.CreateDirectory(imageDirectory);
            }

            //If first time opening, open the new user tab to assign a username
            if (arraySettings[USER_NAME] == "")
            {
                panelFirstUse.Visible = true;
                panelMainMenu.Enabled = false;
            }

            //Add colorbar images to theme array
            arraySingleBarTheme[0, 0] = ((System.Drawing.Image)(Properties.Resources.colorBar_navy));
            arraySingleBarTheme[0, 1] = ((System.Drawing.Image)(Properties.Resources.colorBar_maroon));
            arraySingleBarTheme[0, 2] = ((System.Drawing.Image)(Properties.Resources.colorBar_green));
            arraySingleBarTheme[0, 3] = ((System.Drawing.Image)(Properties.Resources.colorBar_lightBlue));
            arraySingleBarTheme[0, 4] = ((System.Drawing.Image)(Properties.Resources.colorBar_orange));
            arraySingleBarTheme[0, 5] = ((System.Drawing.Image)(Properties.Resources.colorBar_yellow));
            arraySingleBarTheme[0, 6] = ((System.Drawing.Image)(Properties.Resources.colorBar_purple));
            arraySingleBarTheme[0, 7] = ((System.Drawing.Image)(Properties.Resources.colorBar_red));

            //Add header images to theme array
            arraySingleBarTheme[1, 0] = ((System.Drawing.Image)(Properties.Resources.header_navy));
            arraySingleBarTheme[1, 1] = ((System.Drawing.Image)(Properties.Resources.header_maroon));
            arraySingleBarTheme[1, 2] = ((System.Drawing.Image)(Properties.Resources.header_green));
            arraySingleBarTheme[1, 3] = ((System.Drawing.Image)(Properties.Resources.header_lightblue));
            arraySingleBarTheme[1, 4] = ((System.Drawing.Image)(Properties.Resources.header_orange));
            arraySingleBarTheme[1, 5] = ((System.Drawing.Image)(Properties.Resources.header_yellow));
            arraySingleBarTheme[1, 6] = ((System.Drawing.Image)(Properties.Resources.header_purple));
            arraySingleBarTheme[1, 7] = ((System.Drawing.Image)(Properties.Resources.header_red));

            //Add RGB colors to theme colors array
            themeColor[0] = Color.FromArgb(0, 39, 94); //Navy
            themeColor[1] = Color.FromArgb(94, 0, 0); //Maroon
            themeColor[2] = Color.FromArgb(0, 94, 0); //Green
            themeColor[3] = Color.FromArgb(0, 124, 160); //Light Blue
            themeColor[4] = Color.FromArgb(218, 103, 0); //Orange
            themeColor[5] = Color.FromArgb(196, 203, 0); //Yellow
            themeColor[6] = Color.FromArgb(142, 0, 188); //Purple
            themeColor[7] = Color.FromArgb(188, 0, 0); //Red

            for (int i = 0; i < MAX_PAGES; i++)
            {
                //Default Page settings to prevent errors from null values
                arrayWebsite[i, PAGE_NAME] = "Index";
                arrayWebsite[i, PAGE_LAYOUT] = "singleLayout";
                arrayWebsite[i, PAGE_COLOR] = "0";
                arrayWebsite[i, TITLE_FONT] = "0";
                arrayWebsite[i, TITLE_TEXT_COLOR] = "#FFFFFF";
                arrayWebsite[i, TITLE_TEXT] = "Your Title";
                arrayWebsite[i, MENU_FONT] = "0";
                arrayWebsite[i, MENU_TEXT_COLOR] = "#000000";
                arrayWebsite[i, MENU_TEXT_COLOR_HOVER] = "#000000";
                arrayWebsite[i, CONTENT_FONT] = "0";
                arrayWebsite[i, CONTENT_TEXT_COLOR] = "#000000";
                arrayWebsite[i, CONTENT_1_TEXT] = "Welcome to the Bach Web Builder (Cooler Name Pending?)";
                arrayWebsite[i, CONTENT_2_TEXT] = "";
                arrayWebsite[i, IMAGE_LOCATION] = "";
                arrayWebsite[i, IMAGE_ALT] = "";
                arrayWebsite[i, FOOTER_FONT] = "0";
                arrayWebsite[i, FOOTER_TEXT] = "Footer text goes in here";
                arrayWebsite[i, FOOTER_TEXT_COLOR] = "#FFFFFF";
                arrayWebsite[i, IMAGE_LOCATION_TYPE] = "0";
            }

            //Default Project settings to prevent errors from null values
            arrayProject[PROJECT_NAME] = "default";
            arrayProject[PROJECT_LOCATION] = "null";
            arrayProject[PROJECT_THEME] = "singleBars";
            arrayProject[PROJECT_TOTAL_PAGES] = "1";
            arrayProject[PROJECT_AUTHOR] = arraySettings[USER_NAME];


            //Update all menus and the program
            updateProgram();
            updateMenus();

            //Disable the editor on launch
            disableEditor();

            //Get Recent files
            recentFiles();
        }

        //Show the custom message box
        private void showMessageBox(string message)
        {
            labelMessageBoxMessage.Text = message;
            panelMessageBox.Visible = true;
            panelMessageBox.BringToFront();
        }

        //Method to exit the program
        private void exitProgram()
        {
            if (recentlySaved)
            {
                //Close the program
                this.Close();
            }
            else
            {
                //Set save box action to 1 and open the "Do you want to save" panel
                saveBoxAction = 4;
                panelCloseWithoutSaving.Visible = true;
            }
        }


        //Method to create a new page for the project
        private void createNewPage(int pageNumber)
        {
            //Declare variable to update pages
            int intTotalPages;

            arrayWebsite[pageNumber, PAGE_NAME] = textBoxCreatePageName.Text;

            //Increase the total pages
            intTotalPages = int.Parse(arrayProject[PROJECT_TOTAL_PAGES]);
            intTotalPages++;
            arrayProject[PROJECT_TOTAL_PAGES] = intTotalPages.ToString();

            if (createNewPageLayout == 1)
            {
                arrayWebsite[pageNumber, PAGE_LAYOUT] = "singleLayout";
            }
            else if (createNewPageLayout == 2)
            {
                arrayWebsite[pageNumber, PAGE_LAYOUT] = "multipleLayout";
            }

            //Close and reset the create page panel
            panelCreateNewPage.Visible = false;
            textBoxCreatePageName.Text = "";
            createNewPageLayout = 1;

            //Update the Editor Menus and update the site page
            updateMenus();
            updateProgram();
        }


        //Method used to fill in recent files on main menu
        private void recentFiles()
        {
            //Get Recent Files for main menu
            labelProjectName0.Text = arraySettings[RECENT_FILE_NAME_0];
            labelProjectFilePath0.Text = arraySettings[RECENT_FILE_PATH_0];
            if (arraySettings[RECENT_FILE_NAME_0] == "")
            {
                pictureBoxRecentFileIcon0.Visible = false;
                labelProjectName0.Visible = false;
                labelProjectFilePath0.Visible = false;
            }
            else
            {
                pictureBoxRecentFileIcon0.Visible = true;
                labelProjectName0.Visible = true;
                labelProjectFilePath0.Visible = true;
            }


            labelProjectName1.Text = arraySettings[RECENT_FILE_NAME_1];
            labelProjectFilePath1.Text = arraySettings[RECENT_FILE_PATH_1];
            if (arraySettings[RECENT_FILE_NAME_1] == "")
            {
                pictureBoxRecentFileIcon1.Visible = false;
                labelProjectName1.Visible = false;
                labelProjectFilePath1.Visible = false;
            }
            else
            {
                pictureBoxRecentFileIcon1.Visible = true;
                labelProjectName1.Visible = true;
                labelProjectFilePath1.Visible = true;
            }

            labelProjectName2.Text = arraySettings[RECENT_FILE_NAME_2];
            labelProjectFilePath2.Text = arraySettings[RECENT_FILE_PATH_2];
            if (arraySettings[RECENT_FILE_NAME_2] == "")
            {
                pictureBoxRecentFileIcon2.Visible = false;
                labelProjectName2.Visible = false;
                labelProjectFilePath2.Visible = false;
            }
            else
            {
                pictureBoxRecentFileIcon2.Visible = true;
                labelProjectName2.Visible = true;
                labelProjectFilePath2.Visible = true;
            }

            labelProjectName3.Text = arraySettings[RECENT_FILE_NAME_3];
            labelProjectFilePath3.Text = arraySettings[RECENT_FILE_PATH_3];
            if (arraySettings[RECENT_FILE_NAME_3] == "")
            {
                pictureBoxRecentFileIcon3.Visible = false;
                labelProjectName3.Visible = false;
                labelProjectFilePath3.Visible = false;
            }
            else
            {
                pictureBoxRecentFileIcon3.Visible = true;
                labelProjectName3.Visible = true;
                labelProjectFilePath3.Visible = true;
            }

            labelProjectName4.Text = arraySettings[RECENT_FILE_NAME_4];
            labelProjectFilePath4.Text = arraySettings[RECENT_FILE_PATH_4];
            if (arraySettings[RECENT_FILE_NAME_4] == "")
            {
                pictureBoxRecentFileIcon4.Visible = false;
                labelProjectName4.Visible = false;
                labelProjectFilePath4.Visible = false;
            }
            else
            {
                pictureBoxRecentFileIcon4.Visible = true;
                labelProjectName4.Visible = true;
                labelProjectFilePath4.Visible = true;
            }
        }


        //Method used to add to recent files
        private void addRecentFile()
        {
            if (arrayProject[PROJECT_LOCATION] == arraySettings[RECENT_FILE_PATH_0])
            {
                //If project location is same as most recent file, do nothing.
            }
            else if (arrayProject[PROJECT_LOCATION] == arraySettings[RECENT_FILE_PATH_1])
            {
                arraySettings[RECENT_FILE_PATH_1] = arraySettings[RECENT_FILE_PATH_0];
                arraySettings[RECENT_FILE_NAME_1] = arraySettings[RECENT_FILE_NAME_0];

                arraySettings[RECENT_FILE_PATH_0] = arrayProject[PROJECT_LOCATION];
                arraySettings[RECENT_FILE_NAME_0] = arrayProject[PROJECT_NAME];
            }
            else if (arrayProject[PROJECT_LOCATION] == arraySettings[RECENT_FILE_PATH_2])
            {
                arraySettings[RECENT_FILE_PATH_2] = arraySettings[RECENT_FILE_PATH_1];
                arraySettings[RECENT_FILE_NAME_2] = arraySettings[RECENT_FILE_NAME_1];

                arraySettings[RECENT_FILE_PATH_1] = arraySettings[RECENT_FILE_PATH_0];
                arraySettings[RECENT_FILE_NAME_1] = arraySettings[RECENT_FILE_NAME_0];

                arraySettings[RECENT_FILE_PATH_0] = arrayProject[PROJECT_LOCATION];
                arraySettings[RECENT_FILE_NAME_0] = arrayProject[PROJECT_NAME];
            }
            else if (arrayProject[PROJECT_LOCATION] == arraySettings[RECENT_FILE_PATH_3])
            {
                arraySettings[RECENT_FILE_PATH_3] = arraySettings[RECENT_FILE_PATH_2];
                arraySettings[RECENT_FILE_NAME_3] = arraySettings[RECENT_FILE_NAME_2];

                arraySettings[RECENT_FILE_PATH_2] = arraySettings[RECENT_FILE_PATH_1];
                arraySettings[RECENT_FILE_NAME_2] = arraySettings[RECENT_FILE_NAME_1];

                arraySettings[RECENT_FILE_PATH_1] = arraySettings[RECENT_FILE_PATH_0];
                arraySettings[RECENT_FILE_NAME_1] = arraySettings[RECENT_FILE_NAME_0];

                arraySettings[RECENT_FILE_PATH_0] = arrayProject[PROJECT_LOCATION];
                arraySettings[RECENT_FILE_NAME_0] = arrayProject[PROJECT_NAME];
            }
            else if (arrayProject[PROJECT_LOCATION] == arraySettings[RECENT_FILE_PATH_4])
            {
                arraySettings[RECENT_FILE_PATH_4] = arraySettings[RECENT_FILE_PATH_3];
                arraySettings[RECENT_FILE_NAME_4] = arraySettings[RECENT_FILE_NAME_3];

                arraySettings[RECENT_FILE_PATH_3] = arraySettings[RECENT_FILE_PATH_2];
                arraySettings[RECENT_FILE_NAME_3] = arraySettings[RECENT_FILE_NAME_2];

                arraySettings[RECENT_FILE_PATH_2] = arraySettings[RECENT_FILE_PATH_1];
                arraySettings[RECENT_FILE_NAME_2] = arraySettings[RECENT_FILE_NAME_1];

                arraySettings[RECENT_FILE_PATH_1] = arraySettings[RECENT_FILE_PATH_0];
                arraySettings[RECENT_FILE_NAME_1] = arraySettings[RECENT_FILE_NAME_0];

                arraySettings[RECENT_FILE_PATH_0] = arrayProject[PROJECT_LOCATION];
                arraySettings[RECENT_FILE_NAME_0] = arrayProject[PROJECT_NAME];
            }
            else
            {
                //If file is not in recent files
                arraySettings[RECENT_FILE_PATH_4] = arraySettings[RECENT_FILE_PATH_3];
                arraySettings[RECENT_FILE_NAME_4] = arraySettings[RECENT_FILE_NAME_3];

                arraySettings[RECENT_FILE_PATH_3] = arraySettings[RECENT_FILE_PATH_2];
                arraySettings[RECENT_FILE_NAME_3] = arraySettings[RECENT_FILE_NAME_2];

                arraySettings[RECENT_FILE_PATH_2] = arraySettings[RECENT_FILE_PATH_1];
                arraySettings[RECENT_FILE_NAME_2] = arraySettings[RECENT_FILE_NAME_1];

                arraySettings[RECENT_FILE_PATH_1] = arraySettings[RECENT_FILE_PATH_0];
                arraySettings[RECENT_FILE_NAME_1] = arraySettings[RECENT_FILE_NAME_0];

                arraySettings[RECENT_FILE_PATH_0] = arrayProject[PROJECT_LOCATION];
                arraySettings[RECENT_FILE_NAME_0] = arrayProject[PROJECT_NAME];

            }

            saveSettings();
            recentFiles();

        }


        //Method used to save the settings
        private void saveSettings()
        {
            StreamWriter outputFile;

            outputFile = File.CreateText("settings.wdsettings");

            for (int i = 0; i < arraySettings.GetLength(0); i++)
            {
                outputFile.WriteLine(arraySettings[i]);
            }

            outputFile.Close();
        }


        //Method used to delete a page from the project
        private void deletePage(int pageNumber)
        {
            //Remove 1 page from project
            int intTotalPages = int.Parse(arrayProject[PROJECT_TOTAL_PAGES]);
            intTotalPages--;
            arrayProject[PROJECT_TOTAL_PAGES] = intTotalPages.ToString();

            //For moves data stored in each subscript up.
            for (int x = pageNumber; pageNumber < arrayWebsite.GetLength(0) && x + 1 < 5; x++)
            {
                for (int y = 0; y < arrayWebsite.GetLength(1); y++)
                {
                    arrayWebsite[x, y] = arrayWebsite[x + 1, y];
                }
            }

            //Kick the user off the page they're deleting if they're on it
            if (pageNumber == currentPage)
            {
                currentPage = 0;
            }

            updateProgram();
            updateMenus();
        }


        //Method used to update the editors menus
        private void updateMenus()
        {
            //Update Author Name
            labelWelcomeBack.Text = "Welcome back, " + arraySettings[USER_NAME];

            //Update Project Name
            labelProjectName.Text = arrayProject[PROJECT_NAME];

            //Update the Pages Panel
            switch (int.Parse(arrayProject[PROJECT_TOTAL_PAGES]))
            {
                case 1:
                    //Rename and Delete buttons
                    buttonRenamePage2.Visible = false;
                    buttonDeletePage2.Visible = false;
                    buttonRenamePage3.Visible = false;
                    buttonDeletePage3.Visible = false;
                    buttonRenamePage4.Visible = false;
                    buttonDeletePage4.Visible = false;
                    buttonRenamePage5.Visible = false;
                    buttonDeletePage5.Visible = false;

                    //AddPage Buttons
                    buttonAddPage2.Visible = true;
                    buttonAddPage3.Visible = false;
                    buttonAddPage4.Visible = false;
                    buttonAddPage5.Visible = false;

                    //Page Name Labels;
                    labelPage2Name.Visible = false;
                    labelPage3Name.Visible = false;
                    labelPage4Name.Visible = false;
                    labelPage5Name.Visible = false;
                    break;

                case 2:
                    //Rename and Delete buttons
                    buttonRenamePage2.Visible = true;
                    buttonDeletePage2.Visible = true;
                    buttonRenamePage3.Visible = false;
                    buttonDeletePage3.Visible = false;
                    buttonRenamePage4.Visible = false;
                    buttonDeletePage4.Visible = false;
                    buttonRenamePage5.Visible = false;
                    buttonDeletePage5.Visible = false;

                    //AddPage Buttons
                    buttonAddPage2.Visible = false;
                    buttonAddPage3.Visible = true;
                    buttonAddPage4.Visible = false;
                    buttonAddPage5.Visible = false;

                    //Page Name Labels;
                    labelPage2Name.Visible = true;
                    labelPage3Name.Visible = false;
                    labelPage4Name.Visible = false;
                    labelPage5Name.Visible = false;

                    break;
                case 3:
                    //Rename and Delete buttons
                    buttonRenamePage2.Visible = true;
                    buttonDeletePage2.Visible = true;
                    buttonRenamePage3.Visible = true;
                    buttonDeletePage3.Visible = true;
                    buttonRenamePage4.Visible = false;
                    buttonDeletePage4.Visible = false;
                    buttonRenamePage5.Visible = false;
                    buttonDeletePage5.Visible = false;

                    //AddPage Buttons
                    buttonAddPage2.Visible = false;
                    buttonAddPage3.Visible = false;
                    buttonAddPage4.Visible = true;
                    buttonAddPage5.Visible = false;

                    //Page Name Labels;
                    labelPage2Name.Visible = true;
                    labelPage3Name.Visible = true;
                    labelPage4Name.Visible = false;
                    labelPage5Name.Visible = false;

                    break;
                case 4:
                    //Rename and Delete buttons
                    buttonRenamePage2.Visible = true;
                    buttonDeletePage2.Visible = true;
                    buttonRenamePage3.Visible = true;
                    buttonDeletePage3.Visible = true;
                    buttonRenamePage4.Visible = true;
                    buttonDeletePage4.Visible = true;
                    buttonRenamePage5.Visible = false;
                    buttonDeletePage5.Visible = false;

                    //AddPage Buttons
                    buttonAddPage2.Visible = false;
                    buttonAddPage3.Visible = false;
                    buttonAddPage4.Visible = false;
                    buttonAddPage5.Visible = true;

                    //Page Name Labels;
                    labelPage2Name.Visible = true;
                    labelPage3Name.Visible = true;
                    labelPage4Name.Visible = true;
                    labelPage5Name.Visible = false;
                    break;
                case 5:
                    //Rename and Delete buttons
                    buttonRenamePage2.Visible = true;
                    buttonDeletePage2.Visible = true;
                    buttonRenamePage3.Visible = true;
                    buttonDeletePage3.Visible = true;
                    buttonRenamePage4.Visible = true;
                    buttonDeletePage4.Visible = true;
                    buttonRenamePage5.Visible = true;
                    buttonDeletePage5.Visible = true;

                    //AddPage Buttons
                    buttonAddPage2.Visible = false;
                    buttonAddPage3.Visible = false;
                    buttonAddPage4.Visible = false;
                    buttonAddPage5.Visible = false;

                    //Page Name Labels;
                    labelPage2Name.Visible = true;
                    labelPage3Name.Visible = true;
                    labelPage4Name.Visible = true;
                    labelPage5Name.Visible = true;
                    break;
            }

            //Update Page Names in page panel
            labelPage1Name.Text = arrayWebsite[0, PAGE_NAME];
            labelPage2Name.Text = arrayWebsite[1, PAGE_NAME];
            labelPage3Name.Text = arrayWebsite[2, PAGE_NAME];
            labelPage4Name.Text = arrayWebsite[3, PAGE_NAME];
            labelPage5Name.Text = arrayWebsite[4, PAGE_NAME];
        }


        //Method used to enable the editor
        private void enableEditor()
        {
            //Show the editor panel and update everything
            panelEditor.Visible = true;
            currentPage = 0;
            updateProgram();

            //Show all buttons related to the editor
            buttonPreview.Visible = true;
            buttonThemeSettings.Visible = true;
            buttonFontSettings.Visible = true;
            buttonPages.Visible = true;
            buttonCloseProject.Visible = true;
            buttonExit.Visible = true;

            //Enable Menu Strip items related to editor
            //File
            saveProjectToolStripMenuItem.Enabled = true;
            closeProjectToolStripMenuItem.Enabled = true;
            exportSiteToolStripMenuItem.Enabled = true;
            //View
            previewToolStripMenuItem.Enabled = true;
            themeSettingsToolStripMenuItem.Enabled = true;
            fontSettingsToolStripMenuItem.Enabled = true;
            pagesToolStripMenuItem.Enabled = true;
            //Project
            resetPageToolStripMenuItem.Enabled = true;
        }


        //Method used to disable the editor
        private void disableEditor()
        {
            //Hide all panels related to the editor
            panelCloseWithoutSaving.Visible = false;
            panelEditor.Visible = false;
            panelFontSettings.Visible = false;
            panelImageSettings.Visible = false;
            panelPages.Visible = false;
            panelThemeSettings.Visible = false;

            //Hide all buttons related to the editor
            buttonPreview.Visible = false;
            buttonThemeSettings.Visible = false;
            buttonFontSettings.Visible = false;
            buttonPages.Visible = false;
            buttonCloseProject.Visible = false;
            buttonExit.Visible = false;

            //Disable Menu Strip items related to editor
            //File
            saveProjectToolStripMenuItem.Enabled = false;
            closeProjectToolStripMenuItem.Enabled = false;
            exportSiteToolStripMenuItem.Enabled = false;
            //View
            previewToolStripMenuItem.Enabled = false;
            themeSettingsToolStripMenuItem.Enabled = false;
            fontSettingsToolStripMenuItem.Enabled = false;
            pagesToolStripMenuItem.Enabled = false;
            //Project
            resetPageToolStripMenuItem.Enabled = false;
        }


        //Method used to create a new project
        private void newProject()
        {
            //Show main menu
            panelMainMenu.Visible = true;

            //Show the New Project Panel and make sure its infront
            panelNewProject.Visible = true;
            panelNewProject.BringToFront();

            //Set focus to 1st textbox
            textBoxNewProjectName.Focus();

            //Set the default file path and set selection to the end so the end of the file path shows.
            textBoxNewProjectLocation.Text = Path.Combine(Environment.CurrentDirectory, @"Projects");
            textBoxNewProjectLocation.SelectionStart = textBoxNewProjectLocation.Text.Length;

            //Set theme selection to 0 (None)
            themeSelected = 0;
            buttonNewProjectThemeSingleBars.Image = ((System.Drawing.Image)(Properties.Resources.singleBars_ThemeBtn_notselected));

            //Hide Error Messages
            labelNewProjectLengthError.Visible = false;
            labelNewProjectThemeError.Visible = false;
        }


        //Method used to save a project
        private void saveProject()
        {
            try
            {
                //Set recently saved to true. This allows the project to be closed without the save prompt opening.
                recentlySaved = true;

                //Use StreamWriter to write the Project Array and website array to a file
                StreamWriter outputFile;

                //Set filter and initial directory
                saveFileProject.Filter = "Web Development Project (*.wdproject)|*.wdproject";
                saveFileProject.InitialDirectory = Path.Combine(Environment.CurrentDirectory, @"Projects");

                outputFile = File.CreateText(arrayProject[PROJECT_LOCATION]);

                for (int i = 0; i < arrayProject.GetLength(0); i++)
                {
                    outputFile.WriteLine(arrayProject[i]);
                }

                for (int x = 0; x < arrayWebsite.GetLength(0); x++)
                {
                    for (int y = 0; y < arrayWebsite.GetLength(1); y++)
                    {
                        outputFile.WriteLine(arrayWebsite[x, y]);
                    }
                }

                outputFile.Close();

                //Show the save success message
                showMessageBox("File Saved Successfully");

            }
            catch (Exception ex)
            {
                //If save fails, show an error message
                showMessageBox(ex.Message);
            }


        }


        //Method used to load a project
        private void loadProject()
        {
            //Set current page to 0
            currentPage = 0;

            //Clear the website array
            for (int x = 0; x < arrayWebsite.GetLength(0); x++)
            {
                for (int y = 0; y < arrayWebsite.GetLength(1); y++)
                {
                    arrayWebsite[x, y] = "";
                }
            }

            //Clear text boxes
            richTextBoxContent1.Text = "";
            richTextBoxContent2.Text = "";

            //Load a project file
            StreamReader inputFile;

            //Set filter and initial directory
            openFileProject.Filter = "Web Development Project (*.wdproject)|*.wdproject";
            openFileProject.InitialDirectory = Path.Combine(Environment.CurrentDirectory, @"Projects");

            if (openFileProject.ShowDialog() == DialogResult.OK)
            {
                inputFile = File.OpenText(openFileProject.FileName);

                //Read the first part of the project file which holds the Project variables
                for (int i = 0; i < arrayProject.GetLength(0); i++)
                {
                    arrayProject[i] = inputFile.ReadLine();
                }

                //Read the second part which holds the website variables
                for (int x = 0; x < arrayWebsite.GetLength(0); x++)
                {
                    for (int y = 0; y < arrayWebsite.GetLength(1); y++)
                    {
                        arrayWebsite[x, y] = inputFile.ReadLine();
                    }
                }

                panelMainMenu.Visible = false;

                //Enable the editor
                enableEditor();

                //Update the menus and program
                updateMenus();
                updateProgram();

                //Add to recent files
                addRecentFile();

                //Close the file
                inputFile.Close();
            }
        }

        private void loadRecentProject(string filePath)
        {
            //Set current page to 0
            currentPage = 0;

            //Clear the website array
            for (int x = 0; x < arrayWebsite.GetLength(0); x++)
            {
                for (int y = 0; y < arrayWebsite.GetLength(1); y++)
                {
                    arrayWebsite[x, y] = "";
                }
            }

            //Clear text boxes
            richTextBoxContent1.Text = "";
            richTextBoxContent2.Text = "";

            //Load a project file
            StreamReader inputFile;

            if (File.Exists(filePath))
            {
                inputFile = File.OpenText(filePath);

                //Read the first part of the project file which holds the Project variables
                for (int i = 0; i < arrayProject.GetLength(0); i++)
                {
                    arrayProject[i] = inputFile.ReadLine();
                }

                //Read the second part which holds the website variables
                for (int x = 0; x < arrayWebsite.GetLength(0); x++)
                {
                    for (int y = 0; y < arrayWebsite.GetLength(1); y++)
                    {
                        arrayWebsite[x, y] = inputFile.ReadLine();
                    }
                }

                panelMainMenu.Visible = false;

                //Enable the editor
                enableEditor();

                //Update the menus and program
                updateMenus();
                updateProgram();

                //Add to recent files
                addRecentFile();

                //Close the file
                inputFile.Close();
            }
            else
            {
                //If file fails to load, display error message
                showMessageBox("File Not Found");
            }

        }

        
        //Method to show help documents
        private void helpDocuments(int helpFile)
        {
            //Make the guide panel visible
            panelGuide.Visible = true;

            //On click, determine what image to show and what text.
            switch (helpFile)
            {
                case 1:
                    pictureBoxHelp.Image = ((System.Drawing.Image)(Properties.Resources.helpNew_Project));
                    labelHelpDescription.Text = "To start a new project, simply type in the title for your project, pick a location for the file to be saved" +
                        " and the theme for your website.";
                    break;
                case 2:
                    pictureBoxHelp.Image = ((System.Drawing.Image)(Properties.Resources.helpChanging_Content));
                    labelHelpDescription.Text = "To Change content in your website, select a textbox to determine what box you wish to update..";
                    break;
                case 3:
                    pictureBoxHelp.Image = ((System.Drawing.Image)(Properties.Resources.helpTheme_Settings));
                    labelHelpDescription.Text = "Once you have opened your project into the editor, you can use the theme settings to change the look and layout" +
                        "of your website. ";
                    break;
                case 4:
                    pictureBoxHelp.Image = ((System.Drawing.Image)(Properties.Resources.helpFont_Settings));
                    labelHelpDescription.Text = "The font settings can be used to set the fonts of the title, menu items, content, and footers. Here you can" +
                        "also set the font colors for your website.";
                    break;
                case 5:
                    pictureBoxHelp.Image = ((System.Drawing.Image)(Properties.Resources.helpPages_Panel1));
                    labelHelpDescription.Text = "To change pages, add a page, or rename a page; open the pages menu to create new pages for your site.";
                    break;
                case 6:
                    pictureBoxHelp.Image = ((System.Drawing.Image)(Properties.Resources.helpCreating_a_page));
                    labelHelpDescription.Text = "To create a page, click the plus symbol in the pages menu. You can create up to 5 pages for your project! Here" +
                        " you can determine what layout you wish to use.";
                    break;
                case 7:
                    pictureBoxHelp.Image = ((System.Drawing.Image)(Properties.Resources.helpRenaming_Pages));
                    labelHelpDescription.Text = "Once you are into the pages menu, click the rename text to rename the page to what you want.";
                    break;
            }
        }


        //Method to Export Site to HTML
        private void exportSite()
        {
            string exportPath = Environment.CurrentDirectory;

            //Use the WebsiteBuilder class to export the site
            WebsiteBuilder_singleBars exportSite = new WebsiteBuilder_singleBars();

            //Select the folder to use
            if (folderBrowserDialogExportHtml.ShowDialog() == DialogResult.OK)
            {
                exportPath = folderBrowserDialogExportHtml.SelectedPath;
                if (exportSite.buildWebsite(arrayWebsite, arrayProject, exportPath))
                {
                    //If the site exports successfully, show a message informing the user.
                    showMessageBox("Your Site Exported Successfully");
                }
                else
                {
                    //If export fails, show a message informing the user
                    showMessageBox("Your Site Was Not Exported");
                }
            }
        }


        //Method used to close the project.
        private void closeProject()
        {
            //Refresh recent files
            recentFiles();

            if (recentlySaved)
            {
                disableEditor();
                panelMainMenu.Visible = true;
                renamePanelOpen = false;
                createNewPagePanelOpen = false;
                colorPickerPanelOpen = false;
                themeSettingsOpen = false;
                fontSettingsOpen = false;
                pagesOpen = false;
            }
            else
            {
                //Set save box action to 1 and open the "Do you want to save" panel
                saveBoxAction = 1;
                panelCloseWithoutSaving.Visible = true;
            }
        }


        //Method used to Update all the editors graphics/controllers within the program
        private void updateProgram()
        {
            //Set recently saved variable to false
            recentlySaved = false;

            //Update Page Name Label
            labelPageCurrentlyOn.Text = arrayWebsite[currentPage, PAGE_NAME];

            //Update Page color theme
            pictureBoxColorBar.Image = arraySingleBarTheme[0, int.Parse(arrayWebsite[currentPage, PAGE_COLOR])];
            pictureBoxSiteHeader.Image = arraySingleBarTheme[1, int.Parse(arrayWebsite[currentPage, PAGE_COLOR])];
            textBoxTitle.BackColor = themeColor[int.Parse(arrayWebsite[currentPage, PAGE_COLOR])];

            //Update Fonts for editable site parts
            //Title
            if (int.Parse(arrayWebsite[currentPage, TITLE_FONT]) == 0)
            {
                textBoxTitle.Font = new Font("Arial", textBoxTitle.Font.Size);
            }
            else if (int.Parse(arrayWebsite[currentPage, TITLE_FONT]) == 1)
            {
                textBoxTitle.Font = new Font("Arial Black", textBoxTitle.Font.Size);
            }
            else if (int.Parse(arrayWebsite[currentPage, TITLE_FONT]) == 2)
            {
                textBoxTitle.Font = new Font("Comic Sans MS", textBoxTitle.Font.Size);
            }
            else if (int.Parse(arrayWebsite[currentPage, TITLE_FONT]) == 3)
            {
                textBoxTitle.Font = new Font("Impact", textBoxTitle.Font.Size);
            }
            else if (int.Parse(arrayWebsite[currentPage, TITLE_FONT]) == 4)
            {
                textBoxTitle.Font = new Font("Courier New", textBoxTitle.Font.Size);
            }
            else if (int.Parse(arrayWebsite[currentPage, TITLE_FONT]) == 5)
            {
                textBoxTitle.Font = new Font("Times New Roman", textBoxTitle.Font.Size);
            }
            else if (int.Parse(arrayWebsite[currentPage, TITLE_FONT]) == 6)
            {
                textBoxTitle.Font = new Font("Verdana", textBoxTitle.Font.Size);
            }
            //Menu
            if (int.Parse(arrayWebsite[currentPage, MENU_FONT]) == 0)
            {
                labelMenu.Font = new Font("Arial", labelMenu.Font.Size);
            }
            else if (int.Parse(arrayWebsite[currentPage, MENU_FONT]) == 1)
            {
                labelMenu.Font = new Font("Arial Black", labelMenu.Font.Size);
            }
            else if (int.Parse(arrayWebsite[currentPage, MENU_FONT]) == 2)
            {
                labelMenu.Font = new Font("Comic Sans MS", labelMenu.Font.Size);
            }
            else if (int.Parse(arrayWebsite[currentPage, MENU_FONT]) == 3)
            {
                labelMenu.Font = new Font("Impact", labelMenu.Font.Size);
            }
            else if (int.Parse(arrayWebsite[currentPage, MENU_FONT]) == 4)
            {
                labelMenu.Font = new Font("Courier New", labelMenu.Font.Size);
            }
            else if (int.Parse(arrayWebsite[currentPage, MENU_FONT]) == 5)
            {
                labelMenu.Font = new Font("Times New Roman", labelMenu.Font.Size);
            }
            else if (int.Parse(arrayWebsite[currentPage, MENU_FONT]) == 6)
            {
                labelMenu.Font = new Font("Verdana", labelMenu.Font.Size);
            }
            //Content
            if (int.Parse(arrayWebsite[currentPage, CONTENT_FONT]) == 0)
            {
                richTextBoxContent1.Font = new Font("Arial", richTextBoxContent1.Font.Size);
                richTextBoxContent2.Font = new Font("Arial", richTextBoxContent2.Font.Size);
            }
            else if (int.Parse(arrayWebsite[currentPage, CONTENT_FONT]) == 1)
            {
                richTextBoxContent1.Font = new Font("Arial Black", richTextBoxContent1.Font.Size);
                richTextBoxContent2.Font = new Font("Arial Black", richTextBoxContent2.Font.Size);
            }
            else if (int.Parse(arrayWebsite[currentPage, CONTENT_FONT]) == 2)
            {
                richTextBoxContent1.Font = new Font("Comic Sans MS", richTextBoxContent1.Font.Size);
                richTextBoxContent2.Font = new Font("Comic Sans MS", richTextBoxContent2.Font.Size);
            }
            else if (int.Parse(arrayWebsite[currentPage, CONTENT_FONT]) == 3)
            {
                richTextBoxContent1.Font = new Font("Impact", richTextBoxContent1.Font.Size);
                richTextBoxContent2.Font = new Font("Impact", richTextBoxContent2.Font.Size);
            }
            else if (int.Parse(arrayWebsite[currentPage, CONTENT_FONT]) == 4)
            {
                richTextBoxContent1.Font = new Font("Courier New", richTextBoxContent1.Font.Size);
                richTextBoxContent2.Font = new Font("Courier New", richTextBoxContent2.Font.Size);
            }
            else if (int.Parse(arrayWebsite[currentPage, CONTENT_FONT]) == 5)
            {
                richTextBoxContent1.Font = new Font("Times New Roman", richTextBoxContent1.Font.Size);
                richTextBoxContent2.Font = new Font("Times New Roman", richTextBoxContent2.Font.Size);
            }
            else if (int.Parse(arrayWebsite[currentPage, CONTENT_FONT]) == 6)
            {
                richTextBoxContent1.Font = new Font("Verdana", richTextBoxContent1.Font.Size);
                richTextBoxContent2.Font = new Font("Verdana", richTextBoxContent2.Font.Size);
            }
            //Footer
            if (int.Parse(arrayWebsite[currentPage, FOOTER_FONT]) == 0)
            {
                textBoxFooter.Font = new Font("Arial", textBoxFooter.Font.Size);
            }
            else if (int.Parse(arrayWebsite[currentPage, FOOTER_FONT]) == 1)
            {
                textBoxFooter.Font = new Font("Arial Black", textBoxFooter.Font.Size);
            }
            else if (int.Parse(arrayWebsite[currentPage, FOOTER_FONT]) == 2)
            {
                textBoxFooter.Font = new Font("Comic Sans MS", textBoxFooter.Font.Size);
            }
            else if (int.Parse(arrayWebsite[currentPage, FOOTER_FONT]) == 3)
            {
                textBoxFooter.Font = new Font("Impact", textBoxFooter.Font.Size);
            }
            else if (int.Parse(arrayWebsite[currentPage, FOOTER_FONT]) == 4)
            {
                textBoxFooter.Font = new Font("Courier New", textBoxFooter.Font.Size);
            }
            else if (int.Parse(arrayWebsite[currentPage, FOOTER_FONT]) == 5)
            {
                textBoxFooter.Font = new Font("Times New Roman", textBoxFooter.Font.Size);
            }
            else if (int.Parse(arrayWebsite[currentPage, FOOTER_FONT]) == 6)
            {
                textBoxFooter.Font = new Font("Verdana", textBoxFooter.Font.Size);
            }

            //Update the Font color
            //Title
            System.Drawing.Color titleColor = System.Drawing.ColorTranslator.FromHtml(arrayWebsite[currentPage, TITLE_TEXT_COLOR]);
            textBoxTitle.ForeColor = titleColor;
            buttonTitleFontColor.BackColor = titleColor;

            // Menu
            System.Drawing.Color menuColor = System.Drawing.ColorTranslator.FromHtml(arrayWebsite[currentPage, MENU_TEXT_COLOR]);
            labelMenu.ForeColor = menuColor;
            buttonMenuFontColor.BackColor = menuColor;

            // Menu On Hover
            System.Drawing.Color menuHoverColor = System.Drawing.ColorTranslator.FromHtml(arrayWebsite[currentPage, MENU_TEXT_COLOR_HOVER]);
            buttonMenuOnHoverFontColor.BackColor = menuHoverColor;

            // Content
            System.Drawing.Color contentColor = System.Drawing.ColorTranslator.FromHtml(arrayWebsite[currentPage, CONTENT_TEXT_COLOR]);
            richTextBoxContent1.ForeColor = contentColor;
            richTextBoxContent2.ForeColor = contentColor;
            buttonContentFontColor.BackColor = contentColor;

            // Footer
            System.Drawing.Color footerColor = System.Drawing.ColorTranslator.FromHtml(arrayWebsite[currentPage, FOOTER_TEXT_COLOR]);
            textBoxFooter.ForeColor = footerColor;
            buttonFooterFontColor.BackColor = footerColor;


            //Update the image content
            //Try to load image URL, if it fails, fill with a blank image.
            try
            {
                pictureBoxSiteImage.Load(arrayWebsite[currentPage, IMAGE_LOCATION]);
            }
            catch
            {
                pictureBoxSiteImage.Image = ((System.Drawing.Image)(Properties.Resources.topBar));
            }

            //Update the page layout
            if (arrayWebsite[currentPage, PAGE_LAYOUT] == "singleLayout")
            {
                //Set layout to single. Works by setting Content 1 to a larger size and hiding content 2 and the image.
                pictureBoxLayoutSingle.Image = ((System.Drawing.Image)(Properties.Resources.layoutIconSingle_selected));
                pictureBoxLayoutMultiple.Image = ((System.Drawing.Image)(Properties.Resources.layoutIconMultiple));
                richTextBoxContent1.Height = 437;
                richTextBoxContent1.Width = 511;

                richTextBoxContent2.Enabled = false;
                richTextBoxContent2.Visible = false;
                pictureBoxSiteImage.Visible = false;
                panelImageSettings.Visible = false;
                panelImageSettings.Enabled = false;

            }
            else if (arrayWebsite[currentPage, PAGE_LAYOUT] == "multipleLayout")
            {
                //Set layout to multiple. Works by shrinking content 1 to take up less space and showing content 2 and the image.
                pictureBoxLayoutMultiple.Image = ((System.Drawing.Image)(Properties.Resources.layoutIconMultiple_selected));
                pictureBoxLayoutSingle.Image = ((System.Drawing.Image)(Properties.Resources.layoutIconSingle));
                richTextBoxContent1.Height = 215;
                richTextBoxContent1.Width = 250;


                richTextBoxContent2.Enabled = true;
                richTextBoxContent2.Visible = true;
                pictureBoxSiteImage.Visible = true;
            }

            //Update title, Content, and Footer
            textBoxTitle.Text = arrayWebsite[currentPage, TITLE_TEXT];

            //Add New lines where ¶ is stored in the array
            var str1 = arrayWebsite[currentPage, CONTENT_1_TEXT].Replace('¶', '\n');
            richTextBoxContent1.Text = str1;

            //Add New lines where ¶ is stored in the array
            var str2 = arrayWebsite[currentPage, CONTENT_2_TEXT].Replace('¶', '\n');
            richTextBoxContent2.Text = str2;

            textBoxFooter.Text = arrayWebsite[currentPage, FOOTER_TEXT];

            //Update Side Bar Page Label
            labelPageCurrentlyOn.Text = arrayWebsite[currentPage, PAGE_NAME];

            //Set page label color based on what page is being edited.
            switch (currentPage)
            {
                case 0:
                    labelPage1Name.ForeColor = Color.DodgerBlue;
                    labelPage2Name.ForeColor = Color.FromArgb(208, 208, 208);
                    labelPage3Name.ForeColor = Color.FromArgb(208, 208, 208);
                    labelPage4Name.ForeColor = Color.FromArgb(208, 208, 208);
                    labelPage5Name.ForeColor = Color.FromArgb(208, 208, 208);
                    break;
                case 1:
                    labelPage2Name.ForeColor = Color.DodgerBlue;
                    labelPage1Name.ForeColor = Color.FromArgb(208, 208, 208);
                    labelPage3Name.ForeColor = Color.FromArgb(208, 208, 208);
                    labelPage4Name.ForeColor = Color.FromArgb(208, 208, 208);
                    labelPage5Name.ForeColor = Color.FromArgb(208, 208, 208);
                    break;
                case 2:
                    labelPage3Name.ForeColor = Color.DodgerBlue;
                    labelPage1Name.ForeColor = Color.FromArgb(208, 208, 208);
                    labelPage2Name.ForeColor = Color.FromArgb(208, 208, 208);
                    labelPage4Name.ForeColor = Color.FromArgb(208, 208, 208);
                    labelPage5Name.ForeColor = Color.FromArgb(208, 208, 208);
                    break;
                case 3:
                    labelPage4Name.ForeColor = Color.DodgerBlue;
                    labelPage1Name.ForeColor = Color.FromArgb(208, 208, 208);
                    labelPage2Name.ForeColor = Color.FromArgb(208, 208, 208);
                    labelPage3Name.ForeColor = Color.FromArgb(208, 208, 208);
                    labelPage5Name.ForeColor = Color.FromArgb(208, 208, 208);
                    break;
                case 4:
                    labelPage5Name.ForeColor = Color.DodgerBlue;
                    labelPage1Name.ForeColor = Color.FromArgb(208, 208, 208);
                    labelPage2Name.ForeColor = Color.FromArgb(208, 208, 208);
                    labelPage3Name.ForeColor = Color.FromArgb(208, 208, 208);
                    labelPage4Name.ForeColor = Color.FromArgb(208, 208, 208);
                    break;

            }

            //Update Menu Page Text
            switch (int.Parse(arrayProject[PROJECT_TOTAL_PAGES]))
            {
                case 1:
                    labelMenu.Text = "Home";
                    break;
                case 2:
                    labelMenu.Text = "Home" + "      " + arrayWebsite[1, PAGE_NAME];
                    break;
                case 3:
                    labelMenu.Text = "Home" + "      " + arrayWebsite[1, PAGE_NAME] + "      " + arrayWebsite[2, PAGE_NAME];
                    break;
                case 4:
                    labelMenu.Text = "Home" + "      " + arrayWebsite[1, PAGE_NAME] + "      " + arrayWebsite[2, PAGE_NAME]
                                     + "      " + arrayWebsite[3, PAGE_NAME];
                    break;
                case 5:
                    labelMenu.Text = "Home" + "      " + arrayWebsite[1, PAGE_NAME] + "      " + arrayWebsite[2, PAGE_NAME]
                                     + "      " + arrayWebsite[3, PAGE_NAME] + "      " + arrayWebsite[4, PAGE_NAME];
                    break;
            }
        }


        //Method used to change the color of the website theme
        private void changeThemeColor(int color)
        {
            //Changes the theme of the web page
            arrayWebsite[currentPage, PAGE_COLOR] = color.ToString();
            pictureBoxColorBar.Image = arraySingleBarTheme[0, color];
            pictureBoxSiteHeader.Image = arraySingleBarTheme[1, color];
            textBoxTitle.BackColor = themeColor[color];
        }

        //Method that compiles an individual page and opens it for viewing
        private void openPreview()
        {
            string exportPath = Path.Combine(Environment.CurrentDirectory, @"Preview");

            //Use the WebsiteBuilder Class to export the site
            WebsiteBuilder_singleBars exportSite = new WebsiteBuilder_singleBars();

            if (exportSite.buildWebsite(arrayWebsite, arrayProject, exportPath))
            {
                  //If the site exports successfully, Open the page for previewing
                  System.Diagnostics.Process.Start(Path.Combine(exportSite.getExportLocation(),"index.html"));
            }
            else
            {
                  //If export fails, show a message informing the user
                  showMessageBox("Your Site Was Not able to be previewed");
            }
        }


        //Method for opening and closing the Theme Settings Panel
        private void toggleThemeSettings()
        {
            //Open theme settings menu if it isn't open, close it if it is open.
            if (themeSettingsOpen)
            {
                themeSettingsOpen = false;
                this.buttonThemeSettings.Image = ((System.Drawing.Image)(Properties.Resources.themeSettings_btn_onHover));
                panelThemeSettings.Visible = false;
                themeSettingsToolStripMenuItem.Checked = false;
            }
            else
            {
                themeSettingsOpen = true;
                this.buttonThemeSettings.Image = ((System.Drawing.Image)(Properties.Resources.themeSettings_btn_selected));
                panelThemeSettings.Visible = true;
                themeSettingsToolStripMenuItem.Checked = true;

                //Hide Font Settings
                fontSettingsOpen = false;
                buttonFontSettings.Image = ((System.Drawing.Image)(Properties.Resources.fontSettings_btn_static));
                panelFontSettings.Visible = false;
                fontSettingsToolStripMenuItem.Checked = false;

                //Hide Pages
                pagesOpen = false;
                panelPages.Visible = false;
                buttonPages.Image = ((System.Drawing.Image)(Properties.Resources.pages_btn_static));
                pagesToolStripMenuItem.Checked = false;

                //Ensure the proper layout is selected on the menu
                if (arrayWebsite[currentPage, PAGE_LAYOUT] == "singleLayout")
                {
                    pictureBoxLayoutSingle.Image = ((System.Drawing.Image)(Properties.Resources.layoutIconSingle_selected));
                    pictureBoxLayoutMultiple.Image = ((System.Drawing.Image)(Properties.Resources.layoutIconMultiple));
                }
                else if (arrayWebsite[currentPage, PAGE_LAYOUT] == "multipleLayout")
                {
                    pictureBoxLayoutSingle.Image = ((System.Drawing.Image)(Properties.Resources.layoutIconSingle));
                    pictureBoxLayoutMultiple.Image = ((System.Drawing.Image)(Properties.Resources.layoutIconMultiple_selected));
                }
            }
        }


        //Method used for opening and closing the Font Settings Panel
        private void toggleFontSettings()
        {
            if (fontSettingsOpen)
            {
                fontSettingsOpen = false;
                this.buttonFontSettings.Image = ((System.Drawing.Image)(Properties.Resources.fontSettings_btn_onHover));
                panelFontSettings.Visible = false;
                fontSettingsToolStripMenuItem.Checked = false;
            }
            else
            {
                fontSettingsOpen = true;
                this.buttonFontSettings.Image = ((System.Drawing.Image)(Properties.Resources.fontSettings_btn_selected));
                panelFontSettings.Visible = true;
                fontSettingsToolStripMenuItem.Checked = true;

                //Hide Theme Settings
                themeSettingsOpen = false;
                panelThemeSettings.Visible = false;
                buttonThemeSettings.Image = ((System.Drawing.Image)(Properties.Resources.themeSettings_btn_static));
                themeSettingsToolStripMenuItem.Checked = false;

                //Hide Pages
                pagesOpen = false;
                panelPages.Visible = false;
                buttonPages.Image = ((System.Drawing.Image)(Properties.Resources.pages_btn_static));
                pagesToolStripMenuItem.Checked = false;
            }
        }


        //Method used for opening and closing the Pages PAne
        private void togglePageSettings()
        {
            if (pagesOpen)
            {
                pagesOpen = false;
                this.buttonPages.Image = ((System.Drawing.Image)(Properties.Resources.pages_btn_onHover));
                panelPages.Visible = false;
                pagesToolStripMenuItem.Checked = false;
            }
            else
            {
                pagesOpen = true;
                this.buttonPages.Image = ((System.Drawing.Image)(Properties.Resources.pages_btn_selected));
                panelPages.Visible = true;
                pagesToolStripMenuItem.Checked = true;

                //Hide Theme Settings
                themeSettingsOpen = false;
                panelThemeSettings.Visible = false;
                buttonThemeSettings.Image = ((System.Drawing.Image)(Properties.Resources.themeSettings_btn_static));
                themeSettingsToolStripMenuItem.Checked = false;

                //Hide Font Settings
                fontSettingsOpen = false;
                panelFontSettings.Visible = false;
                buttonFontSettings.Image = ((System.Drawing.Image)(Properties.Resources.fontSettings_btn_static));
                fontSettingsToolStripMenuItem.Checked = false;
            }
        }


        //Method for changing the layout of the theme
        private void toggleLayout(int layoutSelected)
        {
            if (layoutSelected == 1)
            {
                //Set layout to single.
                arrayWebsite[currentPage, PAGE_LAYOUT] = "singleLayout";

            }
            else if (layoutSelected == 2)
            {
                //Set layout to multiple.
                arrayWebsite[currentPage, PAGE_LAYOUT] = "multipleLayout";
            }

            updateProgram();
        }


        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /*//////////////////////////////Top Bar Buttons: Save Project/Export Site////////////////////////////////////*/
        private void buttonSaveProject_Click(object sender, EventArgs e)
        {
            //Save the Project
            saveProject();
        }

        private void buttonSaveProject_MouseEnter(object sender, EventArgs e)
        {
            //On hover image
            this.buttonSaveProject.Image = ((System.Drawing.Image)(Properties.Resources.saveProject_btn_onHover));
        }

        private void buttonSaveProject_MouseLeave(object sender, EventArgs e)
        {
            //Static image when not hovering
            this.buttonSaveProject.Image = ((System.Drawing.Image)(Properties.Resources.saveProject_btn_static));
        }

        private void buttonExportSite_Click(object sender, EventArgs e)
        {
            exportSite();
        }

        private void buttonExportSite_MouseEnter(object sender, EventArgs e)
        {
            //On hover image
            this.buttonExportSite.Image = ((System.Drawing.Image)(Properties.Resources.exportSite_btn_onHover));
        }

        private void buttonExportSite_MouseLeave(object sender, EventArgs e)
        {
            //Static image when not hovering
            this.buttonExportSite.Image = ((System.Drawing.Image)(Properties.Resources.exportSite_btn_static));
        }





        /*//////////////////////////Side Bar Button Events (Click, Mouse Enter, Mouse Leave///////////////////////////////*/
        private void buttonPreview_Click(object sender, EventArgs e)
        {
            openPreview();
        }


        private void buttonPreview_MouseEnter(object sender, EventArgs e)
        {
            buttonPreview.Image = ((System.Drawing.Image)(Properties.Resources.preview_btn_onHover));
        }

        private void buttonPreview_MouseLeave(object sender, EventArgs e)
        {
            buttonPreview.Image = ((System.Drawing.Image)(Properties.Resources.preview_btn_static));
        }

        private void buttonThemeSettings_Click(object sender, EventArgs e)
        {
            toggleThemeSettings();
        }

        private void buttonThemeSettings_MouseEnter(object sender, EventArgs e)
        {
            //On hover image
            if (!themeSettingsOpen)
            {
                this.buttonThemeSettings.Image = ((System.Drawing.Image)(Properties.Resources.themeSettings_btn_onHover));
            }
        }

        private void buttonThemeSettings_MouseLeave(object sender, EventArgs e)
        {
            //Static image when not hovering
            if (!themeSettingsOpen)
            {
                this.buttonThemeSettings.Image = ((System.Drawing.Image)(Properties.Resources.themeSettings_btn_static));
            }
        }

        private void buttonFontSettings_Click(object sender, EventArgs e)
        {
            //Open/Close the Font Settings
            toggleFontSettings();
        }

        private void buttonPages_Click(object sender, EventArgs e)
        {
            //Open/Close the Page Settings
            togglePageSettings();
        }

        private void buttonFontSettings_MouseEnter(object sender, EventArgs e)
        {
            if (!fontSettingsOpen)
            {
                //On hover image
                this.buttonFontSettings.Image = ((System.Drawing.Image)(Properties.Resources.fontSettings_btn_onHover));
            }
        }

        private void buttonFontSettings_MouseLeave(object sender, EventArgs e)
        {
            if (!fontSettingsOpen)
            {
                //Static image when not hovering
                this.buttonFontSettings.Image = ((System.Drawing.Image)(Properties.Resources.fontSettings_btn_static));
            }
        }

        private void buttonPages_MouseEnter(object sender, EventArgs e)
        {
            if (!pagesOpen)
            {
                //On hover image
                this.buttonPages.Image = ((System.Drawing.Image)(Properties.Resources.pages_btn_onHover));
            }
        }

        private void buttonPages_MouseLeave(object sender, EventArgs e)
        {
            if (!pagesOpen)
            {
                //Static image when not hovering
                this.buttonPages.Image = ((System.Drawing.Image)(Properties.Resources.pages_btn_static));
            }
        }

        private void buttonCloseProject_Click(object sender, EventArgs e)
        {
            closeProject();
        }

        private void buttonCloseProject_MouseEnter(object sender, EventArgs e)
        {
            //On hover image
            this.buttonCloseProject.Image = ((System.Drawing.Image)(Properties.Resources.closeProject_btn_onHover));
        }

        private void buttonCloseProject_MouseLeave(object sender, EventArgs e)
        {
            //Static image when not hovering
            this.buttonCloseProject.Image = ((System.Drawing.Image)(Properties.Resources.closeProject_btn_static));
        }

        private void buttonExit_Click(object sender, EventArgs e)
        {
            exitProgram();
        }

        private void buttonExit_MouseEnter(object sender, EventArgs e)
        {
            //On hover image
            this.buttonExit.Image = ((System.Drawing.Image)(Properties.Resources.exit_btn_onHover));
        }

        private void buttonExit_MouseLeave(object sender, EventArgs e)
        {
            //Static image when not hovering
            this.buttonExit.Image = ((System.Drawing.Image)(Properties.Resources.exit_btn_static));
        }





        /*////////////////////////////Site Image Picture Box////////////////////////////////////*/
        private void pictureBoxSiteImage_Click(object sender, EventArgs e)
        {
            //Show the Image Panel
            panelImageSettings.Visible = true;
            panelImageSettings.Enabled = true;

            //Fill text boxes with location info for image
            switch (int.Parse(arrayWebsite[currentPage, IMAGE_LOCATION_TYPE]))
            {
                case 0:
                    textBoxImageFile.Text = "";
                    textBoxImageUrl.Text = "";
                    radioButtonImageLocalFile.Checked = true;
                    break;
                case 1:
                    textBoxImageFile.Text = arrayWebsite[currentPage, IMAGE_LOCATION];
                    break;
                case 2:
                    textBoxImageUrl.Text = arrayWebsite[currentPage, IMAGE_LOCATION];
                    break;
            }

            //Ensure panel is on top
            panelImageSettings.BringToFront();
        }





        /*//////////////////////////Theme Settings Panel//////////////////////////////////*/
        private void buttonThemeNavy_Click(object sender, EventArgs e)
        {
            //Change to the Navy theme color
            changeThemeColor(0);
        }

        private void buttonThemeMaroon_Click(object sender, EventArgs e)
        {
            //Change to the Maroon theme color
            changeThemeColor(1);
        }

        private void buttonThemeGreen_Click(object sender, EventArgs e)
        {
            //Change to the Green theme color
            changeThemeColor(2);
        }

        private void buttonThemeLightBlue_Click(object sender, EventArgs e)
        {
            //Change to the Light Blue theme color
            changeThemeColor(3);
        }

        private void buttonThemeOrange_Click(object sender, EventArgs e)
        {
            //Change to the Orange theme color
            changeThemeColor(4);
        }

        private void buttonThemeYellow_Click(object sender, EventArgs e)
        {
            //Change to the Yellow theme color
            changeThemeColor(5);
        }

        private void buttonThemePurple_Click(object sender, EventArgs e)
        {
            //Change to the Purple theme color
            changeThemeColor(6);
        }

        private void buttonThemeRed_Click(object sender, EventArgs e)
        {
            //Change to the Red theme color
            changeThemeColor(7);
        }

        private void pictureBoxLayoutSingle_Click(object sender, EventArgs e)
        {
            //Change to layout 1 (single)
            toggleLayout(1);
        }

        private void pictureBoxLayoutMultiple_Click(object sender, EventArgs e)
        {
            //Change to layout 2 (multiple)
            toggleLayout(2);
        }





        /*//////////////////////////Image Location Selection Panel//////////////////////////////////*/
        private void radioButtonImageLocalFile_CheckedChanged(object sender, EventArgs e)
        {
            //Ensure only a one local file or a URL can be used. Not Both.
            if (radioButtonImageLocalFile.Checked)
            {
                textBoxImageUrl.Enabled = false;
            }
            else
            {
                textBoxImageUrl.Enabled = true;
            }
        }

        private void radioButtonImageUrl_CheckedChanged(object sender, EventArgs e)
        {
            //Ensure only a one local file or a URL can be used. Not Both.
            if (radioButtonImageUrl.Checked)
            {
                textBoxImageFile.Enabled = false;
            }
            else
            {
                textBoxImageFile.Enabled = true;
            }
        }

        private void buttonBrowseImages_Click(object sender, EventArgs e)
        {
            // image filters  
            openFileUserImage.Filter = "Image(*.jpg; *.jpeg; *.gif; *.bmp; *.png)|*.jpg; *.jpeg; *.gif; *.bmp; *.png";

            if (openFileUserImage.ShowDialog() == DialogResult.OK)
            {
                // display image in picture box  
                textBoxImageFile.Text = openFileUserImage.FileName;
            }
        }

        private void buttonUpdateImage_Click(object sender, EventArgs e)
        {
            //Determine what radioButton is checked and set image file type
            if (radioButtonImageLocalFile.Checked)
            {
                arrayWebsite[currentPage, IMAGE_LOCATION] = textBoxImageFile.Text;
                arrayWebsite[currentPage, IMAGE_LOCATION_TYPE] = "1";
            }
            else if (radioButtonImageUrl.Checked)
            {
                arrayWebsite[currentPage, IMAGE_LOCATION] = textBoxImageUrl.Text;
                arrayWebsite[currentPage, IMAGE_LOCATION_TYPE] = "2";
            }

            //Update Image URL alt
            arrayWebsite[currentPage, IMAGE_ALT] = textBoxImageAlt.Text;

            //Close the image panel
            panelImageSettings.Visible = false;
            panelImageSettings.Enabled = false;

            try
            {
                //Try loading the image
                pictureBoxSiteImage.Load(arrayWebsite[currentPage, IMAGE_LOCATION]);
                var destFileName = Path.GetFileName(arrayWebsite[currentPage, IMAGE_LOCATION]);

                if (!File.Exists(@"User Images\" + destFileName))
                {
                    pictureBoxSiteImage.Image.Save(@"User Images\" + destFileName);
                }

                arrayWebsite[currentPage, IMAGE_LOCATION] = Path.Combine(Environment.CurrentDirectory, @"User Images\", destFileName);
            }
            catch
            {
                //If it fails, mention the bad link in the message box
                showMessageBox("Bad image link, please try again");
            }

            updateProgram();
        }

        private void buttonCancelImageUpdate_Click(object sender, EventArgs e)
        {
            //Close URL Panel without making changes
            panelImageSettings.Visible = false;
            panelImageSettings.Enabled = false;
        }


        /*//////////////////////////panelFontSettings//////////////////////////////////*/
        //Font Settings: Font Color Boxes
        private void buttonTitleFontColor_Click(object sender, EventArgs e)
        {
            //Only open dialog if other menus are closed
            if (!colorPickerPanelOpen && !createNewPagePanelOpen && !renamePanelOpen)
            {
                //Use the color dialog to select a color and convert it to an html readable color
                if (colorDialog.ShowDialog() == DialogResult.OK)
                {
                    buttonTitleFontColor.BackColor = colorDialog.Color;
                    String htmlColor = System.Drawing.ColorTranslator.ToHtml(colorDialog.Color);

                    //Update Array
                    arrayWebsite[currentPage, TITLE_TEXT_COLOR] = htmlColor;

                    updateProgram();
                }
            }
        }

        private void buttonMenuFontColor_Click(object sender, EventArgs e)
        {
            //Only open dialog if other menus are closed
            if (!colorPickerPanelOpen && !createNewPagePanelOpen && !renamePanelOpen)
            {
                //Use the color dialog to select a color and convert it to an html readable color
                if (colorDialog.ShowDialog() == DialogResult.OK)
                {
                    buttonTitleFontColor.BackColor = colorDialog.Color;
                    String htmlColor = System.Drawing.ColorTranslator.ToHtml(colorDialog.Color);

                    //Update Array
                    arrayWebsite[currentPage, MENU_TEXT_COLOR] = htmlColor;

                    updateProgram();
                }
            }
        }

        private void buttonMenuOnHoverFontColor_Click(object sender, EventArgs e)
        {
            //Only open dialog if other menus are closed
            if (!colorPickerPanelOpen && !createNewPagePanelOpen && !renamePanelOpen)
            {
                //Use the color dialog to select a color and convert it to an html readable color
                if (colorDialog.ShowDialog() == DialogResult.OK)
                {
                    buttonTitleFontColor.BackColor = colorDialog.Color;
                    String htmlColor = System.Drawing.ColorTranslator.ToHtml(colorDialog.Color);

                    //Update Array
                    arrayWebsite[currentPage, MENU_TEXT_COLOR_HOVER] = htmlColor;

                    updateProgram();
                }
            }
        }

        private void buttonContentFontColor_Click(object sender, EventArgs e)
        {
            //Only open dialog if other menus are closed
            if (!colorPickerPanelOpen && !createNewPagePanelOpen && !renamePanelOpen)
            {
                //Use the color dialog to select a color and convert it to an html readable color
                if (colorDialog.ShowDialog() == DialogResult.OK)
                {
                    buttonTitleFontColor.BackColor = colorDialog.Color;
                    String htmlColor = System.Drawing.ColorTranslator.ToHtml(colorDialog.Color);

                    //Update Array
                    arrayWebsite[currentPage, CONTENT_TEXT_COLOR] = htmlColor;

                    updateProgram();
                }
            }
        }

        private void buttonFooterFontColor_Click(object sender, EventArgs e)
        {
            //Only open dialog if other menus are closed
            if (!colorPickerPanelOpen && !createNewPagePanelOpen && !renamePanelOpen)
            {
                //Use the color dialog to select a color and convert it to an html readable color
                if (colorDialog.ShowDialog() == DialogResult.OK)
                {
                    buttonTitleFontColor.BackColor = colorDialog.Color;
                    String htmlColor = System.Drawing.ColorTranslator.ToHtml(colorDialog.Color);

                    //Update Array
                    arrayWebsite[currentPage, FOOTER_TEXT_COLOR] = htmlColor;

                    updateProgram();
                }
            }
        }

        //Font Settings: Font Combo Boxes
        private void comboBoxTitleFont_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Changing the font Updates the displayed font and the stored value in the array.
            arrayWebsite[currentPage, TITLE_FONT] = comboBoxTitleFont.SelectedIndex.ToString();
            updateProgram();
        }

        private void comboBoxMenuFont_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Changing the font Updates the displayed font and the stored value in the array. In the menus case, the onhover combo box text is updated.
            arrayWebsite[currentPage, MENU_FONT] = comboBoxMenuFont.SelectedIndex.ToString();
            comboBoxMenuOnHoverFont.Text = comboBoxMenuFont.Text;
            updateProgram();
        }

        private void comboBoxContentFont_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Changing the font Updates the displayed font and the stored value in the array.
            arrayWebsite[currentPage, CONTENT_FONT] = comboBoxContentFont.SelectedIndex.ToString();
            updateProgram();
        }

        private void comboBoxFooterFont_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Changing the font Updates the displayed font and the stored value in the array.
            arrayWebsite[currentPage, FOOTER_FONT] = comboBoxFooterFont.SelectedIndex.ToString();
            updateProgram();
        }





        /*///////////////////////panelCloseWithoutSaving/////////////////////////*/
        private void buttonCloseSave_Click(object sender, EventArgs e)
        {
            //Save the Project
            saveProject();

            if (saveBoxAction == 1) //Close Project
            {
                disableEditor();
                panelMainMenu.Visible = true;
                saveBoxAction = 0;
                renamePanelOpen = false;
                createNewPagePanelOpen = false;
                colorPickerPanelOpen = false;
                themeSettingsOpen = false;
                fontSettingsOpen = false;
                pagesOpen = false;
            }
            else if (saveBoxAction == 2) //Open Project
            {
                loadProject();
                saveBoxAction = 0;
            }
            else if (saveBoxAction == 3) //Reset Page
            {
                //Reset all the text boxes and go to the editor page.
                toggleLayout(1);
                changeThemeColor(0);
                arrayWebsite[currentPage, PAGE_LAYOUT] = "1";
                arrayWebsite[currentPage, PAGE_COLOR] = "0";
                arrayWebsite[currentPage, TITLE_FONT] = "0";
                arrayWebsite[currentPage, TITLE_TEXT_COLOR] = "#FFFFFF";
                arrayWebsite[currentPage, TITLE_TEXT] = "Your Title";
                arrayWebsite[currentPage, MENU_FONT] = "0";
                arrayWebsite[currentPage, MENU_TEXT_COLOR] = "#000000";
                arrayWebsite[currentPage, MENU_TEXT_COLOR_HOVER] = "#000000";
                arrayWebsite[currentPage, CONTENT_FONT] = "0";
                arrayWebsite[currentPage, CONTENT_TEXT_COLOR] = "#000000";
                arrayWebsite[currentPage, CONTENT_1_TEXT] = "Welcome to the Bach Web Builder! (Cooler Name Pending?)";
                arrayWebsite[currentPage, CONTENT_2_TEXT] = "";
                arrayWebsite[currentPage, IMAGE_LOCATION] = "";
                arrayWebsite[currentPage, IMAGE_ALT] = "";
                arrayWebsite[currentPage, FOOTER_FONT] = "0";
                arrayWebsite[currentPage, FOOTER_TEXT] = "Footer text goes in here";
                arrayWebsite[currentPage, FOOTER_TEXT_COLOR] = "#FFFFFF";

                updateProgram();
                saveBoxAction = 0;
            }
            else if (saveBoxAction == 4)
            {
                //Close the program
                this.Close();
            }

            //Close the panel
            panelCloseWithoutSaving.Visible = false;
        }

        private void buttonCloseDontSave_Click(object sender, EventArgs e)
        {
            //Continue without saving
            if (saveBoxAction == 1) //Close Project
            {
                disableEditor();
                panelMainMenu.Visible = true;
                saveBoxAction = 0;
            }
            else if (saveBoxAction == 2) //Open Project
            {
                loadProject();
                saveBoxAction = 0;
            }
            else if (saveBoxAction == 3) //Reset Page
            {
                //Reset all the text boxes and go to the editor page.
                toggleLayout(1);
                changeThemeColor(0);
                arrayWebsite[currentPage, PAGE_LAYOUT] = "1";
                arrayWebsite[currentPage, PAGE_COLOR] = "0";
                arrayWebsite[currentPage, TITLE_FONT] = "0";
                arrayWebsite[currentPage, TITLE_TEXT_COLOR] = "#FFFFFF";
                arrayWebsite[currentPage, TITLE_TEXT] = "Your Title";
                arrayWebsite[currentPage, MENU_FONT] = "0";
                arrayWebsite[currentPage, MENU_TEXT_COLOR] = "#000000";
                arrayWebsite[currentPage, MENU_TEXT_COLOR_HOVER] = "#000000";
                arrayWebsite[currentPage, CONTENT_FONT] = "0";
                arrayWebsite[currentPage, CONTENT_TEXT_COLOR] = "#000000";
                arrayWebsite[currentPage, CONTENT_1_TEXT] = "Welcome to the Bach Web Builder! (Cooler Name Pending?)";
                arrayWebsite[currentPage, CONTENT_2_TEXT] = "";
                arrayWebsite[currentPage, IMAGE_LOCATION] = "";
                arrayWebsite[currentPage, IMAGE_ALT] = "";
                arrayWebsite[currentPage, FOOTER_FONT] = "0";
                arrayWebsite[currentPage, FOOTER_TEXT] = "Footer text goes in here";
                arrayWebsite[currentPage, FOOTER_TEXT_COLOR] = "#FFFFFF";

                updateProgram();
                saveBoxAction = 0;
            }
            else if (saveBoxAction == 4)
            {
                //Close the program
                this.Close();
            }

            //Close the panel
            panelCloseWithoutSaving.Visible = false;
        }

        private void buttonCloseCancel_Click(object sender, EventArgs e)
        {
            //Close the panel and set the save box action to 0
            panelCloseWithoutSaving.Visible = false;
            saveBoxAction = 0;
        }



        /*//////////////////////////////////Pages Panel//////////////////////////////////////////*/
        private void labelPage1Name_Click(object sender, EventArgs e)
        {
            //Set the page to 0 - Index
            currentPage = 0;
            updateProgram();
        }

        private void labelPage2Name_Click(object sender, EventArgs e)
        {
            //Set the page to 1
            currentPage = 1;
            updateProgram();
        }

        private void labelPage3Name_Click(object sender, EventArgs e)
        {
            //Set the page to 2
            currentPage = 2;
            updateProgram();
        }

        private void labelPage4Name_Click(object sender, EventArgs e)
        {
            //Set the page to 3
            currentPage = 3;
            updateProgram();
        }

        private void labelPage5Name_Click(object sender, EventArgs e)
        {
            //Set the page to 4
            currentPage = 4;
            updateProgram();
        }

        private void buttonAddPage2_Click(object sender, EventArgs e)
        {
            //Start with layout 1 selected
            createNewPageLayout = 1;
            buttonCreatePageLayout1.Image = ((System.Drawing.Image)(Properties.Resources.layoutIconSingle_selected));
            buttonCreatePageLayout2.Image = ((System.Drawing.Image)(Properties.Resources.layoutIconMultiple));

            //Open the New Page panel in the rename panel isn't open
            if (!renamePanelOpen && !colorPickerPanelOpen)
            {
                newPage = 1;
                textBoxCreatePageName.Focus();
                createNewPagePanelOpen = true;
                panelCreateNewPage.Visible = true;
            }
        }

        private void buttonAddPage3_Click(object sender, EventArgs e)
        {
            //Start with layout 1 selected
            createNewPageLayout = 1;
            buttonCreatePageLayout1.Image = ((System.Drawing.Image)(Properties.Resources.layoutIconSingle_selected));
            buttonCreatePageLayout2.Image = ((System.Drawing.Image)(Properties.Resources.layoutIconMultiple));

            //Open the New Page panel in the rename panel isn't open
            if (!renamePanelOpen && !colorPickerPanelOpen)
            {
                newPage = 2;
                textBoxCreatePageName.Focus();
                createNewPagePanelOpen = true;
                panelCreateNewPage.Visible = true;
            }
        }

        private void buttonAddPage4_Click(object sender, EventArgs e)
        {
            //Start with layout 1 selected
            createNewPageLayout = 1;
            buttonCreatePageLayout1.Image = ((System.Drawing.Image)(Properties.Resources.layoutIconSingle_selected));
            buttonCreatePageLayout2.Image = ((System.Drawing.Image)(Properties.Resources.layoutIconMultiple));

            //Open the New Page panel in the rename panel isn't open
            if (!renamePanelOpen && !colorPickerPanelOpen)
            {
                newPage = 3;
                textBoxCreatePageName.Focus();
                createNewPagePanelOpen = true;
                panelCreateNewPage.Visible = true;
            }
        }

        private void buttonAddPage5_Click(object sender, EventArgs e)
        {
            //Start with layout 1 selected
            createNewPageLayout = 1;
            buttonCreatePageLayout1.Image = ((System.Drawing.Image)(Properties.Resources.layoutIconSingle_selected));
            buttonCreatePageLayout2.Image = ((System.Drawing.Image)(Properties.Resources.layoutIconMultiple));

            //Open the New Page panel in the rename panel isn't open
            if (!renamePanelOpen && !colorPickerPanelOpen)
            {
                newPage = 4;
                textBoxCreatePageName.Focus();
                createNewPagePanelOpen = true;
                panelCreateNewPage.Visible = true;
            }
        }

        private void buttonRenamePage2_Click(object sender, EventArgs e)
        {
            //Open the Rename Page panel if other panels arent open
            if (!createNewPagePanelOpen && !colorPickerPanelOpen)
            {
                pageToRename = 1;
                renamePanelOpen = true;
                textBoxRenamePage.Text = "";
                labelRenamePageTitle.Text = "Rename Page: " + arrayWebsite[pageToRename, PAGE_NAME];
                panelRenamePage.Location = new Point(500, 320);
                panelRenamePage.Visible = true;
            }
        }

        private void buttonDeletePage2_Click(object sender, EventArgs e)
        {
            //Delete page if panels arent open
            if (!renamePanelOpen && !createNewPagePanelOpen)
            {
                deletePage(1);
            }
        }

        private void buttonRenamePage3_Click(object sender, EventArgs e)
        {
            //Open the Rename Page panel if other panels arent open
            if (!createNewPagePanelOpen && !colorPickerPanelOpen)
            {
                pageToRename = 2;
                renamePanelOpen = true;
                textBoxRenamePage.Text = "";
                labelRenamePageTitle.Text = "Rename Page: " + arrayWebsite[pageToRename, PAGE_NAME];
                panelRenamePage.Location = new Point(500, 380);
                panelRenamePage.Visible = true;
            }
        }

        private void buttonDeletePage3_Click(object sender, EventArgs e)
        {
            //Delete page if panels arent open
            if (!renamePanelOpen && !createNewPagePanelOpen)
            {
                deletePage(2);
            }
        }

        private void buttonRenamePage4_Click(object sender, EventArgs e)
        {
            //Open the Rename Page panel if other panels arent open
            if (!createNewPagePanelOpen && !colorPickerPanelOpen)
            {
                pageToRename = 3;
                renamePanelOpen = true;
                textBoxRenamePage.Text = "";
                labelRenamePageTitle.Text = "Rename Page: " + arrayWebsite[pageToRename, PAGE_NAME];
                panelRenamePage.Location = new Point(500, 435);
                panelRenamePage.Visible = true;
            }
        }

        private void buttonDeletePage4_Click(object sender, EventArgs e)
        {
            //Delete page if panels arent open
            if (!renamePanelOpen && !createNewPagePanelOpen)
            {
                deletePage(3);
            }
        }

        private void buttonRenamePage5_Click(object sender, EventArgs e)
        {
            //Open the Rename Page panel if other panels arent open
            if (!createNewPagePanelOpen && !colorPickerPanelOpen)
            {
                pageToRename = 4;
                renamePanelOpen = true;
                textBoxRenamePage.Text = "";
                labelRenamePageTitle.Text = "Rename Page: " + arrayWebsite[pageToRename, PAGE_NAME];
                panelRenamePage.Location = new Point(500, 495);
                panelRenamePage.Visible = true;
            }
        }

        private void buttonDeletePage5_Click(object sender, EventArgs e)
        {
            //Delete page if panels arent open
            if (!renamePanelOpen && !createNewPagePanelOpen)
            {
                deletePage(4);
            }
        }

        private void buttonRenamePage2_MouseEnter(object sender, EventArgs e)
        {
            buttonRenamePage2.ForeColor = Color.DodgerBlue;
        }

        private void buttonRenamePage2_MouseLeave(object sender, EventArgs e)
        {
            buttonRenamePage2.ForeColor = Color.FromArgb(208, 208, 208);
        }

        private void buttonRenamePage3_MouseEnter(object sender, EventArgs e)
        {
            buttonRenamePage3.ForeColor = Color.DodgerBlue;
        }

        private void buttonRenamePage3_MouseLeave(object sender, EventArgs e)
        {
            buttonRenamePage3.ForeColor = Color.FromArgb(208, 208, 208);
        }

        private void buttonRenamePage4_MouseEnter(object sender, EventArgs e)
        {
            buttonRenamePage4.ForeColor = Color.DodgerBlue;
        }

        private void buttonRenamePage4_MouseLeave(object sender, EventArgs e)
        {
            buttonRenamePage4.ForeColor = Color.FromArgb(208, 208, 208);
        }

        private void buttonRenamePage5_MouseEnter(object sender, EventArgs e)
        {
            buttonRenamePage5.ForeColor = Color.DodgerBlue;
        }

        private void buttonRenamePage5_MouseLeave(object sender, EventArgs e)
        {
            buttonRenamePage5.ForeColor = Color.FromArgb(208, 208, 208);
        }

        private void buttonAddPage2_MouseEnter(object sender, EventArgs e)
        {
            //On hover image
            this.buttonAddPage2.Image = ((System.Drawing.Image)(Properties.Resources.pagesAddPage_btn_onHover));
        }

        private void buttonAddPage2_MouseLeave(object sender, EventArgs e)
        {
            //Static image when not hovering
            this.buttonAddPage2.Image = ((System.Drawing.Image)(Properties.Resources.pagesAddPage_btn_static));
        }

        private void buttonAddPage3_MouseEnter(object sender, EventArgs e)
        {
            //On hover image
            this.buttonAddPage3.Image = ((System.Drawing.Image)(Properties.Resources.pagesAddPage_btn_onHover));
        }

        private void buttonAddPage3_MouseLeave(object sender, EventArgs e)
        {
            //Static image when not hovering
            this.buttonAddPage3.Image = ((System.Drawing.Image)(Properties.Resources.pagesAddPage_btn_static));
        }

        private void buttonAddPage4_MouseEnter(object sender, EventArgs e)
        {
            //On hover image
            this.buttonAddPage4.Image = ((System.Drawing.Image)(Properties.Resources.pagesAddPage_btn_onHover));
        }

        private void buttonAddPage4_MouseLeave(object sender, EventArgs e)
        {
            //Static image when not hovering
            this.buttonAddPage4.Image = ((System.Drawing.Image)(Properties.Resources.pagesAddPage_btn_static));
        }

        private void buttonAddPage5_MouseEnter(object sender, EventArgs e)
        {
            //On hover image
            this.buttonAddPage5.Image = ((System.Drawing.Image)(Properties.Resources.pagesAddPage_btn_onHover));
        }

        private void buttonAddPage5_MouseLeave(object sender, EventArgs e)
        {
            //Static image when not hovering
            this.buttonAddPage5.Image = ((System.Drawing.Image)(Properties.Resources.pagesAddPage_btn_static));
        }

        private void buttonDeletePage2_MouseEnter(object sender, EventArgs e)
        {
            buttonDeletePage2.ForeColor = Color.Red;
        }

        private void buttonDeletePage2_MouseLeave(object sender, EventArgs e)
        {
            buttonDeletePage2.ForeColor = Color.FromArgb(208, 208, 208);
        }

        private void buttonDeletePage3_MouseEnter(object sender, EventArgs e)
        {
            buttonDeletePage3.ForeColor = Color.Red;
        }

        private void buttonDeletePage3_MouseLeave(object sender, EventArgs e)
        {
            buttonDeletePage3.ForeColor = Color.FromArgb(208, 208, 208);
        }

        private void buttonDeletePage4_MouseEnter(object sender, EventArgs e)
        {
            buttonDeletePage4.ForeColor = Color.Red;
        }

        private void buttonDeletePage4_MouseLeave(object sender, EventArgs e)
        {
            buttonDeletePage4.ForeColor = Color.FromArgb(208, 208, 208);
        }

        private void buttonDeletePage5_MouseEnter(object sender, EventArgs e)
        {
            buttonDeletePage5.ForeColor = Color.Red;
        }

        private void buttonDeletePage5_MouseLeave(object sender, EventArgs e)
        {
            buttonDeletePage5.ForeColor = Color.FromArgb(208, 208, 208);
        }





        /*//////////////////////////////////Create New Page Panel//////////////////////////////////////////*/
        //This panel is responsible for creating a new page for the website
        private void buttonCreatePageCreate_Click(object sender, EventArgs e)
        {
            //Declare variable to check for a naming error
            bool nameError = false;

            //Check each array page name element
            for (int i = 0; i < arrayWebsite.GetLength(0); i++)
            {
                if (textBoxCreatePageName.Text == arrayWebsite[i, PAGE_NAME])
                {
                    showMessageBox("That Page already Exists!");
                    nameError = true;
                }
                else
                {
                    nameError = false;
                }
            }

            //If no error, Create a new page, checking for length
            if (!nameError)
            {
                if (textBoxCreatePageName.TextLength > 0)
                {
                    createNewPage(newPage);
                    createNewPagePanelOpen = false;
                    labelCreatePageError.Visible = false;
                }
                else
                {
                    //If name length too short, show the error
                    labelCreatePageError.Visible = true;
                }
            }
        }

        private void buttonCreatePageLayout1_Click(object sender, EventArgs e)
        {
            //Change to layout 1
            createNewPageLayout = 1;
            buttonCreatePageLayout1.Image = ((System.Drawing.Image)(Properties.Resources.layoutIconSingle_selected));
            buttonCreatePageLayout2.Image = ((System.Drawing.Image)(Properties.Resources.layoutIconMultiple));
        }

        private void buttonCreatePageLayout2_Click(object sender, EventArgs e)
        {
            //Change to layout 2
            createNewPageLayout = 2;
            buttonCreatePageLayout1.Image = ((System.Drawing.Image)(Properties.Resources.layoutIconSingle));
            buttonCreatePageLayout2.Image = ((System.Drawing.Image)(Properties.Resources.layoutIconMultiple_selected));
        }

        private void buttonCreatePageCancel_Click(object sender, EventArgs e)
        {
            //Close the create page panel
            panelCreateNewPage.Visible = false;
            createNewPagePanelOpen = false;
            labelCreatePageError.Visible = false;
        }


        private void buttonRenameCancel_Click(object sender, EventArgs e)
        {
            //Close the rename panel
            panelRenamePage.Visible = false;
            renamePanelOpen = false;
        }

        private void buttonRenameConfirm_Click(object sender, EventArgs e)
        {
            //If text length isn't less than 0, rename the page
            if (textBoxRenamePage.TextLength > 0)
            {
                arrayWebsite[pageToRename, PAGE_NAME] = (textBoxRenamePage.Text).Trim(' ');
                panelRenamePage.Visible = false;
                renamePanelOpen = false;
                updateProgram();
                updateMenus();
            }
            else
            {
                //Show message box if page name is too short.
                showMessageBox("The page name is too short.");
            }
        }





        /*//////////////////////////////////Editor Text Changed Events///////////////////////////////////////////*/
        //These EventHandlers Ensure the array gets updated whenever the text is changed in the textbox.
        private void textBoxTitle_TextChanged(object sender, EventArgs e)
        {
            //Set the recently save variable to false.
            recentlySaved = false;

            //When the text changes, update the website array.
            arrayWebsite[currentPage, TITLE_TEXT] = textBoxTitle.Text;
        }

        private void richTextBoxContent1_TextChanged(object sender, EventArgs e)
        {
            //Set the recently save variable to false.
            recentlySaved = false;

            //This prevents new lines being stored in the file when it gets written.
            var str = richTextBoxContent1.Text.Replace('\n', '¶');
            //When the text changes, update the website array.
            arrayWebsite[currentPage, CONTENT_1_TEXT] = str;
        }

        private void richTextBoxContent2_TextChanged(object sender, EventArgs e)
        {
            //Set the recently save variable to false.
            recentlySaved = false;

            //This prevents new lines being stored in the file when it gets written.
            var str = richTextBoxContent2.Text.Replace('\n', '¶');
            //When the text changes, update the website array.
            arrayWebsite[currentPage, CONTENT_2_TEXT] = str;
        }

        private void textBoxFooter_TextChanged(object sender, EventArgs e)
        {
            //Set the recently save variable to false.
            recentlySaved = false;

            //Footer will be the same on every page. Loop through the website array to set footer text on all pages.
            for (int i = 0; i < MAX_PAGES; i++)
            {
                arrayWebsite[i, FOOTER_TEXT] = textBoxFooter.Text;
            }
        }





        /*//////////////////////Main Menu Panel/////////////////////////*/
        private void panelMainMenu_VisibleChanged(object sender, EventArgs e)
        {
            //When made visible this will update the recent files.
            recentFiles();
        }

        private void buttonExitMainMenu_Click(object sender, EventArgs e)
        {
            //Close the program
            this.Close();
        }

        private void buttonExitMainMenu_MouseEnter(object sender, EventArgs e)
        {
            buttonExitMainMenu.Image = ((System.Drawing.Image)(Properties.Resources.buttonMenu_exit_onHover));
        }

        private void buttonExitMainMenu_MouseLeave(object sender, EventArgs e)
        {
            buttonExitMainMenu.Image = ((System.Drawing.Image)(Properties.Resources.buttonMenu_exit_static));
        }

        private void buttonLoadProject_Click(object sender, EventArgs e)
        {
            //Method used to load the project using the openFile Dialog
            loadProject();
        }

        private void buttonLoadProject_MouseEnter(object sender, EventArgs e)
        {
            buttonLoadProject.Image = ((System.Drawing.Image)(Properties.Resources.buttonMenu_loadProject_onHover));
        }

        private void buttonLoadProject_MouseLeave(object sender, EventArgs e)
        {
            buttonLoadProject.Image = ((System.Drawing.Image)(Properties.Resources.buttonMenu_loadProject_static));
        }

        private void buttonNewProject_Click(object sender, EventArgs e)
        {
            //Accessing from main menu. Determines back buttons behavior.
            fromMainMenu = true;

            //Create a new project
            newProject();
        }

        private void buttonNewProject_MouseEnter(object sender, EventArgs e)
        {
            buttonNewProject.Image = ((System.Drawing.Image)(Properties.Resources.buttonMenu_newProject_onHover));
        }

        private void buttonNewProject_MouseLeave(object sender, EventArgs e)
        {
            buttonNewProject.Image = ((System.Drawing.Image)(Properties.Resources.buttonMenu_newProject_static));
        }

        private void labelProjectName0_Click(object sender, EventArgs e)
        {
            //Load method used by passing file path
            if (File.Exists(arraySettings[RECENT_FILE_PATH_0]))
            {
                loadRecentProject(arraySettings[RECENT_FILE_PATH_0]);
            }
            else
            {
                //Slot 1
                arraySettings[RECENT_FILE_NAME_0] = arraySettings[RECENT_FILE_NAME_1];
                arraySettings[RECENT_FILE_PATH_0] = arraySettings[RECENT_FILE_PATH_1];
                //Slot 2
                arraySettings[RECENT_FILE_NAME_1] = arraySettings[RECENT_FILE_NAME_2];
                arraySettings[RECENT_FILE_PATH_1] = arraySettings[RECENT_FILE_PATH_2];
                //Slot 3
                arraySettings[RECENT_FILE_NAME_2] = arraySettings[RECENT_FILE_NAME_3];
                arraySettings[RECENT_FILE_PATH_2] = arraySettings[RECENT_FILE_PATH_3];
                //Slot 4
                arraySettings[RECENT_FILE_NAME_3] = arraySettings[RECENT_FILE_NAME_4];
                arraySettings[RECENT_FILE_PATH_3] = arraySettings[RECENT_FILE_PATH_4];
                //Slot 5
                arraySettings[RECENT_FILE_NAME_4] = "";
                arraySettings[RECENT_FILE_PATH_4] = "";

                //Show the messagebox if file not found
                showMessageBox("File Not Found");

                saveSettings();
                recentFiles();
            }

        }

        private void labelProjectName0_MouseEnter(object sender, EventArgs e)
        {
            //Set backcolor to on Hover
            labelProjectName0.BackColor = Color.FromArgb(54, 58, 63);
            labelProjectFilePath0.BackColor = Color.FromArgb(54, 58, 63);
        }

        private void labelProjectName0_MouseLeave(object sender, EventArgs e)
        {
            //Set backcolor to default
            labelProjectName0.BackColor = Color.FromArgb(44, 48, 53);
            labelProjectFilePath0.BackColor = Color.FromArgb(44, 48, 53);
        }

        private void labelProjectName1_Click(object sender, EventArgs e)
        {
            //Load method used by passing file path
            if (File.Exists(arraySettings[RECENT_FILE_PATH_1]))
            {
                loadRecentProject(arraySettings[RECENT_FILE_PATH_1]);
            }
            else
            {
                //Slot 2
                arraySettings[RECENT_FILE_NAME_1] = arraySettings[RECENT_FILE_NAME_2];
                arraySettings[RECENT_FILE_PATH_1] = arraySettings[RECENT_FILE_PATH_2];
                //Slot 3
                arraySettings[RECENT_FILE_NAME_2] = arraySettings[RECENT_FILE_NAME_3];
                arraySettings[RECENT_FILE_PATH_2] = arraySettings[RECENT_FILE_PATH_3];
                //Slot 4
                arraySettings[RECENT_FILE_NAME_3] = arraySettings[RECENT_FILE_NAME_4];
                arraySettings[RECENT_FILE_PATH_3] = arraySettings[RECENT_FILE_PATH_4];
                //Slot 5
                arraySettings[RECENT_FILE_NAME_4] = "";
                arraySettings[RECENT_FILE_PATH_4] = "";

                //Show the messagebox if file not found
                showMessageBox("File Not Found");

                saveSettings();
                recentFiles();
            }
        }

        private void labelProjectName1_MouseEnter(object sender, EventArgs e)
        {
            //Set backcolor to on Hover
            labelProjectName1.BackColor = Color.FromArgb(54, 58, 63);
            labelProjectFilePath1.BackColor = Color.FromArgb(54, 58, 63);
        }

        private void labelProjectName1_MouseLeave(object sender, EventArgs e)
        {
            //Set backcolor to default
            labelProjectName1.BackColor = Color.FromArgb(44, 48, 53);
            labelProjectFilePath1.BackColor = Color.FromArgb(44, 48, 53);
        }

        private void labelProjectName2_Click(object sender, EventArgs e)
        {
            //Load method used by passing file path
            if (File.Exists(arraySettings[RECENT_FILE_PATH_2]))
            {
                loadRecentProject(arraySettings[RECENT_FILE_PATH_2]);
            }
            else
            {
                //Slot 3
                arraySettings[RECENT_FILE_NAME_2] = arraySettings[RECENT_FILE_NAME_3];
                arraySettings[RECENT_FILE_PATH_2] = arraySettings[RECENT_FILE_PATH_3];
                //Slot 4
                arraySettings[RECENT_FILE_NAME_3] = arraySettings[RECENT_FILE_NAME_4];
                arraySettings[RECENT_FILE_PATH_3] = arraySettings[RECENT_FILE_PATH_4];
                //Slot 5
                arraySettings[RECENT_FILE_NAME_4] = "";
                arraySettings[RECENT_FILE_PATH_4] = "";

                //Show the messagebox if file not found
                showMessageBox("File Not Found");

                saveSettings();
                recentFiles();
            }
        }

        private void labelProjectName2_MouseEnter(object sender, EventArgs e)
        {
            //Set backcolor to on Hover
            labelProjectName2.BackColor = Color.FromArgb(54, 58, 63);
            labelProjectFilePath2.BackColor = Color.FromArgb(54, 58, 63);
        }

        private void labelProjectName2_MouseLeave(object sender, EventArgs e)
        {
            //Set backcolor to default
            labelProjectName2.BackColor = Color.FromArgb(44, 48, 53);
            labelProjectFilePath2.BackColor = Color.FromArgb(44, 48, 53);
        }

        private void labelProjectName3_Click(object sender, EventArgs e)
        {
            //Load method used by passing file path
            if (File.Exists(arraySettings[RECENT_FILE_PATH_3]))
            {
                loadRecentProject(arraySettings[RECENT_FILE_PATH_3]);
            }
            else
            {
                //Slot 4
                arraySettings[RECENT_FILE_NAME_3] = arraySettings[RECENT_FILE_NAME_4];
                arraySettings[RECENT_FILE_PATH_3] = arraySettings[RECENT_FILE_PATH_4];
                //Slot 5
                arraySettings[RECENT_FILE_NAME_4] = "";
                arraySettings[RECENT_FILE_PATH_4] = "";

                //Show the messagebox if file not found
                showMessageBox("File Not Found");

                saveSettings();
                recentFiles();
            }
        }

        private void labelProjectName3_MouseEnter(object sender, EventArgs e)
        {
            //Set backcolor to on Hover
            labelProjectName3.BackColor = Color.FromArgb(54, 58, 63);
            labelProjectFilePath3.BackColor = Color.FromArgb(54, 58, 63);
        }

        private void labelProjectName3_MouseLeave(object sender, EventArgs e)
        {
            //Set backcolor to default
            labelProjectName3.BackColor = Color.FromArgb(44, 48, 53);
            labelProjectFilePath3.BackColor = Color.FromArgb(44, 48, 53);
        }

        private void labelProjectName4_Click(object sender, EventArgs e)
        {
            //Load method used by passing file path
            if (File.Exists(arraySettings[RECENT_FILE_PATH_4]))
            {
                loadRecentProject(arraySettings[RECENT_FILE_PATH_4]);
            }
            else
            {
                //Slot 5
                arraySettings[RECENT_FILE_NAME_4] = "";
                arraySettings[RECENT_FILE_PATH_4] = "";

                //Show the messagebox if file not found
                showMessageBox("File Not Found");

                saveSettings();
                recentFiles();
            }
        }

        private void labelProjectName4_MouseEnter(object sender, EventArgs e)
        {
            //Set backcolor to on Hover
            labelProjectName4.BackColor = Color.FromArgb(54, 58, 63);
            labelProjectFilePath4.BackColor = Color.FromArgb(54, 58, 63);
        }

        private void labelProjectName4_MouseLeave(object sender, EventArgs e)
        {
            //Set backcolor to default
            labelProjectName4.BackColor = Color.FromArgb(44, 48, 53);
            labelProjectFilePath4.BackColor = Color.FromArgb(44, 48, 53);
        }


        /*//////////////////First Use Panel////////////////////////*/
        private void buttonSetName_Click(object sender, EventArgs e)
        {
            //Set username if textbox text is greater than 0
            if (textBoxChooseName.TextLength > 0)
            {
                arraySettings[USER_NAME] = (textBoxChooseName.Text).Trim(' ');
                panelFirstUse.Visible = false;

                panelMainMenu.Enabled = true;

                saveSettings();
            }
            else
            {
                labelFirstUseError.Visible = true;
            }
        }


        /*////////////////////////////New Project Panel//////////////////////////////////////*/
        private void buttonNewProjectLocationBrowse_Click(object sender, EventArgs e)
        {
            //Choose file location
            if (folderBrowserDialogNewProject.ShowDialog() == DialogResult.OK)
            {
                textBoxNewProjectLocation.Text = folderBrowserDialogNewProject.SelectedPath;
            }
        }

        private void pictureBoxThemeSingleBars_Click(object sender, EventArgs e)
        {
            //Select the single bars theme
            themeSelected = 1;
            buttonNewProjectThemeSingleBars.Image = ((System.Drawing.Image)(Properties.Resources.singleBars_ThemeBtn_selected));

            //Deselect future themes (When they get added eventually)
        }

        private void buttonNewProjectBack_Click(object sender, EventArgs e)
        {
            if (fromMainMenu)
            {
                //Close the new project panel
                panelNewProject.Visible = false;
            }
            else
            {
                //Close the new project panel
                panelMainMenu.Visible = false;
                panelNewProject.Visible = false;
            }
        }

        private void buttonCreateNewProject_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(textBoxNewProjectLocation.Text))
            {
                if (textBoxNewProjectName.TextLength > 0)
                {
                    //Hide the error message
                    labelNewProjectLengthError.Visible = false;

                    if (themeSelected != 0)
                    {
                        //Hide the error message
                        labelNewProjectThemeError.Visible = false;

                        //Setup New Project
                        arrayProject[PROJECT_NAME] = textBoxNewProjectName.Text;
                        arrayProject[PROJECT_LOCATION] = Path.Combine(textBoxNewProjectLocation.Text, arrayProject[PROJECT_NAME] + ".wdproject");
                        arrayProject[PROJECT_THEME] = "singleBars";
                        arrayProject[PROJECT_TOTAL_PAGES] = "1";
                        arrayProject[PROJECT_AUTHOR] = arraySettings[USER_NAME];

                        for (int i = 0; i < MAX_PAGES; i++)
                        {
                            //Default Page settings to prevent errors from null values
                            arrayWebsite[i, PAGE_NAME] = "Index";
                            arrayWebsite[i, PAGE_LAYOUT] = "singleLayout";
                            arrayWebsite[i, PAGE_COLOR] = "0";
                            arrayWebsite[i, TITLE_FONT] = "0";
                            arrayWebsite[i, TITLE_TEXT_COLOR] = "#FFFFFF";
                            arrayWebsite[i, TITLE_TEXT] = "Your Title";
                            arrayWebsite[i, MENU_FONT] = "0";
                            arrayWebsite[i, MENU_TEXT_COLOR] = "#000000";
                            arrayWebsite[i, MENU_TEXT_COLOR_HOVER] = "#000000";
                            arrayWebsite[i, CONTENT_FONT] = "0";
                            arrayWebsite[i, CONTENT_TEXT_COLOR] = "#000000";
                            arrayWebsite[i, CONTENT_1_TEXT] = "Welcome to the Bach Web Builder (Cooler Name Pending?)";
                            arrayWebsite[i, CONTENT_2_TEXT] = "";
                            arrayWebsite[i, IMAGE_LOCATION] = "";
                            arrayWebsite[i, IMAGE_ALT] = "";
                            arrayWebsite[i, FOOTER_FONT] = "0";
                            arrayWebsite[i, FOOTER_TEXT] = "Footer text goes in here";
                            arrayWebsite[i, FOOTER_TEXT_COLOR] = "#FFFFFF";
                            arrayWebsite[i, IMAGE_LOCATION_TYPE] = "0";
                        }

                        //Hide main menu and create project panels
                        panelMainMenu.Visible = false;
                        panelNewProject.Visible = false;

                        enableEditor();
                        saveProject();

                        //Add to recent files
                        addRecentFile();
                    }
                    else
                    {
                        //Show error message for theme not being selected.
                        labelNewProjectThemeError.Visible = true;

                    }
                }
                else
                {
                    //Show error message for length not being long enough
                    labelNewProjectLengthError.Visible = true;
                }
            }
            else
            {
                showMessageBox("An invalid project location was entered.");
                textBoxNewProjectLocation.Text = Path.Combine(Environment.CurrentDirectory, @"Projects");
            }
        }

        private void buttonNewProjectBack_MouseEnter(object sender, EventArgs e)
        {
            //onHover Image
            buttonNewProjectBack.Image = ((System.Drawing.Image)(Properties.Resources.buttonNewProject_back_onHover));
        }

        private void buttonNewProjectBack_MouseLeave(object sender, EventArgs e)
        {
            //Static Image
            buttonNewProjectBack.Image = ((System.Drawing.Image)(Properties.Resources.buttonNewProject_back_static));
        }

        private void buttonNewProjectCreate_MouseEnter(object sender, EventArgs e)
        {
            //onHover Image
            buttonNewProjectCreate.Image = ((System.Drawing.Image)(Properties.Resources.buttonNewProject_create_onHover));
        }

        private void buttonNewProjectCreate_MouseLeave(object sender, EventArgs e)
        {
            //Static Image
            buttonNewProjectCreate.Image = ((System.Drawing.Image)(Properties.Resources.buttonNewProject_create_static));
        }

        private void buttonNewProjectThemeSingleBars_MouseEnter(object sender, EventArgs e)
        {
            if (themeSelected != 1)
            {
                buttonNewProjectThemeSingleBars.Image = ((System.Drawing.Image)(Properties.Resources.singleBars_ThemeBtn_selected));
            }
        }

        private void buttonNewProjectThemeSingleBars_MouseLeave(object sender, EventArgs e)
        {
            if (themeSelected != 1)
            {
                buttonNewProjectThemeSingleBars.Image = ((System.Drawing.Image)(Properties.Resources.singleBars_ThemeBtn_notselected));
            }
        }


        /*/////////////////////Message Box Panel///////////////////////////////*/
        private void buttonSaveMessageClose_Click(object sender, EventArgs e)
        {
            //Close the saved message
            panelMessageBox.Visible = false;
        }


        /*//////////////////////////Guide PAnel///////////////////////*/
        private void buttonCloseGuide_Click(object sender, EventArgs e)
        {
            //Hide the guide panel
            panelGuide.Visible = false;
        }

        private void labelHelp1New_Click(object sender, EventArgs e)
        {
            helpDocuments(1);
        }

        private void labelHelp1New_MouseEnter(object sender, EventArgs e)
        {
            labelHelp1New.ForeColor = Color.DodgerBlue;
        }

        private void labelHelp1New_MouseLeave(object sender, EventArgs e)
        {
            labelHelp1New.ForeColor = Color.FromArgb(208, 208, 208);
        }

        private void labelHelp2Content_Click(object sender, EventArgs e)
        {
            helpDocuments(2);
        }

        private void labelHelp2Content_MouseEnter(object sender, EventArgs e)
        {
            labelHelp2Content.ForeColor = Color.DodgerBlue;
        }

        private void labelHelp2Content_MouseLeave(object sender, EventArgs e)
        {
            labelHelp2Content.ForeColor = Color.FromArgb(208, 208, 208);
        }

        private void labelHelp3Theme_Click(object sender, EventArgs e)
        {
            helpDocuments(3);
        }

        private void labelHelp3Theme_MouseEnter(object sender, EventArgs e)
        {
            labelHelp3Theme.ForeColor = Color.DodgerBlue;
        }

        private void labelHelp3Theme_MouseLeave(object sender, EventArgs e)
        {
            labelHelp3Theme.ForeColor = Color.FromArgb(208, 208, 208);
        }

        private void labelHelp4Font_Click(object sender, EventArgs e)
        {
            helpDocuments(4);
        }

        private void labelHelp4Font_MouseEnter(object sender, EventArgs e)
        {
            labelHelp4Font.ForeColor = Color.DodgerBlue;
        }

        private void labelHelp4Font_MouseLeave(object sender, EventArgs e)
        {
            labelHelp4Font.ForeColor = Color.FromArgb(208, 208, 208);
        }

        private void labelHelp5Pages_Click(object sender, EventArgs e)
        {
            helpDocuments(5);
        }

        private void labelHelp5Pages_MouseEnter(object sender, EventArgs e)
        {
            labelHelp5Pages.ForeColor = Color.DodgerBlue;
        }

        private void labelHelp5Pages_MouseLeave(object sender, EventArgs e)
        {
            labelHelp5Pages.ForeColor = Color.FromArgb(208, 208, 208);
        }

        private void labelHelp6NewPage_Click(object sender, EventArgs e)
        {
            helpDocuments(6);
        }

        private void labelHelp6NewPage_MouseEnter(object sender, EventArgs e)
        {
            labelHelp6NewPage.ForeColor = Color.DodgerBlue;
        }

        private void labelHelp6NewPage_MouseLeave(object sender, EventArgs e)
        {
            labelHelp6NewPage.ForeColor = Color.FromArgb(208, 208, 208);
        }

        private void labelHelp7Rename_Click(object sender, EventArgs e)
        {
            helpDocuments(7);
        }

        private void labelHelp7Rename_MouseEnter(object sender, EventArgs e)
        {
            labelHelp7Rename.ForeColor = Color.DodgerBlue;
        }

        private void labelHelp7Rename_MouseLeave(object sender, EventArgs e)
        {
            labelHelp7Rename.ForeColor = Color.FromArgb(208, 208, 208);
        }








        /*/////////////////////////////////MENU STRIP CODE//////////////////////////////////////////*/
        /*/////////////////////////////////File Tool Strip//////////////////////////////////////////*/
        //New Project
        private void newProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Not Accessing from main menu. Determines back buttons behavior.
            fromMainMenu = false;

            //Create a new project
            newProject();
        }


        //Save Project
        private void saveProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Save the project
            saveProject();
        }


        //Save Project As
        private void saveProjectAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Set recently saved to true. This allows the project to be closed without the save prompt opening.
            recentlySaved = true;

            //Use StreamWriter to write the Project Array and website array to a file
            StreamWriter outputFile;

            //Set filter and initial directory
            saveFileProject.Filter = "Web Development Project (*.wdproject)|*.wdproject";
            saveFileProject.InitialDirectory = Path.Combine(Environment.CurrentDirectory, @"Projects");

            if (saveFileProject.ShowDialog() == DialogResult.OK)
            {
                outputFile = File.CreateText(saveFileProject.FileName);

                for (int i = 0; i < arrayProject.GetLength(0); i++)
                {
                    outputFile.WriteLine(arrayProject[i]);
                }

                for (int x = 0; x < arrayWebsite.GetLength(0); x++)
                {
                    for (int y = 0; y < arrayWebsite.GetLength(1); y++)
                    {
                        outputFile.WriteLine(arrayWebsite[x, y]);
                    }
                }

                outputFile.Close();
            }
        }

        //Open Project
        private void openProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (recentlySaved == true)
            {
                loadProject();
            }
            else
            {
                panelCloseWithoutSaving.Visible = true;
                saveBoxAction = 2; //Open File
            }
        }

        //Close Project
        private void closeProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            closeProject();
        }

        //Export Site
        private void exportSiteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            exportSite();
        }

        //Exit
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Exit the Program
            exitProgram();
        }


        /*/////////////////////////////////View Tool Strip///////////////////////////////////////*/
        //Preview
        private void previewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Open the preview Page
            openPreview();
        }

        //Font Settings
        private void fontSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Open/Close the Font Settings
            toggleFontSettings();
        }

        //Pages
        private void pagesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Open/Close the Page Settings
            togglePageSettings();
        }

        //Theme Settings
        private void themeSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Open/Close the Theme Settings
            toggleThemeSettings();
        }


        /*/////////////////////////////////Project Tool Strip///////////////////////////////////////*/
        //Reset Page
        private void resetPageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (recentlySaved)
            {
                //Reset all the text boxes and go to the editor page.
                toggleLayout(1);
                changeThemeColor(0);
                arrayWebsite[currentPage, PAGE_LAYOUT] = "1";
                arrayWebsite[currentPage, PAGE_COLOR] = "0";
                arrayWebsite[currentPage, TITLE_FONT] = "0";
                arrayWebsite[currentPage, TITLE_TEXT_COLOR] = "#FFFFFF";
                arrayWebsite[currentPage, TITLE_TEXT] = "Your Title";
                arrayWebsite[currentPage, MENU_FONT] = "0";
                arrayWebsite[currentPage, MENU_TEXT_COLOR] = "#000000";
                arrayWebsite[currentPage, MENU_TEXT_COLOR_HOVER] = "#000000";
                arrayWebsite[currentPage, CONTENT_FONT] = "0";
                arrayWebsite[currentPage, CONTENT_TEXT_COLOR] = "#000000";
                arrayWebsite[currentPage, CONTENT_1_TEXT] = "Welcome to the Bach Web Builder! (Cooler Name Pending?)";
                arrayWebsite[currentPage, CONTENT_2_TEXT] = "";
                arrayWebsite[currentPage, IMAGE_LOCATION] = "";
                arrayWebsite[currentPage, IMAGE_ALT] = "";
                arrayWebsite[currentPage, FOOTER_FONT] = "0";
                arrayWebsite[currentPage, FOOTER_TEXT] = "Footer text goes in here";
                arrayWebsite[currentPage, FOOTER_TEXT_COLOR] = "#FFFFFF";

                updateProgram();
            }
            else
            {
                //Set save box action to 1 and open the "Do you want to save" panel
                saveBoxAction = 3;
                panelCloseWithoutSaving.Visible = true;
            }
        }


        /*///////////////////////////////////Help Tool Strip//////////////////////////////////////*/
        //About
        private void viewHelpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Set the guide to the first entry and make it visible
            helpDocuments(1);
            panelGuide.Visible = true;
        }

        private void aboutBachWebDeveloperToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Show the about information
            MessageBox.Show("Bach Web Builder 2020" + Environment.NewLine + "Version " + VERSION + Environment.NewLine + "Last Update: 12/10/20"
                            + Environment.NewLine + Environment.NewLine + "Programmed by Andrew Bach");
        }
    }
}
