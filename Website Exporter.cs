/*The WebsiteBuilder_singleBars class is used to build the single bars theme. This change allows me to make individual classes for each theme I
 * add in the future. This class would have been unmanagable had I done multiple themes on a single class.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.ComponentModel;
using System.Data;
using System.Drawing;

namespace Website_Developer
{
    class WebsiteBuilder_singleBars
    {
        //Declare variables which will store the them code and website code
        string[] arrayThemeCode = new string[1000];
        string[] arrayWebsite = new string[1000];

        //Export location
        string pathExportLocation = "";

        //Constants to target Project array elements by name
        const int PROJECT_NAME = 0;
        const int PROJECT_LOCATION = 1;
        const int PROJECT_TOTAL_PAGES = 2;
        const int PROJECT_THEME = 3;
        const int PROJECT_AUTHOR = 4;

        //Constants to target array elements by name
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

        //Method used to read the theme file, edit it with new code, and export it for the user
        public bool buildWebsite(string[,] websiteData, string[] projectData, string exportLocation)
        {
            try 
            {
                //Build the directory and web pages
                pathExportLocation = Path.Combine(exportLocation, projectData[PROJECT_NAME]);
                string pathImageFolderLocation = pathExportLocation + @"\Images\";

                //Create new directories for the project to export to
                if (Directory.Exists(pathExportLocation))
                {
                    Directory.Delete(pathExportLocation, true);
                }

                Directory.CreateDirectory(pathExportLocation);
                Directory.CreateDirectory(pathImageFolderLocation);

                //Read the theme file
                StreamReader inputFile;


            //Location of theme file.
            inputFile = File.OpenText(@"Website Themes\singleBar\singleBars.html");

            for (int i = 0; !inputFile.EndOfStream; i++)
            {
                arrayThemeCode[i] = inputFile.ReadLine();
            }

            inputFile.Close();

                //For each page:
                for (int page = 0; page < int.Parse(projectData[PROJECT_TOTAL_PAGES]); page++)
                {
                    //x represents the line currently being used in the theme code array.
                    int x = 0;

                    //Clear the array for the each page
                    for (int i = 0; i < arrayWebsite.GetLength(0); i++)
                    {
                        arrayWebsite[i] = "";
                    }

                    //Read web data and add code to website array
                    for (int i = 0; i < arrayThemeCode.GetLength(0); i++, x++)
                    {
                        //Menu Text Color
                        if (arrayThemeCode[x] == "/*CSS-Menu-Text-Color*/")
                        {
                            arrayWebsite[i] = "            color: " + websiteData[page, MENU_TEXT_COLOR] + ";";
                        }
                        //Menu Font
                        else if (arrayThemeCode[x] == "/*CSS-Menu-Font*/")
                        {
                            switch (int.Parse(websiteData[page, MENU_FONT]))
                            {
                                case 0:
                                    arrayWebsite[i] = "            font-family: Arial, Helvetica, sans-serif;";
                                    break;
                                case 1:
                                    arrayWebsite[i] = "            font-family: 'Arial Black', sans-serif;";
                                    break;
                                case 2:
                                    arrayWebsite[i] = "            font-family:  'Comic Sans MS', cursive;";
                                    break;
                                case 3:
                                    arrayWebsite[i] = "            font-family: Impact, sans-serif;";
                                    break;
                                case 4:
                                    arrayWebsite[i] = "            font-family: Courier, monospace;";
                                    break;
                                case 5:
                                    arrayWebsite[i] = "            font-family: 'Times New Roman', Times, serif; ";
                                    break;
                                case 6:
                                    arrayWebsite[i] = "            font-family: Verdana, sans-serif;";
                                    break;
                            }
                        }
                        //Menu Text Color on Hover
                        else if (arrayThemeCode[x] == "/*CSS-Menu-Text-Color-Hover*/")
                        {
                            arrayWebsite[i] = "            color: " + websiteData[page, MENU_TEXT_COLOR_HOVER] + ";";
                        }
                        //Footer Text Color
                        else if (arrayThemeCode[x] == "/*CSS-footer-font-color*/")
                        {
                            arrayWebsite[i] = "            color: " + websiteData[page, FOOTER_TEXT_COLOR] + ";";
                        }
                        //Footer Font
                        else if (arrayThemeCode[x] == "/*CSS-footer-font*/")
                        {
                            switch (int.Parse(websiteData[page, FOOTER_FONT]))
                            {
                                case 0:
                                    arrayWebsite[i] = "            font-family: Arial, Helvetica, sans-serif;";
                                    break;
                                case 1:
                                    arrayWebsite[i] = "            font-family: 'Arial Black', sans-serif;";
                                    break;
                                case 2:
                                    arrayWebsite[i] = "            font-family:  'Comic Sans MS', cursive;";
                                    break;
                                case 3:
                                    arrayWebsite[i] = "            font-family: Impact, sans-serif;";
                                    break;
                                case 4:
                                    arrayWebsite[i] = "            font-family: Courier, monospace;";
                                    break;
                                case 5:
                                    arrayWebsite[i] = "            font-family: 'Times New Roman', Times, serif; ";
                                    break;
                                case 6:
                                    arrayWebsite[i] = "            font-family: Verdana, sans-serif;";
                                    break;
                            }
                        }
                        //Title Font
                        else if (arrayThemeCode[x] == "/*CSS-title-font*/")
                        {
                            switch (int.Parse(websiteData[page, TITLE_FONT]))
                            {
                                case 0:
                                    arrayWebsite[i] = "            font-family: Arial, Helvetica, sans-serif;";
                                    break;
                                case 1:
                                    arrayWebsite[i] = "            font-family: 'Arial Black', sans-serif;";
                                    break;
                                case 2:
                                    arrayWebsite[i] = "            font-family:  'Comic Sans MS', cursive;";
                                    break;
                                case 3:
                                    arrayWebsite[i] = "            font-family: Impact, sans-serif;";
                                    break;
                                case 4:
                                    arrayWebsite[i] = "            font-family: Courier, monospace;";
                                    break;
                                case 5:
                                    arrayWebsite[i] = "            font-family: 'Times New Roman', Times, serif; ";
                                    break;
                                case 6:
                                    arrayWebsite[i] = "            font-family: Verdana, sans-serif;";
                                    break;
                            }
                        }
                        //Title Text Color
                        else if (arrayThemeCode[x] == "/*CSS-title-font-color*/")
                        {
                            arrayWebsite[i] = "            color: " + websiteData[page, TITLE_TEXT_COLOR] + ";";
                        }
                        //Content Font
                        else if (arrayThemeCode[x] == "/*CSS-content-font*/")
                        {
                            switch (int.Parse(websiteData[page, CONTENT_FONT]))
                            {
                                case 0:
                                    arrayWebsite[i] = "            font-family: Arial, Helvetica, sans-serif;";
                                    break;
                                case 1:
                                    arrayWebsite[i] = "            font-family: 'Arial Black', sans-serif;";
                                    break;
                                case 2:
                                    arrayWebsite[i] = "            font-family:  'Comic Sans MS', cursive;";
                                    break;
                                case 3:
                                    arrayWebsite[i] = "            font-family: Impact, sans-serif;";
                                    break;
                                case 4:
                                    arrayWebsite[i] = "            font-family: Courier, monospace;";
                                    break;
                                case 5:
                                    arrayWebsite[i] = "            font-family: 'Times New Roman', Times, serif; ";
                                    break;
                                case 6:
                                    arrayWebsite[i] = "            font-family: Verdana, sans-serif;";
                                    break;
                            }
                        }
                        //Content Text Color
                        else if (arrayThemeCode[x] == "/*CSS-content-font-color*/")
                        {
                            arrayWebsite[i] = "            color: " + websiteData[page, CONTENT_TEXT_COLOR] + ";";
                        }
                        //Title Text
                        else if (arrayThemeCode[x] == "<!--#CONTENT:Title-->")
                        {
                            arrayWebsite[i] = "            " + "<h1>" + websiteData[page, TITLE_TEXT] + "</h1>";
                        }
                        //Menu Items
                        else if (arrayThemeCode[x] == "<!--#MENU-ITEMS-->")
                        {
                            for (int y = 0; y < int.Parse(projectData[PROJECT_TOTAL_PAGES]); y++)
                            {
                                if (websiteData[y, PAGE_NAME] == "Index")
                                {
                                    arrayWebsite[i] = "            <a href=\u0022" + websiteData[y, PAGE_NAME] + ".html\u0022 " + "class=\u0022menu-text\u0022 target =\u0022_self\u0022 >Home</a>";
                                    i++;
                                }
                                else
                                {
                                    arrayWebsite[i] = "            <a href=\u0022" + websiteData[y, PAGE_NAME] + ".html\u0022 " + "class=\u0022menu-text\u0022 target =\u0022_self\u0022 >" + websiteData[y, PAGE_NAME] + "</a>";
                                    i++;
                                }
                            }
                        }
                        //Content 1 Text
                        else if (arrayThemeCode[x] == "<!--#CONTENT1-->")
                        {
                            var str1 = websiteData[page, CONTENT_1_TEXT].Replace('¶', '\n');
                            arrayWebsite[i] = "            " + "<pre>" + str1 + "</pre>";
                        }
                        //Content 2 Text
                        else if (arrayThemeCode[x] == "<!--#CONTENT2-->")
                        {
                            var str2 = websiteData[page, CONTENT_2_TEXT].Replace('¶', '\n');
                            arrayWebsite[i] = "            " + "<pre>" + str2 + "</pre>";
                        }
                        //Footer Text
                        else if (arrayThemeCode[x] == "<!--#CONTENT:Footer-->")
                        {
                            arrayWebsite[i] = "            " + "<p>" + websiteData[page, FOOTER_TEXT] + "</p>";
                        }
                        //Content 1 Layout
                        else if (arrayThemeCode[x] == "/*CSS-CONTENT1-LAYOUT*/")
                        {
                            if (websiteData[page, PAGE_LAYOUT] == "singleLayout")
                            {
                                arrayWebsite[i] = "            position: absolute;";
                                i++;
                                arrayWebsite[i] = "            top: 9vw;";
                                i++;
                                arrayWebsite[i] = "            left: 25vw;";
                                i++;
                                arrayWebsite[i] = "            width: 50vw;";
                                i++;
                                arrayWebsite[i] = "            height: 41vw;";

                            }
                            else
                            {
                                arrayWebsite[i] = "            position: absolute;";
                                i++;
                                arrayWebsite[i] = "            top: 9vw;";
                                i++;
                                arrayWebsite[i] = "            left: 25vw;";
                                i++;
                                arrayWebsite[i] = "            width: 25vw;";
                                i++;
                                arrayWebsite[i] = "            height: 20vw;";
                            }
                        }
                        //Content 2 Layout
                        else if (arrayThemeCode[x] == "/*CSS-CONTENT2-LAYOUT*/")
                        {
                            if (websiteData[page, PAGE_LAYOUT] == "singleLayout")
                            {
                                arrayWebsite[i] = "            display: none;";
                            }
                            else
                            {
                                arrayWebsite[i] = "            position: absolute;";
                                i++;
                                arrayWebsite[i] = "            top: 20.5vw;";
                                i++;
                                arrayWebsite[i] = "            left: -0.2vw;";
                                i++;
                                arrayWebsite[i] = "            width: 50vw;";
                                i++;
                                arrayWebsite[i] = "            height: 20vw;";
                            }
                        }
                        //Picture Holder Layout
                        else if (arrayThemeCode[x] == "/*CSS-PICTUREHOLDER-LAYOUT*/")
                        {
                            if (websiteData[page, PAGE_LAYOUT] == "singleLayout")
                            {
                                arrayWebsite[i] = "            display: none;";
                            }
                            else
                            {
                                arrayWebsite[i] = "            position: absolute;";
                                i++;
                                arrayWebsite[i] = "            top: 0.2vw;";
                                i++;
                                arrayWebsite[i] = "            left: 25.5vw;";
                                i++;
                                arrayWebsite[i] = "            width: 24vw;";
                                i++;
                                arrayWebsite[i] = "            height: 19vw;";
                            }
                        }
                        //Header image for theme color
                        else if (arrayThemeCode[x] == "<!--HEADER-IMAGE-->")
                        {
                            switch (int.Parse(websiteData[page, PAGE_COLOR]))
                            {
                                case 0: //Navy
                                    arrayWebsite[i] = "<img id=\u0022header\u0022 src =\u0022Images/header-navy.png\u0022 > ";
                                    break;
                                case 1: //Maroon
                                    arrayWebsite[i] = "<img id=\u0022header\u0022 src =\u0022Images/header-maroon.png\u0022 > ";
                                    break;
                                case 2: //Green
                                    arrayWebsite[i] = "<img id=\u0022header\u0022 src =\u0022Images/header-green.png\u0022 > ";
                                    break;
                                case 3: //Light Blue
                                    arrayWebsite[i] = "<img id=\u0022header\u0022 src =\u0022Images/header-lightblue.png\u0022 > ";
                                    break;
                                case 4: //Orange
                                    arrayWebsite[i] = "<img id=\u0022header\u0022 src =\u0022Images/header-orange.png\u0022 > ";
                                    break;
                                case 5: //Yellow
                                    arrayWebsite[i] = "<img id=\u0022header\u0022 src =\u0022Images/header-yellow.png\u0022 > ";
                                    break;
                                case 6: //Purple
                                    arrayWebsite[i] = "<img id=\u0022header\u0022 src =\u0022Images/header-purple.png\u0022 > ";
                                    break;
                                case 7: //Red
                                    arrayWebsite[i] = "<img id=\u0022header\u0022 src =\u0022Images/header-red.png\u0022 > ";
                                    break;
                            }
                        }
                        //If an image is present, set the image source and alt text
                        else if (arrayThemeCode[x] == "<!--IMAGE-->")
                        {
                            if (websiteData[page, IMAGE_LOCATION_TYPE] != "0")
                            {
                                arrayWebsite[i] = "<img id=\u0022imageFile\u0022 src =\u0022" + websiteData[page, IMAGE_LOCATION] + "\u0022 Alt=\u0022" + websiteData[page, IMAGE_ALT] + "\u0022> ";
                            }
                        }

                        //If the theme code doesn't have any code call outs, write the next line of the theme code.
                        else
                        {
                            arrayWebsite[i] = arrayThemeCode[x];
                        }
                    }


                    
                    //Add images to new image directory
                    //If theme equals single bars (Not used due to only 1 theme in the current version.)
                    string pathSiteImages = Path.Combine(Environment.CurrentDirectory, @"Website Themes\SingleBar\Images");

                    //Copy the header image into the new directory
                    switch (int.Parse(websiteData[page, PAGE_COLOR]))
                    {
                        case 0: //Navy
                            File.Copy(Path.Combine(pathSiteImages, "header-navy.png"), Path.Combine(pathImageFolderLocation, "header-navy.png"), true);
                            break;
                        case 1: //Maroon
                            File.Copy(Path.Combine(pathSiteImages, "header-maroon.png"), Path.Combine(pathImageFolderLocation, "header-maroon.png"), true);
                            break;
                        case 2: //Green
                            File.Copy(Path.Combine(pathSiteImages, "header-green.png"), Path.Combine(pathImageFolderLocation, "header-green.png"), true);
                            break;
                        case 3: //Light Blue
                            File.Copy(Path.Combine(pathSiteImages, "header-lightblue.png"), Path.Combine(pathImageFolderLocation, "header-lightblue.png"), true);
                            break;
                        case 4: //Orange
                            File.Copy(Path.Combine(pathSiteImages, "header-orange.png"), Path.Combine(pathImageFolderLocation, "header-orange.png"), true);
                            break;
                        case 5: //Yellow
                            File.Copy(Path.Combine(pathSiteImages, "header-yellow.png"), Path.Combine(pathImageFolderLocation, "header-yellow.png"), true);
                            break;
                        case 6: //Purple
                            File.Copy(Path.Combine(pathSiteImages, "header-purple.png"), Path.Combine(pathImageFolderLocation, "header-purple.png"), true);
                            break;
                        case 7: //Red
                            File.Copy(Path.Combine(pathSiteImages, "header-red.png"), Path.Combine(pathImageFolderLocation, "header-red.png"), true);
                            break;
                    }

                    //Copy the body, footer, and background into the new directory
                    //Body
                    File.Copy(Path.Combine(pathSiteImages, "body.png"), Path.Combine(pathImageFolderLocation, "body.png"), true);
                    //Footer
                    File.Copy(Path.Combine(pathSiteImages, "footer.png"), Path.Combine(pathImageFolderLocation, "footer.png"), true);
                    //Background
                    File.Copy(Path.Combine(pathSiteImages, "background.png"), Path.Combine(pathImageFolderLocation, "background.png"), true);

                    //Copy users images to new directory
                    for (int i = 0; i < int.Parse(projectData[PROJECT_TOTAL_PAGES]); i++)
                    {
                        if (websiteData[i,IMAGE_LOCATION_TYPE] == "1")
                        {
                            var imageFileName = Path.GetFileName(websiteData[i, IMAGE_LOCATION]);

                            File.Copy(websiteData[i, IMAGE_LOCATION], Path.Combine(pathImageFolderLocation, imageFileName), true);
                        }
                    }
                    

                    //output the pages to the new directory
                    StreamWriter outputFile;

                    string pathPageName = Path.Combine(pathExportLocation, websiteData[page, PAGE_NAME] + ".html");
                    outputFile = File.CreateText(pathPageName);

                    for (int i = 0; i < arrayWebsite.GetLength(0) && arrayWebsite[i] != "<!--End of site-->"; i++)
                    {
                        outputFile.WriteLine(arrayWebsite[i]);
                    }

                    outputFile.Close();
                }

                //If export is a success, return true
                return true;

            }

            catch
            {
                //if export fails, return false
                return false;
            }
        }

        public string getExportLocation()
        {
            //Return the export location
            return pathExportLocation;
        }
    }
}
