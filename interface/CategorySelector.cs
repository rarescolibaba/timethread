using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace @interface
{
    /// <summary>
    /// Control for selecting process categories
    /// </summary>
    public class CategorySelector : Panel
    {
        /// <summary>
        /// Available categories
        /// </summary>
        private List<string> _categories;

        /// <summary>
        /// Currently selected category
        /// </summary>
        private string _selectedCategory;

        /// <summary>
        /// Event raised when a category is selected
        /// </summary>
        public event EventHandler<string> CategorySelected;

        /// <summary>
        /// Gets or sets the available categories
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
        /// Gets or sets the selected category
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
        /// Constructor for CategorySelector
        /// </summary>
        public CategorySelector()
        {
            _categories = new List<string>();
            _selectedCategory = "All";
            
            // Set panel properties
            BorderStyle = BorderStyle.FixedSingle;
            Padding = new Padding(5);
            
            // Add default title
            Label titleLabel = new Label
            {
                Text = "Categories",
                Font = new Font("Arial", 10, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(5, 5)
            };
            
            Controls.Add(titleLabel);
        }

        /// <summary>
        /// Creates buttons for each category
        /// </summary>
        private void CreateCategoryButtons()
        {
            // Clear existing buttons
            foreach (Control control in Controls)
            {
                if (control is Button)
                {
                    Controls.Remove(control);
                }
            }
            
            // Add "All" category button
            AddCategoryButton("All", 0);
            
            // Add buttons for each category
            for (int i = 0; i < _categories.Count; i++)
            {
                AddCategoryButton(_categories[i], i + 1);
            }
            
            // Update button states
            UpdateButtonStates();
        }

        /// <summary>
        /// Adds a button for a category
        /// </summary>
        /// <param name="category">Category name</param>
        /// <param name="index">Button index</param>
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

        /// <summary>
        /// Updates the visual state of category buttons
        /// </summary>
        private void UpdateButtonStates()
        {
            foreach (Control control in Controls)
            {
                if (control is Button button)
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
        /// Handles click events on category buttons
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void CategoryButton_Click(object sender, EventArgs e)
        {
            if (sender is Button button)
            {
                string category = button.Tag as string;
                SelectedCategory = category;
                
                // Raise the CategorySelected event
                CategorySelected?.Invoke(this, category);
            }
        }
    }
} 