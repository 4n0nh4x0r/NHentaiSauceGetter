using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NHentai_Sauce_Getter
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        async private void getSauce()
        {

            string[] includedTags = tagsIncluded.Text.Split(',');
            string[] excludedTags = tagsExcluded.Text.Split(',');

            // There must be at least one tag in the include list
            if(includedTags.Length == 1 && includedTags[0] == "")
            {
                MessageBox.Show("You need to have at least one tag in the include box", "Error");
                return;
            }

            // Formatting the include taglist to have a tag followed by ", " (Inportant for the nhentai library to work)
            string[] outputArray = new string[includedTags.Length];
            for (int i = 0; i < includedTags.Length; i++)
            {
                outputArray[i] = NHentaiSharp.Core.SearchClient.GetExcludeTag(includedTags[i].Trim());
            }
            string outputStringIncludes = String.Join(", ", outputArray);


            // Formatting the exclude taglist to have a tag followed by ", " (Inportant for the nhentai library to work)
            outputArray = new string[excludedTags.Length];
            for (int i = 0; i < excludedTags.Length; i++)
            {
                outputArray[i] = NHentaiSharp.Core.SearchClient.GetExcludeTag(excludedTags[i].Trim());
            }
            string outputStringExcludes = String.Join(", ", outputArray);

            // Here we set the tags that we want to include and exclude from our search
            string[] tags = new[] {
                outputStringIncludes,
                outputStringExcludes
            };


            Random r = new Random();
            try
            {
                // We do a search with the tags
                var result = await NHentaiSharp.Core.SearchClient.SearchWithTagsAsync(tags);
                int page = r.Next(0, result.numPages) + 1; // Page count begin at 1
                                                           // We do a new search at a random page
                result = await NHentaiSharp.Core.SearchClient.SearchWithTagsAsync(tags, page);
                var doujinshi = result.elements[r.Next(0, result.elements.Length)]; // We get a random doujinshi

                // Here we add the doujin information to our listview box on the right side
                listView1.Items.Add(new ListViewItem(new string[] { (doujinshi.id).ToString(), doujinshi.englishTitle, (doujinshi.numPages).ToString()}));
            }
            catch(Exception e)
            {   
                // In case no doujin is being found, the upper code will error, to prevent the program from crashing we do this catch
                MessageBox.Show("No results could be found with these tags", "Search Error");
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            // We simply execude our function that handles the entire process to get the sauces
            getSauce();
        }

        private void sauceLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // Open link in browser
            System.Diagnostics.Process.Start(sauceLinkLabel.Text);
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                switch (checkBox1.Checked)
                {
                    case true: // Open the link directly in the browser
                        System.Diagnostics.Process.Start("https://www.nhentai.net/g/" + listView1.SelectedItems[0].Text.ToString());
                        break;
                    case false: // Change the linklabel on the form to the sauce for manual opening
                        sauceLinkLabel.Text = "https://www.nhentai.net/g/" + listView1.SelectedItems[0].Text.ToString();
                        break;
                }
            }catch (Exception err){
                // An error can occur while trying to change the linklabel text, dont ask me why, just accept it and....whatever
                Console.WriteLine(err);
            }
        }
    }
}
