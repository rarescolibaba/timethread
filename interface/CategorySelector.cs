// AUTORI: Bostan Sorina-Gabirela, Brinza Denis-Stefan, Colibaba Rares-Andrei, Dodita Alexandru-Tomi
// UNIVERSITATEA TEHNICA GHEORGHE ASACHI, GRUPA 1312A
// Functionalitate:
//Control pentru selectarea categoriilor de procese si accesarea help-ului in aplicatia Process Time Tracker.
// ---------------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Reflection; 

namespace @interface
{
    /// <summary>
    /// Control pentru selectarea categoriilor de procese si accesarea help-ului
    /// </summary>
    public class CategorySelector : Panel
    {
        private List<string> _categories;
        private string _selectedCategory;
        private Button _helpButton; 

        /// <summary>
        /// Eveniment declansat cand se selecteaza o categorie
        /// </summary>
        public event EventHandler<string> CategorySelected;

        /// <summary>
        /// Eveniment declansat cand se apasa butonul de help
        /// </summary>
        public event EventHandler HelpButtonClicked;

        /// <summary>
        /// Returneaza sau seteaza categoriile disponibile
        /// </summary>
        public List<string> Categories
        {
            get { return _categories; }
            set
            {
                _categories = value;
                CreateCategoryButtons(); 
            }
        }

        /// <summary>
        /// Returneaza sau seteaza categoria selectata
        /// </summary>
        public string SelectedCategory
        {
            get { return _selectedCategory; }
            set
            {
                _selectedCategory = value;
                UpdateButtonStates();
            }
        }

        /// <summary>
        /// Constructor pentru CategorySelector
        /// </summary>
        public CategorySelector()
        {
            _categories = new List<string>();
            _selectedCategory = "All";

            BorderStyle = BorderStyle.FixedSingle;
            Padding = new Padding(5);

            Label titleLabel = new Label
            {
                Text = "Categories",
                Font = new Font("Arial", 10, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(5, 5)
            };
            Controls.Add(titleLabel);
            CreateCategoryButtons();
        }

        /// <summary>
        /// Actualizeaza starea butoanelor in functie de categoria selectata
        /// </summary>
        private void CreateCategoryButtons()
        {
            List<Control> controlsToRemove = new List<Control>();
            foreach (Control control in Controls)
            {
                if (control is Button) 
                {
                    controlsToRemove.Add(control);
                }
            }
            foreach (Control control in controlsToRemove)
            {
                Controls.Remove(control);
                control.Dispose();
            }

            AddCategoryButton("All", 0);

            for (int i = 0; i < _categories.Count; i++)
            {
                AddCategoryButton(_categories[i], i + 1);
            }

            int helpButtonYPosition = 30 + ((_categories.Count > 0 ? _categories.Count : 0) + 1) * 35; 

            _helpButton = new Button
            {
                Text = "Help",
                Tag = "HelpButton", 
                Width = 100,
                Height = 30,
                Location = new Point(10, helpButtonYPosition),
                FlatStyle = FlatStyle.System 
            };
            _helpButton.Click += HelpButton_Click;
            Controls.Add(_helpButton);


            UpdateButtonStates();
        }

        private void AddCategoryButton(string category, int index)
        {
            Button button = new Button
            {
                Text = category,
                Tag = category,
                Width = 100,
                Height = 30,
                Location = new Point(10, 30 + (index * 35)),
                FlatStyle = FlatStyle.Flat
            };

            button.Click += CategoryButton_Click;
            Controls.Add(button);
        }

        private void UpdateButtonStates()
        {
            foreach (Control control in Controls)
            {
                if (control is Button button && button.Tag?.ToString() != "HelpButton") 
                {
                    string category = button.Tag as string;

                    if (category == _selectedCategory)
                    {
                        button.BackColor = SystemColors.Highlight;
                        button.ForeColor = Color.White;
                    }
                    else
                    {
                        button.BackColor = SystemColors.Control;
                        button.ForeColor = SystemColors.ControlText;
                    }
                }
            }
        }

        /// <summary>
        /// Eveniment la apasarea unui buton de categorie
        /// </summary>
        private void CategoryButton_Click(object sender, EventArgs e)
        {
            if (sender is Button button)
            {
                string category = button.Tag as string;
                SelectedCategory = category;
                CategorySelected?.Invoke(this, category);
            }
        }

        /// <summary>
        /// Eveniment la apasarea butonului de help
        /// </summary>
        private void HelpButton_Click(object sender, EventArgs e)
        {
            HelpButtonClicked?.Invoke(this, EventArgs.Empty);
        }
    }
}